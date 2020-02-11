using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Text;
using System.Threading.Tasks;
using A2v10.ProcS.Infrastructure;
using BotCore;
using System.Threading;

namespace A2v10.ProcS.Chatbot
{
	public enum BotEngine
	{
		Telegram,
		Viber
	}

	internal class BotManager
	{
		private IDictionary<BotEngine, IBotFactory> factories;
		private IDictionary<BotEngine, ConcurrentDictionary<String, (Boolean isInit, SemaphoreSlim sem, IBot bot)>> bots;

		public BotManager(Microsoft.Extensions.Configuration.IConfiguration configuration)
		{
			factories = new Dictionary<BotEngine, IBotFactory>();
			factories.Add(BotEngine.Telegram, new TelegramBotFactory(configuration.GetSection("Telegram")));

			bots = new Dictionary<BotEngine, ConcurrentDictionary<String, (Boolean, SemaphoreSlim, IBot)>>();
			bots.Add(BotEngine.Telegram, new ConcurrentDictionary<String, (Boolean, SemaphoreSlim, IBot)>());
		}

		public async Task<IBot> GetBotAsync(BotEngine engine, String key)
		{
			var bts = bots[engine];
			var x = bts.GetOrAdd(key, k =>
			{
				var f = factories[engine];
				return (false, new SemaphoreSlim(1, 1), f.CreateBot(k));
			});
			if (x.isInit) return x.bot;
			await x.sem.WaitAsync();
			try
			{
				if (x.isInit) return x.bot;
				await x.bot.InitAsync();
				x.isInit = true;
				return x.bot;
			}
			finally
			{
				x.sem.Release();
			}
		}
	}

	internal interface IBotFactory
	{
		IBot CreateBot(String key);
	}

	internal class TelegramBotFactory : IBotFactory
	{
		private Microsoft.Extensions.Configuration.IConfiguration confs;

		public TelegramBotFactory(Microsoft.Extensions.Configuration.IConfiguration configuration)
		{
			confs = configuration;
		}

		public IBot CreateBot(String key)
		{
			var cs = confs.GetSection(key);

			var cfg = new BotCore.Types.Base.Configure();
			cfg.Token = cs["Token"];
			cfg.WebHook = "";
			return new BotCore.Telegram.TelegramBot(cfg);
		}
	}

	internal class EndpointHandlerFactory : IEndpointHandlerFactory
	{
		private readonly BotEngine engine;
		private readonly BotManager botManager;
		private readonly IServiceBus bus;

		public EndpointHandlerFactory(IServiceBus bus, BotEngine engine, BotManager botManager)
		{
			this.bus = bus;
			this.engine = engine;
			this.botManager = botManager;
		}

		public IEndpointHandler CreateHandler()
		{
			return new EndpointHandler(bus, engine, botManager);
		}
	}

	internal class EndpointHandler : IEndpointHandler
	{
		private readonly BotEngine engine;
		private readonly BotManager botManager;
		private readonly IServiceBus bus;

		public EndpointHandler(IServiceBus bus, BotEngine engine, BotManager botManager)
		{
			this.bus = bus;
			this.engine = engine;
			this.botManager = botManager;
		}

		public async Task<(string body, string type)> HandleAsync(string body, string path)
		{
			var pathes = path.Split('/');
			var proc = new MessageProcessor(bus, engine, pathes[0]);
			var bot = await botManager.GetBotAsync(engine, pathes[0]);
			await bot.ProcessIncomingMessageAsync(body, proc);
			return ("", "text/plain");
		}
	}

    internal class MessageProcessor : IMessageProcessor
    {
		private readonly BotEngine engine;
		private readonly String key;
		private readonly IServiceBus bus;

		public MessageProcessor(IServiceBus bus, BotEngine engine, String key)
        {
			this.bus = bus;
            this.engine = engine;
			this.key = key;
		}

		public IEnumerable<IOutgoingMessage> ProcessIncomingMessage(IChatSession sess, IIncomingMessage msg)
        {
			var m = new IncomeMessage(sess.ChatId);
			m.BotEngine = engine;
			m.BotKey = key;
			m.Message = msg;
			bus.Send(m);
			yield break;
		}
    }
}

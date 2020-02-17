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
		private IDictionary<BotEngine, ConcurrentDictionary<String, BotWrapper>> bots;

		public BotManager(Microsoft.Extensions.Configuration.IConfiguration configuration)
		{
			factories = new Dictionary<BotEngine, IBotFactory>();
			factories.Add(BotEngine.Telegram, new TelegramBotFactory(configuration.GetSection("Telegram")));

			bots = new Dictionary<BotEngine, ConcurrentDictionary<String, BotWrapper>>();
			bots.Add(BotEngine.Telegram, new ConcurrentDictionary<String, BotWrapper>(StringComparer.InvariantCultureIgnoreCase));
		}

		protected class BotWrapper
		{
			private readonly IBot bot;
			private readonly SemaphoreSlim sem;
			private Boolean isInit;

			public IBot Bot
			{
				get
				{
					if (!isInit) throw new InvalidOperationException("Bot is not Init");
					return bot;
				}
			}

			public BotWrapper(IBot bot)
			{
				isInit = false;
				this.bot = bot;
				sem = new SemaphoreSlim(1, 1);
			}

			public async Task SafeInit()
			{
				if (isInit) return;
				await sem.WaitAsync();
				try
				{
					if (isInit) return;
					await bot.InitAsync();
					isInit = true;
				}
				finally
				{
					sem.Release();
				}
			}
		}

		public async Task<IBot> GetBotAsync(BotEngine engine, String key)
		{
			var bts = bots[engine];
			var w = bts.GetOrAdd(key, k =>
			{
				var f = factories[engine];
				return new BotWrapper(f.CreateBot(k));
			});
			await w.SafeInit();
			return w.Bot;
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
			cfg.WebHook = cs["WebHookUri"];
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
			var bot = await botManager.GetBotAsync(engine, pathes[0]);
			var proc = new MessageProcessor(bus, engine, pathes[0]);
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

	internal class ChatSession : IChatSession
	{
		public ChatSession(Guid guid)
		{
			ChatId = guid;
		}
		public Guid ChatId { get; private set; }
	}
}

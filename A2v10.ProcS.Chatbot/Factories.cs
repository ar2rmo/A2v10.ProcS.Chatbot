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
		private BotEngine engine;
		private BotManager botManager;

		public EndpointHandlerFactory(BotEngine engine, BotManager botManager)
		{
			this.engine = engine;
			this.botManager = botManager;
		}

		public IEndpointHandler CreateHandler()
		{
			return new EndpointHandler(engine, botManager);
		}
	}

	internal class EndpointHandler : IEndpointHandler
	{
		private BotEngine engine;
		private BotManager botManager;

		public EndpointHandler(BotEngine engine, BotManager botManager)
		{
			this.engine = engine;
			this.botManager = botManager;
		}

		public Task<(string body, string type)> HandleAsync(string body, string path)
		{
			throw new NotImplementedException();
		}
	}
}

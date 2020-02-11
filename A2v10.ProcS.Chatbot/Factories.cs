using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using A2v10.ProcS.Infrastructure;
using BotCore;

namespace A2v10.ProcS.Chatbot
{
	public enum BotEngine
	{
		Telegram,
		Viber
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
			confs = configuration.GetSection("ChatBots").GetSection("Telegram");
		}

		public IBot CreateBot(String key)
		{
			var cs = confs.GetSection(key);

			var cfg = new BotCore.Types.Base.Configure();
			cfg.Token = cs["Token"];
			return new BotCore.Telegram.TelegramBot(cfg);
		}
	}

	internal class EndpointHandlerFactory : IEndpointHandlerFactory
	{
		private IBotFactory botFactory;

		public EndpointHandlerFactory(IBotFactory botFactory)
		{
			this.botFactory = botFactory;
		}

		public IEndpointHandler CreateHandler()
		{
			return new EndpointHandler(botFactory);
		}
	}

	internal class EndpointHandler : IEndpointHandler
	{
		private IBotFactory botFactory;

		public EndpointHandler(IBotFactory botFactory)
		{
			this.botFactory = botFactory;
		}

		public Task<(string body, string type)> HandleAsync(string body, string path)
		{
			throw new NotImplementedException();
		}
	}
}

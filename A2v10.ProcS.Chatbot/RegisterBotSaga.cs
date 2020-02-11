using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using A2v10.ProcS.Infrastructure;
using BotCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace A2v10.ProcS.Chatbot
{
	public class RegisterBotMessage : MessageBase<String>
	{
		public BotEngine BotEngine { get; set; }
		public String BotKey { get; set; }

		public RegisterBotMessage(BotEngine botEngine, String botKey) : base($"{botEngine}:{botKey}")
		{
		}
	}

	public class RegisterBotSaga : SagaBaseDispatched<String, RegisterBotMessage>
	{
		private BotManager botManager;

		internal RegisterBotSaga(BotManager botManager) : base(nameof(RegisterBotSaga))
		{
			this.botManager = botManager;
		}

		protected override async Task Handle(IHandleContext context, RegisterBotMessage message)
		{
			var bot = await botManager.GetBotAsync(message.BotEngine, message.BotKey);
            CorrelationId.Value = message.CorrelationId.Value;
		}
	}

	internal class RegisterBotSagaFactory : ISagaFactory
	{
		private BotManager botManager;
		
		public RegisterBotSagaFactory(BotManager botManager)
		{
			this.botManager = botManager;
		}

		public string SagaKind => nameof(RegisterBotSaga);

		public ISaga CreateSaga()
		{
			return new RegisterBotSaga(botManager);
		}
	}

	public class RegisterBotSagaRegistrar : ISagaRegistrar
	{
		private Plugin plugin;

		public RegisterBotSagaRegistrar(Plugin plugin)
		{
			this.plugin = plugin;
		}

		public void Register(ISagaManager mgr)
		{
			var factory = new RegisterBotSagaFactory(plugin.BotManager);
			mgr.RegisterSagaFactory(factory, RegisterBotSaga.GetHandledTypes());
		}
	}
}

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using A2v10.ProcS.Infrastructure;
using BotCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace A2v10.ProcS.Chatbot
{
	public class RegisterBotProcessingMessage : MessageBase<String>
	{
		public Guid MasterProcessId { get; set; }
		public BotEngine BotEngine { get; set; }
		public String BotKey { get; set; }
		public String ChatProcessIdentity { get; set; }

		public RegisterBotProcessingMessage(BotEngine botEngine, String botKey) : base($"{botEngine}:{botKey.ToLowerInvariant()}")
		{
			BotEngine = botEngine;
			BotKey = botKey;
		}
	}

	public class InitBotChatMessage : MessageBase<String>
	{
		public BotEngine BotEngine { get; set; }
		public String BotKey { get; set; }
		public Guid ChatId { get; set; }
		public IIncomingMessage Message { get; set; }

		public InitBotChatMessage(BotEngine botEngine, String botKey) : base($"{botEngine}:{botKey.ToLowerInvariant()}")
		{
		}
	}

	public class RegisterBotProcessingSaga : SagaBaseDispatched<String, RegisterBotProcessingMessage, InitBotChatMessage>
	{
		private BotManager botManager;

		private Guid MasterProcessId { get; set; }
		private String ChatProcessIdentity { get; set; }

		internal RegisterBotProcessingSaga(BotManager botManager) : base(nameof(RegisterBotProcessingSaga))
		{
			this.botManager = botManager;
		}

		protected override async Task Handle(IHandleContext context, RegisterBotProcessingMessage message)
		{
			var bot = await botManager.GetBotAsync(message.BotEngine, message.BotKey);
			MasterProcessId = message.MasterProcessId;
			ChatProcessIdentity = message.ChatProcessIdentity;
			CorrelationId.Value = message.CorrelationId.Value;
		}

		protected override Task Handle(IHandleContext context, InitBotChatMessage message)
		{
			var sp = new StartProcessMessage(MasterProcessId);
			sp.ProcessId = ChatProcessIdentity;
			sp.Parameters = DynamicObject.From<InitBotChatMessage, IIncomingMessage>(message);
			context.SendMessage(sp);
			var m = new IncomeMessage(message.ChatId);
			m.BotEngine = message.BotEngine;
			m.BotKey = message.BotKey;
			m.Message = message.Message;
			context.SendMessage(m);
			return Task.CompletedTask;
		}
	}

	internal class RegisterBotSagaFactory : ISagaFactory
	{
		private BotManager botManager;
		
		public RegisterBotSagaFactory(BotManager botManager)
		{
			this.botManager = botManager;
		}

		public string SagaKind => nameof(RegisterBotProcessingSaga);

		public ISaga CreateSaga()
		{
			return new RegisterBotProcessingSaga(botManager);
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
			mgr.RegisterSagaFactory(factory, RegisterBotProcessingSaga.GetHandledTypes());
		}
	}
}

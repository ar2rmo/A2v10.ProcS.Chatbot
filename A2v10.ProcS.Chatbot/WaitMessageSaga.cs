using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using A2v10.ProcS.Infrastructure;
using BotCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace A2v10.ProcS.Chatbot
{
	public class IncomeMessage : MessageBase<Guid>
	{
		public BotEngine BotEngine { get; set; }
		public String BotKey { get; set; }
		public String RawBody { get; set; }

		public IncomeMessage(Guid chatId) : base(chatId)
		{
		}
	}

	public class WaitMessageSaga : SagaBaseDispatched<Guid, WaitMessageMessage, IncomeMessage>
	{
		private BotManager botManager;

		internal WaitMessageSaga(BotManager botManager) : base(nameof(SendMessageSaga))
		{
			this.botManager = botManager;
		}

		protected override Task Handle(IHandleContext context, WaitMessageMessage message)
		{
			return Task.CompletedTask;
		}

		protected override Task Handle(IHandleContext context, IncomeMessage message)
		{
			return Task.CompletedTask;
		}
	}

	internal class WaitMessageSagaFactory : ISagaFactory
	{
		private BotManager botManager;
		
		public WaitMessageSagaFactory(BotManager botManager)
		{
			this.botManager = botManager;
		}

		public string SagaKind => nameof(WaitMessageSaga);

		public ISaga CreateSaga()
		{
			return new WaitMessageSaga(botManager);
		}
	}

	public class WaitMessageSagaRegistrar : ISagaRegistrar
	{
		private Plugin plugin;

		public WaitMessageSagaRegistrar(Plugin plugin)
		{
			this.plugin = plugin;
		}

		public void Register(ISagaManager mgr)
		{
			var factory = new WaitMessageSagaFactory(plugin.BotManager);
			mgr.RegisterSagaFactory(factory, WaitMessageSaga.GetHandledTypes());
		}
	}
}

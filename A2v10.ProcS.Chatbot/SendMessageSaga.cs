using System;
using System.Threading.Tasks;
using A2v10.ProcS.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace A2v10.ProcS.Chatbot
{
	public class SendMessageSaga : SagaBaseDispatched<Guid, SendMessageMessage>
	{
		private BotManager botManager;

		internal SendMessageSaga(BotManager botManager) : base(nameof(SendMessageSaga))
		{
			this.botManager = botManager;
		}

		protected override Task Handle(IHandleContext context, SendMessageMessage message)
		{
			//bot.SendMessageAsync(message)
			return Task.CompletedTask;
		}
	}

	internal class SendMessageSagaFactory : ISagaFactory
	{
		private BotManager botManager;
		
		public SendMessageSagaFactory(BotManager botManager)
		{
			this.botManager = botManager;
		}

		public string SagaKind => nameof(SendMessageSaga);

		public ISaga CreateSaga()
		{
			return new SendMessageSaga(botManager);
		}
	}

	public class SendMessageSagaRegistrar : ISagaRegistrar
	{
		private Plugin plugin;

		public SendMessageSagaRegistrar(Plugin plugin)
		{
			this.plugin = plugin;
		}

		public void Register(ISagaManager mgr)
		{
			var factory = new SendMessageSagaFactory(plugin.BotManager);
			mgr.RegisterSagaFactory(factory, SendMessageSaga.GetHandledTypes());
		}
	}
}

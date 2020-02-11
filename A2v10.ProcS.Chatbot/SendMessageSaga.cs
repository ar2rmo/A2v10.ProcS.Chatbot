using System;
using System.Threading.Tasks;
using A2v10.ProcS.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace A2v10.ProcS.Chatbot
{
	public class SendMessageSaga : SagaBaseDispatched<Guid, SendMessageMessage>
	{
		private IBotFactory botFactory;

		internal SendMessageSaga(IBotFactory botFactory) : base(nameof(SendMessageSaga))
		{
			this.botFactory = botFactory;
		}

		protected override Task Handle(IHandleContext context, SendMessageMessage message)
		{
			//bot.SendMessageAsync(message)
			return Task.CompletedTask;
		}
	}

	internal class SendMessageSagaFactory : ISagaFactory
	{
		private IBotFactory botFactory;
		
		public SendMessageSagaFactory(IBotFactory botFactory)
		{
			this.botFactory = botFactory;
		}

		public string SagaKind => nameof(SendMessageSaga);

		public ISaga CreateSaga()
		{
			return new SendMessageSaga(botFactory);
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
			var factory = new SendMessageSagaFactory(plugin.TelegramBotFactory);
			mgr.RegisterSagaFactory(factory, SendMessageSaga.GetHandledTypes());
		}
	}
}

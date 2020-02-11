using System;
using System.Threading.Tasks;
using A2v10.ProcS.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace A2v10.ProcS.Chatbot
{
	public class WaitMessageSaga : SagaBaseDispatched<Guid, WaitMessageMessage>
	{
		private IBotFactory botFactory;

		internal WaitMessageSaga(IBotFactory botFactory) : base(nameof(SendMessageSaga))
		{
			this.botFactory = botFactory;
		}

		protected override Task Handle(IHandleContext context, WaitMessageMessage message)
		{
			//bot.SendMessageAsync(message)
			return Task.CompletedTask;
		}
	}

	internal class WaitMessageSagaFactory : ISagaFactory
	{
		private IBotFactory botFactory;
		
		public WaitMessageSagaFactory(IBotFactory botFactory)
		{
			this.botFactory = botFactory;
		}

		public string SagaKind => nameof(WaitMessageSaga);

		public ISaga CreateSaga()
		{
			return new WaitMessageSaga(botFactory);
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
			var factory = new WaitMessageSagaFactory(plugin.TelegramBotFactory);
			mgr.RegisterSagaFactory(factory, WaitMessageSaga.GetHandledTypes());
		}
	}
}

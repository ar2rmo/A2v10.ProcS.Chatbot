using System;
using System.Threading.Tasks;
using A2v10.ProcS.Infrastructure;
using BotCore;

namespace A2v10.ProcS.Chatbot
{
	public class SendMessageSaga : SagaBaseDispatched<Guid, SendMessageMessage>
	{
		private IBot bot;

		public SendMessageSaga(IBot bot) : base(nameof(SendMessageSaga))
		{
			this.bot = bot;
		}

		protected override Task Handle(IHandleContext context, SendMessageMessage message)
		{
			//bot.SendMessageAsync(message)
			return Task.CompletedTask;
		}
	}

	public class SendMessageSagaFactory : ISagaFactory
	{
		private Lazy<IBot> botProvider;
		
		public SendMessageSagaFactory(Lazy<IBot> botProvider)
		{
			this.botProvider = botProvider;
		}

		public string SagaKind => nameof(SendMessageSaga);

		public ISaga CreateSaga()
		{
			return new SendMessageSaga(botProvider.Value);
		}
	}

	public class SendMessageSagaRegistrar : ISagaRegistrar
	{
		private IBot CreateBot()
		{
			var cfg = new BotCore.Types.Base.Configure();
			return new BotCore.Telegram.ActiveTelegramBot(cfg);
		}

		public void Register(ISagaManager mgr, IServiceProvider provider)
		{
			var factory = new SendMessageSagaFactory(new Lazy<IBot>(CreateBot, true));
			mgr.RegisterSagaFactory(factory, SendMessageSaga.GetHandledTypes());
		}
	}
}

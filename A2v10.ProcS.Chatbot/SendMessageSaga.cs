using System;
using System.Threading.Tasks;
using A2v10.ProcS.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace A2v10.ProcS.Chatbot
{
	public class SendMessageSaga : SagaBaseDispatched<Guid, SendMessageMessage>
	{
		public const string ukey = Plugin.Name + ":" + nameof(SendMessageSaga);

		private readonly BotManager botManager;

		public override IDynamicObject Store(IResourceWrapper wrapper)
		{
			var store = new DynamicObject();
			return store;
		}

		public override void Restore(IDynamicObject store, IResourceWrapper wrapper)
		{
			
		}

		internal SendMessageSaga(BotManager botManager) : base(ukey)
		{
			this.botManager = botManager;
		}

		protected override async Task Handle(IHandleContext context, SendMessageMessage message)
		{
			var bot = await botManager.GetBotAsync(message.BotEngine, message.BotKey);
			await bot.SendMessageAsync(new ChatSession(message.ChatId), message.Message.Message);
		}
	}

	internal class SendMessageSagaFactory : ISagaFactory
	{
		private readonly BotManager botManager;
		
		public SendMessageSagaFactory(BotManager botManager)
		{
			this.botManager = botManager;
		}

		public string SagaKind => SendMessageSaga.ukey;

		public ISaga CreateSaga()
		{
			return new SendMessageSaga(botManager);
		}
	}

	public class SendMessageSagaRegistrar : ISagaRegistrar
	{
		private readonly Plugin plugin;

		public SendMessageSagaRegistrar(Plugin plugin)
		{
			this.plugin = plugin;
		}

		public void Register(IResourceManager rmgr, ISagaManager smgr)
		{
			var factory = new SendMessageSagaFactory(plugin.BotManager);
			rmgr.RegisterResourceFactory(factory.SagaKind, new SagaResourceFactory(factory));
			rmgr.RegisterResources(SendMessageSaga.GetHandledTypes());
			smgr.RegisterSagaFactory(factory, SendMessageSaga.GetHandledTypes());
		}
	}
}

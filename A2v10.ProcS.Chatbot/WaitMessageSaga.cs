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
        public IIncomingMessage Message { get; set; }

		public IncomeMessage(Guid chatId) : base(chatId)
		{
		}
	}

	public class WaitMessageSaga : SagaBaseDispatched<Guid, WaitMessageMessage, IncomeMessage>
	{
		private BotManager botManager;

		
		private Boolean IsWaiting { get; set; }
		private Guid BookmarkId { get; set; }
		private BotEngine BotEngine { get; set; }
		private String BotKey { get; set; }

		internal WaitMessageSaga(BotManager botManager) : base(nameof(WaitMessageSaga))
		{
			IsWaiting = false;
			this.botManager = botManager;
		}

		protected override Task Handle(IHandleContext context, WaitMessageMessage message)
		{
			BookmarkId = message.BookmarkId;
			BotEngine = message.BotEngine;
			BotKey = message.BotKey;
			CorrelationId.Value = message.CorrelationId.Value;
			IsWaiting = true;
			return Task.CompletedTask;
		}

		protected override Task Handle(IHandleContext context, IncomeMessage message)
		{
			if (IsWaiting)
			{
				context.ResumeBookmark(BookmarkId, DynamicObject.From(message));
				IsComplete = true;
			}
			else
			{
				var m = new InitBotChatMessage(message.BotEngine, message.BotKey);
				m.ChatId = message.CorrelationId.Value;
				m.Message = message.Message;
				context.SendMessage(m);
				IsComplete = true;
			}
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

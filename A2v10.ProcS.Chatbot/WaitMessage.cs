using System;
using System.Threading.Tasks;
using A2v10.ProcS.Infrastructure;
using BotCore;

namespace A2v10.ProcS.Chatbot
{
	public class WaitMessage : IActivity
	{
		public WaitMessage()
		{

		}

		public BotEngine BotEngine { get; set; }
		public String BotKey { get; set; }
		public String ChatId { get; set; }

		public ActivityExecutionResult Execute(IExecuteContext context)
		{
			if (context.IsContinue) return ActivityExecutionResult.Complete;
			var book = context.SetBookmark();
			var ch = Guid.Parse(context.Resolve(ChatId));
			var mess = new WaitMessageMessage(ch);
			mess.BookmarkId = book;
			mess.BotEngine = BotEngine;
			mess.BotKey = BotKey;
			context.SendMessage(mess);
			return ActivityExecutionResult.Idle;
		}
	}

	public class WaitMessageMessage : MessageBase<Guid>
	{
        public Guid BookmarkId { get; set; }
        public BotEngine BotEngine { get; set; }
		public String BotKey { get; set; }

		public WaitMessageMessage(Guid chatId) : base(chatId)
		{
		}
	}
}

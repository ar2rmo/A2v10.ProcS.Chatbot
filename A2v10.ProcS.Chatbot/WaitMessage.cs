using System;
using System.Threading.Tasks;
using A2v10.ProcS.Infrastructure;
using BotCore;

namespace A2v10.ProcS.Chatbot
{
	public class WaitMessage : IWorkflowAction
	{
		public WaitMessage()
		{

		}

		public BotEngine BotEngine { get; set; }
		public String BotKey { get; set; }
		public String ChatId { get; set; }

		public async Task<ActionResult> Execute(IExecuteContext context)
		{
			var ch = Guid.Parse(context.Resolve(ChatId));
			await context.SaveInstance();
			var mess = new WaitMessageMessage(ch);
			mess.ProcessId = context.Instance.Id;
			mess.BotEngine = BotEngine;
			mess.BotKey = BotKey;
			context.SendMessage(mess);
			return ActionResult.Idle;
		}
	}

	public class WaitMessageMessage : MessageBase<Guid>
	{
        public Guid ProcessId { get; set; }
        public BotEngine BotEngine { get; set; }
		public String BotKey { get; set; }

		public WaitMessageMessage(Guid chatId) : base(chatId)
		{
		}
	}
}

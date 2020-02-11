using System;
using System.Threading.Tasks;
using A2v10.ProcS.Infrastructure;
using BotCore;

namespace A2v10.ProcS.Chatbot
{
	public class WaitMessage : IWorkflowAction
	{
		BotEngine BotEngine { get; set; }
		String BotKey { get; set; }
		Guid ChatId { get; set; }

		public async Task<ActionResult> Execute(IExecuteContext context)
		{
			await context.SaveInstance();
			var mess = new WaitMessageMessage(ChatId);
			mess.BotEngine = BotEngine;
			mess.BotKey = BotKey;
			context.SendMessage(mess);
			return ActionResult.Idle;
		}
	}

	public class WaitMessageMessage : MessageBase<Guid>
	{
		public BotEngine BotEngine { get; set; }
		public String BotKey { get; set; }

		public WaitMessageMessage(Guid chatId) : base(chatId)
		{
		}
	}
}

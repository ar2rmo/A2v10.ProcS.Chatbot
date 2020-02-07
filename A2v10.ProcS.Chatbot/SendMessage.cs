using System;
using System.Threading.Tasks;
using A2v10.ProcS.Infrastructure;
using BotCore;

namespace A2v10.ProcS.Chatbot
{
	public class SendMessage : IWorkflowAction
	{
		BotEngine BotEngine { get; set; }
		Guid ChatId { get; set; }
		OutgoingMessage Message { get; set; }

		public Task<ActionResult> Execute(IExecuteContext context)
		{
			var mess = new SendMessageMessage(Message);
			context.SendMessage(mess);
			return Task.FromResult(ActionResult.Success);
		}
	}

	public class SendMessageMessage : MessageBase<Guid>
	{
		BotEngine BotEngine { get; set; }

		Guid ChatId { get; set; }

		OutgoingMessage Message { get; set; }

		public SendMessageMessage(OutgoingMessage message) : base(Guid.NewGuid())
		{
			Message = message;
		}
	}
}

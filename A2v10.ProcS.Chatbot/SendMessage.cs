using System;
using System.Threading.Tasks;
using A2v10.ProcS.Interfaces;
using BotCore;

namespace A2v10.ProcS.Chatbot
{
	public class SendMessage : IWorkflowAction
	{
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
		OutgoingMessage Message { get; set; }

		public SendMessageMessage(OutgoingMessage message) : base(Guid.NewGuid())
		{
			Message = message;
		}
	}
}

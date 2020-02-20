using System;
using System.Threading.Tasks;
using A2v10.ProcS.Infrastructure;
using BotCore;

namespace A2v10.ProcS.Chatbot
{
	public class SendMessage : IActivity
	{
		public BotEngine BotEngine { get; set; }
		public String BotKey { get; set; }
		public String ChatId { get; set; }
		public OutgoingMessage Message { get; set; }

		public ActivityExecutionResult Execute(IExecuteContext context)
		{
			Message.Text = context.Resolve(Message.Text);
			var mess = new SendMessageMessage(Message);
			mess.BotEngine = BotEngine;
			mess.BotKey = BotKey;
			mess.ChatId = Guid.Parse(context.Resolve(ChatId));
			context.SendMessage(mess);
			return ActivityExecutionResult.Complete;
		}
	}

	public class SendMessageMessage : MessageBase<Guid>
	{
		public BotEngine BotEngine { get; set; }
		public String BotKey { get; set; }
		public Guid ChatId { get; set; }
		public OutgoingMessage Message { get; set; }

        [RestoreWith]
        public SendMessageMessage(Guid correlationId) : base(correlationId)
        {

        }
		public SendMessageMessage(OutgoingMessage message) : base(Guid.NewGuid())
		{
			Message = message;
		}
	}
}

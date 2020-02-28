using System;
using System.Threading.Tasks;
using A2v10.ProcS.Infrastructure;
using BotCore;

namespace A2v10.ProcS.Chatbot
{

	[ResourceKey(Plugin.Name + ":" + nameof(SendMessageActivity))]
	public class SendMessageActivity : IActivity
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

	[ResourceKey(ukey)]
	public class SendMessageMessage : MessageBase<Guid>
	{
		public const string ukey = Plugin.Name + ":" + nameof(SendMessageMessage);
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

		public override void Store(IDynamicObject storage, IResourceWrapper wrapper)
		{
			storage.Set("correlationId", CorrelationId.Value);
			storage.Set("botEngine", BotEngine.ToString());
			storage.Set("botKey", BotKey);
			storage.Set("chatId", ChatId);
			storage.Set("message", DynamicObjectConverters.From(Message));
		}

		public override void Restore(IDynamicObject store, IResourceWrapper wrapper)
		{
			BotEngine = store.Get<BotEngine>("botEngine");
			BotKey = store.Get<String>("botKey");
			ChatId = store.Get<Guid>("chatId");
			Message = store.GetDynamicObject("message").To<OutgoingMessage>();
		}
	}
}

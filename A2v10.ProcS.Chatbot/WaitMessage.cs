using System;
using System.Threading.Tasks;
using A2v10.ProcS.Infrastructure;
using BotCore;

namespace A2v10.ProcS.Chatbot
{
	[ResourceKey(Plugin.Name + ":" + nameof(WaitMessageActivity))]
	public class WaitMessageActivity : IActivity
	{
		public WaitMessageActivity()
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
			var mess = new WaitMessageMessage(ch)
			{
				BookmarkId = book,
				BotEngine = BotEngine,
				BotKey = BotKey
			};
			context.SendMessage(mess);
			return ActivityExecutionResult.Idle;
		}
	}

	[ResourceKey(ukey)]
	public class WaitMessageMessage : MessageBase<Guid>
	{
		public const string ukey = Plugin.Name + ":" + nameof(WaitMessageMessage);
		public Guid BookmarkId { get; set; }
		public BotEngine BotEngine { get; set; }
		public String BotKey { get; set; }

		[RestoreWith]
		public WaitMessageMessage(Guid chatId) : base(chatId)
		{
		}

		public override void Store(IDynamicObject storage, IResourceWrapper wrapper)
		{
			storage.Set("chatId", CorrelationId.Value);
			storage.Set("bookmarkId", BookmarkId);
			storage.Set("botEngine", BotEngine.ToString());
			storage.Set("botKey", BotKey);
		}

		public override void Restore(IDynamicObject store, IResourceWrapper wrapper)
		{
			BookmarkId = store.Get<Guid>("bookmarkId");
			BotEngine = store.Get<BotEngine>("botEngine");
			BotKey = store.Get<String>("botKey");
		}
	}
}

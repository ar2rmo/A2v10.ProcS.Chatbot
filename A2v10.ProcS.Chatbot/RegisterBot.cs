using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using A2v10.ProcS.Infrastructure;
using BotCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace A2v10.ProcS.Chatbot
{
	[ResourceKey(Plugin.Name + ":" + nameof(RegisterBotProcessingActivity))]
	public class RegisterBotProcessingActivity : IActivity
	{
		public BotEngine BotEngine { get; set; }
		public String BotKey { get; set; }
		public String ChatProcessIdentity { get; set; }

		public ActivityExecutionResult Execute(IExecuteContext context)
		{
			var mess = new RegisterBotProcessingMessage(BotEngine, BotKey);
			mess.ChatProcessIdentity = ChatProcessIdentity;
			context.SendMessage(mess);
			return ActivityExecutionResult.Complete;
		}
	}
}

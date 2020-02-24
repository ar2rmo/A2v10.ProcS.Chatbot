using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Text;
using System.Threading.Tasks;
using A2v10.ProcS.Infrastructure;
using BotCore;
using System.Threading;

namespace A2v10.ProcS.Chatbot
{
	public class MockingBot : IBot
	{
		public Task InitAsync()
		{
			return Task.CompletedTask;
		}

		public Task ProcessIncomingMessageAsync(String json, IMessageProcessor proc)
		{
			return Task.CompletedTask;
		}

		public Task SendMessageAsync(IChatSession sess, IOutgoingMessage msg)
		{
			return Task.CompletedTask;
		}
	}
}

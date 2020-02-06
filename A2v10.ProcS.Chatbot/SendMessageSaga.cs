using System;
using System.Threading.Tasks;
using A2v10.ProcS.Interfaces;
using BotCore;

namespace A2v10.ProcS.Chatbot
{
	public class SendMessageSaga : SagaBaseDispatched<Guid, SendMessageMessage>
	{
		public SendMessageSaga() : base(nameof(SendMessageSaga))
		{

		}

		protected override Task Handle(IHandleContext context, SendMessageMessage message)
		{
			throw new NotImplementedException();
		}
	}
}

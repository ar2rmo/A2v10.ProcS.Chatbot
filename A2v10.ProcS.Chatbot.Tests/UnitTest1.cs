using A2v10.ProcS.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace A2v10.ProcS.Chatbot.Tests
{
	[TestClass]
	public class ChatbotTest1
	{
		[TestMethod]
		public async Task RunWorkflow()
		{
			var nm = new NotifyManager();

			var configuration = new ConfigurationBuilder().Build();

			var scriptEngine = new ScriptEngine();

			var lg = new A2v10.ProcS.Tests.FakeLogger();
			
			var epm = new EndpointManager();

			var sp = new Services(epm);

			var rm = new ResourceManager(sp);

			var storage = new A2v10.ProcS.Tests.FakeStorage(rm, "../../../workflows/");

			var repository = new Repository(storage, storage, configuration);

			var mgr = new SagaManager(sp);

			var pmr = new PluginManager(sp);

			String pluginPath = ChatbotTests.GetPluginPath();

			ProcS.RegisterSagas(rm, mgr, scriptEngine, repository);
			ProcS.RegisterActivities(rm);

			pmr.LoadPlugins(pluginPath, configuration);


			pmr.RegisterResources(rm, mgr);


			var taskManager = new SyncTaskManager();
			var keeper = new InMemorySagaKeeper(mgr.Resolver);
			var bus = new ServiceBus(taskManager, keeper, lg, nm);
			var engine = new WorkflowEngine(repository, bus, scriptEngine, lg, nm);
			sp.Add(bus);
			sp.Add(engine);

			var param = new DynamicObject
			{
				["ChatId"] = "0c3af6d2-0000-0000-d2f6-3a0c00000000"
			};
			IInstance inst = await engine.StartWorkflow(new Identity("ChatBotTest.json"), param);

			await bus.Process();

			var plug = pmr.GetPlugin<Plugin>(Plugin.Name);
			await plug.GetBotAsync(BotEngine.Mocking, "TestBot");

			var m1json = msg1.Replace('\'', '"');
			var m1do = DynamicObjectConverters.FromJson(m1json);
			var m1 = rm.Unwrap<IMessage>(m1do);
			bus.Send(m1);

			await bus.Process();

			var ni = await repository.Get(inst.Id);

			Assert.AreEqual("WaitForName", ni.CurrentState);
		}

		private const string msg1 = @"
				{
					'$res': 'com.a2.procs.chatbot:IncomeMessage',
					'chatId': '0c3af6d2-0000-0000-d2f6-3a0c00000000',
					'botEngine': 'Mocking',
					'botKey': 'TestBot',
					'message': {
						'Type': 'text',
						'Text': '/start',
						'User': {
							'Id': '205190866',
							'Username': 'artur_moshkola',
							'FirstName': 'Artur',
							'LastName': 'Moshkola'
						}
					}
				}
		";
	}
}

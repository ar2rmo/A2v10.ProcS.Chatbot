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
		public class Services : IServiceProvider
		{
			private List<Object> services;

			public Services(params Object[] svcs)
			{
				services = new List<object>(svcs);
			}

            public void Add(Object obj)
            {
				services.Add(obj);
			}

			public object GetService(Type serviceType)
			{
				foreach (var s in services)
				{
					if (serviceType.IsAssignableFrom(s.GetType())) return s;
				}
				return null;
			}
		}

		[TestMethod]
        public async Task RunWorkflow()
        {
			var epm = new EndpointManager();

			var sp = new Services(epm);

			var storage = new ProcS.Tests.FakeStorage("../../../workflows/");
			var mgr = new SagaManager(sp);
			
			String pluginPath = GetPluginPath();

			var configuration = new ConfigurationBuilder().Build();

			mgr.LoadPlugins(pluginPath, configuration);

			var taskManager = new SyncTaskManager();
			var keeper = new InMemorySagaKeeper(mgr.Resolver);
			var scriptEngine = new ScriptEngine();
			var repository = new Repository(storage, storage);
			var bus = new ServiceBus(taskManager, keeper, repository, scriptEngine);
			var engine = new WorkflowEngine(repository, bus, scriptEngine);
			sp.Add(bus);
			sp.Add(engine);

			IInstance inst = await engine.StartWorkflow(new Identity("ChatBotExample.json"));
			
			await bus.Run(bus.CancelWhenEmpty.Token);

			Assert.AreEqual("End", inst.CurrentState);
		}

		static String GetPluginPath()
		{
			var path = new Uri(Assembly.GetExecutingAssembly().CodeBase).LocalPath;
			var pathes = path.Split(Path.DirectorySeparatorChar);
			var debugRelease = pathes[^3];
			var newPathes = pathes.Take(pathes.Length - 5).ToList();
			newPathes.Add($"A2v10.ProcS.Chatbot");
			newPathes.Add($"bin");
			newPathes.Add(debugRelease);
			newPathes.Add("netstandard2.0");
			return (newPathes[0] == "" ? new string(new char[] { Path.DirectorySeparatorChar }) : "") + Path.Combine(newPathes.ToArray());
		}
	}
}

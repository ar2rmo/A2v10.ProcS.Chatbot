using A2v10.ProcS.Infrastructure;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
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
			var storage = new ProcS.Tests.FakeStorage("../../../workflows/");
			var mgr = new SagaManager();
			
			String pluginPath = GetPluginPath();

			mgr.LoadPlugins(pluginPath);

			var keeper = new InMemorySagaKeeper(mgr);
			var scriptEngine = new ScriptEngine();
			var repository = new Repository(storage, storage);
			var bus = new ServiceBus(keeper, repository, scriptEngine);
			var engine = new WorkflowEngine(repository, bus, scriptEngine);

			IInstance inst = await engine.StartWorkflow(new Identity("ChatBotExample.json"));
			
			await bus.Run();

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

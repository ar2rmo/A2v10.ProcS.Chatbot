// Copyright © 2020 Alex Kukhtin, Artur Moshkola. All rights reserved.

using System;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Threading.Tasks;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using A2v10.ProcS.Infrastructure;
using System.Linq;
using A2v10.ProcS.Tests.StoreRestore;
using DynamicObject = A2v10.ProcS.Infrastructure.DynamicObject;
using Newtonsoft.Json.Linq;

using BaseClass = A2v10.ProcS.Tests.StoreRestore.StoreRestore;
using Microsoft.Extensions.Configuration;
using BotCore.Types.Enums;
using BotCore;

namespace A2v10.ProcS.Chatbot.Tests
{

	[TestClass]
	public class StoreRestore
	{

		[TestMethod]
		public void StoreRestore1()
		{
			var epm = new EndpointManager();

			var sp = new Services(epm);

			var frm = new FakeResourceManager();
			var mgr2 = new SagaManager(sp);

			var rm = new ResourceManager(sp);
			var mgr = new SagaManager(sp);
			var pmr = new PluginManager(sp);

			String pluginPath = ChatbotTests.GetPluginPath();

			var configuration = new ConfigurationBuilder().Build();

			//ProcS.RegisterSagas(rm, mgr);
			//ProcS.RegisterActivities(rm);

			//ProcS.RegisterSagas(frm, mgr2);
			//ProcS.RegisterActivities(frm);

			pmr.LoadPlugins(pluginPath, configuration);
			pmr.RegisterResources(rm, mgr);
			pmr.RegisterResources(frm, mgr2);



			var impl = new Dictionary<Type, Type>
			{
				{ typeof(IIncomingMessage), typeof(BotIncMess) },
				{ typeof(IKeyboard), null }
			};
			//impl.Add(typeof(BotCore.IButton), typeof(Button));

			BaseClass.TestRegistred(rm, frm.TheList, impl);
		}
	}

	internal class BotIncMess : BotCore.IIncomingMessage
	{
		public MessageInType Type { get; set; }
		public User User { get; set; }

		public Location Location { get; set; }

		public string Text { get; set; }
	}
}

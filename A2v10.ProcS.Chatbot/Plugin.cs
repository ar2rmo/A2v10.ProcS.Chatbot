using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;
using A2v10.ProcS.Infrastructure;

[assembly: ProcSPlugin(A2v10.ProcS.Chatbot.Plugin.Name, typeof(A2v10.ProcS.Chatbot.Plugin))]

namespace A2v10.ProcS.Chatbot
{
	public class Plugin : IPlugin
	{
		public const string Name = "com.a2.procs.chatbot";

		internal BotManager BotManager { get; private set; }

		public void Init(IServiceProvider provider, IConfiguration configuration)
		{
			BotManager = new BotManager(configuration.GetSection("ChatBots"));

			var epm = provider.GetService<IEndpointManager>();
			var bus = provider.GetService<IServiceBus>();

			epm.RegisterEndpoint("telegram", new EndpointHandlerFactory(bus, BotEngine.Telegram, BotManager));
		}

		public async Task<BotCore.IBot> GetBotAsync(BotEngine engine, String key)
		{
			return await BotManager.GetBotAsync(engine, key);
		}
	}
}

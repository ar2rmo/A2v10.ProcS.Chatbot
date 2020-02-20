using A2v10.ProcS.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;

[assembly: ProcSPlugin(typeof(A2v10.ProcS.Chatbot.Plugin))]

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
    }
}

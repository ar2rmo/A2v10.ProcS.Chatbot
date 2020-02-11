using A2v10.ProcS.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;

[assembly: ProcSPlugin(typeof(A2v10.ProcS.Chatbot.Plugin))]

namespace A2v10.ProcS.Chatbot
{
    public class Plugin : IPlugin
    {
        internal TelegramBotFactory TelegramBotFactory { get; private set; }

        public void Init(IServiceProvider provider, IConfiguration configuration)
        {
            var epm = provider.GetService<IEndpointManager>();

            var tbf = new TelegramBotFactory(configuration);
            var tephf = new EndpointHandlerFactory(tbf);

            epm.RegisterEndpoint("telegram", tephf);

            TelegramBotFactory = tbf;
        }
    }
}

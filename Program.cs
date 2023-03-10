using Autofac;
using OAHouseChatGpt.Services.ChatGpt;
using OAHouseChatGpt.Services.OADiscord;
using OAHouseChatGpt.Services.Configuration;
using Microsoft.Extensions.Configuration;

namespace OAHouseChatGpt
{
    public class Program
    {
        public static async Task Main()
        {
            var config = new ConfigurationBuilder()
                .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                .AddJsonFile("appsettings.local.json")
                .Build();

            var builder = new ContainerBuilder();
            builder.RegisterType<ChatGptService>().As<IChatGpt>();
            builder.RegisterType<OADiscordService>().As<IOaDiscord>();
            builder.Register((c, p) =>
            {
                return new oAHouseChatGptConfigurationService(
                    config.GetConnectionString("DiscordToken"),
                    config.GetConnectionString("OpenAiApiKey"),
                    config.GetConnectionString("DiscordBotId"));
            }).As<IOAHouseChatGptConfiguration>();
            var container = builder.Build();

            using (var scope = container.BeginLifetimeScope())
            {
                var discordService = scope.Resolve<IOaDiscord>();
                await discordService.Start();
            }
        }
    }
}


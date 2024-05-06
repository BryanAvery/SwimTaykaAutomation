using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.WindowsAzure.Storage.Shared.Protocol;

[assembly: FunctionsStartup(typeof(SwimTaykaStreak.Startup))]

namespace SwimTaykaStreak
{
    public class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            builder.Services.AddSingleton<IStreakClient, StreakClient>();
            builder.Services.AddSingleton<IJustGivingScrape, JustGivingScrape>();
        }

        public override void ConfigureAppConfiguration(IFunctionsConfigurationBuilder builder)
        {
            FunctionsHostBuilderContext context = builder.GetContext();

            builder.ConfigurationBuilder
                .SetBasePath(context.ApplicationRootPath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{context.EnvironmentName}.json", optional: true, reloadOnChange: true)
                .AddEnvironmentVariables(); // Ensuring environment variables are still considered
        }
    }
}

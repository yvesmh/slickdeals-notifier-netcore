using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SlickdealsNotifier.Business;
using SlickdealsNotifier.Data;
using SlickdealsNotifier.Notification;
using SlickdealsNotifier.Scraping;

namespace SlickdealsNotifier
{
    class Program
    {
        public static async Task Main(string[] args)
        {
            var configurationRoot = new ConfigurationBuilder()
                .SetBasePath(Directory.GetParent(AppContext.BaseDirectory).FullName)
                .AddJsonFile("appsettings.json", false)
                .Build();

            var appConfiguration = configurationRoot.Get<ApplicationConfiguration>();

            var serviceProvider = new ServiceCollection()
                .AddLogging(builder => builder.AddConsole().SetMinimumLevel(LogLevel.Debug))
                .AddSingleton<ISlickDealsNotifierBusiness, SlickDealsNotifierBusiness>()
                .AddSingleton<IHtmlContentLoader, HtmlContentLoader>()
                .AddSingleton<IHtmlContentParser, HtmlContentParser>()
                .AddSingleton<IDealDataAccess, DealDataAccess>()
                // maybe in the future if more types of notifications are added
                // use factory to decide which implementation to register in DI
                .AddSingleton<IDealNotifier, SendGridEmailNotifier>()
                .BuildServiceProvider();

            var logger = serviceProvider.GetService<ILoggerFactory>()
                .CreateLogger<Program>();

            logger.LogDebug("Starting application");

            var business = serviceProvider.GetService<ISlickDealsNotifierBusiness>();
            await business.NotifyNewDeals(appConfiguration);

            logger.LogDebug("Completed scraping and notifying");
        }

    }
}

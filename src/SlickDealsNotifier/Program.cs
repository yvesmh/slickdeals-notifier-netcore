using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog;
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

            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .WriteTo.Console()
                .WriteTo.File("log-.txt", rollingInterval: RollingInterval.Day)
                .CreateLogger();

            var serviceProvider = new ServiceCollection()
                .AddLogging( configure => configure.AddSerilog())
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

            var stopwatch = Stopwatch.StartNew();

            logger.LogDebug("Starting application");

            var business = serviceProvider.GetService<ISlickDealsNotifierBusiness>();
            await business.NotifyNewDeals(appConfiguration);
            stopwatch.Stop();
            logger.LogDebug($"Completed scraping and notifying in {stopwatch.Elapsed}");
        }

    }
}

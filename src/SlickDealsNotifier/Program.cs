using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SlickdealsNotifier.Business;
using SlickdealsNotifier.Data;
using SlickdealsNotifier.Scraping;

namespace SlickdealsNotifier
{
    class Program
    {
        public static async Task Main(string[] args)
        {
            var serviceProvider = new ServiceCollection()
                .AddLogging(builder => builder.AddConsole().SetMinimumLevel(LogLevel.Debug))
                .AddSingleton<ISlickDealsNotifierBusiness, SlickDealsNotifierBusiness>()
                .AddSingleton<IHtmlContentLoader, HtmlContentLoader>()
                .AddSingleton<IHtmlContentParser, HtmlContentParser>()
                .AddSingleton<IDealDataAccess, DealDataAccess>()
                .BuildServiceProvider();

            var logger = serviceProvider.GetService<ILoggerFactory>()
                .CreateLogger<Program>();

            logger.LogDebug("Starting application");

            var business = serviceProvider.GetService<ISlickDealsNotifierBusiness>();
            await business.NotifyNewDeals();

            logger.LogDebug("Completed scraping and notifying");
        }

    }
}

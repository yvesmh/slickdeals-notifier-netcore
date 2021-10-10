using Microsoft.Extensions.Logging;
using SlickdealsNotifier.Data;
using SlickdealsNotifier.Notification;
using SlickdealsNotifier.Scraping;
using System.Linq;
using System.Threading.Tasks;

namespace SlickdealsNotifier.Business
{
    public class SlickDealsNotifierBusiness : ISlickDealsNotifierBusiness
    {
        private readonly ILogger<SlickDealsNotifierBusiness> _logger;
        private readonly IDealDataAccess _dealDataAccess;
        private readonly IHtmlContentLoader _htmlContentLoader;
        private readonly IHtmlContentParser _htmlContentParser;
        private readonly IDealNotifier _dealNotifier;

        public SlickDealsNotifierBusiness(
            ILogger<SlickDealsNotifierBusiness> logger,
            IDealDataAccess dealDataAccess,
            IHtmlContentLoader htmlContentLoader,
            IHtmlContentParser htmlContentParser,
            IDealNotifier dealNotifier)
        {
            _logger = logger;
            _dealDataAccess = dealDataAccess;
            _htmlContentLoader = htmlContentLoader;
            _htmlContentParser = htmlContentParser;
            _dealNotifier = dealNotifier;
        }

        public async Task NotifyNewDeals(ApplicationConfiguration applicationConfiguration)
        {
            var htmlContent = await _htmlContentLoader.Load();
            var deals = _htmlContentParser.Parse(htmlContent);

            // TODO refactor to detect good deals with different criteria
            var bestDeals = deals.Where(d => d.Votes > 100);

            foreach (var deal in bestDeals)
            {
                var isDealNew = await _dealDataAccess.IsDealNew(deal);
                if (isDealNew)
                {
                    _logger.LogDebug($"Deal with title {deal.Title} at price {deal.Price} found. Attempting to notify");

                    var notificationSuccessful = await _dealNotifier.Notify(deal, applicationConfiguration);

                    // dont save the deal to the DB in case email notification failed due to 
                    // having exceeded the free quota, if the deal is still alive next day, we can still notify
                    if (notificationSuccessful)
                    {
                        await _dealDataAccess.SaveDeal(deal);
                        _logger.LogInformation($"Successfully notified and saved deal {deal}");
                    }
                    
                }
                else
                {
                    _logger.LogDebug($"Deal was found but is not new, skipping notification. Deal: {deal}");
                }
            }

        }
    }
}

using Microsoft.Extensions.Logging;
using SlickdealsNotifier.Data;
using SlickdealsNotifier.Scraping;
using System.Linq;
using System.Threading.Tasks;

namespace SlickdealsNotifier.Business
{
    class SlickDealsNotifierBusiness : ISlickDealsNotifierBusiness
    {
        private readonly ILogger<SlickDealsNotifierBusiness> _logger;
        private readonly IDealDataAccess _dealDataAccess;
        private readonly IHtmlContentLoader _htmlContentLoader;
        private readonly IHtmlContentParser _htmlContentParser;

        public SlickDealsNotifierBusiness(
            ILogger<SlickDealsNotifierBusiness> logger,
            IDealDataAccess dealDataAccess,
            IHtmlContentLoader htmlContentLoader,
            IHtmlContentParser htmlContentParser)
        {
            _logger = logger;
            _dealDataAccess = dealDataAccess;
            _htmlContentLoader = htmlContentLoader;
            _htmlContentParser = htmlContentParser;
        }

        public async Task NotifyNewDeals()
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
                    _logger.LogDebug($"Deal with title {deal.Title} at price {deal.Price} found");
                    await _dealDataAccess.SaveDeal(deal);
                }
            }

        }
    }
}

using System;
using System.Linq;
using System.Threading.Tasks;
using SlickdealsNotifier.Data;
using SlickdealsNotifier.Scraping;

namespace SlickdealsNotifier
{
    class Program
    {
        public static async Task Main(string[] args)
        {
            var htmlContent = await new HtmlContentLoader().Load();
            var parser = new HtmlContentParser();
            var deals = parser.Parse(htmlContent);

            // TODO refactor to detect good deals with different criteria
            var bestDeals = deals.Where(d => d.Votes > 100);
            var dealDataAccess = new DealDataAccess();

            foreach (var deal in bestDeals)
            {
                var isDealNew = await dealDataAccess.IsDealNew(deal);
                if (isDealNew)
                {
                    Console.WriteLine($"Deal with title {deal.Title} at price {deal.Price} found");
                    await dealDataAccess.SaveDeal(deal);
                }
            }
        }

    }
}

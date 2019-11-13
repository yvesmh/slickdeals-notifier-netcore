using System;
using System.Threading.Tasks;
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

            foreach (var deal in deals)
            {
                Console.WriteLine($"Deal with title {deal.Title} at price {deal.Price} found");
            }
        }
    }
}

using System.Threading.Tasks;
using HtmlAgilityPack;

namespace SlickdealsNotifier.Scraping
{
    public class HtmlContentLoader
    {
        private const string Url = "https://slickdeals.net/";

        public async Task<HtmlDocument> Load()
        {
            var web = new HtmlWeb();
            return await web.LoadFromWebAsync(Url);
        }
    }
}
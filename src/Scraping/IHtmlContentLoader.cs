using HtmlAgilityPack;
using System.Threading.Tasks;

namespace SlickdealsNotifier.Scraping
{
    interface IHtmlContentLoader
    {
        Task<HtmlDocument> Load();
    }
}

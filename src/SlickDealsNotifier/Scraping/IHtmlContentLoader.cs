using HtmlAgilityPack;
using System.Threading.Tasks;

namespace SlickdealsNotifier.Scraping
{
    public interface IHtmlContentLoader
    {
        Task<HtmlDocument> Load();
    }
}

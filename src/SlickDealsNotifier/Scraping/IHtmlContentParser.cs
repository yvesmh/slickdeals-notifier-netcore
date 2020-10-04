using HtmlAgilityPack;
using SlickdealsNotifier.Models;
using System.Collections.Generic;

namespace SlickdealsNotifier.Scraping
{
    public interface IHtmlContentParser
    {
        IReadOnlyCollection<Deal> Parse(HtmlDocument document);
    }
}

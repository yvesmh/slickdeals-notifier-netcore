using HtmlAgilityPack;
using SlickdealsNotifier.Models;
using System.Collections.Generic;

namespace SlickdealsNotifier.Scraping
{
    interface IHtmlContentParser
    {
        IReadOnlyCollection<Deal> Parse(HtmlDocument document);
    }
}

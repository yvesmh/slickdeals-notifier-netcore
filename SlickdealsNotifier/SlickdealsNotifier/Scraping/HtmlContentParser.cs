using System;
using System.Collections.Generic;
using System.Linq;
using HtmlAgilityPack;
using HtmlAgilityPack.CssSelectors.NetCore;
using SlickdealsNotifier.Models;

namespace SlickdealsNotifier.Scraping
{
    public class HtmlContentParser
    {
        public IReadOnlyCollection<Deal> Parse(HtmlDocument document)
        {
            var nodes = document.DocumentNode
                .QuerySelectorAll(".fpGridBox .grid .frontpage");

            var deals = nodes
                .Select(ParseNode)
                .Where(deal => deal != null)
                .ToList();

            return deals.AsReadOnly();
        }

        private Deal ParseNode(HtmlNode node)
        {
            try
            {
                var titleHtmlNode = node.QuerySelector(".itemTitle");

                var title = titleHtmlNode.Attributes["title"].Value;
                var url = titleHtmlNode.Attributes["href"].Value;

                var store = node.QuerySelector(".itemStore").InnerText;
                
                var price = node.QuerySelector(".itemPrice").InnerText;

                var votesString = node.QuerySelector(".count").InnerText;
                int.TryParse(votesString, out var votes);

                var isFire = node.QuerySelectorAll(".fire .icon .icon-fire")
                                 .Count > 0;

                return new Deal
                {
                    Title = title,
                    Url = url,
                    IsFire = isFire,
                    Price = price,
                    Store = store,
                    Votes = votes
                };
            }
            // sometimes slickdeals has some special deals where their html looks different, handle
            // gracefully by allowing to process other deals if one fails
            catch (Exception e)
            {
                //TODO serilog
                Console.WriteLine($"Error parsing node. {e.Message} {e.StackTrace}");
                return null;
            }
        }
    }
}
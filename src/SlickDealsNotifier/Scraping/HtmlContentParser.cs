using System;
using System.Collections.Generic;
using System.Linq;
using HtmlAgilityPack;
using HtmlAgilityPack.CssSelectors.NetCore;
using SlickdealsNotifier.Models;

namespace SlickdealsNotifier.Scraping
{
    public class HtmlContentParser : IHtmlContentParser
    {
        public IReadOnlyCollection<Deal> Parse(HtmlDocument document)
        {
            var nodes = document.DocumentNode
                .QuerySelectorAll(".dealTiles.gridDeals");

            var frontpageDealsDiv = document.QuerySelectorAll(".gridCategory")
                .FirstOrDefault(x => x.GetAttributeValue("data-module-name", string.Empty) == "Frontpage Slickdeals");

            var dealsList = frontpageDealsDiv.QuerySelector(".dealTiles")
                .Descendants("li");

            var deals = dealsList
                .Select(ParseNode)
                .Where(deal => deal != null)
                .ToList();

            return deals.AsReadOnly();
        }

        private Deal ParseNode(HtmlNode node)
        {
            try
            {
                // <a href="{url}">{Title}</a>
                var titleHtmlNode = node.QuerySelector(".bp-c-card_title");

                var title = titleHtmlNode.InnerText;
                var url = titleHtmlNode.GetAttributeValue("href", string.Empty);

                // <button>{Store}</button>
                var store = node.QuerySelector(".bp-p-storeLink").InnerText;
                
                // <span>{price}</span>
                var price = node.QuerySelector(".bp-p-dealCard_price").InnerText;

                // <span>{Votes}</span>
                var votesString = node.QuerySelector(".bp-p-votingThumbsPopup_voteCount").InnerText;
                int.TryParse(votesString, out var votes);

                // <span class="..."></span> -- this one is only present when the deal is on fire
                var isFire = node.QuerySelectorAll(".bp-c-icon bp-i-fire")
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
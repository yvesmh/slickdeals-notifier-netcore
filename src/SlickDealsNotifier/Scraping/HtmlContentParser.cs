using System;
using System.Collections.Generic;
using System.Linq;
using HtmlAgilityPack;
using HtmlAgilityPack.CssSelectors.NetCore;
using Microsoft.Extensions.Logging;
using SlickdealsNotifier.Models;

namespace SlickdealsNotifier.Scraping
{
    public class HtmlContentParser : IHtmlContentParser
    {
        private readonly ILogger<HtmlContentParser> _logger;

        public HtmlContentParser(
            ILogger<HtmlContentParser> logger)
        {
            _logger = logger;
        }

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

            _logger.LogInformation($"Successfully parsed {deals.Count} deals out of {dealsList.Count()} nodes");

            return deals.AsReadOnly();
        }

        private Deal ParseNode(HtmlNode node)
        {
            // declare outside of try scope so catch block can log
            // any info we had up until the exceptoin was thrown, for easier troubleshooting of edge cases
            var deal = new Deal();

            try
            {
                // <a href="{url}">{Title}</a>
                var titleHtmlNode = node.QuerySelector(".bp-c-card_title");

                // sometimes Slickdeals puts ads or featured promos that look like deals but don't have titles
                // ignore those and avoid catch
                if (titleHtmlNode == null)
                {
                    _logger.LogDebug($"Found a node with empty title. Skip parsing. Node InnerText: {node.InnerText}");
                    return null;
                }

                deal.Title =titleHtmlNode.InnerText;
                deal.Url = titleHtmlNode.GetAttributeValue("href", string.Empty);

                // <button>{Store}</button>
                deal.Store = node.QuerySelector(".bp-p-storeLink").InnerText;
                
                // <span>{price}</span>
                deal.Price = node.QuerySelector(".bp-p-dealCard_price").InnerText;

                // <span>{Votes}</span>
                var votesString = node.QuerySelector(".bp-p-votingThumbsPopup_voteCount").InnerText;
                int.TryParse(votesString, out var votes);
                deal.Votes = votes;

                // <span class="..."></span> -- this one is only present when the deal is on fire
                deal.IsFire = node.QuerySelectorAll(".bp-c-icon bp-i-fire")
                                 .Count > 0;

                return deal;

            }

            // sometimes slickdeals has some special deals where their html looks different, handle
            // gracefully by allowing to process other deals if one fails
            catch (Exception e)
            {
                _logger.LogError($"Error parsing deal node. Deal up to exception thrown is {deal}. Exception: {e.Message}{Environment.NewLine} {e.StackTrace}");
                return null;
            }
        }
    }
}
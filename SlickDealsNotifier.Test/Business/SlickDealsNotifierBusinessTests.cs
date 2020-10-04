using HtmlAgilityPack;
using Microsoft.Extensions.Logging;
using Moq;
using SlickdealsNotifier.Business;
using SlickdealsNotifier.Data;
using SlickdealsNotifier.Models;
using SlickdealsNotifier.Scraping;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace SlickDealsNotifier.Test.Business
{
    public class SlickDealsNotifierBusinessTests
    {
        // SUT
        private readonly SlickDealsNotifierBusiness _busines;

        private readonly Mock<ILogger<SlickDealsNotifierBusiness>> _logger;
        private readonly Mock<IDealDataAccess> _dealDataAccess;
        private readonly Mock<IHtmlContentLoader> _htmlContentLoader;
        private readonly Mock<IHtmlContentParser> _htmlContentParser;

        public SlickDealsNotifierBusinessTests()
        {
            _logger = new Mock<ILogger<SlickDealsNotifierBusiness>>();
            _dealDataAccess = new Mock<IDealDataAccess>();
            _htmlContentLoader = new Mock<IHtmlContentLoader>();
            _htmlContentParser = new Mock<IHtmlContentParser>();

            _busines = new SlickDealsNotifierBusiness(
                _logger.Object,
                _dealDataAccess.Object,
                _htmlContentLoader.Object,
                _htmlContentParser.Object);
        }

        [Fact]
        public async Task NotifyNewDeals_WhenThereAreNoDeals_DoesNothing()
        {
            // arrange
            SetupContentLoader();
            SetupContentParser(Enumerable.Empty<Deal>());

            // act
            await _busines.NotifyNewDeals();

            // assert
            _dealDataAccess.Verify(x =>
                    x.IsDealNew(It.IsAny<Deal>()),
                Times.Never);

        }

        [Fact]
        public async Task NotifyNewDeals_WhenDealsFoundButNotOver100Votes_DoesNothing()
        {
            var deals = new[]
            {
                new Deal
                {
                    Votes = 99
                },
                new Deal
                {
                    Votes = 47
                }
            };
            // arrange
            SetupContentLoader();
            SetupContentParser(deals);

            // act
            await _busines.NotifyNewDeals();

            // assert
            _dealDataAccess.Verify(x =>
                    x.IsDealNew(It.IsAny<Deal>()),
                Times.Never);
        }

        [Fact]
        public async Task NotifyNewDeals_WhenMultipleDealsFound_OnlyNotifiesDealsOver100Votes()
        {
            var deals = new[]
            {
                new Deal
                {
                    Votes = 101,
                    Title = "Free Stuff man"
                },
                new Deal
                {
                    Votes = 47,
                    Title = "This deal isn't that great"
                }
            };

            // arrange
            SetupContentLoader();
            SetupContentParser(deals);
            SetupIsDealNewToAlwaysReturn(true);

            // act
            await _busines.NotifyNewDeals();

            // assert
            // should only be called once, only with the deal with 101 votes
            _dealDataAccess.Verify(x =>
                    x.SaveDeal(It.Is<Deal>(d => d.Votes == 101)),
                Times.Once);
        }

        [Fact]
        public async Task NotifyNewDeals_WhenMultipleDealsWithOver100VotesFound_OnlyNotifiesNewDeals()
        {
            var deals = new[]
            {
                new Deal
                {
                    Votes = 101,
                    Title = "Free Stuff man",
                },
                new Deal
                {
                    Votes = 9001,
                    Title = "Power level (already in sqlite)"
                }
            };

            // arrange
            SetupContentLoader();
            SetupContentParser(deals);

            // 1 deal is new, 1 isn't 
            _dealDataAccess.SetupSequence(x => x.IsDealNew(
                It.IsAny<Deal>()))
                .ReturnsAsync(true)
                .ReturnsAsync(false);

            // act
            await _busines.NotifyNewDeals();

            // assert
            // should only be called once, only with the deal with 101 votes
            _dealDataAccess.Verify(x =>
                    x.SaveDeal(It.Is<Deal>(d => d.Votes == 101)),
                Times.Once);
        }


        private void SetupContentLoader()
        {
            _htmlContentLoader.Setup(x => x.Load())
                .ReturnsAsync(new HtmlDocument());
        }

        private void SetupContentParser(IEnumerable<Deal> dealsToReturn)
        {
            var asReadOnly = dealsToReturn.ToList().AsReadOnly();
            _htmlContentParser.Setup(x =>
                x.Parse(It.IsAny<HtmlDocument>()))
                .Returns(asReadOnly);

        }

        private void SetupIsDealNewToAlwaysReturn(bool valueToReturn)
        {
            _dealDataAccess.Setup(x => x.IsDealNew(It.IsAny<Deal>()))
                .ReturnsAsync(valueToReturn);
        }

    }
}

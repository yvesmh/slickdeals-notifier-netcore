using System.Threading.Tasks;

namespace SlickdealsNotifier.Business
{
    public interface ISlickDealsNotifierBusiness
    {
        Task NotifyNewDeals(ApplicationConfiguration applicationConfiguration);
    }
}

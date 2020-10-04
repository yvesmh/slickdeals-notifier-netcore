using SlickdealsNotifier.Models;
using System.Threading.Tasks;

namespace SlickdealsNotifier.Data
{
    interface IDealDataAccess
    {
        Task<bool> IsDealNew(Deal deal);
        Task SaveDeal(Deal deal);
    }
}

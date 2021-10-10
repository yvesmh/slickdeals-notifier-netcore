using SlickdealsNotifier.Models;
using System.Threading.Tasks;

namespace SlickdealsNotifier.Notification
{
    public interface IDealNotifier
    {
        Task<bool> Notify(Deal deal, ApplicationConfiguration applicationConfiguration);
    }
}

using Microsoft.EntityFrameworkCore;
using SlickdealsNotifier.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SlickdealsNotifier.Data
{
    class DealDataAccess : IDealDataAccess
    {
        public async Task<bool> IsDealNew(Deal deal)
        {
            using (var db = new DealContext())
            {
                // for now use Url as unique identifier. There are probably better ways to detect uniqueness even scraping
                return !await db.Deals.AnyAsync(d => d.Url == deal.Url);
            }
        }

        public async Task<List<Deal>> GetAllDeals()
        {
            using (var db = new DealContext())
            {
                return await db.Deals.AsQueryable().ToListAsync();
            }
        }

        public async Task SaveDeal(Deal deal)
        {
            using (var db = new DealContext())
            {
                db.Add(deal);
                await db.SaveChangesAsync();
            }
        }
    }
}

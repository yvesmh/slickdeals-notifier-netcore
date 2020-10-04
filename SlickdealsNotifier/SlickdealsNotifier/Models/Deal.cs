using Microsoft.EntityFrameworkCore;
using System;

namespace SlickdealsNotifier.Models
{
    
    public class DealContext: DbContext
    {
        public DbSet<Deal> Deals { get; set; }

        private static bool _created = false;
        public DealContext()
        {
            if (_created) return;
            
            Database.EnsureCreated();
            _created = true;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder options) =>
            options.UseSqlite("Data Source=slick.db");

    }

    public class Deal
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Store { get; set; }
        public string Price { get; set; }
        public int Votes { get; set; }
        public bool IsFire { get; set; }
        public string Url { get; set; }
    }
}
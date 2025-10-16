using Microsoft.EntityFrameworkCore;
using MyEmailWebApi.Models;

namespace MyEmailWebApi.Data
{
    public class KpiContext(DbContextOptions<KpiContext> options) : DbContext(options)
    {
        public DbSet<Company> Companies { get; set; }
        public DbSet<MarketingKpi> MarketingKpis { get; set; }
    }
}

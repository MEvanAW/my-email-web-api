using Microsoft.AspNetCore.Identity;
using MyEmailWebApi.Constants;
using MyEmailWebApi.Data;
using MyEmailWebApi.Models;

namespace MyEmailWebApi
{
    public class Seeder
    {
        private readonly UserContext _userContext;
        private readonly KpiContext _kpiContext;

        public Seeder(UserContext userContext, KpiContext kpiContext)
        {
            _userContext = userContext;
            _kpiContext = kpiContext;
        }

        public Task SeedRoles()
        {
            var expectedRoleNames = new string[] { RoleNames.EMPLOYEE, RoleNames.MANAGER, RoleNames.ADMIN };
            var dbRoleNames = _userContext.Roles.Where((role) => expectedRoleNames.Contains(role.Name)).Select((role) => role.Name).ToArray();
            bool anyAddition = false;
            if (!dbRoleNames.Any((role) => role == RoleNames.EMPLOYEE))
            {
                _userContext.Add(new IdentityRole
                {
                    Name = RoleNames.EMPLOYEE,
                    NormalizedName = RoleNames.EMPLOYEE.ToUpperInvariant()
                });
                anyAddition = true;
            }
            if (!dbRoleNames.Any((role) => role == RoleNames.MANAGER))
            {
                _userContext.Add(new IdentityRole
                {
                    Name = RoleNames.MANAGER,
                    NormalizedName = RoleNames.MANAGER.ToUpperInvariant()
                });
                anyAddition = true;
            }
            if (!dbRoleNames.Any((role) => role == RoleNames.ADMIN))
            {
                _userContext.Add(new IdentityRole
                {
                    Name = RoleNames.ADMIN,
                    NormalizedName = RoleNames.ADMIN.ToUpperInvariant()
                });
                anyAddition = true;
            }
            if (anyAddition)
            {
                return _userContext.SaveChangesAsync();
            }
            return Task.CompletedTask;
        }

        public async Task SeedKpis()
        {
            var geekGarden = "Geek Garden";
            var indomaret = "Indomaret";
            var expectedCompanyNames = new string[] { geekGarden, indomaret };
            var expectedCompanyGuid = new Guid[] { Guid.NewGuid(), Guid.NewGuid() };
            var dbCompanyNames = _kpiContext.Companies.Where((company) => expectedCompanyNames.Contains(company.CompanyName)).Select((company) => company.CompanyName).ToArray();
            bool anyAddition = false;
            for (int i = 0; i < expectedCompanyNames.Length; ++i)
            {
                if (!dbCompanyNames.Any((name) => name.ToUpperInvariant() != expectedCompanyNames[i].ToUpperInvariant()))
                {
                    _kpiContext.Add(new Company
                    {
                        ID = expectedCompanyGuid[i],
                        CompanyName = expectedCompanyNames[i]
                    });
                    anyAddition = true;
                }
            }
            var runningTask = Task.CompletedTask;
            if (anyAddition)
            {
                runningTask = _kpiContext.SaveChangesAsync();
                var random = new Random();
                // Assume KPIs needs to be seeded too if Companies needs to be seeded
                var expectedKpis = new MarketingKpi[]
                {
                new() { CompanyID = expectedCompanyGuid[0], Date = new DateOnly(2024, 11, 1), Impressions = 148263, Clicks = 1210, Leads = 13, Orders = 1 },
                new() { CompanyID = expectedCompanyGuid[1], Date = new DateOnly(2024, 11, 1), Impressions = 220688, Clicks = 1640, Leads = 48, Orders = 3 },
                new() { CompanyID = expectedCompanyGuid[0], Date = new DateOnly(2024, 12, 1), Impressions = 22850, Clicks = 457, Leads = 9, Orders = 1 },
                new() { CompanyID = expectedCompanyGuid[1], Date = new DateOnly(2024, 12, 1), Impressions = 147038, Clicks = 1196, Leads = 24, Orders = 1 },
                new() { CompanyID = expectedCompanyGuid[0], Date = new DateOnly(2025, 1, 1), Impressions = random.Next(667, 420000000), Clicks = random.Next(20, 61200), Leads = random.Next(0, 1678), Orders = random.Next(0,369) },
                new() { CompanyID = expectedCompanyGuid[1], Date = new DateOnly(2025, 1, 1), Impressions = random.Next(667, 420000000), Clicks = random.Next(20, 61200), Leads = random.Next(0, 1678), Orders = random.Next(0,369) },
                new() { CompanyID = expectedCompanyGuid[0], Date = new DateOnly(2025, 2, 1), Impressions = random.Next(667, 420000000), Clicks = random.Next(20, 61200), Leads = random.Next(0, 1678), Orders = random.Next(0,369) },
                new() { CompanyID = expectedCompanyGuid[1], Date = new DateOnly(2025, 2, 1), Impressions = random.Next(667, 420000000), Clicks = random.Next(20, 61200), Leads = random.Next(0, 1678), Orders = random.Next(0,369) },
                new() { CompanyID = expectedCompanyGuid[0], Date = new DateOnly(2025, 3, 1), Impressions = random.Next(667, 420000000), Clicks = random.Next(20, 61200), Leads = random.Next(0, 1678), Orders = random.Next(0,369) },
                new() { CompanyID = expectedCompanyGuid[1], Date = new DateOnly(2025, 3, 1), Impressions = random.Next(667, 420000000), Clicks = random.Next(20, 61200), Leads = random.Next(0, 1678), Orders = random.Next(0,369) },
                new() { CompanyID = expectedCompanyGuid[0], Date = new DateOnly(2025, 4, 1), Impressions = random.Next(667, 420000000), Clicks = random.Next(20, 61200), Leads = random.Next(0, 1678), Orders = random.Next(0,369) },
                new() { CompanyID = expectedCompanyGuid[1], Date = new DateOnly(2025, 4, 1), Impressions = random.Next(667, 420000000), Clicks = random.Next(20, 61200), Leads = random.Next(0, 1678), Orders = random.Next(0,369) },
                new() { CompanyID = expectedCompanyGuid[0], Date = new DateOnly(2025, 5, 1), Impressions = random.Next(667, 420000000), Clicks = random.Next(20, 61200), Leads = random.Next(0, 1678), Orders = random.Next(0,369) },
                new() { CompanyID = expectedCompanyGuid[1], Date = new DateOnly(2025, 5, 1), Impressions = random.Next(667, 420000000), Clicks = random.Next(20, 61200), Leads = random.Next(0, 1678), Orders = random.Next(0,369) },
                new() { CompanyID = expectedCompanyGuid[0], Date = new DateOnly(2025, 6, 1), Impressions = random.Next(667, 420000000), Clicks = random.Next(20, 61200), Leads = random.Next(0, 1678), Orders = random.Next(0,369) },
                new() { CompanyID = expectedCompanyGuid[1], Date = new DateOnly(2025, 6, 1), Impressions = random.Next(667, 420000000), Clicks = random.Next(20, 61200), Leads = random.Next(0, 1678), Orders = random.Next(0,369) },
                new() { CompanyID = expectedCompanyGuid[0], Date = new DateOnly(2025, 7, 1), Impressions = random.Next(667, 420000000), Clicks = random.Next(20, 61200), Leads = random.Next(0, 1678), Orders = random.Next(0,369) },
                new() { CompanyID = expectedCompanyGuid[1], Date = new DateOnly(2025, 7, 1), Impressions = random.Next(667, 420000000), Clicks = random.Next(20, 61200), Leads = random.Next(0, 1678), Orders = random.Next(0,369) },
                new() { CompanyID = expectedCompanyGuid[0], Date = new DateOnly(2025, 8, 1), Impressions = random.Next(667, 420000000), Clicks = random.Next(20, 61200), Leads = random.Next(0, 1678), Orders = random.Next(0,369) },
                new() { CompanyID = expectedCompanyGuid[1], Date = new DateOnly(2025, 8, 1), Impressions = random.Next(667, 420000000), Clicks = random.Next(20, 61200), Leads = random.Next(0, 1678), Orders = random.Next(0,369) },
                new() { CompanyID = expectedCompanyGuid[0], Date = new DateOnly(2025, 9, 1), Impressions = random.Next(667, 420000000), Clicks = random.Next(20, 61200), Leads = random.Next(0, 1678), Orders = random.Next(0,369) },
                new() { CompanyID = expectedCompanyGuid[1], Date = new DateOnly(2025, 9, 1), Impressions = random.Next(667, 420000000), Clicks = random.Next(20, 61200), Leads = random.Next(0, 1678), Orders = random.Next(0,369) },
                new() { CompanyID = expectedCompanyGuid[0], Date = new DateOnly(2025, 10, 1), Impressions = random.Next(667, 420000000), Clicks = random.Next(20, 61200), Leads = random.Next(0, 1678), Orders = random.Next(0,369) },
                new() { CompanyID = expectedCompanyGuid[1], Date = new DateOnly(2025, 10, 1), Impressions = random.Next(667, 420000000), Clicks = random.Next(20, 61200), Leads = random.Next(0, 1678), Orders = random.Next(0,369) },
                };
                _kpiContext.MarketingKpis.AddRange(expectedKpis);
                await runningTask;
                runningTask = _kpiContext.SaveChangesAsync();
            }
            await runningTask;
        }
    }
}

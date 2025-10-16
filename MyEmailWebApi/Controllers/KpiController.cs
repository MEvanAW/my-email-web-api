using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyEmailWebApi.Data;
using MyEmailWebApi.DTO.KpiDto;

namespace MyEmailWebApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class KpiController(KpiContext kpiContext) : ControllerBase
    {
        private readonly KpiContext _kpiContext = kpiContext;

        [HttpGet("kpi-summary", Name = "KpiSummary")]
        public async Task<IActionResult> KpiSummary()
        {
            // Aggregating average via queryable is possible,
            // but as I hope I can visualize the data in a simple front end later,
            // I opted to aggregate average in the controller action instead.
            var companiesTask = _kpiContext.Companies.Include((company) => company.MarketingKpis).ToArrayAsync();
            var responseList = new List<CompanyKpiSummary>();
            var companies = await companiesTask;
            foreach (var company in companies)
            {
                decimal impressionSum = 0;
                decimal clicksSum = 0;
                decimal leadsSum = 0;
                decimal ordersSum = 0;
                foreach (var kpi in company.MarketingKpis)
                {
                    impressionSum += kpi.Impressions;
                    clicksSum += kpi.Clicks;
                    leadsSum += kpi.Leads;
                    ordersSum += kpi.Orders;
                }
                responseList.Add(new()
                {
                    CompanyName = company.CompanyName,
                    ImpressionsAverage = impressionSum / company.MarketingKpis.Count,
                    ClicksAverage = clicksSum / company.MarketingKpis.Count,
                    LeadsAverage = leadsSum / company.MarketingKpis.Count,
                    OrdersAverage = ordersSum / company.MarketingKpis.Count,
                    MonthlyMarketingKpi = company.MarketingKpis.OrderBy((kpi) => kpi.Date).Select((kpi) => new MarketingKpiDto
                    {
                        Date = kpi.Date,
                        Impressions = kpi.Impressions,
                        Clicks = kpi.Clicks,
                        Leads = kpi.Leads,
                        Orders = kpi.Orders
                    })
                });
            }

            return Ok(responseList);
        }
    }
}

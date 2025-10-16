namespace MyEmailWebApi.DTO.KpiDto
{
    public class CompanyKpiSummary
    {
        public string? CompanyName { get; set; }
        public decimal ImpressionsAverage { get; set; }
        public decimal ClicksAverage { get; set; }
        public decimal LeadsAverage { get; set; }
        public decimal OrdersAverage { get; set; }

        public IEnumerable<MarketingKpiDto> MonthlyMarketingKpi { get; set; } = [];
    }

    public class MarketingKpiDto
    {
        public DateOnly Date { get; set; }

        public int Impressions { get; set; }

        public int Clicks { get; set; }

        public int Leads { get; set; }

        public int Orders { get; set; }
    }
}

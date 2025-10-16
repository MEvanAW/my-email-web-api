using System.ComponentModel.DataAnnotations;

namespace MyEmailWebApi.Models
{
    public class Company
    {
        public Guid ID { get; set; }

        [Required]
        public string? CompanyName { get; set; }

        public ICollection<MarketingKpi> MarketingKpis { get; set; }
    }
}

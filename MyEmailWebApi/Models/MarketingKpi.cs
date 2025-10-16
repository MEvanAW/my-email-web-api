using System.ComponentModel.DataAnnotations;

namespace MyEmailWebApi.Models
{
    public class MarketingKpi
    {
        public Guid ID { get; set; }

        public Guid CompanyID { get; set; }

        [Required]
        public DateOnly Date { get; set; }

        public int Impressions { get; set; }

        public int Clicks { get; set; }

        public int Leads { get; set; }

        public int Orders { get; set; }

        public Company Company { get; set; }
    }
}

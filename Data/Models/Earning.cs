using System.ComponentModel.DataAnnotations;

namespace JPFData.Models
{
    public class Earning
    {
        [Key]
        public int Id { get; set; }

        [DataType(DataType.Currency)]
        public decimal? Rate { get; set; }

        public double? Hours { get; set; }

        public double? Units { get; set; }

        [DataType(DataType.Currency)]
        public decimal? Amount { get; set; }

        [Display(Name = "Year-To-Date Hours")]
        public double? YTDHours { get; set; }

        [Display(Name = "Year-To-Date Units")]
        public double? YTDUnits { get; set; }

        [Display(Name = "Year-To-Date Amount")]
        public decimal? YTDAmount { get; set; }
    }
}
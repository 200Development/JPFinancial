using System.ComponentModel.DataAnnotations;

namespace JPFData.Models.JPFinancial
{
    public class Benefit
    {
        [Key]
        public int Id { get; set; }

        [Required, Display(Name = "Benefit")]
        public string Name { get; set; }

        [Required, DataType(DataType.Currency), Display(Name = "Amount")]
        public decimal Amount { get; set; }

        [DataType(DataType.Currency), Display(Name = "Year-To-Date")]
        public decimal? YTDAmount { get; set; }

        [Required, Display(Name = "Before-Tax")]
        public bool Taxable { get; set; }
    }
}
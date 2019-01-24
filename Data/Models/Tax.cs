using System.ComponentModel.DataAnnotations;

namespace JPFData.Models
{
    public class Tax
    {
        [Key]
        public int Id { get; set; }

        [Required, Display(Name = "Tax")]
        public TaxesEnum Name { get; set; }

        [Required, DataType(DataType.Currency), Display(Name = "Amount")]
        public decimal Amount { get; set; }

        [DataType(DataType.Currency), Display(Name = "Year-To-Date")]
        public decimal? YTDAmount { get; set; }
    }
}
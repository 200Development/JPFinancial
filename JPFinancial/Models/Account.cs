using JPFinancial.Models.Interfaces;
using System.ComponentModel.DataAnnotations;

namespace JPFinancial.Models
{
    public class Account : IAccount
    {
        public Account()
        {
            Balance = decimal.Zero;
            Goal = decimal.Zero;
        }

        [Key]
        public int Id { get; set; }

        [Required, StringLength(255)]
        public string Name { get; set; }

        [Required, DataType(DataType.Currency)]
        public decimal Balance { get; set; }

        [DataType(DataType.Currency)]
        public decimal? Goal { get; set; }

        [DataType(DataType.Currency), Display(Name = "Required Savings")]
        public decimal? RequiredSavings { get; set; }

        [DataType(DataType.Currency), Display(Name = "Surplus/Deficit")]
        public decimal? BalanceSurplus { get; set; }
    }
}
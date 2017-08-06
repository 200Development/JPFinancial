using JPFinancial.Models.Interfaces;
using System.ComponentModel.DataAnnotations;

namespace JPFinancial.Models
{
    public class SavingsAccount : IAccount
    {
        [Key]
        public int Id { get; set; }

        [Required, StringLength(255)]
        public string Name { get; set; }

        [Required, DataType(DataType.Currency)]
        public decimal Balance { get; set; }

        [Display(Name = "Interest Rate")]
        public decimal InterestRate { get; set; }

        [DataType(DataType.Currency), Display(Name = "Account Maintenance Fee")]
        public decimal MaintenanceFee { get; set; }

        [DataType(DataType.Currency), Display(Name = "Overdraft Fee")]
        public decimal OneTimeOverdraftFee { get; set; }

        [DataType(DataType.Currency), Display(Name = "Daily Overdraft Charge")]
        public decimal DailyOverdraftFee { get; set; }

        [Display(Name = "Days Before Daily Overdraft Charge")]
        public int DaysUntilDailyOverdraftFee { get; set; }
    }
}
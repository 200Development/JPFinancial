using System.ComponentModel.DataAnnotations;
using JPFData.Enumerations;

namespace JPFData.ViewModels
{
    public class CreateSalaryViewModel
    {
        [Required,Display(Name = "Payee")]
        public string Payee { get; set; }

        [Required, Display(Name = "Pay Type")]
        public PayTypesEnum PayTypesEnum { get; set; }

        [Required, Display(Name = "Pay Frequency")]
        public FrequencyEnum PayFrequency { get; set; }

        [Display(Name = "Gross Pay"), DataType(DataType.Currency)]
        public decimal? GrossPay { get; set; }

        [Display(Name = "Net Income"), DataType(DataType.Currency)]
        public decimal? NetIncome { get; set; }

        public string Expense { get; set; }

        [Display(Name = "Cost")]
        public decimal? ExpenseAmount { get; set; }

        public string Benefit { get; set; }

        [Display(Name = "Amount")]
        public decimal? BenefitAmount { get; set; }

        [Display(Name = "Payday of Week")]
        public DayEnum PaydayOfWeek { get; set; }

        [Display(Name = "1st Payday")]
        public string FirstPayday { get; set; }

        [Display(Name = "Last Payday")]
        public string LastPayday { get; set; }

        public DaysOfMonthEnum Paydays { get; set; }
    }
}
using System.ComponentModel.DataAnnotations;
using JPFData.Enumerations;

namespace JPFData.Models.JPFinancial
{
    public class Salary
    {
        [Key]
        public int Id { get; set; }

        [Required, Display(Name = "Payee")]
        public string Payee { get; set; }

        [Required, Display(Name = "Pay Type")]
        public PayTypesEnum PayTypesEnum { get; set; }

        [Required, Display(Name = "Pay Frequency")]
        public FrequencyEnum PayFrequency { get; set; }

        [Display(Name = "1st Payday")]
        public string FirstPayday { get; set; }

        [Display(Name = "Last Payday")]
        public string LastPayday { get; set; }

        [Display(Name = "Gross Pay"), DataType(DataType.Currency)]
        public decimal? GrossPay { get; set; }

        [Display(Name = "Net Income"), DataType(DataType.Currency)]
        public decimal? NetIncome { get; set; }

        public DayEnum PaydayOfWeek { get; set; }
    }
}


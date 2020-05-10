using System.ComponentModel.DataAnnotations;

namespace JPFData.Enumerations
{
    public enum FrequencyEnum
    {
        Daily = 1,
        Weekly = 2,
        [Display(Name = "Every 2 Weeks")]
        BiWeekly = 3,
        Monthly = 4,
        [Display(Name = "Twice a Month")]
        SemiMonthly = 5,
        Quarterly = 6,
        [Display(Name = "Semi-Annually")]
        SemiAnnually = 7,
        Annually = 8
    }
}
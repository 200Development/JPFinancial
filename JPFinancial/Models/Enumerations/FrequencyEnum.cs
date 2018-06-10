using Newtonsoft.Json;

namespace JPFinancial.Models.Enumerations
{
    public enum FrequencyEnum
    {
        Daily = 1,
        Weekly = 2,
        [JsonProperty(PropertyName = "Every 2 Weeks")]
        BiWeekly = 3,
        Monthly = 4,
        [JsonProperty(PropertyName = "Twice a Month")]
        SemiMonthly = 5,
        Quarterly = 6,
        [JsonProperty(PropertyName = "Semi-Annually")]
        SemiAnnually = 7,
        Annually = 8
    }
}
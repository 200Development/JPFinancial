using Newtonsoft.Json;

namespace JPFinancial.Models.Enumerations
{
    public enum FrequencyEnum
    {
        Daily = 1,
        Weekly = 2,
        [JsonProperty(PropertyName = "Bi-Weekly")]
        BiWeekly = 3,
        [JsonProperty(PropertyName = "Semi-Monthly")]
        SemiMonthly = 4,
        Monthly = 5,
        [JsonProperty(PropertyName = "Twice a Month")]
        BiMonthly = 6,
        Quarterly = 7,
        [JsonProperty(PropertyName = "Semi-Annually")]
        SemiAnnually = 8,
        Annually = 9
    }
}
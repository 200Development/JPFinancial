using Newtonsoft.Json;

namespace JPFinancial.Models.Enumerations
{
    public enum FrequencyEnum
    {
        Daily = 1,
        Weekly = 2,
        [JsonProperty(PropertyName = "Bi-Weekly")]
        BiWeekly = 3,
        Monthly = 4,
        [JsonProperty(PropertyName = "Twice a Month")]
        BiMonthly = 5,
        Quarterly = 6,
        [JsonProperty(PropertyName = "Semi-Annually")]
        SemiAnnually = 7,
        Annually = 8
    }
}
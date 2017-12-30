using Newtonsoft.Json;

namespace JPFinancial.Models.Enumerations
{
    public enum IncomeCategoriesEnum
    {
        Bonus,
        Dividend,
        Dental,
        Medical,
        Vision,
        Interest,
        [JsonProperty(PropertyName = "Net Salary")]
        NetSalary,
        Other,
        [JsonProperty(PropertyName = "Profit Sharing")]
        ProfitSharing,
        [JsonProperty(PropertyName = "Holiday Pay")]
        HolidayPay,
        [JsonProperty(PropertyName = "Sick Pay")]
        SickPay,
        [JsonProperty(PropertyName = "Vacation Pay")]
        VacationPay,
        [JsonProperty(PropertyName = "Flex Account Contribution")]
        FlexContribution,
        [JsonProperty(PropertyName = "Dependent Care Contribution")]
        DependentCareContribution
    }
}
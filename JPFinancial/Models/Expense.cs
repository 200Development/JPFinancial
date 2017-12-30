using JPFinancial.Models.Interfaces;
using Newtonsoft.Json;

namespace JPFinancial.Models
{
    public class Expense : ISalaryLineItem
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public decimal Amount { get; set; }
        public int SalaryId { get; set; }
        [JsonProperty(PropertyName = "Pre-Taxed?")]
        public bool IsPreTax { get; set; }
    }
}
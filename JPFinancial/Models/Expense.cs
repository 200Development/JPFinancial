using JPFinancial.Models.Interfaces;

namespace JPFinancial.Models
{
    public class Expense : ISalaryLineItem
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public decimal Amount { get; set; }
        public int SalaryId { get; set; }
        public bool IsPreTax { get; set; }
    }
}
using JPFinancial.Models.Interfaces;

namespace JPFinancial.Models
{
    public class CreditCard : IAccount
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public decimal Balance { get; set; }
        public decimal PrincipalBalance { get; set; }
        public decimal CompoundedInterest { get; set; }
        public int EndOfCycleDay { get; set; }
        public int DueDayOfMonth { get; set; }
        public decimal? APR { get; set; }
    }
}
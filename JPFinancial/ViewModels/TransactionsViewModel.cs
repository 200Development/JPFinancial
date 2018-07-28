using System.Collections.Generic;
using JPFData;
using JPFData.Models;
using JPFinancial.Models;

namespace JPFinancial.ViewModels
{
    public class TransactionsViewModel
    {
        public IEnumerable<Transaction> Transactions { get; set; }
        public decimal SpentSincePayday { get; set; }
        public decimal MonthlySpending { get; set; }
        public decimal TotalAccountBalances { get; set; }
    }
}
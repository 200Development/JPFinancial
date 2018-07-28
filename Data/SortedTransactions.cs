using System.Collections.Generic;

namespace JPFData
{
    public class SortedTransactions
    {
        public Dictionary<string,decimal> BalanceByMonth { get; set; }
        public Dictionary<string,decimal> LastYearBalanceByMonth { get; set; }
        public Dictionary<string,decimal> BalanceByQuarter { get; set; }
        public Dictionary<string,decimal> LastYearBalanceByQuarter { get; set; }
        public Dictionary<string,decimal> BalanceByYear { get; set; }
        public Dictionary<string,decimal> LastYearBalanceByYear { get; set; }
        public Dictionary<string,decimal> BalanceByType { get; set; }
        public Dictionary<string,decimal> LastYearBalanceByType { get; set; }
        public Dictionary<string,decimal> BalanceByCategory { get; set; }
        public Dictionary<string,decimal> LastYearBalanceByCategory { get; set; }
    }
}
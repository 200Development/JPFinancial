using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using JPFData.Models.JPFinancial;

namespace JPFData
{
    public class AccountRebalanceReport
    {
        public AccountRebalanceReport()
        {
            NewReport = false;
            Surplus = 0.0m;;
            Deficit = 0.0m;;
            TotalSurplus = 0.0m;;
            AccountsWithSurplus = new List<Account>();
            AccountsWithDeficit = new List<Account>();
        }

        public bool NewReport { get; set; }

        [DataType(DataType.Currency)]
        public decimal Surplus { get; set; }

        [DataType(DataType.Currency)]
        public decimal Deficit { get; set; }

        [DataType(DataType.Currency)]
        public decimal TotalSurplus { get; set; }

        [DataType(DataType.Currency)]
        public decimal PaycheckSurplus { get; set; }

        public List<Account> AccountsWithSurplus { get; set; }
        public List<Account> AccountsWithDeficit { get; set; }
    }
}
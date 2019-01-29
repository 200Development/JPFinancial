﻿using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using JPFData.Models;

namespace JPFData
{
    public class AccountRebalanceReport
    {
        public AccountRebalanceReport()
        {
            newReport = false;
            Surplus = decimal.Zero;
            Deficit = decimal.Zero;
            TotalSurplus = decimal.Zero;
            AccountsWithSurplus = new List<Account>();
            AccountsWithDeficit = new List<Account>();
        }

        public bool newReport { get; set; }

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
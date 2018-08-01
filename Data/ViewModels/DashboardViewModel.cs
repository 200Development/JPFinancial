using System;
using System.Collections.Generic;
using Base;
using JPFData.DTO;
using JPFData.Models;
using BaseViewModel = JPFData.Base.BaseViewModel;

namespace JPFData.ViewModels
{
    public class DashboardViewModel : BaseViewModel
    {
        public DashboardViewModel()
        {

        }

        public string SelectedFVType { get; set; }
        public string CurrentMonth { get; set; }

        public List<Account> Accounts { get; set; }
        public List<Transaction> Transactions { get; set; }
        public List<Transaction> TopTransactions { get; set; }
        public Transaction Transaction { get; set; }
        public TransactionViewModel CreateTransaction { get; set; }
        public LoanViewModel LoanViewModel { get; set; }
        public List<Dictionary<DateTime, LoanViewModel>> LoanViewModelByMonth { get; set; }
        public StaticFinancialMetrics StaticFinancialMetrics { get; set; }
        public TimePeriodFinancialMetrics TimePeriodFinancialMetrics { get; set; }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using JPFData.DTO;
using JPFData.Models;
using JPFData.ViewModels;


namespace JPFData.Managers
{
    public class DashboardManager
    {
        private readonly ApplicationDbContext _db = new ApplicationDbContext();

        public DashboardManager()
        {
            ValidationErrors = new List<KeyValuePair<string, string>>();
        }

        public List<KeyValuePair<string, string>> ValidationErrors { get; set; }


        public DashboardDTO Get()
        {
            return Get(new DashboardDTO());
        }

        public DashboardDTO Get(DashboardDTO entity)
        {
            DashboardDTO ret = new DashboardDTO();

            ret = RefreshDashboardDTO();

            //Search
            //if (!string.IsNullOrEmpty(entity.Transaction.Id.ToString()))
            //{
            //    ret.Transactions = ret.Transactions.FindAll(
            //        t => t.Payee.ToLower().StartsWith(entity.Transaction.Payee,
            //            StringComparison.CurrentCultureIgnoreCase));
            //}

            return ret;
        }

        private DashboardDTO RefreshDashboardDTO()
        {
            DashboardDTO ret = new DashboardDTO();

            //ret.Transaction = new Transaction();
            //ret.CreateTransaction = new CreateTransactionViewModel();
            //ret.FinancialMetrics = new StaticFinancialMetrics();
            //ret.TimePeriodMetrics = new TimePeriodFinancialMetrics();
            //ret.Transactions = new List<Transaction>();

            ret.Transactions = _db.Transactions.ToList();

            return ret;
        }
    }
}

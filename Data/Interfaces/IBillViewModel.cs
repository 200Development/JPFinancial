using System.Collections.Generic;
using JPFData.Metrics;
using JPFData.Models.JPFinancial;
using PagedList;

namespace JPFData.Interfaces
{
    public interface IBillViewModel
    {
        Bill Bill { get; set; }
        List<Bill> Bills { get; set; }
        IPagedList<Bill> PagedBills { get; set; }
        List<Expense> Expenses { get; set; }
        IPagedList<Expense> PagedExpenses { get; set; }
        List<Account> Accounts { get; set; }
        BillMetrics Metrics { get; set; }
    }
}
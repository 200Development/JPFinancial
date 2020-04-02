using System.Collections.Generic;
using JPFData.Interfaces;
using JPFData.Metrics;
using JPFData.Models.JPFinancial;
using PagedList;

namespace JPFData.ViewModels
{
    public class BillViewModel : IBillViewModel
    {
        public BillViewModel()
        {
            Bill = new Bill();
            Bills = new List<Bill>();
            Expenses = new List<Expense>();
            Metrics = new BillMetrics();
        }

        public Bill Bill { get; set; }
        public List<Bill> Bills { get; set; }
        public IPagedList<Bill> PagedBills { get; set; }
        public List<Expense> Expenses { get; set; }
        public IPagedList<Expense> PagedExpenses { get; set; }
        public List<Account> Accounts { get; set; }
        public BillMetrics Metrics { get; set; }
    }

    public class PagedBillsList : PagedList<Bill>
    {
        public PagedBillsList() : base(new List<Bill>(), 1, 1)
        {

        }
    }

    public class PagedExpensesList : PagedList<Expense>
    {
        public PagedExpensesList() : base(new List<Expense>(), 1, 1)
        {

        }
    }
}
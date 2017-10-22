using JPFinancial.Models;
using JPFinancial.ViewModels;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web.Mvc;

namespace JPFinancial.Controllers
{
    public class DashboardController : Controller
    {
        private readonly ApplicationDbContext _db = new ApplicationDbContext();
        private readonly Calculations _calculations = new Calculations();

        // GET: Dashboard
        public ActionResult Index()
        {
            var viewModel = new DashboardViewModel();
            
            viewModel.CurrentMonth = DateTime.Today.ToString("MMMM", CultureInfo.CurrentCulture);
            viewModel.OneMonthSavings = _calculations.CalculateFv(DateTime.Today.AddMonths(1), 1828.44m).ToString("C", CultureInfo.CurrentCulture);
            viewModel.ThreeMonthsSavings = _calculations.CalculateFv(DateTime.Today.AddMonths(3), 1828.44m).ToString("C", CultureInfo.CurrentCulture);
            viewModel.SixMonthsSavings = _calculations.CalculateFv(DateTime.Today.AddMonths(6), 1828.44m).ToString("C", CultureInfo.CurrentCulture);
            viewModel.OneYearSavings = _calculations.CalculateFv(DateTime.Today.AddYears(1), 1828.44m).ToString("C", CultureInfo.CurrentCulture);

            var savingsAccountBalances = new Dictionary<string, decimal>();
            var bills = (from b in _db.Bills select b).ToList();
            var accounts = (from a in _db.Accounts select a).ToList();
            var firstDayOfMonth = Calculations.GetFirstDayOfMonth(DateTime.Today.Year, DateTime.Today.Month);
            var lastDayOfMonth = Calculations.GetLastDayOfMonth(DateTime.Today);
            var firstPaycheck = new DateTime(DateTime.Today.Year,DateTime.Today.Month,15);  //ToDo: make dynamic
            var lastPaycheck = Calculations.GetLastDayOfMonth(DateTime.Today);
            var billsDue = _calculations.GetBillsDue(firstPaycheck, firstDayOfMonth, new DateTime(DateTime.Today.Year, DateTime.Today.Month, lastPaycheck));
            var totalDue = billsDue.Sum(bill => Convert.ToDecimal(bill.AmountDue));
            savingsAccountBalances = _calculations.SavingsReqForBills(bills, savingsAccountBalances);


            Calculations.UpdateAccountGoals(accounts, savingsAccountBalances);


            viewModel.MonthlyExpenses = totalDue.ToString("C", CultureInfo.CurrentCulture);

            viewModel.MonthlyIncome = (Convert.ToDecimal(_db.Salaries.Select(s => s.NetIncome).FirstOrDefault()) * 2).ToString("C", CultureInfo.CurrentCulture);
            viewModel.SavedUp = Calculations.SavingsReqForBills(bills).ToString("C", CultureInfo.CurrentCulture);
            viewModel.TotalDue = totalDue.ToString("C", CultureInfo.CurrentCulture);
            viewModel.Accounts = accounts;

            return View(viewModel);
        }


        public ActionResult GetFutureValue(DashboardViewModel model)
        {
            if (!ModelState.IsValid) return View("Index", model);
            if (model.SelectedFVType.Equals("futureValue"))
            {
                var fv = model.FutureAmount;
                model.FutureDate = Calculations.CalculateFvDate(Convert.ToDecimal(fv),model.NetIncome);
            }
            else if (model.SelectedFVType.Equals("futureDate"))
            {
                model.FutureAmount = _calculations.CalculateFv(Convert.ToDateTime(model.FutureDate), model.NetIncome);
            }
            return View("Index", model);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}

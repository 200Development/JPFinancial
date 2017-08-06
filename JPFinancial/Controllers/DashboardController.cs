﻿using Financial_Tracking;
using Financial_Tracking.ViewModels;
using JPFinancial.Models;
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
            var savingsAccountBalances = new Dictionary<string, decimal>();
            var bills = (from b in _db.Bills select b).ToList();
            var accounts = (from a in _db.Accounts select a).ToList();
            var firstDayOfMonth = Calculations.GetFirstDayOfMonth(DateTime.Today.Year, DateTime.Today.Month);
            var firstPaycheck = new DateTime(DateTime.Today.Year,DateTime.Today.Month,15);  //ToDo: make dynamic
            var lastPaycheck = Calculations.GetLastDayOfMonth(DateTime.Today);
            var billsDue = _calculations.GetBillsDue(firstPaycheck, firstDayOfMonth, new DateTime(DateTime.Today.Year, DateTime.Today.Month, lastPaycheck));
            var totalDue = billsDue.Sum(bill => Convert.ToDecimal(bill.AmountDue));
            savingsAccountBalances = _calculations.SavingsReqForBills(bills, savingsAccountBalances);

            Calculations.UpdateAccountGoals(accounts, savingsAccountBalances);

            ViewBag.SavedUp = Calculations.SavingsReqForBills(bills);
            ViewBag.TotalDue = totalDue.ToString("C", CultureInfo.CurrentCulture);
            ViewBag.Bills = billsDue;
            ViewBag.Accounts = accounts;

            return View();
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
﻿using System;
using System.Web.Mvc;
using JPFData;
using JPFData.Enumerations;
using JPFData.Managers;
using JPFData.Models.JPFinancial;
using JPFData.ViewModels;
using PagedList;

namespace JPFinancial.Controllers
{
    [Authorize]
    public class BillsController : Controller
    {
        private readonly ApplicationDbContext _db = new ApplicationDbContext();
        private readonly BillManager _billManager = new BillManager();
        private readonly ExpenseManager _expenseManager = new ExpenseManager();
        private readonly AccountManager _accountManager = new AccountManager();
        private readonly TransactionManager _transactionManager = new TransactionManager();

       
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Create(BillViewModel billVM)
        {
            try
            {
                // Get hidden values that are set for dropdowns on change event
                var selectedAccountId = Request.Form["selectedAccountId"];
                billVM.Bill.AccountId = Convert.ToInt32(selectedAccountId);

                var selectedFrequencyId = Convert.ToInt32(Request.Form["selectedFrequencyId"]);
                if (selectedFrequencyId > 0) billVM.Bill.PaymentFrequency = (FrequencyEnum)selectedFrequencyId;


                if (!_billManager.Create(billVM.Bill)) return View("Error");


                return RedirectToAction("Index");
            }
            catch (Exception e)
            {
                Logger.Instance.Error(e);
                return View("Error");
            }
        }

        public JsonResult EditBill(Bill bill)
        {
            try
            {
                return Json(_billManager.Edit(bill) ? "Success" : "Failed");
            }
            catch (Exception e)
            {
                Logger.Instance.Error(e);
                return Json("Error");
            }
        }

        public JsonResult DeleteBill(Bill bill)
        {
            try
            {
                return bill.Id > 0 ? Json(_billManager.Delete(bill.Id) ? "Success" : "Failed") : Json("ID is invalid");
            }
            catch (Exception e)
            {
                Logger.Instance.Error(e);
                return Json("Error");
            }
        }

        public JsonResult SetExpenseToPaid(Expense expense)
        {
            try
            {
                if (expense.Id >= 1)
                    return _expenseManager.SetExpenseToPaid(expense.Id) ? Json("Success") : Json("Failed");


                Logger.Instance.Info("Invalid Expense Id");
                return Json("Invalid Expense Id");
            }
            catch (Exception e)
            {
                Logger.Instance.Error(e);
                return Json("Error");
            }
        }

        public JsonResult SetExpenseToPaidWithTransaction(Expense expense)
        {
            try
            {
                if (expense.Id < 1)
                {
                    Logger.Instance.Info("Invalid Expense Id");
                    return Json("Invalid Expense Id");
                }

                if (expense.CreditAccountId < 1)
                {
                    Logger.Instance.Info("Invalid Credit Account Id");
                    return Json("Invalid Credit Account Id");
                }

                var transactionVM = new TransactionViewModel();

                var transaction = new Transaction();
                transaction.Type = TransactionTypesEnum.Expense;
                transaction.Category = CategoriesEnum.ManualPayment;
                transaction.Amount = expense.Amount;
                transaction.CreditAccountId = expense.CreditAccountId;
                transaction.SelectedExpenseId = expense.Id;
                transaction.Date = expense.Due;
                transaction.Payee = "Manual Expense Payment";

                transactionVM.Transaction = transaction;


                return _transactionManager.Create(transactionVM) ? Json("Success") : Json("Failed");
            }
            catch (Exception e)
            {
                Logger.Instance.Error(e);
                return Json("Error");
            }
        }

        public JsonResult PageBills(int? page)
        {
            try
            {
                var pageSize = 10;
                var pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;

                var pagedBills = _billManager.GetAllBills().ToPagedList(pageIndex, pageSize);


                return Json(pagedBills);
            }
            catch (Exception e)
            {
                Logger.Instance.Error(e);
                return Json("Error: " + e);
            }
        }

        public ActionResult ExpensesTablePartial(int page = 1, int pageSize = 10)
        {

            BillViewModel billVM = new BillViewModel();
            billVM.Accounts = _accountManager.GetAllAccounts();
            billVM.Bills = _billManager.GetAllBills();
            billVM.PagedBills = billVM.Bills.ToPagedList(page, pageSize);
            billVM.Expenses = _expenseManager.GetAllUnpaidExpenses();
            billVM.Metrics = _billManager.GetBillMetrics();
            billVM.PagedExpenses = _expenseManager.GetAllUnpaidExpenses().ToPagedList(page, pageSize);


            return PartialView("_ExpensesTable", billVM.PagedExpenses);
        }

        public ActionResult BillsTablePartial(int page = 1, int pageSize = 10)
        {

            BillViewModel billVM = new BillViewModel();
            billVM.Accounts = _accountManager.GetAllAccounts();
            billVM.Bills = _billManager.GetAllBills();
            billVM.PagedBills = billVM.Bills.ToPagedList(page, pageSize);
            billVM.Expenses = _expenseManager.GetAllUnpaidExpenses();
            billVM.Metrics = _billManager.GetBillMetrics();
            billVM.PagedExpenses = _expenseManager.GetAllUnpaidExpenses().ToPagedList(page, pageSize);


            return PartialView("_BillsTable", billVM);
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

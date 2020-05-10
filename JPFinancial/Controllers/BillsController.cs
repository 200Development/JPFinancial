using System;
using System.Linq;
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

        [HttpGet]       
        public ActionResult Index()
        {

            var billVM = GetBillVM(1);


            return View(billVM);
        }

        [HttpPost]
        public JsonResult Create(BillWithAccount vm)
        {
            try
            {
                if (vm.FrequencyId > 0) vm.Frequency = (FrequencyEnum)vm.FrequencyId;


                var bill = MapToBill(vm);
                if (vm.AccountId != 0) return Json(!_billManager.Create(bill) ? "Error" : "Success");


                var account = MapToAccount(vm);
                _accountManager.Create(account);
                bill.AccountId = account.Id;

                return Json(!_billManager.Create(bill) ? "Error" : "Success");
            }
            catch (Exception e)
            {
                Logger.Instance.Error(e);
                return Json("Error");
            }
        }

        [HttpPost]
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

        [HttpPost]
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

        [HttpPost]
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

        [HttpPost]
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

        [HttpGet]
        public ActionResult PageExpenses(int page = 1)
        {

            var billVM = GetBillVM(page);


            return PartialView("_ExpensesTable", billVM.PagedExpenses);
        }

        [HttpGet]
        public ActionResult PageBills(int page = 1)
        {
            
            var billVM = GetBillVM(page);


            return PartialView("_BillsTable", billVM);
        }

        private BillViewModel GetBillVM (int page = 1)
        {

            var pageSize = Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings["defaultPageSize"]);

            BillViewModel billVM = new BillViewModel();
            billVM.Accounts = _accountManager.GetAllAccounts();
            billVM.Bills = _billManager.GetAllBills().OrderBy(b => b.DueDate).ThenBy(b => b.Name).ToList();
            billVM.PagedBills = billVM.Bills.ToPagedList(page, pageSize);
            billVM.Expenses = _expenseManager.GetAllUnpaidExpenses().OrderBy(e => e.Due).ThenBy(e => e.Name).ToList();
            billVM.Metrics = _billManager.GetBillMetrics();
            billVM.PagedExpenses = billVM.Expenses.ToPagedList(page, pageSize);


            return billVM;
        }

        private Bill MapToBill(BillWithAccount vm)
        {
            var bill = new Bill();

            bill.Name = vm.Name;
            bill.AmountDue = vm.AmountDue;
            bill.PaymentFrequency = vm.Frequency;
            bill.DueDate = vm.DueDate;
            if (vm.AccountId != null) bill.AccountId = (int) vm.AccountId;


            return bill;
        }

        private Account MapToAccount(BillWithAccount vm)
        {
            var account = new Account();

            account.Name = vm.AccountName;
            account.Balance = vm.AccountBalance;
            account.PaycheckContribution = vm.AccountPaycheckContribution;


            return account;
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

    public class BillWithAccount
    {
        public string Name { get; set; }
        public decimal AmountDue { get; set; }
        public DateTime DueDate { get; set; }
        public int? FrequencyId { get; set; }
        public FrequencyEnum Frequency { get; set; }
        public int? AccountId { get; set; }
        public string AccountName { get; set; }
        public decimal AccountBalance { get; set; }
        public decimal AccountPaycheckContribution { get; set; }
    }
}

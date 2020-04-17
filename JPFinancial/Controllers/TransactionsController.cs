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
    public class TransactionsController : Controller
    {
        private readonly ApplicationDbContext _db = new ApplicationDbContext();
        private readonly TransactionManager _transactionManager = new TransactionManager();

        [HttpGet]
        public ActionResult Index()
        {
            try
            {
                var transactionVM = GetTransactionVM(1);


                return View(transactionVM);
            }
            catch (Exception e)
            {
                Logger.Instance.Error(e);
                return View("Error");
            }
        }

        [HttpPost]
        public ActionResult Create(TransactionViewModel transactionVM)
        {
            try
            {
                switch (transactionVM.Type)
                {
                    case TransactionTypesEnum.Expense:
                        {
                            if (transactionVM.IsBill)
                            {
                                ExpenseManager expenseManager = new ExpenseManager();
                                transactionVM.Transaction.Payee = expenseManager.GetExpense(transactionVM.Transaction?.SelectedExpenseId).Name ??
                                    string.Empty;
                                ModelState["Transaction.Payee"].Errors.Clear();
                                UpdateModel(transactionVM.Transaction);
                            }
                            else
                            {
                                UpdateModel(transactionVM.Transaction);
                            }


                            if (!ModelState.IsValid) return View("Error");
                            if (!_transactionManager.Create(transactionVM)) return View("Error");
                            break;
                        }
                    case TransactionTypesEnum.Income:
                        {
                            if (!transactionVM.AutoTransferPaycheckContributions)
                                _transactionManager.Create(transactionVM);
                            else
                            {
                                if (!_transactionManager.HandlePaycheckContributions(transactionVM.Transaction))
                                    return View("Error");
                            }

                            break;
                        }
                    //case TransactionTypesEnum.Transfer:
                    //    throw new NotImplementedException();
                    default:
                        throw new NotImplementedException();
                }
                

                return RedirectToAction("Index");
            }
            catch (Exception e)
            {
                Logger.Instance.Error(e);
                return View("Error");
            }
        }

        [HttpPost]
        public JsonResult EditTransaction(Transaction transaction)
        {
            try
            {
                return Json(_transactionManager.Edit(transaction) ? "Success" : "Failed");
            }
            catch (Exception e)
            {
                Logger.Instance.Error(e);
                return Json("Error");
            }
        }

        [HttpPost]
        public JsonResult DeleteTransaction(Transaction transaction)
        {
            try
            {
                return transaction.Id > 0
                    ? Json(_transactionManager.Delete(transaction.Id) ? "Success" : "Failed")
                    : Json("ID is invalid");
            }
            catch (Exception e)
            {
                Logger.Instance.Error(e);
                return Json("Error");
            }
        }

        [HttpGet]
        public ActionResult PageTransactions(int page = 1)
        {
            try
            {
                var transactionVM = GetTransactionVM(page);


                return PartialView("_TransactionsTable", transactionVM);
            }
            catch (Exception e)
            {
                Logger.Instance.Error(e);
                return View("Error");
            }
        }

        private TransactionViewModel GetTransactionVM(int page)
        {
            var pageSize = Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings["defaultPageSize"]);

            var transactionVM = new TransactionViewModel();
            transactionVM.Transactions = _transactionManager.GetAllTransactions().OrderByDescending(t => t.Date).ThenBy(t => t.Amount);
            transactionVM.PagedTransactions = transactionVM.Transactions.ToPagedList(page, pageSize);
            transactionVM.Metrics = _transactionManager.GetMetrics();


            return transactionVM;
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

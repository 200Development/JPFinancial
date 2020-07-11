using System;
using System.Net;
using System.Web.Mvc;
using JPFData;
using JPFData.Enumerations;
using JPFData.Managers;
using JPFData.Models.JPFinancial;
using JPFData.ViewModels;

namespace JPFinancial.Controllers
{
    [Authorize]
    public class TransactionsController : Controller
    {
        private readonly ApplicationDbContext _db = new ApplicationDbContext();
        private readonly TransactionManager _transactionManager = new TransactionManager();
        private readonly AccountManager _accountManager = new AccountManager();

        public ActionResult Index()
        {
            try
            {
                TransactionViewModel transactionVM = new TransactionViewModel();
                transactionVM.Transactions = _transactionManager.GetAllTransactions();
                transactionVM.Metrics = _transactionManager.GetMetrics();


                return View(transactionVM);
            }
            catch (Exception e)
            {
                Logger.Instance.Error(e);
                return View(new TransactionViewModel());
            }
        }

        public ActionResult Create()
        {
            try
            {
                TransactionViewModel transactionVM = new TransactionViewModel();
                transactionVM.Accounts = _accountManager.GetAllAccounts();


                return View(transactionVM);
            }
            catch (Exception e)
            {
                Logger.Instance.Error(e);
                return HttpNotFound();
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
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


                            if (!ModelState.IsValid) return View(transactionVM);
                            _transactionManager.Create(transactionVM);
                            break;
                        }
                    case TransactionTypesEnum.Income:
                        {
                            if (!transactionVM.AutoTransferPaycheckContributions)
                                _transactionManager.Create(transactionVM);
                            else
                            {
                                if (!_transactionManager.HandlePaycheckContributions(transactionVM.Transaction))
                                    return View(transactionVM);
                            }

                            break;
                        }
                    case TransactionTypesEnum.Transfer:
                        throw new NotImplementedException();
                    default:
                        throw new NotImplementedException();
                }


                if (transactionVM.moreTransactions)
                {
                    Create();

                }


                return RedirectToAction("Index");
            }
            catch (Exception e)
            {
                Logger.Instance.Error(e);
                return View(new TransactionViewModel());
            }
        }

        public ActionResult Edit(int? id)
        {
            try
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                TransactionViewModel transactionVM = new TransactionViewModel();
                AccountManager accountManager = new AccountManager();
                transactionVM.Transaction = _transactionManager.GetTransaction(id);


                if (transactionVM.Transaction == null)
                {
                    Logger.Instance.Debug("Returned Transaction is null - (error)");
                    return HttpNotFound();
                }

                transactionVM.Accounts = accountManager.GetAllAccounts();

                return View(transactionVM);
            }
            catch (Exception e)
            {
                Logger.Instance.Error(e);
                return View(new TransactionViewModel());
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(TransactionViewModel transactionVM)
        {
            try
            {
                if (!ModelState.IsValid) return View(transactionVM);
                if (_transactionManager.Edit(transactionVM))
                    return RedirectToAction("Index");


                return View(transactionVM);
            }
            catch (Exception e)
            {
                Logger.Instance.Error(e);
                return View(new TransactionViewModel());
            }
        }

        public ActionResult Delete(int? id)
        {
            try
            {
                if (id == null)
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

                Transaction transaction = _transactionManager.GetTransaction(id);
                if (transaction == null)
                {
                    Logger.Instance.Debug("Returned Transaction is null - (error)");
                    return HttpNotFound();
                }



                return View(transaction);
            }
            catch (Exception e)
            {
                Logger.Instance.Error(e);
                return View(new Transaction());
            }
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            try
            {
                if (_transactionManager.Delete(id))
                    return RedirectToAction("Index");

                // Send Transaction back to Delete View because Delete failed
                Transaction transaction = _transactionManager.GetTransaction(id);
                if (transaction == null)
                {
                    Logger.Instance.Debug("Returned Transaction is null - (error)");
                    return HttpNotFound();
                }


                return View("Index");
            }
            catch (Exception e)
            {
                Logger.Instance.Error(e);
                return RedirectToAction("Index");
            }
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

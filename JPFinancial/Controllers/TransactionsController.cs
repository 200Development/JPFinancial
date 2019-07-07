using System;
using System.Net;
using System.Web.Mvc;
using JPFData;
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

        // GET: Transactions
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

        // GET: Transactions/Create
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

        // POST: Transactions/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(TransactionViewModel transactionVM)
        {
            try
            {
                ExpenseManager expenseManager = new ExpenseManager();
                transactionVM.Transaction.UserId = Global.Instance.User.Id;
                transactionVM.Transaction.Payee = expenseManager.GetExpense(transactionVM.Transaction.SelectedExpenseId).Name;

                ModelState["Transaction.Payee"].Errors.Clear();
                UpdateModel(transactionVM.Transaction); //throws invalidoperationexception


                if (!ModelState.IsValid) return View(transactionVM);
                if (!transactionVM.AutoTransferPaycheckContributions)
                    _transactionManager.Create(transactionVM);
                else
                {
                    if (!_transactionManager.Create(transactionVM) ||
                        !_transactionManager.HandlePaycheckContributions(transactionVM.Transaction))
                        return View(transactionVM);
                }

                return RedirectToAction("Index");
            }
            catch (Exception e)
            {
                Logger.Instance.Error(e);
                return View(new TransactionViewModel());
            }
        }

        // GET: Transactions/Edit/5
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

                return HttpNotFound();
            }
            catch (Exception e)
            {
                Logger.Instance.Error(e);
                return View(new TransactionViewModel());
            }
        }

        // POST: Transactions/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
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

        // GET: Transactions/Delete/5
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

        // POST: Transactions/Delete/5
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

using System;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using JPFData;
using JPFData.Enumerations;
using JPFData.Models;
using JPFData.ViewModels;

namespace JPFinancial.Controllers
{
    public class TransactionsController : Controller
    {
        private readonly ApplicationDbContext _db = new ApplicationDbContext();

        // GET: Transactions
        public ActionResult Index()
        {
            try
            {
                Logger.Instance.DataFlow($"Index");
                TransactionViewModel transactionVM = new TransactionViewModel();
                transactionVM.EventArgument = EventArgumentEnum.Read;
                transactionVM.EventCommand = EventCommandEnum.Get;
                transactionVM.HandleRequest();
                Logger.Instance.DataFlow($"TransactionViewModel returned to View");
                return View(transactionVM);
            }
            catch (Exception e)
            {
                Logger.Instance.Error(e);
                return View(new TransactionViewModel());
            }
        }

        // GET: Transactions/Details/5
        //public ActionResult Details(int? id)
        //{
        //    try
        //    {
        //        Logger.Instance.DataFlow($"Details");
        //        if (id == null)
        //        {
        //            return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //        }
        //        Transaction transaction = _db.Transactions.Find(id);
        //        if (transaction == null)
        //        {
        //            Logger.Instance.DataFlow($"Transaction returned is null.  return HttpNotFound to View");
        //            return HttpNotFound();
        //        }
        //        Logger.Instance.DataFlow($"Transaction returned to View");
        //        return View(transaction);
        //    }
        //    catch (Exception e)
        //    {
        //        Logger.Instance.Error(e);
        //        return View(new Transaction());
        //    }
        //}

        // GET: Transactions/Create

        public ActionResult Create()
        {
            return View(new TransactionViewModel());
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
                Logger.Instance.DataFlow($"Create");
                if (!ModelState.IsValid) return View(transactionVM);

                //if (transactionVM.Entity.Transaction.CreditAccountId != null)
                //{
                //    transactionVM.Entity.Transaction.CreditAccount = _db.Accounts.Find(transactionVM.Entity.Transaction.CreditAccountId);
                //    Logger.Instance.DataFlow($"Credit card set");
                //}

                //if (transactionVM.Entity.Transaction.DebitAccountId != null)
                //{
                //    transactionVM.Entity.Transaction.DebitAccount = _db.Accounts.Find(transactionVM.Entity.Transaction.DebitAccountId);
                //    Logger.Instance.DataFlow($"Debit account set");
                //}

                //if (transactionVM.IsBill)
                //{
                //    transactionVM.Entity.Transaction.SelectedBillId = transactionVM.Entity.Transaction.SelectedBillId;
                //    Logger.Instance.DataFlow($"Bill set");
                //}

                transactionVM.Entity.Transaction.Type = transactionVM.Type;
                transactionVM.Entity.Transaction.Date = Convert.ToDateTime(transactionVM.Date);
                transactionVM.Entity.Transaction.UsedCreditCard = transactionVM.UsedCreditCard;
                transactionVM.EventArgument = EventArgumentEnum.Create;

                if (!transactionVM.HandleRequest()) return View(transactionVM);
                Logger.Instance.DataFlow($"Return to index View");
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
                Logger.Instance.DataFlow($"Edit");
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                TransactionViewModel transactionVM = new TransactionViewModel();
                transactionVM.EventArgument = EventArgumentEnum.Update;
                transactionVM.EventCommand = EventCommandEnum.Get;
                transactionVM.Entity.Transaction.Id = (int)id;
                transactionVM.HandleRequest();

                if (transactionVM.Entity.Transaction != null) return View(transactionVM);
                Logger.Instance.DataFlow($"Transaction returned is null.  return HttpNotFound to View");
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
                Logger.Instance.DataFlow($"Edit");
                if (!ModelState.IsValid) return View(transactionVM);


                //if (transactionVM.Entity.Transaction.CreditAccountId != null)
                //{
                //    transactionVM.Entity.Transaction.CreditAccount = _db.Accounts.Find(transactionVM.Entity.Transaction.CreditAccountId);
                //    Logger.Instance.DataFlow($"Credit card set");
                //}

                //if (transactionVM.Entity.Transaction.DebitAccountId != null)
                //{
                //    transactionVM.Entity.Transaction.DebitAccount = _db.Accounts.Find(transactionVM.Entity.Transaction.DebitAccountId);
                //    Logger.Instance.DataFlow($"Debit account set");
                //}


                transactionVM.EventArgument = EventArgumentEnum.Update;
                transactionVM.EventCommand = EventCommandEnum.Edit;

                if (!transactionVM.HandleRequest()) return View(transactionVM);
                Logger.Instance.DataFlow($"Redirected to Index");
                return RedirectToAction("Index");

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
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                Transaction transaction = _db.Transactions.Find(id);
                if (transaction == null)
                {
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
                TransactionViewModel transactionVM = new TransactionViewModel();
                transactionVM.Entity.Transaction.Id = id;
                transactionVM.EventArgument = EventArgumentEnum.Delete;
                transactionVM.EventCommand = EventCommandEnum.Delete;
                if (!transactionVM.HandleRequest())
                    return View(transactionVM.Entity.Transaction);


                return RedirectToAction("Index");
            }
            catch (Exception e)
            {
                Logger.Instance.Error(e);
                return View(new Transaction());
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

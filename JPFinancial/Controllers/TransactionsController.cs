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
                TransactionViewModel transactionVM = new TransactionViewModel();
                transactionVM.EventArgument = EventArgumentEnum.Read;
                transactionVM.EventCommand = EventCommandEnum.Get;
                transactionVM.HandleRequest();
                return View(transactionVM);
            }
            catch (Exception e)
            {
                Logger.Instance.Error(e);
                return View(new TransactionViewModel());
            }
        }

        // GET: Transactions/Details/5
        public ActionResult Details(int? id)
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
                if (!ModelState.IsValid) return View(transactionVM);

                if (transactionVM.Entity.Transaction.CreditAccountId != null)
                    transactionVM.Entity.Transaction.CreditAccount = _db.Accounts.Find(transactionVM.Entity.Transaction.CreditAccountId);
                if (transactionVM.Entity.Transaction.DebitAccountId != null)
                    transactionVM.Entity.Transaction.DebitAccount = _db.Accounts.Find(transactionVM.Entity.Transaction.DebitAccountId);

                transactionVM.Entity.Transaction.Type = transactionVM.Type;
                transactionVM.Entity.Transaction.Date = Convert.ToDateTime(transactionVM.Date);
                transactionVM.Entity.Transaction.UsedCreditCard = transactionVM.UsedCreditCard;
                transactionVM.EventArgument = EventArgumentEnum.Create;
                if (transactionVM.HandleRequest())
                    return RedirectToAction("Index");

                return View(transactionVM);
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
                transactionVM.EventArgument = EventArgumentEnum.Update;
                transactionVM.EventCommand = EventCommandEnum.Get;
                transactionVM.Entity.Transaction.Id = (int)id;
                transactionVM.HandleRequest();
                if (transactionVM.Entity.Transaction == null)
                {
                    return HttpNotFound();
                }


                return View(transactionVM);
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
                transactionVM.EventArgument = EventArgumentEnum.Update;
                transactionVM.EventCommand = EventCommandEnum.Edit;
                if (transactionVM.HandleRequest())
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
            Transaction transaction = _db.Transactions.Find(id);
            try
            {
                _db.Transactions.Remove(transaction);

                if (transaction.UsedCreditCard)
                {
                    var creditCards = _db.CreditCards.ToList();
                    var creditCard = creditCards.FirstOrDefault(c => c.Id == transaction.SelectedCreditCardAccount);
                    _db.Entry(creditCard).State = EntityState.Modified;
                }

                _db.SaveChanges();
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

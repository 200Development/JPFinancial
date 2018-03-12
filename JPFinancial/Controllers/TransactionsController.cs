using JPFinancial.Models;
using JPFinancial.ViewModels;
using System;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;

namespace JPFinancial.Controllers
{
    public class TransactionsController : Controller
    {
        private readonly ApplicationDbContext _db = new ApplicationDbContext();

        // GET: Transactions
        public ActionResult Index()
        {
            return View(_db.Transactions.ToList());
        }

        // GET: Transactions/Details/5
        public ActionResult Details(int? id)
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

        // GET: Transactions/Create
        public ActionResult Create()
        {
            var accounts = _db.Accounts.ToList();
            var viewModel = new CreateTransactionViewModel();
            viewModel.DebitAccounts = accounts;
            viewModel.CreditAccounts = accounts;
            viewModel.Date = DateTime.Today;

            return View(viewModel);
        }

        // POST: Transactions/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Date,Payee,Memo,Type,Category,SelectedTransferToAccount,SelectedTransferFromAccount,Amount")] CreateTransactionViewModel transaction)
        {
            if (ModelState.IsValid)
            {
                var debitAccount = _db.Accounts.FirstOrDefault(a => a.Id == transaction.SelectedTransferFromAccount);
                var creditAccount = _db.Accounts?.FirstOrDefault(a => a.Id == transaction.SelectedTransferToAccount);
                var newTransaction = new Transaction();

                newTransaction.Date = transaction.Date;
                newTransaction.Payee = transaction.Payee;
                newTransaction.Category = transaction.Category;
                newTransaction.Memo = transaction.Memo;
                newTransaction.Type = transaction.Type;
                newTransaction.DebitAccount = debitAccount;
                newTransaction.CreditAccount = creditAccount;
                newTransaction.Amount = transaction.Amount;

                if (debitAccount != null)
                {
                    debitAccount.Balance += transaction.Amount;
                    _db.Entry(debitAccount).State = EntityState.Modified;
                }
                if (creditAccount != null)
                {
                    creditAccount.Balance -= transaction.Amount;
                    _db.Entry(creditAccount).State = EntityState.Modified;
                }

                _db.Transactions.Add(newTransaction);
                _db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(transaction);
        }

        // GET: Transactions/Edit/5
        public ActionResult Edit(int? id)
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

        // POST: Transactions/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Date,Payee,Memo,Type,Category,Amount")] Transaction transaction)
        {
            if (ModelState.IsValid)
            {
                _db.Entry(transaction).State = EntityState.Modified;
                _db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(transaction);
        }

        // GET: Transactions/Delete/5
        public ActionResult Delete(int? id)
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

        // POST: Transactions/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Transaction transaction = _db.Transactions.Find(id);
            _db.Transactions.Remove(transaction);
            _db.SaveChanges();
            return RedirectToAction("Index");
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

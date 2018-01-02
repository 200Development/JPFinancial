using JPFinancial.Models;
using System;
using System.Collections.Generic;
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
            var transactions = new List<Transaction>();
            var allTransactions = _db.Transactions.ToList();

            foreach (var transaction in allTransactions)
            {
                var newTransaction = new Transaction();
                newTransaction.Amount = transaction.Amount;
                newTransaction.Date = Convert.ToDateTime(transaction.Date.ToString("D"));
                newTransaction.Payee = transaction.Payee;
                newTransaction.Type = transaction.Type;
                newTransaction.Category = transaction.Category;
                newTransaction.Memo = transaction.Memo;
                newTransaction.TransferTo = transaction.TransferTo;
                newTransaction.TransferFrom = transaction.TransferFrom;

                transactions.Add(newTransaction);
            }

            return View(transactions);
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
            var date = DateTime.Today;
            var viewModel = new TransactionDTO
            {
                CreditAccounts = _db.Accounts.ToList(),
                DebitAccounts = _db.Accounts.ToList(),
                Date = date.ToString("d")
            };
            return View(viewModel);
        }

        // POST: Transactions/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Date,Payee,Memo,Type,Category,TransferTo,TransferFrom,Amount")] TransactionDTO transaction)
        {
            if (ModelState.IsValid)
            {
                var newTransaction = new Transaction();
                newTransaction.Payee = transaction.Payee;
                newTransaction.Date = Convert.ToDateTime(transaction.Date);
                newTransaction.Amount = transaction.Amount;
                newTransaction.Category = transaction.CategoriesEnum.ToString();
                if (transaction.SelectedDebitAccount != null)
                    newTransaction.TransferTo = transaction.SelectedDebitAccount.Name;
                if (transaction.SelectedCreditAccount != null)
                    newTransaction.TransferFrom = transaction.SelectedCreditAccount.Name;
                newTransaction.Memo = transaction.Memo;
                newTransaction.Type = transaction.TypesEnum.ToString();


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
        public ActionResult Edit([Bind(Include = "Id,Date,Payee,Memo,Type,Category,TransferTo,TransferFrom,Spend,Receive,Amount")] Transaction transaction)
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

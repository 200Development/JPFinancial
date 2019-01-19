﻿using System;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using JPFData;
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
            var creditCards = _db.CreditCards.ToList();
            var viewModel = new TransactionViewModel();
            viewModel.Accounts = accounts;
            viewModel.CreditCards = creditCards;
            viewModel.Date = DateTime.Today;

            return View(viewModel);
        }

        // POST: Transactions/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Date,Payee,Memo,Type,Category,SelectedCreditAccount,SelectedDebitAccount,Amount, UsedCreditCard, SelectedCreditCardAccount")] TransactionViewModel transaction)
        {
            if (ModelState.IsValid)
            {
                var newTransaction = ConvertViewModelToTransaction(transaction);
                UpdateAccountBalances(newTransaction, "create");
                if (transaction.UsedCreditCard)
                    UpdateCreditCard(newTransaction, "create");

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
        public ActionResult Edit([Bind(Include = "Id,Date,Payee,Memo,Type,Category,Amount, UsedCreditCard, SelectedCreditCardAccount")] Transaction transaction)
        {
            if (ModelState.IsValid)
            {
                // AsNoTracking() is essential or EF will throw an error
                UpdateAccountBalances(transaction, "edit");
                UpdateCreditCard(transaction, "edit");

                _db.Entry(transaction).State = EntityState.Modified;

                if (transaction.UsedCreditCard)
                {
                    var creditCards = _db.CreditCards.ToList();
                    var creditCard = creditCards.FirstOrDefault(c => c.Id == transaction.SelectedCreditCardAccount);
                    _db.Entry(creditCard).State = EntityState.Modified;
                }
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
            try
            {
                _db.Transactions.Remove(transaction);
                UpdateAccountBalances(transaction, "delete");
                UpdateCreditCard(transaction, "delete");

                if (transaction.UsedCreditCard)
                {
                    var creditCards = _db.CreditCards.ToList();
                    var creditCard = creditCards.FirstOrDefault(c => c.Id == transaction.SelectedCreditCardAccount);
                    _db.Entry(creditCard).State = EntityState.Modified;
                }

                _db.SaveChanges();
                return RedirectToAction("Index");
            }
            catch (Exception)
            {
                throw;
            }
        }

        private Transaction ConvertViewModelToTransaction(TransactionViewModel transactionViewModel)
        {
            try
            {
                var newTransaction = new Transaction();
                newTransaction.Id = transactionViewModel.Id;
                newTransaction.Date = transactionViewModel.Date;
                newTransaction.Payee = transactionViewModel.Payee;
                newTransaction.Memo = transactionViewModel.Memo;
                newTransaction.Type = transactionViewModel.Type;
                newTransaction.Category = transactionViewModel.Category;
                newTransaction.CreditAccount = _db.Accounts.FirstOrDefault(a => a.Id == transactionViewModel.SelectedCreditAccount);
                newTransaction.DebitAccount = _db.Accounts.FirstOrDefault(a => a.Id == transactionViewModel.SelectedDebitAccount);
                newTransaction.Amount = transactionViewModel.Amount;
                newTransaction.UsedCreditCard = transactionViewModel.UsedCreditCard;
                newTransaction.SelectedCreditCardAccount = transactionViewModel.SelectedCreditCardAccount;


                return newTransaction;
            }
            catch (Exception)
            {
                return null;
            }
        }

        private void UpdateAccountBalances(Transaction transaction, string type)
        {
            if (type == "create")
            {
                var newTransaction = new Transaction();
                newTransaction.Date = transaction.Date;
                newTransaction.Payee = transaction.Payee;
                newTransaction.Category = transaction.Category;
                newTransaction.Memo = transaction.Memo;
                newTransaction.Type = transaction.Type;
                newTransaction.DebitAccount = transaction.DebitAccount;
                newTransaction.CreditAccount = transaction.CreditAccount;
                newTransaction.Amount = transaction.Amount;

                if (transaction.DebitAccount != null)
                {
                    transaction.DebitAccount.Balance += transaction.Amount;
                    _db.Entry(transaction.DebitAccount).State = EntityState.Modified;
                }

                if (transaction.CreditAccount != null)
                {
                    transaction.CreditAccount.Balance -= transaction.Amount;
                    _db.Entry(transaction.CreditAccount).State = EntityState.Modified;
                }
            }
            else if (type == "delete" || type == "edit")
            {
                var originalTransaction = _db.Transactions
                    .AsNoTracking()
                    .Where(t => t.Id == transaction.Id)
                    .Cast<Transaction>()
                    .FirstOrDefault();
                if (originalTransaction == null) return;
                var originalCreditAccount =
                    _db.Accounts.FirstOrDefault(a => a.Id == originalTransaction.CreditAccountId);
                var originalDebitAccount = _db.Accounts.FirstOrDefault(a => a.Id == originalTransaction.DebitAccountId);
                var originalAmount = originalTransaction.Amount;

                // Reassign the Debit/Credit Account Id's to Transaction Model
                transaction.CreditAccountId = originalTransaction.CreditAccountId;
                transaction.DebitAccountId = originalTransaction.DebitAccountId;

                if (type == "delete")
                {
                    if (originalDebitAccount != null)
                    {
                        originalDebitAccount.Balance -= transaction.Amount;
                        _db.Entry(originalDebitAccount).State = EntityState.Modified;
                    }

                    if (originalCreditAccount != null)
                    {
                        originalCreditAccount.Balance += transaction.Amount;
                        _db.Entry(originalCreditAccount).State = EntityState.Modified;
                    }
                }
                else if (type == "edit")
                {
                    var amountDifference = transaction.Amount - originalAmount;
                    if (originalDebitAccount != null)
                    {
                        originalDebitAccount.Balance += amountDifference;
                        _db.Entry(originalDebitAccount).State = EntityState.Modified;
                    }

                    if (originalCreditAccount != null)
                    {
                        originalCreditAccount.Balance -= amountDifference;
                        _db.Entry(originalCreditAccount).State = EntityState.Modified;
                    }
                }
            }
        }

        private void UpdateCreditCard(Transaction transaction, string type)
        {
            var creditCardId = transaction?.SelectedCreditCardAccount;
            if (creditCardId == null) return;
            var creditCards = _db.CreditCards.ToList();
            var creditCard = creditCards.FirstOrDefault(c => c.Id == creditCardId);

            if (type == "create")
            {
                if (creditCard != null) creditCard.CurrentBalance += transaction.Amount;
                _db.Entry(creditCard).State = EntityState.Modified;
            }
            else if (type == "delete" || type == "edit")
            {
                var originalTransaction = _db.Transactions
                    .AsNoTracking()
                    .Where(t => t.Id == transaction.Id)
                    .Cast<Transaction>()
                    .FirstOrDefault();
                if (originalTransaction == null) return;
                var originalCreditCard = _db.CreditCards.FirstOrDefault(a => a.Id == originalTransaction.SelectedCreditCardAccount);
                var originalAmount = originalTransaction.Amount;

                // Reassign the credit card Id to Transaction Model
                transaction.SelectedCreditCardAccount = originalTransaction.SelectedCreditCardAccount;

                if (type == "delete")
                {
                    if (originalCreditCard == null) return;
                    originalCreditCard.CurrentBalance -= transaction.Amount;
                    _db.Entry(originalCreditCard).State = EntityState.Modified;
                }
                else if (type == "edit")
                {
                    var amountDifference = transaction.Amount - originalAmount;
                    if (creditCard == null) return;
                    creditCard.CurrentBalance += amountDifference;
                    _db.Entry(creditCard).State = EntityState.Modified;
                }
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

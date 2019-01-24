using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using JPFData.Base;
using JPFData.Models;


namespace JPFData.Managers
{
    public class TransactionsManager : BaseManager
    {
        public TransactionsManager()
        {
        }
        

        public List<Transaction> Get()
        {
            return Get(new Transaction());
        }

        public List<Transaction> Get(Transaction entity)
        {
            var ret = DbContext.Transactions.ToList();

            // Do any searching
            if (!string.IsNullOrEmpty(entity.Payee))
            {
                ret = ret.FindAll(
                    t => t.Payee.ToLower().
                        StartsWith(entity.Payee,
                            StringComparison.CurrentCultureIgnoreCase));
            }

            return ret;
        }

        public Transaction Get(int transactionId)
        {

            var ret = DbContext.Transactions.ToList();

            //Find the specific transaction
            var entity = ret.Find(t => t.Id == transactionId) ?? null;

            return entity;
        }

        public bool Update(Transaction entity)
        {
            var ret = Validate(entity);

            if (ret)
            {
                try
                {
                    // AsNoTracking() is essential or EF will throw an error
                    UpdateAccountBalances(entity, "edit");
                    UpdateCreditCard(entity, "edit");

                    DbContext.Entry(entity).State = EntityState.Modified;

                    if (entity.UsedCreditCard)
                    {
                        var creditCards = DbContext.CreditCards.ToList();
                        var creditCard = creditCards.FirstOrDefault(c => c.Id == entity.SelectedCreditCardAccount);
                        DbContext.Entry(creditCard).State = EntityState.Modified;
                    }
                    DbContext.SaveChanges();
                    return true;
                }
                catch (Exception e)
                {
                    return false;
                }
            }
            return false;
        }

        public bool Delete(Transaction entity)
        {
            try
            {
                //var transaction = _db.Transactions.Find(entity.Id);
                DbContext.Transactions.Remove(entity);
                UpdateAccountBalances(entity, "delete");
                UpdateCreditCard(entity, "delete");

                if (entity.UsedCreditCard)
                {
                    var creditCards = DbContext.CreditCards.ToList();
                    var creditCard = creditCards.FirstOrDefault(c => c.Id == entity.SelectedCreditCardAccount);
                    DbContext.Entry(creditCard).State = EntityState.Modified;
                }

                DbContext.SaveChanges();
                return true;
            }
            catch (Exception e)
            {
                return false;
            }
        }

        public bool Insert(Transaction entity)
        {
            var ret = Validate(entity);

            if (ret)
            {
                try
                {
                    UpdateAccountBalances(entity, "create");
                    if (entity.UsedCreditCard)
                        UpdateCreditCard(entity, "create");

                    DbContext.Transactions.Add(entity);
                    DbContext.SaveChanges();
                    return true;
                }
                catch (Exception e)
                {
                    return false;
                }
            }

            return false;
        }

        private bool Validate(Transaction entity)
        {
            ValidationErrors.Clear();

            if (!string.IsNullOrEmpty(entity.Payee))
            {
                if (entity.Payee.ToLower() == entity.Payee)
                {
                    ValidationErrors.Add(new KeyValuePair<string, string>("Payee", "Payee must not be all lower case."));
                }
            }

            return ValidationErrors.Count == 0;
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
                    DbContext.Entry(transaction.DebitAccount).State = EntityState.Modified;
                }

                if (transaction.CreditAccount != null)
                {
                    transaction.CreditAccount.Balance -= transaction.Amount;
                    DbContext.Entry(transaction.CreditAccount).State = EntityState.Modified;
                }
            }
            else if (type == "delete" || type == "edit")
            {
                var originalTransaction = DbContext.Transactions
                    .AsNoTracking()
                    .Where(t => t.Id == transaction.Id)
                    .Cast<Transaction>()
                    .FirstOrDefault();
                if (originalTransaction == null) return;
                var originalCreditAccount =
                    DbContext.Accounts.FirstOrDefault(a => a.Id == originalTransaction.CreditAccountId);
                var originalDebitAccount = DbContext.Accounts.FirstOrDefault(a => a.Id == originalTransaction.DebitAccountId);
                var originalAmount = originalTransaction.Amount;

                // Reassign the Debit/Credit Account Id's to Transaction Model
                transaction.CreditAccountId = originalTransaction.CreditAccountId;
                transaction.DebitAccountId = originalTransaction.DebitAccountId;

                if (type == "delete")
                {
                    if (originalDebitAccount != null)
                    {
                        originalDebitAccount.Balance -= transaction.Amount;
                        DbContext.Entry(originalDebitAccount).State = EntityState.Modified;
                    }

                    if (originalCreditAccount != null)
                    {
                        originalCreditAccount.Balance += transaction.Amount;
                        DbContext.Entry(originalCreditAccount).State = EntityState.Modified;
                    }
                }
                else if (type == "edit")
                {
                    var amountDifference = transaction.Amount - originalAmount;
                    if (originalDebitAccount != null)
                    {
                        originalDebitAccount.Balance += amountDifference;
                        DbContext.Entry(originalDebitAccount).State = EntityState.Modified;
                    }

                    if (originalCreditAccount != null)
                    {
                        originalCreditAccount.Balance -= amountDifference;
                        DbContext.Entry(originalCreditAccount).State = EntityState.Modified;
                    }
                }
            }
        }

        private void UpdateCreditCard(Transaction transaction, string type)
        {
            var creditCardId = transaction?.SelectedCreditCardAccount;
            if (creditCardId == null) return;
            var creditCards = DbContext.CreditCards.ToList();
            var creditCard = creditCards.FirstOrDefault(c => c.Id == creditCardId);

            if (type == "create")
            {
                if (creditCard != null) creditCard.Balance += transaction.Amount;
                DbContext.Entry(creditCard).State = EntityState.Modified;
            }
            else if (type == "delete" || type == "edit")
            {
                var originalTransaction = DbContext.Transactions
                    .AsNoTracking()
                    .Where(t => t.Id == transaction.Id)
                    .Cast<Transaction>()
                    .FirstOrDefault();
                if (originalTransaction == null) return;
                var originalCreditCard = DbContext.CreditCards.FirstOrDefault(a => a.Id == originalTransaction.SelectedCreditCardAccount);
                var originalAmount = originalTransaction.Amount;

                // Reassign the credit card Id to Transaction Model
                transaction.SelectedCreditCardAccount = originalTransaction.SelectedCreditCardAccount;

                if (type == "delete")
                {
                    if (originalCreditCard == null) return;
                    originalCreditCard.Balance -= transaction.Amount;
                    DbContext.Entry(originalCreditCard).State = EntityState.Modified;
                }
                else if (type == "edit")
                {
                    var amountDifference = transaction.Amount - originalAmount;
                    if (creditCard == null) return;
                    creditCard.Balance += amountDifference;
                    DbContext.Entry(creditCard).State = EntityState.Modified;
                }
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using JPFData.DTO;
using JPFData.Enumerations;
using JPFData.Metrics;
using JPFData.Models;


namespace JPFData.Managers
{
    /// <summary>
    /// Manages all Transaction communication between the application and the database
    /// </summary>
    public class TransactionManager
    {
        /*
        MANAGER STRUCTURE
        private fields
        constructors
        public fields
        public methods
        private methods
        */
        private readonly ApplicationDbContext _db;


        public TransactionManager()
        {
            _db = new ApplicationDbContext();
            ValidationErrors = new List<KeyValuePair<string, string>>();
        }


        public List<KeyValuePair<string, string>> ValidationErrors { get; set; }

        public TransactionDTO Get(TransactionDTO entity)
        {
            try
            {
                entity.Transactions = _db.Transactions.ToList();
                Logger.Instance.DataFlow($"Selected all transactions from DB");
                entity.Metrics = RefreshTransactionMetrics(entity);
                Logger.Instance.DataFlow($"Refreshed transaction metrics");
                return entity;
            }
            catch (Exception e)
            {
                Logger.Instance.Error(e);
                return null;
            }
        }

        public Transaction GetTransaction(TransactionDTO entity)
        {
            try
            {
                Logger.Instance.DataFlow($"Get transaction with Id: {entity.Transaction.Id}");
                return _db.Transactions.Find(entity.Transaction.Id);
            }
            catch (Exception e)
            {
                Logger.Instance.Error(e);
                return null;
            }
        }

        public bool Create(TransactionDTO entity)
        {
            try
            {
                Logger.Instance.DataFlow($"If transaction is not for income, set paycheckId to null");
                if (entity.Transaction.Type != TransactionTypesEnum.Income)
                    entity.Transaction.PaycheckId = null;
                UpdateAccountBalances(entity.Transaction, "create");
                Logger.Instance.DataFlow($"Account balances updated in data context");

                if (entity.Transaction.UsedCreditCard)
                {
                    UpdateCreditCard(entity.Transaction, "create");
                    Logger.Instance.DataFlow($"Credit card used for transaction updated in data context");
                }

                if (!AddTransaction(entity)) return false;
                Logger.Instance.DataFlow($"Transaction added to database context");
                _db.SaveChanges();
                Logger.Instance.DataFlow($"Database context saved to database");


                return true;
            }
            catch (Exception e)
            {
                Logger.Instance.Error(e);
                return false;
            }
        }

        public bool Edit(TransactionDTO entity)
        {
            try
            {
                //AsNoTracking() is essential or EF will throw an error
                UpdateAccountBalances(entity.Transaction, "edit");
                Logger.Instance.DataFlow($"Account balances updated in data context");
                UpdateCreditCard(entity.Transaction, "edit");
                Logger.Instance.DataFlow($"Credit card used for transaction updated in data context");


                _db.Entry(entity.Transaction).State = EntityState.Modified;
                Logger.Instance.DataFlow($"Transaction updated in data context");

                if (entity.Transaction.UsedCreditCard)
                {
                    var creditCards = _db.CreditCards.ToList();
                    var creditCard = creditCards.FirstOrDefault(c => c.Id == entity.Transaction.SelectedCreditCardAccount);
                    _db.Entry(creditCard).State = EntityState.Modified;
                    Logger.Instance.DataFlow($"Credit card updated in data context");
                }


                _db.SaveChanges();
                return true;
            }
            catch (Exception e)
            {
                Logger.Instance.Error(e);
                return false;
            }
        }

        public bool Update(Transaction entity)
        {
            try
            {
                var ret = Validate(entity);

                if (ret)
                {
                    //todo: add update code
                }

                return ret;
            }
            catch (Exception e)
            {
                Logger.Instance.Error(e);
                return false;
            }
        }

        public bool Delete(Transaction entity)
        {
            try
            {
                throw new NotImplementedException();
            }
            catch (Exception e)
            {
                Logger.Instance.Error(e);
                return false;
            }
        }


        private TransactionMetrics RefreshTransactionMetrics(TransactionDTO entity)
        {
            try
            {
                TransactionMetrics metrics = new TransactionMetrics();

                metrics.AccountMetrics = new AccountMetrics();
                metrics.CreditCardMetrics = new CreditCardMetrics();


                return metrics;
            }
            catch (Exception e)
            {
                Logger.Instance.Error(e);
                return null;
            }
        }

        private bool Validate(Transaction entity)
        {
            ValidationErrors.Clear();

            if (string.IsNullOrEmpty(entity.Payee)) return ValidationErrors.Count == 0;
            if (entity.Payee.ToLower() == entity.Payee)
            {
                ValidationErrors.Add(new KeyValuePair<string, string>("Payee", "Payee must not be all lower case."));
            }

            return ValidationErrors.Count == 0;
        }
      
        private bool AddTransaction(TransactionDTO entity)
        {
            try
            {
                var newTransaction = new Transaction();
                newTransaction.Date = entity.Transaction.Date;
                newTransaction.Payee = entity.Transaction.Payee;
                newTransaction.Category = entity.Transaction.Category;
                newTransaction.Memo = entity.Transaction.Memo;
                newTransaction.Type = entity.Transaction.Type;
                newTransaction.DebitAccountId = entity.Transaction.DebitAccountId;
                newTransaction.CreditAccountId = entity.Transaction.CreditAccountId;
                newTransaction.Amount = entity.Transaction.Amount;
                newTransaction.SelectedCreditCardAccount = entity.Transaction.SelectedCreditCardAccount;
                newTransaction.PaycheckId = null;
                newTransaction.UsedCreditCard = entity.Transaction.UsedCreditCard;
                _db.Transactions.Add(newTransaction);


                return true;
            }
            catch (Exception e)
            {
                Logger.Instance.Error(e);
                return false;
            }
        }

        private void UpdateAccountBalances(Transaction transaction, string type)
        {
            try
            {
                switch (type)
                {
                    case "create":
                        {
                            if (transaction.DebitAccount != null && transaction.DebitAccount.Id != 0)
                            {
                                transaction.DebitAccount.Balance += transaction.Amount;
                                transaction.DebitAccount.BalanceSurplus = UpdateBalanceSurplus(transaction.DebitAccount);
                                _db.Entry(transaction.DebitAccount).State = EntityState.Modified;
                            }

                            if (transaction.CreditAccount != null && transaction.CreditAccount.Id != 0)
                            {
                                transaction.CreditAccount.Balance -= transaction.Amount;
                                transaction.CreditAccount.BalanceSurplus = UpdateBalanceSurplus(transaction.CreditAccount);
                                _db.Entry(transaction.CreditAccount).State = EntityState.Modified;
                            }

                            break;
                        }
                    case "delete":
                    case "edit":
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

                            switch (type)
                            {
                                case "delete":
                                    {
                                        if (originalDebitAccount != null)
                                        {
                                            originalDebitAccount.Balance -= transaction.Amount;
                                            originalDebitAccount.BalanceSurplus = UpdateBalanceSurplus(originalDebitAccount);
                                            _db.Entry(originalDebitAccount).State = EntityState.Modified;
                                        }

                                        if (originalCreditAccount != null)
                                        {
                                            originalCreditAccount.Balance += transaction.Amount;
                                            originalCreditAccount.BalanceSurplus = UpdateBalanceSurplus(originalCreditAccount);
                                            _db.Entry(originalCreditAccount).State = EntityState.Modified;
                                        }

                                        break;
                                    }
                                case "edit":
                                    {
                                        var amountDifference = transaction.Amount - originalAmount;
                                        if (originalDebitAccount != null)
                                        {
                                            originalDebitAccount.Balance += amountDifference;
                                            originalDebitAccount.BalanceSurplus = UpdateBalanceSurplus(originalDebitAccount);
                                            _db.Entry(originalDebitAccount).State = EntityState.Modified;
                                        }

                                        if (originalCreditAccount != null)
                                        {
                                            originalCreditAccount.Balance -= amountDifference;
                                            originalCreditAccount.BalanceSurplus = UpdateBalanceSurplus(originalCreditAccount);
                                            _db.Entry(originalCreditAccount).State = EntityState.Modified;
                                        }

                                        break;
                                    }
                                default:
                                    throw new NotImplementedException();
                            }
                            break;
                        }
                    default:
                        throw new NotImplementedException($"{type} is not an accepted type for TransactionController.UpdateAccountBalances method");
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        private decimal? UpdateBalanceSurplus(Account account)
        {
            try
            {
                return account.Balance - account.RequiredSavings;
            }
            catch (Exception)
            {
                return decimal.Zero;
            }
        }

        private void UpdateCreditCard(Transaction transaction, string type)
        {
            var creditCardId = transaction?.SelectedCreditCardAccount;
            if (creditCardId == null) return;
            var creditCards = _db.CreditCards.ToList();
            var creditCard = creditCards.FirstOrDefault(c => c.Id == creditCardId);

            switch (type)
            {
                case "create":
                    {
                        if (creditCard != null) creditCard.CurrentBalance += transaction.Amount;
                        _db.Entry(creditCard).State = EntityState.Modified;
                        break;
                    }
                case "delete":
                case "edit":
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

                        switch (type)
                        {
                            case "delete":
                                {
                                    if (originalCreditCard == null) return;
                                    originalCreditCard.CurrentBalance -= transaction.Amount;
                                    _db.Entry(originalCreditCard).State = EntityState.Modified;
                                    break;
                                }
                            case "edit":
                                {
                                    var amountDifference = transaction.Amount - originalAmount;
                                    if (creditCard == null) return;
                                    creditCard.CurrentBalance += amountDifference;
                                    _db.Entry(creditCard).State = EntityState.Modified;
                                    break;
                                }
                            default:
                                throw new NotImplementedException();
                        }
                        break;
                    }
                default:
                    throw new NotImplementedException($"{type} is not an accepted type for TransactionController.UpdateCreditCard method");
            }
        }
    }
}

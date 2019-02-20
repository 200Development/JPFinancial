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
        private properties
        constructors
        public properties
        public methods
        private methods
        */
        private readonly ApplicationDbContext _db;
        private List<KeyValuePair<string, string>> ValidationErrors { get; set; }


        public TransactionManager()
        {
            _db = new ApplicationDbContext();
            ValidationErrors = new List<KeyValuePair<string, string>>();
        }


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
                GetUsedAccounts(entity);
                UpdateDbAccountBalances(entity.Transaction, EventArgumentEnum.Create);
                Logger.Instance.DataFlow($"Account balances updated in data context");

                if (entity.Transaction.UsedCreditCard)
                {
                    UpdateDbCreditCard(entity.Transaction, "create");
                    Logger.Instance.DataFlow($"Credit card used for transaction updated in data context");
                }

                if (entity.Transaction.SelectedBillId != null)
                {
                    SetBillAsPaid(entity.Transaction.SelectedBillId);
                    Logger.Instance.DataFlow($"Credit card used for transaction updated in data context");
                }

                if (!AddTransactionToDb(entity)) return false;
                Logger.Instance.DataFlow($"Transaction added to database context");


                //Testing to Update accounts after each transaction unless its a transfer
                return entity.Transaction.Type == TransactionTypesEnum.Transfer || new Calculations().Rebalance();
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
                GetUsedAccounts(entity);
                //AsNoTracking() is essential or EF will throw an error
                UpdateDbAccountBalances(entity.Transaction, EventArgumentEnum.Update);
                Logger.Instance.DataFlow($"Account balances updated in data context");
                if (entity.Transaction.UsedCreditCard)
                {
                    UpdateDbCreditCard(entity.Transaction, "edit");
                    Logger.Instance.DataFlow($"Credit card used for transaction updated in data context");
                }


                _db.Entry(entity.Transaction).State = EntityState.Modified;
                Logger.Instance.DataFlow($"Transaction updated in data context");

                if (entity.Transaction.UsedCreditCard)
                {
                    var creditCards = _db.CreditCards.ToList();
                    var creditCard = creditCards.FirstOrDefault(c => c.Id == entity.Transaction.SelectedCreditCardAccountId);
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

        public bool Delete(Transaction entity)
        {
            try
            {
                Transaction transaction = _db.Transactions.Find(entity.Id);

                if (UpdateDbAccountBalances(transaction, EventArgumentEnum.Delete))
                    _db.Transactions.Remove(transaction);

                if (transaction.UsedCreditCard)
                {
                    var creditCards = _db.CreditCards.ToList();
                    var creditCard = creditCards.FirstOrDefault(c => c.Id == transaction.SelectedCreditCardAccountId);
                    _db.Entry(creditCard).State = EntityState.Modified;
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


        /// <summary>
        /// Sets the Credit & Debit Accounts from passed Id's
        /// </summary>
        /// <param name="entity"></param>
        private void GetUsedAccounts(TransactionDTO entity)
        {
            try
            {
                if (entity.Transaction.CreditAccountId != null)
                {
                    entity.Transaction.CreditAccount = _db.Accounts.Find(entity.Transaction.CreditAccountId);
                    Logger.Instance.DataFlow($"Credit Account set");
                }

                if (entity.Transaction.DebitAccountId != null)
                {
                    entity.Transaction.DebitAccount = _db.Accounts.Find(entity.Transaction.DebitAccountId);
                    Logger.Instance.DataFlow($"Debit Account set");
                }
            }
            catch (Exception e)
            {
                Logger.Instance.Error(e);
            }
        }

        private bool UpdateDbAccountBalances(Transaction transaction, EventArgumentEnum eventArgument)
        {
            try
            {
                switch (eventArgument)
                {
                    case EventArgumentEnum.Create:
                        {
                            if (transaction.DebitAccount != null && transaction.DebitAccount.Id != 0)
                            {
                                var originalBalance = transaction.DebitAccount.Balance; // logs beginning balance
                                transaction.DebitAccount.Balance += transaction.Amount;
                                Logger.Instance.Calculation($"{transaction.DebitAccount.Name}.balance ({originalBalance}) + {transaction.Amount} = {transaction.DebitAccount.Balance}");
                                transaction.DebitAccount.BalanceSurplus = UpdateBalanceSurplus(transaction.DebitAccount);
                                _db.Entry(transaction.DebitAccount).State = EntityState.Modified;
                            }

                            if (transaction.CreditAccount != null && transaction.CreditAccount.Id != 0)
                            {
                                var originalBalance = transaction.CreditAccount.Balance; // logs beginning balance
                                transaction.CreditAccount.Balance -= transaction.Amount;
                                if (transaction.SelectedBillId != null)
                                {
                                    var originalRequiredBalance = transaction.CreditAccount.RequiredSavings; // logs beginning balance
                                    transaction.CreditAccount.RequiredSavings -= transaction.Amount;
                                    Logger.Instance.Calculation($"{transaction.CreditAccount.Name}.RequiredSavings ({originalRequiredBalance}) - {transaction.Amount} = {transaction.CreditAccount.RequiredSavings}");
                                }
                                Logger.Instance.Calculation($"{transaction.CreditAccount.Name}.balance ({originalBalance}) + {transaction.Amount} = {transaction.CreditAccount.Balance}");
                                transaction.CreditAccount.BalanceSurplus = UpdateBalanceSurplus(transaction.CreditAccount);
                                _db.Entry(transaction.CreditAccount).State = EntityState.Modified;
                            }


                            _db.SaveChanges();
                            return true;
                        }
                    case EventArgumentEnum.Delete:
                    case EventArgumentEnum.Update:
                        {
                            var originalTransaction = _db.Transactions
                                .AsNoTracking()
                                .Where(t => t.Id == transaction.Id)
                                .Cast<Transaction>()
                                .FirstOrDefault();
                            if (originalTransaction == null) return false;
                            var originalCreditAccount =
                                _db.Accounts.FirstOrDefault(a => a.Id == originalTransaction.CreditAccountId);
                            var originalDebitAccount = _db.Accounts.FirstOrDefault(a => a.Id == originalTransaction.DebitAccountId);
                            var originalAmount = originalTransaction.Amount;

                            // Reassign the Debit/Credit Account Id's to Transaction Model
                            transaction.CreditAccountId = originalTransaction.CreditAccountId;
                            transaction.DebitAccountId = originalTransaction.DebitAccountId;

                            switch (eventArgument)
                            {
                                case EventArgumentEnum.Delete:
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
                                            if ((transaction.SelectedBillId ?? 0) > 0)
                                            {
                                                originalCreditAccount.RequiredSavings -= transaction.Amount;
                                                var bill = _db.Bills.Find(transaction.SelectedBillId);
                                                _db.Entry(bill).State = EntityState.Modified;
                                            }
                                            originalCreditAccount.BalanceSurplus = UpdateBalanceSurplus(originalCreditAccount);
                                            _db.Entry(originalCreditAccount).State = EntityState.Modified;
                                        }


                                        _db.SaveChanges();
                                        return true;
                                    }
                                case EventArgumentEnum.Update:
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


                                        _db.SaveChanges();
                                        return true;
                                    }
                                default:
                                    throw new NotImplementedException();
                            }
                        }
                    default:
                        throw new NotImplementedException($"{eventArgument} is not an accepted type for TransactionController.UpdateDbAccountBalances method");
                }
            }
            catch (Exception e)
            {
                Logger.Instance.Error(e);
                return false;
            }
        }

        private bool AddTransactionToDb(TransactionDTO entity)
        {
            try
            {
                var newTransaction = new Transaction();
                newTransaction.Date = entity.Transaction.Date;
                newTransaction.Payee = entity.Transaction.SelectedBillId != null
                    ? _db.Bills.FirstOrDefault(b => b.Id == entity.Transaction.SelectedBillId)?.Name
                    : entity.Transaction.Payee;
                newTransaction.Category = entity.Transaction.Category;
                newTransaction.Memo = entity.Transaction.Memo;
                newTransaction.Type = entity.Transaction.Type;
                newTransaction.DebitAccountId = entity.Transaction.DebitAccountId;
                newTransaction.CreditAccountId = entity.Transaction.CreditAccountId;
                newTransaction.Amount = entity.Transaction.Amount;
                newTransaction.SelectedCreditCardAccountId = entity.Transaction.SelectedCreditCardAccountId;
                newTransaction.UsedCreditCard = entity.Transaction.UsedCreditCard;
                if (entity.Transaction.SelectedBillId != null)
                    newTransaction.SelectedBillId = entity.Transaction.SelectedBillId;
                _db.Transactions.Add(newTransaction);


                _db.SaveChanges();
                return true;
            }
            catch (Exception e)
            {
                Logger.Instance.Error(e);
                return false;
            }
        }

        //TODO: move to Calculations and replace existing Calculations.UpdateBalanceSurplus Method
        private decimal? UpdateBalanceSurplus(Account account)
        {
            try
            {
                if (account.Balance < 0.0m)
                    return account.BalanceSurplus = account.Balance;

                if (account.BalanceLimit > 0)
                    return account.BalanceSurplus = account.Balance > account.BalanceLimit
                        ? account.Balance - account.BalanceLimit
                        : decimal.Zero;

                return account.BalanceSurplus = account.Balance - (account.RequiredSavings ?? 0.0m);
            }
            catch (Exception)
            {
                return decimal.Zero;
            }
        }

        private void UpdateDbCreditCard(Transaction transaction, string type)
        {
            var creditCardId = transaction?.SelectedCreditCardAccountId;
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
                        var originalCreditCard = _db.CreditCards.FirstOrDefault(a => a.Id == originalTransaction.SelectedCreditCardAccountId);
                        var originalAmount = originalTransaction.Amount;

                        // Reassign the credit card Id to Transaction Model
                        transaction.SelectedCreditCardAccountId = originalTransaction.SelectedCreditCardAccountId;

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
                    throw new NotImplementedException($"{type} is not an accepted type for TransactionController.UpdateDbCreditCard method");
            }

            _db.SaveChanges();
        }

        public bool HandlePaycheckContributions(Transaction transaction)
        {
            try
            {
                if (transaction.Amount <= 0) return true; //only return false when exception is thrown
                var accountsWithContributions = _db.Accounts.Where(a => a.PaycheckContribution != null && a.PaycheckContribution > 0).ToList();
                var totalContributions = accountsWithContributions.Sum(a => a.PaycheckContribution);
                if (totalContributions > transaction.Amount)
                {
                }

                foreach (var account in accountsWithContributions)
                {
                    if (!TransferPaycheckContribution(transaction, account)) return false;
                    if (!AddContributionTransactionToDb(transaction, account)) return false;
                }

                var poolAccount = _db.Accounts.FirstOrDefault(a => a.IsPoolAccount);
                if (poolAccount == null) return false;

                poolAccount.Balance += transaction.Amount;
                _db.Entry(poolAccount).State = EntityState.Modified;


                _db.SaveChanges();
                return true;
            }
            catch (Exception e)
            {
                Logger.Instance.Error(e);
                return false;
            }
        }

        private bool TransferPaycheckContribution(Transaction transaction, Account account)
        {
            try
            {
                if (account.PaycheckContribution == null) return false;

                var contribution = (decimal)account.PaycheckContribution;
                transaction.Amount -= contribution;
                account.Balance += contribution;
                //_db.Entry(transaction).State = EntityState.Modified;
                _db.Entry(account).State = EntityState.Modified;


                _db.SaveChanges();
                return true;
            }
            catch (Exception e)
            {
                Logger.Instance.Error(e);
                return false;
            }
        }

        private bool AddContributionTransactionToDb(Transaction transaction, Account account)
        {
            try
            {
                if (account.PaycheckContribution == null) return false;

                var newTransaction = new Transaction();
                newTransaction.Date = transaction.Date;
                newTransaction.Payee = $"Transfer to {account.Name}";
                newTransaction.Category = CategoriesEnum.PaycheckContribution;
                newTransaction.Memo = "Paycheck Contribution";
                newTransaction.Type = TransactionTypesEnum.Transfer;
                newTransaction.DebitAccountId = account.Id;
                newTransaction.CreditAccountId = null;
                newTransaction.Amount = (decimal)account.PaycheckContribution;
                _db.Transactions.Add(newTransaction);


                _db.SaveChanges();
                return true;
            }
            catch (Exception e)
            {
                Logger.Instance.Error(e);
                return false;
            }
        }

        private void SetBillAsPaid(int? id)
        {
            try
            {
                var selectedBill = _db.Bills.FirstOrDefault(b => b.Id == id);
                if (selectedBill == null || selectedBill.IsPaid) return;
                selectedBill.IsPaid = true;


                _db.Entry(selectedBill).State = EntityState.Modified;
                _db.SaveChanges();
            }
            catch (Exception e)
            {
                Logger.Instance.Error(e);
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
    }
}

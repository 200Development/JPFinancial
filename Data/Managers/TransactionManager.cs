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
            //TODO: Create Transaction method needs to be more efficient.  Currently makes 5 calls to the DB. GetUsedAccounts(1),SetExpenseAsPaid(1),UpdateDbAccountBalances(2),AddTransactionToDb(1)
            
            try
            {
                GetUsedAccounts(entity);

                if (entity.Transaction.SelectedExpenseId != null)
                    if (!SetExpenseAsPaid(entity.Transaction.SelectedExpenseId, EventArgumentEnum.Create)) return false;

                UpdateDbAccountBalances(entity.Transaction, EventArgumentEnum.Create);
                Logger.Instance.DataFlow($"Account balances updated in data context");

                if (!AddTransactionToDb(entity)) return false;
                Logger.Instance.DataFlow($"Transaction added to database context");
                return true;

                
                //Testing to Update accounts after each transaction unless its a transfer
                //return entity.Transaction.Type == TransactionTypesEnum.Transfer || new Calculations().Rebalance();
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
                //if (entity.Transaction.UsedCreditCard)
                //{
                //    UpdateDbCreditCard(entity.Transaction, "edit");
                //    Logger.Instance.DataFlow($"Credit card used for transaction updated in data context");
                //}

                //if(entity.Transaction.SelectedExpenseId != null)

                _db.Entry(entity.Transaction).State = EntityState.Modified;
                Logger.Instance.DataFlow($"Transaction updated in data context");

                //if (entity.Transaction.UsedCreditCard)
                //{
                //    var creditCards = _db.CreditCards.ToList();
                //    var creditCard = creditCards.FirstOrDefault(c => c.Id == entity.Transaction.SelectedCreditCardAccountId);
                //    _db.Entry(creditCard).State = EntityState.Modified;
                //    Logger.Instance.DataFlow($"Credit card updated in data context");
                //}


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

                if (transaction.SelectedExpenseId != null)
                    if (!SetExpenseAsPaid(transaction.SelectedExpenseId, EventArgumentEnum.Delete)) return false;

                if (!UpdateDbAccountBalances(transaction, EventArgumentEnum.Delete)) return false;
                _db.Transactions.Remove(transaction);


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

                                // Update Account's required savings
                                new Calculations().UpdateRequiredSavings(transaction);
                                transaction.CreditAccount.BalanceSurplus = UpdateBalanceSurplus(transaction.CreditAccount);


                                _db.Entry(transaction.DebitAccount).State = EntityState.Modified;
                            }

                            if (transaction.CreditAccount != null && transaction.CreditAccount.Id != 0)
                            {
                                var originalBalance = transaction.CreditAccount.Balance; // logs beginning balance
                                transaction.CreditAccount.Balance -= transaction.Amount;
                                Logger.Instance.Calculation($"{transaction.CreditAccount.Name}.balance ({originalBalance}) + {transaction.Amount} = {transaction.CreditAccount.Balance}");

                                // Update Account's required savings
                                new Calculations().UpdateRequiredSavings(transaction);
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
                                _db.Accounts.FirstOrDefault(a => a.Id == (originalTransaction.CreditAccountId ?? 0));
                            var originalDebitAccount = _db.Accounts.FirstOrDefault(a => a.Id == (originalTransaction.DebitAccountId ?? 0));
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

                                            // Update Account's required savings
                                            new Calculations().UpdateRequiredSavings(transaction);

                                            originalDebitAccount.BalanceSurplus = UpdateBalanceSurplus(originalDebitAccount);
                                            _db.Entry(originalDebitAccount).State = EntityState.Modified;
                                        }

                                        if (originalCreditAccount != null)
                                        {
                                            originalCreditAccount.Balance += transaction.Amount;

                                            // Update Account's required savings
                                            new Calculations().UpdateRequiredSavings(transaction);

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
                newTransaction.Payee = entity.Transaction.SelectedExpenseId != null
                    ? _db.Expenses.FirstOrDefault(e => e.Id == entity.Transaction.SelectedExpenseId)?.Name
                    : entity.Transaction.Payee;
                newTransaction.Category = entity.Transaction.Category;
                newTransaction.Memo = entity.Transaction.Memo;
                newTransaction.Type = entity.Transaction.Type;
                newTransaction.DebitAccountId = entity.Transaction.DebitAccountId;
                newTransaction.CreditAccountId = entity.Transaction.CreditAccountId;
                newTransaction.Amount = entity.Transaction.Amount;
                //newTransaction.SelectedCreditCardAccountId = entity.Transaction.SelectedCreditCardAccountId;
                //newTransaction.UsedCreditCard = entity.Transaction.UsedCreditCard;
                if (entity.Transaction.SelectedExpenseId != null)
                    newTransaction.SelectedExpenseId = entity.Transaction.SelectedExpenseId;
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
        private decimal UpdateBalanceSurplus(Account account)
        {
            try
            {
                if (account.Balance < 0.0m)
                    return account.BalanceSurplus = account.Balance;

                if (account.BalanceLimit > 0)
                    return account.BalanceSurplus = account.Balance > account.BalanceLimit
                        ? account.Balance - account.BalanceLimit
                        : decimal.Zero;

                return account.BalanceSurplus = account.Balance - account.RequiredSavings;
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
                var accountsWithContributions = _db.Accounts.Where(a => a.PaycheckContribution > 0).ToList();
                var totalContributions = accountsWithContributions.Sum(a => a.PaycheckContribution);
                if (totalContributions > transaction.Amount)
                {
                }


                var poolAccount = _db.Accounts.FirstOrDefault(a => a.IsPoolAccount);
                if (poolAccount == null) return false;

                poolAccount.Balance += transaction.Amount;
                _db.Entry(poolAccount).State = EntityState.Modified;
                _db.SaveChanges();

                foreach (var account in accountsWithContributions)
                {
                    if (!TransferPaycheckContribution(poolAccount, account)) return false;
                    if (!AddContributionTransactionToDb(transaction, account)) return false;
                }


                return true;
            }
            catch (Exception e)
            {
                Logger.Instance.Error(e);
                return false;
            }
        }

        private bool TransferPaycheckContribution(Account poolAccount, Account account)
        {
            try
            {
                //if (account.PaycheckContribution == null) return false;

                var contribution = (decimal)account.PaycheckContribution;
                poolAccount.Balance -= contribution;
                account.Balance += contribution;
                //_db.Entry(transaction).State = EntityState.Modified;
                _db.Entry(account).State = EntityState.Modified;
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

        private bool AddContributionTransactionToDb(Transaction transaction, Account account)
        {
            try
            {
                //if (account.PaycheckContribution == null) return false;

                var newTransaction = new Transaction();
                newTransaction.Date = transaction.Date;
                newTransaction.Payee = $"Transfer to {account.Name}";
                newTransaction.Category = CategoriesEnum.PaycheckContribution;
                newTransaction.Memo = "Paycheck Contribution";
                newTransaction.Type = TransactionTypesEnum.Transfer;
                newTransaction.DebitAccountId = account.Id;
                newTransaction.CreditAccountId = null;
                newTransaction.Amount = account.PaycheckContribution;
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

        private bool SetExpenseAsPaid(int? expenseId, EventArgumentEnum eventArgument)
        {
            try
            {
                if (eventArgument == EventArgumentEnum.Create)
                {
                    var selectedExpense = _db.Expenses.FirstOrDefault(e => e.Id == expenseId);
                    if (selectedExpense == null)
                    {
                        Logger.Instance.Debug($"No Expense found with an ID of - {expenseId}");
                        return true;
                    }

                    selectedExpense.IsPaid = true;
                    _db.Entry(selectedExpense).State = EntityState.Modified;


                    _db.SaveChanges();
                    return true;
                }
                else
                {
                    var selectedExpense = _db.Expenses.FirstOrDefault(e => e.Id == expenseId);
                    if (selectedExpense == null)
                    {
                        Logger.Instance.Debug($"No Expense found with an ID of - {expenseId}");
                        return true;
                    }

                    selectedExpense.IsPaid = false;
                    _db.Entry(selectedExpense).State = EntityState.Modified;


                    _db.SaveChanges();
                    return true;
                }
            }
            catch (Exception e)
            {
                Logger.Instance.Error(e);
                return false;
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


                var transactions = _db.Transactions.ToList();
                var bills = _db.Bills.ToList();
                var incomeTransactions = transactions.Where(t => t.Type == TransactionTypesEnum.Income).ToList();
                var expenseTransactions = transactions.Where(t => t.Type == TransactionTypesEnum.Expense).ToList();
                var transferTransactions = transactions.Where(t => t.Type == TransactionTypesEnum.Transfer).ToList();

                var discretionaryExpenseTransactions = expenseTransactions.Join(bills, t => t.CreditAccountId, b => b.AccountId, (t, b) => new { transactions = t, bills = b }).Where(x => x.bills.IsMandatory == false);
                var discretionarySpendingSumsByMonth = discretionaryExpenseTransactions.Select(t => new { t.transactions.Date.Year, t.transactions.Date.Month, t.transactions.Amount })
                    .GroupBy(x => new { x.Year, x.Month }, (key, group) => new { year = key.Year, month = key.Month, expenses = group.Sum(k => k.Amount) }).ToList();

                var mandatoryTransactions = transactions.Join(bills, t => t.CreditAccountId, b => b.AccountId, (t, b) => new { transactions = t, bills = b }).Where(x => x.bills.IsMandatory);
                var mandatoryExpensesByMonth = mandatoryTransactions.Select(t => new { t.transactions.Date.Year, t.transactions.Date.Month, t.transactions.Amount })
                    .GroupBy(x => new { x.Year, x.Month }, (key, group) => new { year = key.Year, month = key.Month, expenses = group.Sum(k => k.Amount) }).ToList();

                var incomeTransactionsByMonth = incomeTransactions.Select(t => new { t.Date.Year, t.Date.Month, t.Amount })
                    .GroupBy(x => new { x.Year, x.Month }, (key, group) => new { year = key.Year, month = key.Month, expenses = group.Sum(k => k.Amount) }).ToList();

                var transferTransactionsByMonth = transferTransactions.Select(t => new { t.Date.Year, t.Date.Month, t.Amount })
                    .GroupBy(x => new { x.Year, x.Month }, (key, group) => new { year = key.Year, month = key.Month, expenses = group.Sum(k => k.Amount) }).ToList();

                var expensesByMonth = new Dictionary<DateTime, decimal>();
                var mandatoryByMonth = new Dictionary<DateTime, decimal>();
                var discretionaryByMonth = new Dictionary<DateTime, decimal>();
                var incomeByMonth = new Dictionary<DateTime, decimal>();
                var transfersByMonth = new Dictionary<DateTime, decimal>();

                foreach (var transaction in discretionarySpendingSumsByMonth)
                {
                    var date = new DateTime(transaction.year, transaction.month, 1);
                    var amount = transaction.expenses;
                    discretionaryByMonth.Add(date, amount);
                    expensesByMonth.Add(date, amount);
                }

                foreach (var transaction in mandatoryExpensesByMonth)
                {
                    var date = new DateTime(transaction.year, transaction.month, 1);
                    var amount = transaction.expenses;
                    mandatoryByMonth.Add(date, amount);
                    expensesByMonth[date] += amount;
                }

                foreach (var transaction in incomeTransactionsByMonth)
                {
                    var date = new DateTime(transaction.year, transaction.month, 1);
                    var amount = transaction.expenses;
                    incomeByMonth.Add(date, amount);
                }

                foreach (var transaction in transferTransactionsByMonth)
                {
                    var date = new DateTime(transaction.year, transaction.month, 1);
                    var amount = transaction.expenses;
                    transfersByMonth.Add(date, amount);
                }

                foreach (KeyValuePair<DateTime, decimal> transaction in mandatoryByMonth)
                {
                    if (mandatoryByMonth.ContainsKey(transaction.Key) == false)
                    {
                        mandatoryByMonth.Add(transaction.Key, 0m);
                    }

                    //if (discretionaryByMonth.ContainsKey(expense.Key) == false)
                    //{
                    //    discretionaryByMonth.Add(expense.Key, 0m);
                    //}
                }

                foreach (KeyValuePair<DateTime, decimal> transaction in discretionaryByMonth)
                {
                    if (discretionaryByMonth.ContainsKey(transaction.Key) == false)
                    {
                        discretionaryByMonth.Add(transaction.Key, 0m);
                    }

                    //if (mandatoryByMonth.ContainsKey(expense.Key) == false)
                    //{
                    //    mandatoryByMonth.Add(expense.Key, 0m);
                    //}
                }

                foreach (KeyValuePair<DateTime, decimal> transaction in expensesByMonth)
                {
                    if (expensesByMonth.ContainsKey(transaction.Key) == false)
                    {
                        expensesByMonth.Add(transaction.Key, 0m);
                    }

                    //if (mandatoryByMonth.ContainsKey(expense.Key) == false)
                    //{
                    //    mandatoryByMonth.Add(expense.Key, 0m);
                    //}
                }

                foreach (KeyValuePair<DateTime, decimal> transaction in incomeByMonth)
                {
                    if (incomeByMonth.ContainsKey(transaction.Key) == false)
                    {
                        incomeByMonth.Add(transaction.Key, 0m);
                    }

                    //if (mandatoryByMonth.ContainsKey(expense.Key) == false)
                    //{
                    //    mandatoryByMonth.Add(expense.Key, 0m);
                    //}
                }

                foreach (KeyValuePair<DateTime, decimal> transaction in transfersByMonth)
                {
                    if (transfersByMonth.ContainsKey(transaction.Key) == false)
                    {
                        transfersByMonth.Add(transaction.Key, 0m);
                    }

                    //if (mandatoryByMonth.ContainsKey(expense.Key) == false)
                    //{
                    //    mandatoryByMonth.Add(expense.Key, 0m);
                    //}
                }


                var oneYearAgo = DateTime.Today.AddYears(-1);
                var index = new DateTime(oneYearAgo.Year, oneYearAgo.Month, 1);

                for (DateTime i = index; i <= DateTime.Today; i = i.AddMonths(1))
                {
                    if (expensesByMonth.ContainsKey(i)) continue;
                    mandatoryByMonth.Add(i, 0m);
                    discretionaryByMonth.Add(i, 0m);
                    expensesByMonth.Add(i, 0m);
                    incomeByMonth.Add(i, 0m);
                    transfersByMonth.Add(i, 0m);
                }

                //var last12 = expensesByMonth.Take(11);
                //var orderedByYear = last12.OrderBy(expense => expense.Key.Year);
                //var thenOrderByMonth = orderedByYear.OrderBy(expense => expense.Key.Month);
                //var toDict = thenOrderByMonth.ToDictionary(expense => expense.Key, expense => expense.Value);

                metrics.MandatoryExpensesByMonth = mandatoryByMonth.Take(12).OrderBy(expense => expense.Key.Year).ThenBy(expense => expense.Key.Month).ToDictionary(expense => $"{ConvertMonthIntToString(expense.Key.Month)}{expense.Key.Year}", expense => expense.Value);
                metrics.DiscretionaryExpensesByMonth = discretionaryByMonth.Take(12).OrderBy(expense => expense.Key).ToDictionary(mandatory => $"{ConvertMonthIntToString(mandatory.Key.Month)}{mandatory.Key.Year}", mandatory => mandatory.Value);
                metrics.ExpensesByMonth = expensesByMonth.Take(12).OrderBy(expense => expense.Key).ToDictionary(disc => $"{ConvertMonthIntToString(disc.Key.Month)}{disc.Key.Year}", disc => disc.Value);
                metrics.IncomeByMonth = incomeByMonth.Take(12).OrderBy(expense => expense.Key).ToDictionary(disc => $"{ConvertMonthIntToString(disc.Key.Month)}{disc.Key.Year}", disc => disc.Value);
                metrics.TransfersByMonth = transfersByMonth.Take(12).OrderBy(expense => expense.Key).ToDictionary(disc => $"{ConvertMonthIntToString(disc.Key.Month)}{disc.Key.Year}", disc => disc.Value);


                return metrics;
            }
            catch (Exception e)
            {
                Logger.Instance.Error(e);
                return null;
            }
        }

        private string ConvertMonthIntToString(int month)
        {
            switch (month)
            {
                case 1:
                    return $"Jan";
                case 2:
                    return "Feb";
                case 3:
                    return "Mar";
                case 4:
                    return "Apr";
                case 5:
                    return "May";
                case 6:
                    return "Jun";
                case 7:
                    return "Jul";
                case 8:
                    return "Aug";
                case 9:
                    return "Sep";
                case 10:
                    return "Oct";
                case 11:
                    return "Nov";
                case 12:
                    return "Dec";
                default:
                    throw new NotImplementedException();
            }
        }
    }
}

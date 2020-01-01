using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using JPFData.Enumerations;
using JPFData.Metrics;
using JPFData.Models.JPFinancial;
using JPFData.ViewModels;


namespace JPFData.Managers
{
    /// <summary>
    /// Manages all read/write to database Transaction Table
    /// </summary>
    public class TransactionManager
    {
        private readonly ApplicationDbContext _db;
        private readonly Calculations _calc;
        private readonly string _userId;


        public TransactionManager()
        {
            _db = new ApplicationDbContext();
            _calc = new Calculations();
            _userId = Global.Instance.User != null ? Global.Instance.User.Id : string.Empty;
        }


        public List<Transaction> GetAllTransactions()
        {
            try
            {
                return _db.Transactions.Where(t => t.UserId == _userId).ToList();
            }
            catch (Exception e)
            {
                Logger.Instance.Error(e);
                throw;
            }
        }

        public Transaction GetTransaction(int? id)
        {
            try
            {
                return _db.Transactions.Find(id);
            }
            catch (Exception e)
            {
                Logger.Instance.Error(e);
                throw;
            }
        }

        public bool Create(TransactionViewModel entity)
        {
            try
            {
                GetUsedAccounts(entity);


                if (entity.Transaction.SelectedExpenseId != null)
                    if (!SetExpenseToPaid(entity.Transaction.SelectedExpenseId)) return false;

                UpdateDbAccountBalances(entity.Transaction, EventArgumentEnum.Create);

                return AddTransactionToDb(entity);
            }
            catch (Exception e)
            {
                Logger.Instance.Error(e);
                return false;
            }
        }

        public bool Edit(TransactionViewModel entity)
        {
            try
            {
                GetUsedAccounts(entity);
                //AsNoTracking() is essential or EF will throw an error
                UpdateDbAccountBalances(entity.Transaction, EventArgumentEnum.Update);
                Logger.Instance.DataFlow($"Account balances updated in data context");

                _db.Entry(entity.Transaction).State = EntityState.Modified;
                Logger.Instance.DataFlow($"Transaction updated in data context");


                _db.SaveChanges();
                return true;
            }
            catch (Exception e)
            {
                Logger.Instance.Error(e);
                return false;
            }
        }

        public bool Delete(int id)
        {
            try
            {
                Transaction transaction = _db.Transactions.Find(id);

                if (transaction.SelectedExpenseId != null)
                    if (!SetExpenseToUnpaid(transaction.SelectedExpenseId)) return false;

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
        private void GetUsedAccounts(TransactionViewModel entity)
        {
            try
            {
                if (entity.Transaction.CreditAccountId != null)
                {
                    entity.Transaction.CreditAccount = _db.Accounts.Find(entity.Transaction.CreditAccountId);
                    Logger.Instance.DataFlow($"Credit Account set");
                }

                //If income transaction, debit to pool account, else get selected debit account
                entity.Transaction.DebitAccount = entity.Transaction.Type == TransactionTypesEnum.Income ? _db.Accounts.FirstOrDefault(a => a.IsPoolAccount && a.UserId == _userId) : _db.Accounts.Find(entity.Transaction.DebitAccountId);
                Logger.Instance.DataFlow($"Debit Account set");
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
                                transaction.CreditAccount.BalanceSurplus = _calc.UpdateBalanceSurplus(transaction.CreditAccount);


                                _db.Entry(transaction.DebitAccount).State = EntityState.Modified;
                            }

                            if (transaction.CreditAccount != null && transaction.CreditAccount.Id != 0)
                            {
                                var originalBalance = transaction.CreditAccount.Balance; // logs beginning balance
                                transaction.CreditAccount.Balance -= transaction.Amount;
                                Logger.Instance.Calculation($"{transaction.CreditAccount.Name}.balance ({originalBalance}) + {transaction.Amount} = {transaction.CreditAccount.Balance}");

                                // Update Account's required savings
                                transaction.CreditAccount.BalanceSurplus = _calc.UpdateBalanceSurplus(transaction.CreditAccount);


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

                                            originalDebitAccount.BalanceSurplus = _calc.UpdateBalanceSurplus(originalDebitAccount);
                                            _db.Entry(originalDebitAccount).State = EntityState.Modified;
                                        }

                                        if (originalCreditAccount != null)
                                        {
                                            originalCreditAccount.Balance += transaction.Amount;

                                            // Update Account's required savings

                                            originalCreditAccount.BalanceSurplus = _calc.UpdateBalanceSurplus(originalCreditAccount);
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
                                            originalDebitAccount.BalanceSurplus = _calc.UpdateBalanceSurplus(originalDebitAccount);
                                            _db.Entry(originalDebitAccount).State = EntityState.Modified;
                                        }

                                        if (originalCreditAccount != null)
                                        {
                                            originalCreditAccount.Balance -= amountDifference;
                                            originalCreditAccount.BalanceSurplus = _calc.UpdateBalanceSurplus(originalCreditAccount);
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

        private bool AddTransactionToDb(TransactionViewModel entity)
        {
            try
            {
                var newTransaction = new Transaction();
                newTransaction.UserId = entity.Transaction.UserId;
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

        public bool HandlePaycheckContributions(Transaction transaction)
        {
            try
            {
                var accountManager = new AccountManager();
                transaction.DebitAccount = accountManager.GetAccount(transaction.DebitAccountId);
                var accountsWithContributions = accountManager.GetAllAccounts().Where(a => a.PaycheckContribution > 0).ToList();
                var totalContributions = accountsWithContributions.Sum(a => a.PaycheckContribution);
                var incomeAfterPaycheckContributions = transaction.Amount;

                if (totalContributions > transaction.Amount)
                {
                    // TODO: How to handle income not enough to cover all paycheck contributions
                }

                foreach (var account in accountsWithContributions)
                {
                    if (!UpdateAccountBalance(account, account.PaycheckContribution, AccountingTypes.Debit)) return false;
                    if (!AddTransferToDb(transaction, account)) return false;
                    incomeAfterPaycheckContributions -= account.PaycheckContribution;
                }

                Logger.Instance.Calculation($"Net income of {incomeAfterPaycheckContributions} added to {transaction.DebitAccount?.Name} after {totalContributions} in paycheck contributions was paid out");
                if(!UpdateAccountBalance(transaction.DebitAccount, incomeAfterPaycheckContributions, AccountingTypes.Debit)) return false;

                //TODO: find a better way to add remainder of income after paycheck contributions to Db
                var incomeAfterContributions = new Transaction();
                incomeAfterContributions.Date = transaction.Date;
                incomeAfterContributions.Payee = $"Transfer to {transaction.DebitAccount?.Name}";
                incomeAfterContributions.Category = transaction.Category;
                incomeAfterContributions.Memo = transaction.Memo;
                incomeAfterContributions.Type = transaction.Type;
                incomeAfterContributions.DebitAccountId = transaction.DebitAccountId;
                incomeAfterContributions.CreditAccountId = null;
                incomeAfterContributions.Amount = incomeAfterPaycheckContributions;

                _db.Transactions.Add(incomeAfterContributions);


                _db.SaveChanges();
                return true;
            }
            catch (Exception e)
            {
                Logger.Instance.Error(e);
                return false;
            }
        }

        private bool UpdateAccountBalance(Account account, decimal amount, AccountingTypes type)
        {
            try
            {
                switch (type)
                {
                    case AccountingTypes.Debit:
                        account.Balance += amount;
                        break;
                    case AccountingTypes.Credit:
                        account.Balance -= amount;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(type), type, null);
                }
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

        private bool AddTransferToDb(Transaction transaction, Account account)
        {
            try
            {
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

        private bool SetExpenseToPaid(int? expenseId)
        {
            try
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
            catch (Exception e)
            {
                Logger.Instance.Error(e);
                return false;
            }
        }

        private bool SetExpenseToUnpaid(int? expenseId)
        {
            try
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
            catch (Exception e)
            {
                Logger.Instance.Error(e);
                return false;
            }
        }

        public TransactionMetrics GetMetrics()
        {
            try
            {
                TransactionMetrics metrics = new TransactionMetrics();

                metrics.AccountMetrics = new AccountMetrics();
                metrics.CreditCardMetrics = new CreditCardMetrics();


                var transactions = _db.Transactions.Where(t => t.UserId == Global.Instance.User.Id).ToList();
                var incomeTransactions = transactions.Where(t => t.Type == TransactionTypesEnum.Income).ToList();
                var expenseTransactions = transactions.Where(t => t.Type == TransactionTypesEnum.Expense).ToList();
                var transferTransactions = transactions.Where(t => t.Type == TransactionTypesEnum.Transfer).ToList();

                var expenseTransactionsByMonth = expenseTransactions.Select(t => new { t.Date.Year, t.Date.Month, t.Amount })
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


                foreach (var transaction in expenseTransactionsByMonth)
                {
                    var date = new DateTime(transaction.year, transaction.month, 1);
                    var amount = transaction.expenses;
                    expensesByMonth.Add(date, amount);
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


                foreach (KeyValuePair<DateTime, decimal> transaction in expensesByMonth)
                {
                    if (expensesByMonth.ContainsKey(transaction.Key) == false)
                    {
                        expensesByMonth.Add(transaction.Key, 0m);
                    }
                }

                foreach (KeyValuePair<DateTime, decimal> transaction in incomeByMonth)
                {
                    if (incomeByMonth.ContainsKey(transaction.Key) == false)
                    {
                        incomeByMonth.Add(transaction.Key, 0m);
                    }
                }

                foreach (KeyValuePair<DateTime, decimal> transaction in transfersByMonth)
                {
                    if (transfersByMonth.ContainsKey(transaction.Key) == false)
                    {
                        transfersByMonth.Add(transaction.Key, 0m);
                    }
                }


                var oneYearAgo = DateTime.Today.AddYears(-1);
                var index = new DateTime(oneYearAgo.Year, oneYearAgo.Month, 1);

                for (DateTime i = index; i <= DateTime.Today; i = i.AddMonths(1))
                {
                    if (!mandatoryByMonth.ContainsKey(i))
                        mandatoryByMonth.Add(i, 0m);
                    if (!discretionaryByMonth.ContainsKey(i))
                        discretionaryByMonth.Add(i, 0m);
                    if (!expensesByMonth.ContainsKey(i))
                        expensesByMonth.Add(i, 0m);
                    if (!incomeByMonth.ContainsKey(i))
                        incomeByMonth.Add(i, 0m);
                    if (!transfersByMonth.ContainsKey(i))
                        transfersByMonth.Add(i, 0m);
                }

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

        public IEnumerable<Transaction> GetTransactionsBetweenDates(DateTime begin, DateTime end)
        {
            try
            {
                return _db.Transactions.Where(t => t.Date >= begin && t.Date < end).ToList();
            }
            catch (Exception e)
            {
                Logger.Instance.Error(e);
                return null;
            }
        }
    }

    public enum AccountingTypes
    {
        Debit,
        Credit
    }
}

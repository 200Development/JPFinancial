using System;
using System.Collections.Generic;
using System.Linq;
using JPFData.Enumerations;
using JPFData.Metrics;
using JPFData.Models.JPFinancial;


namespace JPFData.Managers
{
    public class DashboardManager
    {
        private readonly AccountManager _accountManager;
        private readonly BillManager _billManager;
        private readonly TransactionManager _transactionManager;
        private readonly ExpenseManager _expenseManager;
        //TODO: Solution to accurately calculate and define "savings"
        //TODO: Solution to get/calculate these user variables
        // set MinimumMonthlyExpenses to save duplicate calculations
        private static decimal _minimumMonthlyExpenses = 0.00m;
        private const decimal GrossIncome = 100000;
        private const int Age = 45;


        public DashboardManager()
        {
            _accountManager = new AccountManager();
            _billManager = new BillManager();
            _transactionManager = new TransactionManager();
            _expenseManager = new ExpenseManager();

            ValidationErrors = new List<KeyValuePair<string, string>>();
        }



        public List<KeyValuePair<string, string>> ValidationErrors { get; set; }

        public DashboardMetrics RefreshStaticMetrics()
        {
            var metrics = new DashboardMetrics();

            try
            {
                metrics.DisposableIncome = GetDisposableIncome();
                metrics.TargetedNetWorth = GetTargetedNetWorth();
                metrics.SavingsRate = GetSavingsRate();
                metrics.BudgetRuleExpense = GetBudgetRuleExpenses();
                metrics.BudgetRuleSavings = GetBudgetRuleSavings();
                metrics.BudgetRuleDiscretionary = GetBudgetRuleDiscretionary();
                metrics.MinimumMonthlyExpenses = _minimumMonthlyExpenses = GetMinimumMonthlyExpenses();
                metrics.CashFlowByMonth = GetCashFlowByMonth();
                metrics.EmergencyFundRatio = GetEmergencyFundRatio();
                metrics.DueBeforeNextPayPeriod = GetDueBeforeNextPayPeriod();
                metrics.CashBalance = GetCashBalance();


                return metrics;
            }
            catch (Exception e)
            {
                Logger.Instance.Error(e);
                return new DashboardMetrics();
            }
        }

        private decimal GetDisposableIncome()
        {
            try
            {
                return _accountManager.GetPoolAccount().Balance;
            }
            catch (Exception e)
            {
                Logger.Instance.Error(e);
                throw;
            }
        }

        private static decimal GetTargetedNetWorth()
        {
            try
            {
                // TODO: Add income & age
                return Age * (GrossIncome / 10);
            }
            catch (Exception e)
            {
                Logger.Instance.Error(e);
                throw;
            }
        }

        private decimal GetSavingsRate()
        {
            try
            {
                return  _accountManager.GetEmergencyFundAccount().Balance / GrossIncome;
            }
            catch (Exception e)
            {
                Logger.Instance.Error(e);
                throw;
            }
        }

        private static decimal GetBudgetRuleExpenses()
        {
            try
            {
                return GrossIncome * .50m;
            }
            catch (Exception e)
            {
                Logger.Instance.Error(e);
                throw;
            }
        }

        private static decimal GetBudgetRuleSavings()
        {
            try
            {
                return GrossIncome * .20m;
            }
            catch (Exception e)
            {
                Logger.Instance.Error(e);
                throw;
            }
        }

        private static decimal GetBudgetRuleDiscretionary()
        {
            try
            {
                return GrossIncome * .30m;
            }
            catch (Exception e)
            {
                Logger.Instance.Error(e);
                throw;
            }
        }

        private decimal GetMinimumMonthlyExpenses()
        {
            try
            {
                var totalMonthlyExpense = 0.00m;
                var bills = _billManager.GetAllBills();

                foreach (Bill bill in bills)
                {
                    switch (bill.PaymentFrequency)
                    {
                        case FrequencyEnum.Daily:
                            totalMonthlyExpense += bill.AmountDue * 365 / 12;
                            break;
                        case FrequencyEnum.Weekly:
                            totalMonthlyExpense += bill.AmountDue * 52 / 12;
                            break;
                        case FrequencyEnum.BiWeekly:
                            totalMonthlyExpense += bill.AmountDue * 26 / 12;
                            break;
                        case FrequencyEnum.Monthly:
                            totalMonthlyExpense += bill.AmountDue;
                            break;
                        case FrequencyEnum.SemiMonthly:
                            totalMonthlyExpense += bill.AmountDue * 2;
                            break;
                        case FrequencyEnum.Quarterly:
                            totalMonthlyExpense += bill.AmountDue / 3;
                            break;
                        case FrequencyEnum.SemiAnnually:
                            totalMonthlyExpense += bill.AmountDue / 6;
                            break;
                        case FrequencyEnum.Annually:
                            totalMonthlyExpense += bill.AmountDue / 12;
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }

                return Math.Round(totalMonthlyExpense, 2);
            }
            catch (Exception e)
            {
                Logger.Instance.Error(e);
                throw;
            }
        }
        
        private Dictionary<string, decimal> GetCashFlowByMonth()
        {
            try
            {
                var allTransactions = _transactionManager.GetAllTransactions();

                var transactionsByMonth = allTransactions
                    .Select(t => new { t.Date.Year, t.Date.Month, t.Amount, t.Type })
                    .GroupBy(t => new { t.Year, t.Month },
                        (key, group) => new { year = key.Year, month = key.Month, cashflow = group.Sum(k => k.Type == TransactionTypesEnum.Expense ? -1 * k.Amount : k.Amount) }).ToList();

                var transactionsByMonthDict = new Dictionary<DateTime, decimal>();

                foreach (var item in transactionsByMonth)
                {
                    var date = new DateTime(item.year, item.month, 1);
                    var amount = item.cashflow;
                    transactionsByMonthDict.Add(date, amount);
                }


                foreach (var pair in transactionsByMonthDict.Where(pair => transactionsByMonthDict.ContainsKey(pair.Key) == false))
                {
                    transactionsByMonthDict.Add(pair.Key, 0m);
                }


                var oneYearAgo = DateTime.Today.AddYears(-1);
                var index = new DateTime(oneYearAgo.Year, oneYearAgo.Month, 1);

                for (DateTime i = index; i <= DateTime.Today; i = i.AddMonths(1))
                {
                    if (!transactionsByMonthDict.ContainsKey(i))
                        transactionsByMonthDict.Add(i, 0m);
                }

                return transactionsByMonthDict.Take(12).OrderBy(t => t.Key.Year).ThenBy(t => t.Key.Month)
                    .ToDictionary(t => $"{ConvertMonthIntToString(t.Key.Month)} {t.Key.Year}", t => t.Value);
            }
            catch (Exception e)
            {
                Logger.Instance.Error(e);
                throw;
            }
        }

        private decimal GetEmergencyFundRatio()
        {
            try
            {
                return _accountManager.GetEmergencyFundAccount().Balance / _minimumMonthlyExpenses;

            }
            catch (Exception e)
            {
                Logger.Instance.Error(e);
                throw;
            }
        }

        private decimal GetDueBeforeNextPayPeriod()
        {
            try
            {
                var nextPayday = new DateTime(2020,4,3);
                return _expenseManager.GetAllUnpaidExpenses().Where(e => e.Due < nextPayday).Sum(e => e.Amount);
            }
            catch (Exception e)
            {
                Logger.Instance.Error(e);
                throw;
            }
        }

        private decimal GetCashBalance()
        {
            var accountBalanceSum = _accountManager.GetAllAccounts().Sum(a => a.Balance);
            accountBalanceSum += _accountManager.GetPoolAccount().Balance;


            return accountBalanceSum;
        }

        private string ConvertMonthIntToString(int month)
        {
            switch (month)
            {
                case 1:
                    return "Jan";
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
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}

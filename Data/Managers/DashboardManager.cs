using System;
using System.Collections.Generic;
using System.Linq;
using JPFData.DTO;
using JPFData.Metrics;


namespace JPFData.Managers
{
    public class DashboardManager
    {
        private readonly ApplicationDbContext _db = new ApplicationDbContext();
        private readonly Calculations _calculations = new Calculations();


        public DashboardManager()
        {
            ValidationErrors = new List<KeyValuePair<string, string>>();
        }


        public List<KeyValuePair<string, string>> ValidationErrors { get; set; }


        public DashboardDTO Get()
        {
            return Get(new DashboardDTO());
        }

        public DashboardDTO Get(DashboardDTO entity)
        {
            DashboardDTO ret = new DashboardDTO();
            TransactionManager tManager = new TransactionManager();

 
            ret.StaticFinancialMetrics = RefreshFinancialMetrics(ret);
            ret.TimePeriodMetrics = RefreshTimePeriodMetrics(ret);
            return ret;
        }

        private StaticFinancialMetrics RefreshFinancialMetrics(DashboardDTO dto)
        {
            StaticFinancialMetrics metrics = new StaticFinancialMetrics();
            //TODO: create StaticFinancialMetrics manager class 

            var income = _calculations.GetMonthlyIncome();
            var bills = _db.Bills.ToList();
            var loans = _db.Loans.ToList();
            var transactions = _db.Transactions.ToList();
            var accounts = _db.Accounts.ToList();

            /* LOAN METRICS */
            var monthlyLoanInterest = 0.0m;
            var dailyLoanInterest = 0.0m;
            foreach (var loan in loans)
            {
                monthlyLoanInterest += Calculations.MonthlyInterest(loan);
                dailyLoanInterest += Calculations.DailyInterest(loan);
            }
            if (income > 0)
                metrics.LoanInterestPercentOfIncome = monthlyLoanInterest / income;
            metrics.MonthlyLoanInterest = monthlyLoanInterest;
            if (income > 0)
                metrics.DailyLoanInterestPercentage = dailyLoanInterest / income;
            metrics.DailyLoanInterest = dailyLoanInterest;

            /* CURRENT MONTH'S EXPENSES */
            //TODO: difference between totalDue and billsDue?
            //TODO: what counts as expenses, mandatory expenses, discretionary spending?
            //metrics.MandatoryExpenses = bills.Where(b => b.DueDate.Month == DateTime.Today.Month).Where(b => b.IsMandatory).Sum(b => b.AmountDue);
            //metrics.DiscretionaryExpenses = bills.Where(b => b.DueDate.Month == DateTime.Today.Month).Where(b => !b.IsMandatory).Sum(b => b.AmountDue);
            metrics.Expenses = bills.Where(b => b.DueDate.Month == DateTime.Today.Month && b.DueDate.Year == DateTime.Today.Year).Sum(b => b.AmountDue);

            /* PAST MONTHLY EXPENSES */
            var lastMonth = DateTime.Today.AddMonths(-1);
            var lastMonthFirstDay = _calculations.FirstDayOfMonth(lastMonth.Year, lastMonth.Month);
            var lastMonthLastDay = _calculations.LastDayOfMonth(lastMonth);
            metrics.LastMonthDiscretionarySpending = _calculations.DiscretionarySpendingByDateRange(lastMonthFirstDay, lastMonthLastDay);
            metrics.LastMonthMandatoryExpenses = _calculations.ExpensesByDateRange(lastMonthFirstDay, lastMonthLastDay);

            /* RANKED EXPENSES */
            var costliestExpense = transactions.Where(t => t.Date.Month == lastMonth.Month).OrderByDescending(t => t.Amount).Select(t => t.Amount).Take(1).FirstOrDefault();
            metrics.CostliestExpenseAmount = costliestExpense;
            metrics.CostliestCategory = transactions.Where(t => t.Date.Month == lastMonth.Month).OrderByDescending(t => t.Amount).Select(t => t.Category).Take(1).FirstOrDefault();
            if (income > 0)
                metrics.CostliestExpensePercentage = costliestExpense / income;

            /* AVERAGES */
            var transactionSumsByMonth = transactions.Select(t => new { t.Date.Year, t.Date.Month, t.Amount })
                .GroupBy(x => new { x.Year, x.Month }, (key, group) => new { year = key.Year, month = key.Month, expenses = group.Sum(k => k.Amount) }).ToList();

            //var discretionaryTransactions = transactions.Join(bills, t => t.CreditAccountId, b => b.AccountId, (t, b) => new { transactions = t, bills = b }).Where(x => x.bills.IsMandatory == false);

            //var discretionarySpendingSumsByMonth = discretionaryTransactions.Select(t => new { t.transactions.Date.Year, t.transactions.Date.Month, t.transactions.Amount })
            //    .GroupBy(x => new { x.Year, x.Month }, (key, group) => new { year = key.Year, month = key.Month, expenses = group.Sum(k => k.Amount) }).ToList();

            //var mandatoryTransactions = transactions.Join(bills, t => t.CreditAccountId, b => b.AccountId, (t, b) => new { transactions = t, bills = b }).Where(x => x.bills.IsMandatory);

            //var mandatoryExpensesByMonth = mandatoryTransactions.Select(t => new { t.transactions.Date.Year, t.transactions.Date.Month, t.transactions.Amount })
                //.GroupBy(x => new { x.Year, x.Month }, (key, group) => new { year = key.Year, month = key.Month, expenses = group.Sum(k => k.Amount) }).ToList();

            var last3MonthsTransactions = transactionSumsByMonth.OrderByDescending(t => t.year).ThenByDescending(x => x.month).Take(3);
            var quarterlyTransactionAverageCost = last3MonthsTransactions.Average(t => t?.expenses);
            var lastMonthSpending = transactionSumsByMonth.FirstOrDefault(t => t.year == lastMonth.Year && t.month == lastMonth.Month)?.expenses;
            var lastMonthSpendingVsMovingAvg = (lastMonthSpending - quarterlyTransactionAverageCost) / lastMonthSpending;
            metrics.AverageMonthlyExpenses3MMA = quarterlyTransactionAverageCost;
            metrics.PercentageChangeExpenses = lastMonthSpendingVsMovingAvg;

            var expensesByMonth = new Dictionary<DateTime, decimal>();
            var mandatoryByMonth = new Dictionary<DateTime, decimal>();
            var discretionaryByMonth = new Dictionary<DateTime, decimal>();
            foreach (var transaction in transactionSumsByMonth)
            {
                var date = new DateTime(transaction.year, transaction.month, 1);
                var amount = transaction.expenses;
                expensesByMonth.Add(date, amount);
            }

            //foreach (var transaction in discretionarySpendingSumsByMonth)
            //{
            //    var date = new DateTime(transaction.year, transaction.month, 1);
            //    var amount = transaction.expenses;
            //    discretionaryByMonth.Add(date, amount);
            //}

            //foreach (var transaction in mandatoryExpensesByMonth)
            //{
            //    var date = new DateTime(transaction.year, transaction.month, 1);
            //    var amount = transaction.expenses;
            //    mandatoryByMonth.Add(date, amount);
            //}

            foreach (KeyValuePair<DateTime, decimal> expense in expensesByMonth)
            {
                if (mandatoryByMonth.ContainsKey(expense.Key) == false)
                {
                    mandatoryByMonth.Add(expense.Key, 0m);
                }

                if (discretionaryByMonth.ContainsKey(expense.Key) == false)
                {
                    discretionaryByMonth.Add(expense.Key, 0m);
                }
            }

            foreach (KeyValuePair<DateTime, decimal> expense in mandatoryByMonth)
            {
                if (expensesByMonth.ContainsKey(expense.Key) == false)
                {
                    expensesByMonth.Add(expense.Key, 0m);
                }

                if (discretionaryByMonth.ContainsKey(expense.Key) == false)
                {
                    discretionaryByMonth.Add(expense.Key, 0m);
                }
            }

            foreach (KeyValuePair<DateTime, decimal> expense in discretionaryByMonth)
            {
                if (expensesByMonth.ContainsKey(expense.Key) == false)
                {
                    expensesByMonth.Add(expense.Key, 0m);
                }

                if (mandatoryByMonth.ContainsKey(expense.Key) == false)
                {
                    mandatoryByMonth.Add(expense.Key, 0m);
                }
            }

            var oneYearAgo = DateTime.Today.AddYears(-1);
            var index = new DateTime(oneYearAgo.Year, oneYearAgo.Month, 1);

            for (DateTime i = index; i <= DateTime.Today; i = i.AddMonths(1))
            {
                if (expensesByMonth.ContainsKey(i)) continue;
                expensesByMonth.Add(i, 0m);
                mandatoryByMonth.Add(i, 0m);
                discretionaryByMonth.Add(i, 0m);
            }

            var last12 = expensesByMonth.Take(11);
            var orderedByYear = last12.OrderBy(expense => expense.Key.Year);
            var thenOrderByMonth = orderedByYear.OrderBy(expense => expense.Key.Month);
            var toDict = thenOrderByMonth.ToDictionary(expense => expense.Key, expense => expense.Value);

            metrics.ExpensesByMonth = expensesByMonth.Take(12).OrderBy(expense => expense.Key.Year).ThenBy(expense => expense.Key.Month).ToDictionary(expense => $"{ConvertMonthIntToString(expense.Key.Month)}{expense.Key.Year}", expense => expense.Value);
            metrics.MandatoryExpensesByMonth = mandatoryByMonth.Take(12).OrderBy(expense => expense.Key).ToDictionary(mandatory => $"{ConvertMonthIntToString(mandatory.Key.Month)}{mandatory.Key.Year}", mandatory => mandatory.Value);
            metrics.DiscretionarySpendingByMonth = discretionaryByMonth.Take(12).OrderBy(expense => expense.Key).ToDictionary(disc => $"{ConvertMonthIntToString(disc.Key.Month)}{disc.Key.Year}", disc => disc.Value);


            return metrics;
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

        private TimePeriodFinancialMetrics RefreshTimePeriodMetrics(DashboardDTO entity)
        {
            TimePeriodFinancialMetrics metric = new TimePeriodFinancialMetrics();

            metric.OneMonthSavings = _calculations.FutureValue(DateTime.Today.AddMonths(1), entity.StaticFinancialMetrics.NetIncome);
            metric.ThreeMonthsSavings = _calculations.FutureValue(DateTime.Today.AddMonths(3), entity.StaticFinancialMetrics.NetIncome);
            metric.SixMonthsSavings = _calculations.FutureValue(DateTime.Today.AddMonths(6), entity.StaticFinancialMetrics.NetIncome);
            metric.OneYearSavings = _calculations.FutureValue(DateTime.Today.AddYears(1), entity.StaticFinancialMetrics.NetIncome);
            metric.MonthlyExpenses = entity.StaticFinancialMetrics.TotalDue;
            metric.MonthlyIncome = (Convert.ToDecimal(_db.Salaries.Select(s => s.NetIncome).FirstOrDefault()) * 2);


            return metric;
        }


        private bool Validate(DashboardDTO entity)
        {
            ValidationErrors.Clear();

            // EXAMPLE
            //if (entity.CreateTransaction.Amount <= 0)
            //{
            //    ValidationErrors.Add(new
            //        KeyValuePair<string, string>("Transaction",
            //            "Transaction amount must be greater than 0."));
            //}

            return (ValidationErrors.Count == 0);
        }
    }
}

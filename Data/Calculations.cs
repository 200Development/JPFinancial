using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using JPFData.DTO;
using JPFData.Enumerations;
using JPFData.Models;

namespace JPFData
{
    public class Calculations
    {
        //TODO: Update error handling
        private readonly ApplicationDbContext _db = new ApplicationDbContext();


        public int PayPeriodsTilDue(DateTime? dueDate)
        {
            try
            {
                var payPeriods = 0;
                var today = DateTime.Today;
                var month = DateTime.Today.Month;
                var year = DateTime.Today.Year;
                var firstDayOfMonth = new DateTime(year, month, 1);
                var firstPaycheckDate = new DateTime(year, month, 15);

                while (dueDate > today)
                {
                    if (today >= firstDayOfMonth && today <= firstPaycheckDate) // 1st - 15th of the month
                    {
                        payPeriods += 1;
                        today = firstPaycheckDate.AddDays(1);
                    }
                    else // 16th through the last day of the month
                    {
                        payPeriods += 1;
                        firstDayOfMonth = firstDayOfMonth.AddMonths(1);
                        firstPaycheckDate = firstPaycheckDate.AddMonths(1);
                        today = firstDayOfMonth;
                    }
                }
                return payPeriods - 1 < 0 ? 0 : payPeriods - 1;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public IEnumerable<Bill> BillsDue(DateTime firstPaycheck, DateTime firstDayOfMonth, DateTime lastPaycheck)
        {
            try
            {
                IList<Bill> billsDue;

                if (DateTime.Today < firstPaycheck) // Bills with due dates 1st - 15th of the current month
                {
                    billsDue = (from b in _db.Bills
                                where b.DueDate >= DateTime.Today && (b.DueDate > firstDayOfMonth && b.DueDate <= firstPaycheck)
                                select b).ToList();
                }
                else // Bills with due dates 16th - last day of the current month
                {
                    billsDue = (from b in _db.Bills
                                where b.DueDate > DateTime.Today && (b.DueDate > firstPaycheck && b.DueDate <= lastPaycheck)
                                select b).ToList();
                }

                return billsDue;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public DateTime LastDayOfMonth(DateTime date)
        {
            try
            {
                //return date.AddMonths(1).AddDays(-date.Day).Day;
                return new DateTime(date.Year, date.Month, DateTime.DaysInMonth(date.Year, date.Month));
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public DateTime FirstDayOfMonth(int year, int month)
        {
            try
            {
                return new DateTime(year, month, 1);
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        private void UpdatePaycheckContributions()
        {
            try
            {
                /* Update paycheck contributions for accounts with assigned bills */
                var accounts = _db.Accounts.ToList();
                var bills = _db.Bills.ToList();
                var transactions = _db.Transactions.ToList();

                //Zeros out all accounts req paycheck contributions
                foreach (var account in accounts)
                {
                    account.SuggestedPaycheckContribution = decimal.Zero;
                }

                foreach (var bill in _db.Bills.ToList())
                {
                    var billTotal = bill.AmountDue;

                    // get the account assigned to the bill
                    Account account = accounts.FirstOrDefault(a => a.Id == bill.AccountId);

                    //TODO: Needs to account for all pay frequencies
                    //TODO: Suggested contribution assumes payday twice a month.  need to update to include other options
                    if (account == null) continue;
                    switch (bill.PaymentFrequency)
                    {
                        case FrequencyEnum.Annually:
                            account.SuggestedPaycheckContribution += billTotal / 24;
                            break;
                        case FrequencyEnum.SemiAnnually:
                            account.SuggestedPaycheckContribution += billTotal / 12;
                            break;
                        case FrequencyEnum.Quarterly:
                            account.SuggestedPaycheckContribution += billTotal / 6;
                            break;
                        case FrequencyEnum.SemiMonthly: // every 2 months
                            account.SuggestedPaycheckContribution += billTotal / 4;
                            break;
                        case FrequencyEnum.Monthly:
                            account.SuggestedPaycheckContribution += billTotal / 2;
                            break;
                        case FrequencyEnum.Weekly:
                            account.SuggestedPaycheckContribution += billTotal * 2;
                            break;
                        case FrequencyEnum.BiWeekly:
                            account.SuggestedPaycheckContribution = billTotal;
                            break;
                        case FrequencyEnum.Daily:
                            break;
                        default:
                            account.SuggestedPaycheckContribution += billTotal / 2;
                            break;
                    }
                }


                // SQL that does same calculation as the following code
                /*   select a.Name, sum(t.Amount)
                     from Accounts a
                     left join Transactions t
                     on t.CreditAccountId = a.Id
                     where t.Amount is not null
                     group by a.Name                */
                var joinAccountsBills = accounts.SelectMany(
                    account => bills.Where(bill => account.Id == bill.AccountId).DefaultIfEmpty(),
                    (account, bill) => new
                    {
                        Account = account,
                        Bill = bill
                    });
                //Get only Accounts that don't have any association with Bills
                //Reason: I want all the accounts that don't have regular bills so i can calculate an avg. paycheck contribution suggestion based on past spending
                var accountsWithoutBills = (from @join in joinAccountsBills where @join.Bill == null select @join.Account).ToList();

                foreach (var transaction in transactions)
                {
                    //get transaction account
                    Account account = accountsWithoutBills.FirstOrDefault(a => a.Id == transaction.CreditAccountId);

                    if (account != null)
                        account.SuggestedPaycheckContribution += transaction.Amount;
                }


                //Get last 90 days of transactions
                var filteredTransactions = transactions.Where(t => t.Date > DateTime.Today.AddDays(-90)).ToList();

                //Get oldest transaction within 90 days
                var earliestTransaction = filteredTransactions.OrderBy(t => t.Date).FirstOrDefault();

                var totalDaysAgo = 0;
                if (earliestTransaction != null)
                {
                    totalDaysAgo = DateTime.Today.Subtract(earliestTransaction.Date).Days;
                }

                foreach (var account in accountsWithoutBills)
                {
                    var cost = filteredTransactions.Where(t => t.CreditAccount != null).Where(t => t.CreditAccount.Name == account.Name).Sum(t => t.Amount);
                    var costPerDay = cost / totalDaysAgo;
                    account.SuggestedPaycheckContribution = costPerDay * 15; //rough for paid twice a month todo: add better algorithm to calculate
                }

                _db.SaveChanges();
            }
            catch (Exception e)
            {
                // ignored
            }
        }

        public void UpdateAccountGoals(IEnumerable<Account> accounts, Dictionary<string, decimal> accountBalances)
        {
            try
            {
                foreach (var account in accounts)
                {
                    var valuesFound = false;
                    decimal totalSavings = 0;

                    foreach (var savings in accountBalances)
                    {
                        if (savings.Key != account.Name) continue;
                        totalSavings += savings.Value;
                        valuesFound = true;
                    }
                    if (valuesFound)
                    {
                        account.PaycheckContribution = totalSavings;
                    }
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public Dictionary<string, string> UpdateBillDueDates(Dictionary<string, string> billsDictionary)
        {
            try
            {
                var bills = _db.Bills.ToList();
                var beginDate = Convert.ToDateTime(billsDictionary["currentDate"]);

                foreach (var bill in bills)
                {
                    if (bill.DueDate.Date > beginDate) continue;

                    var frequency = bill.PaymentFrequency;
                    var dueDate = bill.DueDate;
                    var newDueDate = dueDate;

                    while (newDueDate < beginDate)
                    {
                        switch (frequency)
                        {
                            case FrequencyEnum.Daily:
                                newDueDate = newDueDate.AddDays(1);
                                break;
                            case FrequencyEnum.Weekly:
                                newDueDate = newDueDate.AddDays(7);
                                break;
                            case FrequencyEnum.BiWeekly:
                                newDueDate = newDueDate.AddDays(14);
                                break;
                            case FrequencyEnum.Monthly:
                                newDueDate = newDueDate.AddMonths(1);
                                break;
                            case FrequencyEnum.SemiMonthly:
                                newDueDate = newDueDate.AddDays(15);
                                break;
                            case FrequencyEnum.Quarterly:
                                newDueDate = newDueDate.AddMonths(3);
                                break;
                            case FrequencyEnum.SemiAnnually:
                                newDueDate = newDueDate.AddMonths(6);
                                break;
                            case FrequencyEnum.Annually:
                                newDueDate = newDueDate.AddYears(1);
                                break;
                            default:
                                throw new ArgumentOutOfRangeException();
                        }
                        billsDictionary[bill.Name] = newDueDate.ToShortDateString();
                    }
                }
                return billsDictionary;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        //TODO: Improve date range handling
        /// <summary>
        /// Returns summation of bills within the date range of the begin and end parameters  
        /// </summary>
        /// <param name="begin">start date of the date range</param>
        /// <param name="end">end date of the date range</param>
        /// <param name="onlyMandatory">summates only mandatory expenses</param>
        /// <returns></returns>
        public decimal ExpensesByDateRange(DateTime begin, DateTime end, bool onlyMandatory = false)
        {
            try
            {
                // onlyMandatory sums only mandatory expenses
                var bills = onlyMandatory ? _db.Bills.Where(b => b.IsMandatory).ToList() : _db.Bills.ToList();
                var expenses = 0m;

                foreach (var bill in bills)
                {
                    //if (bill.DueDate.Date < beginDate) continue;

                    var frequency = bill.PaymentFrequency;
                    var dueDate = bill.DueDate;
                    var newDueDate = dueDate;

                    //TODO: Fix semi-monthly bills being added 3 times (31st, 16th, 1st)
                    while (newDueDate >= begin)
                    {
                        switch (frequency)
                        {
                            case FrequencyEnum.Daily:
                                newDueDate = newDueDate.AddDays(-1);
                                break;
                            case FrequencyEnum.Weekly:
                                newDueDate = newDueDate.AddDays(-7);
                                break;
                            case FrequencyEnum.BiWeekly:
                                newDueDate = newDueDate.AddDays(-14);
                                break;
                            case FrequencyEnum.Monthly:
                                newDueDate = newDueDate.AddMonths(-1);
                                break;
                            case FrequencyEnum.SemiMonthly:
                                newDueDate = newDueDate.AddDays(-15);
                                break;
                            case FrequencyEnum.Quarterly:
                                newDueDate = newDueDate.AddMonths(-3);
                                break;
                            case FrequencyEnum.SemiAnnually:
                                newDueDate = newDueDate.AddMonths(-6);
                                break;
                            case FrequencyEnum.Annually:
                                newDueDate = newDueDate.AddYears(-1);
                                break;
                            default:
                                throw new ArgumentOutOfRangeException();
                        }
                        // adds expense only if the bill due date falls within the date range
                        if (newDueDate >= begin && newDueDate < end)
                            expenses += bill.AmountDue;
                    }
                }
                return expenses;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public decimal DiscretionarySpendingByDateRange(DateTime begin, DateTime end)
        {
            try
            {
                var ret = 0m;
                var transactions = _db.Transactions.Where(t => t.Date >= begin && t.Date < end).ToList();
                var bills = _db.Bills.ToList();
                var isBill = false;

                foreach (var transaction in transactions)
                {
                    foreach (var bill in bills)
                    {
                        // If the bill name matches the transaction payee, count the transaction as a bill (mandatory expense)
                        if (bill.Name.Equals(transaction.Payee))
                            isBill = true;
                    }
                    // If transaction is not a bill, add to discretionary spending total
                    if (!isBill)
                        ret += transaction.Amount;
                }
                return ret;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public DateTime SavingsDate(decimal futureValue, decimal netPay)
        {
            try
            {
                var date = DateTime.Today;
                var billsFromDb = _db.Bills.ToList();

                var bills = new Dictionary<string, string>
                {
                    {"currentDate", DateTime.Today.ToShortDateString()},
                    {
                        "endDate",
                        DateTime.Today.Day <= 14
                            ? new DateTime(date.Year, date.Month, 15).ToShortDateString()
                            : new DateTime(date.Year, date.Month, LastDayOfMonth(date).Day).ToShortDateString()
                    },
                    {"periodCosts", "0"},
                    {"totalCosts", "0"},
                    {"totalSavings", "0"}
                };

                foreach (var bill in billsFromDb)
                {
                    bills.Add(bill.Name, bill.DueDate.ToShortDateString());
                }

                while (Convert.ToDecimal(bills["totalSavings"]) < futureValue)
                {
                    bills = UpdateBillDueDates(bills);
                    bills = UpdateTotalCosts(bills);
                    SetCurrentAndEndDate(bills);
                    var savings = Convert.ToDecimal(bills["totalSavings"]);
                    savings += netPay;
                    bills["totalSavings"] = savings.ToString(CultureInfo.InvariantCulture);
                }
                return Convert.ToDateTime(bills["endDate"]);
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public decimal FutureValue(DateTime futureDate, decimal? netPay)
        {
            try
            {
                var payperiods = PayPeriodsTilDue(futureDate);
                var date = DateTime.Today;
                var billsFromDb = _db.Bills.ToList();

                var bills = new Dictionary<string, string>
                {
                    {"currentDate", DateTime.Today.ToShortDateString()},
                    {"endDate", DateTime.Today.Day <= 14
                            ? new DateTime(date.Year, date.Month, 15).ToShortDateString()
                            : new DateTime(date.Year, date.Month, LastDayOfMonth(date).Day).ToShortDateString()
                    },
                    {"periodCosts", "0"},
                    {"totalCosts", "0"},
                    {"totalSavings", "0"}
                };

                foreach (var bill in billsFromDb)
                {
                    bills.Add(bill.Name, bill.DueDate.ToShortDateString());
                }

                for (var i = 0; i < payperiods; i++)
                {

                    bills = UpdateBillDueDates(bills);
                    bills = UpdateTotalCosts(bills);
                    SetCurrentAndEndDate(bills);
                    decimal? savings = Convert.ToDecimal(bills["totalSavings"].ToString());
                    var periodCosts = Convert.ToDecimal(bills["periodCosts"].ToString());

                    savings += netPay - periodCosts;
                    bills["totalSavings"] = savings.ToString();
                }
                //var cost = Convert.ToDecimal(bills["periodCosts"]);
                var save = Convert.ToDecimal(bills["totalSavings"]);

                return save;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        private Dictionary<string, string> UpdateTotalCosts(Dictionary<string, string> billsDictionary)
        {
            try
            {
                var billsFromDb = _db.Bills.ToList();
                var currentDate = Convert.ToDateTime(billsDictionary["currentDate"]);
                var endDate = Convert.ToDateTime(billsDictionary["endDate"]);
                var expenses = 0.0m;
                billsDictionary["periodCosts"] = "0";

                foreach (var bill in billsDictionary)
                {
                    if (bill.Key == "currentDate" || bill.Key == "endDate" || bill.Key == "periodCosts" ||
                        bill.Key == "totalSavings" || bill.Key == "totalCosts") continue;

                    var dueDate = Convert.ToDateTime(bill.Value);
                    if (!(dueDate >= currentDate && dueDate <= endDate)) continue;

                    expenses += billsFromDb.Where(b => b.Name == bill.Key).Select(b => b.AmountDue).FirstOrDefault();
                }

                var billCosts = Convert.ToDecimal(billsDictionary["totalCosts"]);
                billsDictionary["totalCosts"] = (expenses + billCosts).ToString(CultureInfo.InvariantCulture);
                billsDictionary["periodCosts"] = expenses.ToString(CultureInfo.InvariantCulture);

                return billsDictionary;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        private void SetCurrentAndEndDate(IDictionary<string, string> billsDictionary)
        {
            try
            {
                var currentDate = Convert.ToDateTime(billsDictionary["currentDate"]);
                var endDate = Convert.ToDateTime(billsDictionary["endDate"]);

                if (Convert.ToDateTime(billsDictionary["currentDate"]).Day <= 14)
                {
                    billsDictionary["currentDate"] = new DateTime(currentDate.Year, currentDate.Month, 16).ToShortDateString();
                    currentDate = Convert.ToDateTime(billsDictionary["currentDate"]);
                    billsDictionary["endDate"] =
                        new DateTime(currentDate.Year, currentDate.Month, LastDayOfMonth(currentDate).Day)
                            .ToShortDateString();
                }
                else
                {
                    billsDictionary["currentDate"] =
                        FirstDayOfMonth(currentDate.AddMonths(1).Year, currentDate.AddMonths(1).Month)
                            .ToString(CultureInfo.InvariantCulture);
                    currentDate = Convert.ToDateTime(billsDictionary["currentDate"]);
                    billsDictionary["endDate"] =
                        new DateTime(currentDate.Year, currentDate.Month, 15).ToShortDateString();
                    //TODO: simplify
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public void FulfillTransaction(Account debitedAccount, Account creditedAccount, decimal? amount)
        {

        }

        /// <summary>
        /// Calculate the remaining balance of a loan
        /// </summary>
        /// <param name="loan">Loan class that contains all pertinent information for the loan</param>
        /// <returns name="remainingBalance" type="decimal">Remaining balance of the loan</returns>
        public decimal RemainingBalance(Loan loan)
        {
            try
            {
                //bug: APR is not being calculated per pay-period
                var remainingBalance = 0.0m;
                if (loan != null)
                {
                    var payments = Convert.ToDouble(loan.Payments);
                    var rate = Convert.ToDouble(decimal.One + loan.APR);
                    var fv = (loan.OutstandingBalance * (1 + loan.APR)) -
                             loan.Payment * (decimal)((Math.Pow(loan.Payments, rate) - 1.0 / (double)loan.APR));


                    return remainingBalance;
                }
                return remainingBalance;
            }
            catch (Exception)
            {

                throw;
            }
        }

        public DateTime PayoffDate(Loan loan)
        {
            try
            {
                var payoffDate = DateTime.Today;
                if (loan != null)
                {
                    double interestRate = 0.00;
                    switch (loan.PaymentFrequency)
                    {
                        case FrequencyEnum.Monthly:
                            interestRate = (double)loan.APR / 12;
                            break;
                        case FrequencyEnum.Weekly:
                            interestRate = (double)loan.APR / 52;
                            break;
                        case FrequencyEnum.Daily:
                            interestRate = (double)loan.APR / 360;
                            break;
                        default:
                            break;
                    }

                    // Formula: N = -log(1-iA/P) / log(1+i)       N = # of payments, i = interest rate (APR/payments per year), A = loan amount, P = payment
                    // step 1 = -log(1-iA/P)
                    decimal interestTimesAmount = (decimal)interestRate * loan.OriginalLoanAmount;
                    decimal divideByPayment = Convert.ToDecimal(interestTimesAmount / loan.Payment);
                    double getNegativeLog = Math.Log((double)divideByPayment) * -1;

                    // step 2 = log(1+i)
                    decimal interestPlusOne = (decimal)(1.0 + interestRate);
                    double getLog = Math.Log((double)interestPlusOne);

                    // step 3 = step 1 / step 2
                    double numberOfPayments = getNegativeLog / getLog;
                    int wholeNumber = (int)numberOfPayments;
                    double fraction = numberOfPayments - wholeNumber;

                    double days = 0.0;
                    switch (loan.PaymentFrequency)
                    {
                        case FrequencyEnum.Monthly:
                            payoffDate = DateTime.Today.AddMonths(wholeNumber);
                            days = Math.Ceiling(30 * fraction);
                            payoffDate = payoffDate.AddDays(days);
                            break;
                        case FrequencyEnum.Weekly:
                            payoffDate = DateTime.Today.AddDays(wholeNumber * 7);
                            days = Math.Ceiling(7 * fraction);
                            payoffDate = payoffDate.AddDays(days);
                            break;
                        case FrequencyEnum.Daily:
                            payoffDate = DateTime.Today.AddDays(wholeNumber + 1);
                            break;
                    }

                    return payoffDate;
                }
                return payoffDate;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public decimal Payment(Loan loan)
        {
            try
            {
                var payment = decimal.Zero;
                if (loan != null)
                {



                    return payment;
                }
                return payment;
            }
            catch (Exception)
            {

                throw;
            }
        }

        public decimal Interest(Loan loan)
        {
            try
            {
                var interestPayment = decimal.Zero;
                if (loan != null)
                {


                    return interestPayment;
                }
                return interestPayment;
            }
            catch (Exception)
            {

                throw;
            }
        }

        public decimal Principal(Loan loan)
        {
            try
            {
                var principalPayment = decimal.Zero;
                if (loan != null)
                {


                    return principalPayment;
                }
                return principalPayment;
            }
            catch (Exception)
            {

                throw;
            }
        }

        public decimal DailyInterest(Loan loan)
        {
            try
            {
                var dailyInterestRate = (loan.APR / 100) / (decimal)364.25;
                return dailyInterestRate * loan.OutstandingBalance;
            }
            catch (Exception e)
            {

                throw;
            }
        }

        public decimal MonthlyInterest(Loan loan)
        {
            try
            {
                var monthlyInterestRate = (loan.APR / 100) / 12;
                return monthlyInterestRate * loan.OutstandingBalance;
            }
            catch (Exception e)
            {

                throw;
            }
        }

        public decimal ExpenseRatio()
        {
            try
            {
                var bills = _db.Bills.ToList();
                var salaries = _db.Salaries.ToList();
                decimal monthlyExpenses = 0.00m;
                decimal? income = 0.00m;

                foreach (var bill in bills)
                {
                    switch (bill.PaymentFrequency)
                    {
                        case FrequencyEnum.Weekly:
                            monthlyExpenses += bill.AmountDue * 4;
                            break;
                        case FrequencyEnum.SemiMonthly:
                            monthlyExpenses += bill.AmountDue * 2;
                            break;
                        case FrequencyEnum.Monthly:
                            monthlyExpenses += bill.AmountDue;
                            break;
                        case FrequencyEnum.SemiAnnually:
                            monthlyExpenses += bill.AmountDue / 6;
                            break;
                        case FrequencyEnum.Annually:
                            monthlyExpenses += bill.AmountDue / 12;
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }

                foreach (var salary in salaries)
                {
                    if (salary.PayTypesEnum == PayTypesEnum.Salary)
                    {
                        switch (salary.PayFrequency)
                        {
                            case FrequencyEnum.Weekly:
                                income += salary.NetIncome * 4;
                                break;
                            case FrequencyEnum.SemiMonthly:
                                income += salary.NetIncome * 2;
                                break;
                            case FrequencyEnum.Monthly:
                                income += salary.NetIncome;
                                break;
                            default:
                                throw new ArgumentOutOfRangeException();
                        }
                    }
                }

                income = 3798.40m;
                if (income != null)
                {
                    decimal expenseRatio = (decimal)income / monthlyExpenses;

                    return expenseRatio;
                }
                return decimal.MinusOne;
            }
            catch (Exception e)
            {
                return decimal.MinusOne;
            }
        }

        /// <summary>
        /// Returns the last payday.  If the current date is the same as the first or last payday, the current date will be returned
        /// </summary>
        /// <param name="salary"></param>
        /// <returns></returns>
        public DateTime PreviousPaydate(Salary salary)
        {
            if (salary == null)
                return DateTime.MinValue;

            var payDate = new DateTime();
            var today = DateTime.Today;

            switch (salary.PayFrequency)
            {
                case FrequencyEnum.Weekly:
                    {
                        switch (salary.PaydayOfWeek)
                        {
                            case DayEnum.Sunday:
                                payDate = DateTime.Today.AddDays((double)(-(int)(DateTime.Today.DayOfWeek) - DayOfWeek.Sunday));
                                break;
                            case DayEnum.Monday:
                                payDate = DateTime.Today.AddDays((double)(-(int)(DateTime.Today.DayOfWeek) - DayOfWeek.Monday));
                                break;
                            case DayEnum.Tuesday:
                                payDate = DateTime.Today.AddDays((double)(-(int)(DateTime.Today.DayOfWeek) - DayOfWeek.Tuesday));
                                break;
                            case DayEnum.Wednesday:
                                payDate = DateTime.Today.AddDays((double)(-(int)(DateTime.Today.DayOfWeek) - DayOfWeek.Wednesday));
                                break;
                            case DayEnum.Thursday:
                                payDate = DateTime.Today.AddDays((double)(-(int)(DateTime.Today.DayOfWeek) - DayOfWeek.Thursday));
                                break;
                            case DayEnum.Friday:
                                payDate = DateTime.Today.AddDays((double)(-(int)(DateTime.Today.DayOfWeek) - DayOfWeek.Friday));
                                break;
                            case DayEnum.Saturday:
                                payDate = DateTime.Today.AddDays((double)(-(int)(DateTime.Today.DayOfWeek) - DayOfWeek.Saturday));
                                break;
                        }
                    }
                    break;
                case FrequencyEnum.BiWeekly:
                    {
                        throw new NotImplementedException();
                    }
                    break;
                case FrequencyEnum.Monthly:
                    {
                        payDate = today.Day == (int)salary.PaydayOfWeek ? today : today.AddMonths(-1);
                    }
                    break;
                case FrequencyEnum.SemiMonthly:
                    {
                        var firstPayDate = new DateTime(today.Year, today.Month, Convert.ToInt32(salary.FirstPayday));
                        var lastPayDate = salary.FirstPayday.ToLower() == "last" ? new DateTime(today.Year, today.Month, LastDayOfMonth(today).Day)
                            : new DateTime(today.Year, today.Month, Convert.ToInt32(salary.LastPayday));

                        if (today == firstPayDate)
                            payDate = firstPayDate;
                        else if (today == lastPayDate)
                            payDate = lastPayDate;
                        else if (today > firstPayDate && today < lastPayDate)
                            payDate = firstPayDate;
                        else if (today < firstPayDate)
                            if (today.Month == 1)
                                payDate = salary.LastPayday.ToLower() == "last" ? new DateTime(today.AddYears(-1).Year, today.AddMonths(-1).Month, LastDayOfMonth(today.AddMonths(-1)).Day)
                                    : new DateTime(today.AddYears(-1).Year, today.AddMonths(-1).Month, lastPayDate.Day);
                            else
                                payDate = salary.LastPayday.ToLower() == "last" ? new DateTime(today.Year, today.AddMonths(-1).Month, LastDayOfMonth(today.AddMonths(-1)).Day)
                                    : new DateTime(today.Year, today.AddMonths(-1).Month, today.Day);
                    }
                    break;
                case FrequencyEnum.Quarterly:
                    {
                        throw new NotImplementedException();
                    }
                    break;
                case FrequencyEnum.SemiAnnually:
                    {
                        throw new NotImplementedException();
                    }
                    break;
                case FrequencyEnum.Annually:
                    {
                        throw new NotImplementedException();
                    }
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            return payDate;
        }

        public decimal GetMonthlyIncome()
        {
            try
            {
                var incomePerPayperiod = Convert.ToDecimal(_db.Salaries.Sum(s => s.NetIncome));
                var paymentFrequency = _db.Salaries.Select(s => s.PayFrequency).FirstOrDefault();
                var monthlyIncome = 0.00m;

                switch (paymentFrequency)
                {
                    case FrequencyEnum.Weekly:
                        monthlyIncome = incomePerPayperiod * 4;
                        break;
                    case FrequencyEnum.SemiMonthly:
                        monthlyIncome = incomePerPayperiod * 2;
                        break;
                    case FrequencyEnum.Monthly:
                        monthlyIncome = incomePerPayperiod;
                        break;
                    default:
                        monthlyIncome = incomePerPayperiod * 2;
                        break;
                }

                return monthlyIncome;
            }
            catch (Exception e)
            {
                return 0.0m;
            }
        }

        public void CalculateRequiredPaycheckContributions()
        {
            try
            {

            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        public AccountRebalanceReport GetRebalancingAccountsReport(AccountDTO accounts)
        {
            AccountRebalanceReport report = accounts.RebalanceReport;
            UpdatePaycheckContributions();

            foreach (var account in accounts.Accounts)
            {
                var surplus = account.Balance - account.RequiredSavings;
                if (surplus == 0) continue;
                if (surplus > 0)
                {
                    report.AccountsWithSurplus.Add(account);
                    report.Surplus += (decimal) surplus;
                }
                else if (surplus < 0)
                {
                    report.AccountsWithDeficit.Add(account);
                    report.Deficit += (decimal) surplus;
                }
                if (!account.ExcludeFromSurplus)
                    report.TotalSurplus += surplus ?? 0m;
            }

            report.newReport = true;
            return report;
        }
    }

    public class AccountRebalanceReport
    {
        public AccountRebalanceReport()
        {
            newReport = false;
            Surplus = decimal.Zero;
            Deficit = decimal.Zero;
            TotalSurplus = decimal.Zero;
            AccountsWithSurplus = new List<Account>();
            AccountsWithDeficit = new List<Account>();
        }

        public bool newReport { get; set; }

        [DataType(DataType.Currency)]
        public decimal Surplus { get; set; }

        [DataType(DataType.Currency)]
        public decimal Deficit { get; set; }

        [DataType(DataType.Currency)]
        public decimal TotalSurplus { get; set; }

        public List<Account> AccountsWithSurplus { get; set; }
        public List<Account> AccountsWithDeficit { get; set; }
    }
}
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Globalization;
using System.Linq;
using JPFData.Enumerations;
using JPFData.Models;
using JPFData.Models.JPFinancial;

namespace JPFData
{
    /// <summary>
    /// Holding place for unused calculation methods
    /// </summary>
    class CalculationsArchive
    {
        private readonly ApplicationDbContext _db = new ApplicationDbContext();


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
                    throw new NotImplementedException();
                case FrequencyEnum.Monthly:
                    payDate = today.Day == (int)salary.PaydayOfWeek ? today : today.AddMonths(-1);
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
                    throw new NotImplementedException();
                case FrequencyEnum.SemiAnnually:
                    throw new NotImplementedException();
                case FrequencyEnum.Annually:
                    throw new NotImplementedException();
                default:
                    throw new ArgumentOutOfRangeException();
            }

            return payDate;
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

        public bool UpdateRequiredBalanceForDiscretionary(bool dbSave = false)
        {
            // public because runs on startup with dbSave = true
            try
            {
                var savingsAccountBalances = new List<KeyValuePair<string, decimal>>();

                foreach (var bill in _db.Bills.ToList())
                {
                    bill.Account = _db.Accounts.FirstOrDefault(a => a.Id == bill.AccountId);
                    if (bill.Account == null) continue;
                    var billTotal = bill.AmountDue;
                    // Next due date
                    var dueDate = bill.DueDate;
                    // How many pay periods to save until next due date
                    var payPeriodsLeft = PayPeriodsTilDue(dueDate);
                    decimal savePerPaycheck = 0;

                    // Calculate how much to save from each pay period
                    switch (bill.PaymentFrequency)
                    {
                        case FrequencyEnum.Annually:
                            savePerPaycheck = billTotal / 24;
                            break;
                        case FrequencyEnum.SemiAnnually:
                            savePerPaycheck = billTotal / 12;
                            break;
                        case FrequencyEnum.Quarterly:
                            savePerPaycheck = billTotal / 6;
                            break;
                        case FrequencyEnum.SemiMonthly:
                            savePerPaycheck = billTotal / 4;
                            break;
                        case FrequencyEnum.Monthly:
                            savePerPaycheck = billTotal / 2;
                            break;
                        case FrequencyEnum.BiWeekly:
                            savePerPaycheck = billTotal;
                            break;
                        case FrequencyEnum.Weekly:
                            savePerPaycheck = billTotal * 2;
                            break;
                        default:
                            savePerPaycheck = billTotal / 2;
                            break;
                    }
                    // required savings = bill amount due - (how many pay periods before due date * how much to save per pay period)
                    var save = billTotal - payPeriodsLeft * savePerPaycheck;
                    // add kvp (account that bill is credited to, amount to save) 

                    savingsAccountBalances.Add(new KeyValuePair<string, decimal>(bill.Account.Name, save));
                }

                // update each account that has a bill credited to it 
                foreach (var account in _db.Accounts.ToList())
                {
                    var valuesFound = false;
                    decimal totalSavings = 0;

                    foreach (var savings in savingsAccountBalances)
                    {
                        if (savings.Key != account.Name) continue;
                        totalSavings += savings.Value;
                        valuesFound = true;
                    }
                    if (!valuesFound) continue;
                    account.RequiredSavings = totalSavings;
                    _db.Entry(account).State = EntityState.Modified;
                    //_dbTransactions += 1;
                }
                if (!dbSave) return true;
                _db.SaveChanges();
                //_dbTransactions = 0;


                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        // Similar to updating suggested paycheck contributions?
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

        public List<Account> UpdateSavingsPercentage(List<Account> accounts)
        {
            if (accounts == null || accounts.Count <= 0) return accounts;


            var totalSavings = accounts.Sum(a => a.RequiredSavings);
            foreach (Account account in accounts)
            {
                var accountSavings = account.RequiredSavings;
                var savingsPercentage = accountSavings / totalSavings;
                //account.PercentageOfSavings = decimal.Round(savingsPercentage * 100, 2, MidpointRounding.AwayFromZero);
            }

            return accounts;
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

        /// <summary>
        /// Calculate the remaining balance of a loan
        /// </summary>
        /// <param name="loan">Loan class that contains all pertinent information for the loan</param>
        /// <returns name="remainingBalance" type="decimal">Remaining balance of the loan</returns>
        public decimal RemainingBalance(Loan loan)
        {
            //bug: APR is not being calculated per pay-period
            var remainingBalance = 0.0m;
            if (loan == null) return remainingBalance;
            var payments = Convert.ToDouble(loan.Payments);
            var rate = Convert.ToDouble(decimal.One + loan.APR);
            var fv = (loan.OutstandingBalance * (1 + loan.APR)) -
                     loan.Payment * (decimal)((Math.Pow(loan.Payments, rate) - 1.0 / (double)loan.APR));

            return remainingBalance;
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
                        case FrequencyEnum.Daily:
                            throw new NotImplementedException();
                        case FrequencyEnum.BiWeekly:
                            break; throw new NotImplementedException();
                        case FrequencyEnum.Quarterly:
                            throw new NotImplementedException();
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }

                foreach (var salary in salaries)
                {
                    if (salary.PayTypesEnum != PayTypesEnum.Salary) continue;
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
                        case FrequencyEnum.Daily:
                            throw new NotImplementedException();
                        case FrequencyEnum.BiWeekly:
                            throw new NotImplementedException();
                        case FrequencyEnum.Quarterly:
                            throw new NotImplementedException();
                        case FrequencyEnum.SemiAnnually:
                            throw new NotImplementedException();
                        case FrequencyEnum.Annually:
                            throw new NotImplementedException();
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }

                income = 3798.40m;
                if (income == null) return decimal.MinusOne;
                decimal expenseRatio = (decimal)income / monthlyExpenses;

                return expenseRatio;
            }
            catch (Exception)
            {
                return decimal.MinusOne;
            }
        }


        // Used methods included to remove reference errors
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

        /// <summary>
        /// Returns the last day of the month for the provided date
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Returns how many times the user will get paid before a due date
        /// </summary>
        /// <param name="dueDate"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Returns Dictionary (string, string) with current and end (next pay period) dates set
        /// </summary>
        /// <param name="billsDictionary"></param>
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

        /// <summary>
        /// Returns the first day of the month for the provided year and month
        /// </summary>
        /// <param name="year"></param>
        /// <param name="month"></param>
        /// <returns></returns>
        public DateTime FirstDayOfMonth(int year, int month)
        {
            //TODO: change to take datetime as parameter
            try
            {
                return new DateTime(year, month, 1);
            }
            catch (Exception e)
            {
                throw e;
            }
        }
    }
}

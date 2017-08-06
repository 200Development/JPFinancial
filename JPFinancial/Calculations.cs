using JPFinancial.Models;
//using QuandlCS.Connection;
//using QuandlCS.Requests;
//using QuandlCS.Types;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Globalization;
using System.Linq;

namespace JPFinancial
{
    public class Calculations
    {
        private static readonly ApplicationDbContext _db = new ApplicationDbContext();

        public void GetRequiredAcctSavings()
        {
            try
            {
                var accounts = _db.Accounts.ToList();
                var bills = _db.Bills.ToList();
                var savingsAccountBalances = new List<KeyValuePair<string, decimal>>();

                foreach (var bill in bills)
                {
                    var billTotal = bill.AmountDue;
                    var dueDate = bill.DueDate;
                    var payPeriodsLeft = PayPeriodsTilDue(dueDate);
                    decimal savePerPaycheck = 0;

                    switch (bill.PaymentFrequency)
                    {
                        case Frequency.Annually:
                            savePerPaycheck = billTotal / 24;
                            break;
                        case Frequency.SemiAnnually:
                            savePerPaycheck = billTotal / 12;
                            break;
                        case Frequency.Quarterly:
                            savePerPaycheck = billTotal / 6;
                            break;
                        case Frequency.BiMonthly:
                            savePerPaycheck = billTotal / 4;
                            break;
                        case Frequency.Monthly:
                            savePerPaycheck = billTotal / 2;
                            break;
                        case Frequency.Weekly:
                            savePerPaycheck = billTotal * 2;
                            break;
                        default:
                            savePerPaycheck = billTotal / 2;
                            break;
                    }
                    var save = billTotal - payPeriodsLeft * savePerPaycheck;
                    savingsAccountBalances.Add(new KeyValuePair<string, decimal>(bill.Account.Name, save));
                }


                foreach (var account in accounts)
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
                    _db.SaveChanges();
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public void GetReqBalanceSurplus()
        {
            try
            {
                var accounts = _db.Accounts.ToList();

                foreach (var account in accounts)
                {
                    var acctBalance = account.Balance;
                    var reqbalance = account.RequiredSavings;
                    account.BalanceSurplus = acctBalance - reqbalance;
                    _db.Entry(account).State = EntityState.Modified;
                    _db.SaveChanges();

                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        //public object GetStockQuote(string ticker, string startDate = null, string endDate = null)
        //{
        //    var request = new QuandlDownloadRequest();
        //    var connection = new QuandlConnection();
        //    var newStartDate = new DateTime();
        //    var newEndDate = new DateTime();

        //    if (startDate == null || startDate.Trim().Equals(""))
        //    {
        //        newStartDate = DateTime.Today.AddDays(-1);
        //    }
        //    else
        //    {
        //        newStartDate = Convert.ToDateTime(startDate);
        //    }

        //    if (endDate == null || endDate.Trim().Equals(""))
        //    {
        //        newEndDate = DateTime.Today;
        //    }
        //    else
        //    {
        //        newEndDate = Convert.ToDateTime(endDate);
        //    }

        //    request.APIKey = "jujaGvER5aTaVzznmCE8";
        //    request.Datacode = new Datacode("YAHOO", ticker);
        //    request.StartDate = newStartDate;
        //    request.EndDate = newEndDate;
        //    request.Format = FileFormats.JSON;
        //    request.Frequency = Frequencies.Daily;
        //    request.Headers = true;
        //    request.Sort = SortOrders.Ascending;
        //    request.Transformation = Transformations.None;
        //    request.ToRequestString();



        //    var data = connection.Request(request);
        //    JObject yahooDataset = JObject.Parse(data);
        //    IList<JToken> results = yahooDataset["data"].Children().ToList();

        //    return results;
        //}

        //public object GetStockQuote(string ticker)
        //{
        //    var request = new QuandlDownloadRequest();
        //    var connection = new QuandlConnection();

        //    //    request.APIKey = "jujaGvER5aTaVzznmCE8";
        //    //    request.Datacode = new Datacode("YAHOO", ticker);
        //    //    request.StartDate = DateTime.Today.AddDays(-1);
        //    //    request.EndDate = DateTime.Today;
        //    //    request.Format = FileFormats.JSON;
        //    request.Frequency = Frequencies.Daily;
        //    //    request.Headers = true;
        //    //    request.Sort = SortOrders.Ascending;
        //    //    request.Transformation = Transformations.None;
        //    //    request.ToRequestString();

        //    var data = connection.Request(request);
        //    var yahooDataset = JObject.Parse(data);
        //    IList<JToken> results = yahooDataset["data"].Children().ToList();

        //    //    // serialize JSON results into .NET objects
        //    //    foreach (JToken result in results)
        //    //    {
        //    //        var date = result[0];
        //    //        var open = result[1];
        //    //        var high = result[2];
        //    //        var low = result[3];
        //    //        var close = result[4];
        //    //        var volume = result[5];
        //    //        var adjustedClose = result[6];
        //    //    }
        //    return results;
        //}

        private static int PayPeriodsTilDue(DateTime? dueDate)
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
                return payPeriods;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public IList<Bill> GetBillsDue(DateTime firstPaycheck, DateTime firstDayOfMonth, DateTime lastPaycheck)
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

        public static int GetLastDayOfMonth(DateTime date)
        {
            try
            {
                return date.AddMonths(1).AddDays(-date.Day).Day;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public static DateTime GetFirstDayOfMonth(int year, int month)
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

        public Dictionary<string, decimal> SavingsReqForBills(IEnumerable<Bill> bills, Dictionary<string, decimal> savingsAccountBalances)
        {
            try
            {
                foreach (var bill in bills)
                {
                    var billTotal = bill.AmountDue;
                    var dueDate = bill.DueDate;
                    var payPeriodsLeft = PayPeriodsTilDue(dueDate);
                    decimal savePerPaycheck = 0;

                    switch (bill.PaymentFrequency)
                    {
                        case Frequency.Annually:
                            savePerPaycheck = billTotal / 24;
                            break;
                        case Frequency.SemiAnnually:
                            savePerPaycheck = billTotal / 12;
                            break;
                        case Frequency.Quarterly:
                            savePerPaycheck = billTotal / 6;
                            break;
                        case Frequency.BiMonthly:
                            savePerPaycheck = billTotal / 4;
                            break;
                        case Frequency.Monthly:
                            savePerPaycheck = billTotal / 2;
                            break;
                        case Frequency.Weekly:
                            savePerPaycheck = billTotal * 2;
                            break;
                        default:
                            savePerPaycheck = billTotal / 2;
                            break;
                    }
                    decimal save = billTotal - payPeriodsLeft * savePerPaycheck;
                    if (savingsAccountBalances.ContainsKey(bill.Account.Name))
                    {
                        savingsAccountBalances[bill.Account.Name] = save;
                    }
                    else
                    {
                        savingsAccountBalances.Add(bill.Account.Name, save);
                    }
                }
                return savingsAccountBalances;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public static decimal SavingsReqForBills(IEnumerable<Bill> bills)
        {
            try
            {
                decimal savedup = 0;
                foreach (var bill in bills)
                {
                    var billTotal = bill.AmountDue;
                    var dueDate = bill.DueDate;
                    var payPeriodsLeft = PayPeriodsTilDue(dueDate);
                    decimal savePerPaycheck = 0;

                    switch (bill.PaymentFrequency)
                    {
                        case Frequency.Annually:
                            savePerPaycheck = billTotal / 24;
                            break;
                        case Frequency.SemiAnnually:
                            savePerPaycheck = billTotal / 12;
                            break;
                        case Frequency.Quarterly:
                            savePerPaycheck = billTotal / 6;
                            break;
                        case Frequency.BiMonthly:
                            savePerPaycheck = billTotal / 4;
                            break;
                        case Frequency.Monthly:
                            savePerPaycheck = billTotal / 2;
                            break;
                        case Frequency.Weekly:
                            savePerPaycheck = billTotal * 2;
                            break;
                        default:
                            savePerPaycheck = billTotal / 2;
                            break;
                    }

                    savedup += billTotal - payPeriodsLeft * savePerPaycheck;
                }
                return savedup;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public static void UpdateAccountGoals(IEnumerable<Account> accounts, Dictionary<string, decimal> savingsAccountBalances)
        {
            try
            {
                foreach (var account in accounts)
                {
                    var valuesFound = false;
                    decimal totalSavings = 0;

                    foreach (var savings in savingsAccountBalances)
                    {
                        if (savings.Key != account.Name) continue;
                        totalSavings += savings.Value;
                        valuesFound = true;
                    }
                    if (valuesFound)
                    {
                        account.Goal = totalSavings;
                    }
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public void UpdateBillDueDates(DateTime currentDate)
        {
            try
            {
                var bills = _db.Bills.ToList();

                foreach (var bill in bills)
                {
                    var frequency = bill.PaymentFrequency;
                    var dueDate = bill.DueDate;
                    var newDueDate = dueDate;

                    while (newDueDate < currentDate)
                    {
                        switch (frequency)
                        {
                            case Frequency.Daily:
                                newDueDate = newDueDate.AddDays(1);
                                break;
                            case Frequency.Weekly:
                                newDueDate = newDueDate.AddDays(7);
                                break;
                            case Frequency.BiWeekly:
                                newDueDate = newDueDate.AddDays(14);
                                break;
                            case Frequency.SemiMonthly:
                                newDueDate = newDueDate.AddDays(15);
                                break;
                            case Frequency.Monthly:
                                newDueDate = newDueDate.AddMonths(1);
                                break;
                            case Frequency.BiMonthly:
                                newDueDate = newDueDate.AddMonths(2);
                                break;
                            case Frequency.Quarterly:
                                newDueDate = newDueDate.AddMonths(3);
                                break;
                            case Frequency.SemiAnnually:
                                newDueDate = newDueDate.AddMonths(6);
                                break;
                            case Frequency.Annually:
                                newDueDate = newDueDate.AddYears(1);
                                break;
                            default:
                                throw new ArgumentOutOfRangeException();
                        }
                        bill.DueDate = newDueDate;
                        _db.Entry(bill).State = EntityState.Modified;
                        _db.SaveChanges();
                    }
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        private static Dictionary<string, string> UpdateBillDueDates(Dictionary<string, string> billsDue)
        {
            try
            {
                var bills = _db.Bills.ToList();
                var beginDate = Convert.ToDateTime(billsDue["currentDate"]);

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
                            case Frequency.Daily:
                                newDueDate = newDueDate.AddDays(1);
                                break;
                            case Frequency.Weekly:
                                newDueDate = newDueDate.AddDays(7);
                                break;
                            case Frequency.BiWeekly:
                                newDueDate = newDueDate.AddDays(14);
                                break;
                            case Frequency.SemiMonthly:
                                newDueDate = newDueDate.AddDays(15);
                                break;
                            case Frequency.Monthly:
                                newDueDate = newDueDate.AddMonths(1);
                                break;
                            case Frequency.BiMonthly:
                                newDueDate = newDueDate.AddMonths(2);
                                break;
                            case Frequency.Quarterly:
                                newDueDate = newDueDate.AddMonths(3);
                                break;
                            case Frequency.SemiAnnually:
                                newDueDate = newDueDate.AddMonths(6);
                                break;
                            case Frequency.Annually:
                                newDueDate = newDueDate.AddYears(1);
                                break;
                            default:
                                throw new ArgumentOutOfRangeException();
                        }
                        billsDue[bill.Name] = newDueDate.ToShortDateString();
                    }
                }
                return billsDue;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public static DateTime CalculateFvDate(decimal futureValue, decimal netPay)
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
                            : new DateTime(date.Year, date.Month, GetLastDayOfMonth(date)).ToShortDateString()
                    },
                    {"totalCost", "0"},
                    {"totalSavings", "0"}
                };

                foreach (var bill in billsFromDb)
                {
                    bills.Add(bill.Name, bill.DueDate.ToShortDateString());
                }

                while (Convert.ToDecimal(bills["totalSavings"]) < futureValue)
                {
                    bills = UpdateBillDueDates(bills);
                    bills = UpdateTotalCostsAndSavings(bills);
                    UpdateCurrentAndEndDate(bills);
                    var savings = Convert.ToDecimal(bills["totalSavings"]);
                    savings += netPay;
                    bills["totalSavings"] = savings.ToString(CultureInfo.InvariantCulture);
                }

                var cost = Convert.ToDecimal(bills["totalCost"]);
                var save = Convert.ToDecimal(bills["totalSavings"]);

                return Convert.ToDateTime(bills["endDate"]);
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public decimal CalculateFv(DateTime futureDate, decimal netPay)
        {
            try
            {
                var payperiods = PayPeriodsTilDue(futureDate);
                var date = DateTime.Today;
                var billsFromDb = _db.Bills.ToList();

                var bills = new Dictionary<string, string>
                {
                    {"currentDate", DateTime.Today.ToShortDateString()},
                    {
                        "endDate",
                        DateTime.Today.Day <= 14
                            ? new DateTime(date.Year, date.Month, 15).ToShortDateString()
                            : new DateTime(date.Year, date.Month, GetLastDayOfMonth(date)).ToShortDateString()
                    },
                    {"totalCost", "0"},
                    {"totalSavings", "0"}
                };

                foreach (var bill in billsFromDb)
                {
                    bills.Add(bill.Name, bill.DueDate.ToShortDateString());
                }

                for (var i = 0; i < payperiods; i++)
                {
                    bills = UpdateBillDueDates(bills);
                    bills = UpdateTotalCostsAndSavings(bills);
                    UpdateCurrentAndEndDate(bills);
                    var savings = Convert.ToDecimal(bills["totalSavings"]);
                    savings += netPay;
                    bills["totalSavings"] = savings.ToString(CultureInfo.InvariantCulture);
                }

                var cost = Convert.ToDecimal(bills["totalCost"]);
                var save = Convert.ToDecimal(bills["totalSavings"]);
                var netSavings = save - cost;

                return netSavings;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        private static Dictionary<string, string> UpdateTotalCostsAndSavings(Dictionary<string, string> bills)
        {
            try
            {
                var billsFromDb = _db.Bills.ToList();
                var currentDate = Convert.ToDateTime(bills["currentDate"]);
                var endDate = Convert.ToDateTime(bills["endDate"]);
                var expenses = 0.0m;

                foreach (var bill in bills)
                {
                    if (bill.Key == "currentDate" || bill.Key == "endDate" || bill.Key == "totalCost" ||
                        bill.Key == "totalSavings") continue;

                    var dueDate = Convert.ToDateTime(bill.Value);
                    if (!(dueDate >= currentDate && dueDate <= endDate)) continue;

                    expenses += billsFromDb.Where(b => b.Name == bill.Key).Select(b => b.AmountDue).FirstOrDefault();
                }

                var billCosts = Convert.ToDecimal(bills["totalCost"]);
                bills["totalCost"] = (expenses + billCosts).ToString(CultureInfo.InvariantCulture);

                return bills;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        private static void UpdateCurrentAndEndDate(IDictionary<string, string> bills)
        {
            try
            {
                var currentDate = Convert.ToDateTime(bills["currentDate"]);
                var endDate = Convert.ToDateTime(bills["endDate"]);

                if (Convert.ToDateTime(bills["currentDate"]).Day <= 14)
                {
                    bills["currentDate"] = new DateTime(currentDate.Year, currentDate.Month, 16).ToShortDateString();
                    currentDate = Convert.ToDateTime(bills["currentDate"]);
                    bills["endDate"] =
                        new DateTime(currentDate.Year, currentDate.Month, GetLastDayOfMonth(currentDate))
                            .ToShortDateString();
                }
                else
                {
                    bills["currentDate"] =
                        GetFirstDayOfMonth(currentDate.AddMonths(1).Year, currentDate.AddMonths(1).Month)
                            .ToString(CultureInfo.InvariantCulture);
                    endDate = Convert.ToDateTime(bills["endDate"]);
                    bills["endDate"] =
                        new DateTime(endDate.AddMonths(1).Year, endDate.Month + 1, 15).ToShortDateString();
                    //TODO: simplify
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }
    }
}
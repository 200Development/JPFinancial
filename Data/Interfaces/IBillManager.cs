using System;
using System.Collections.Generic;
using JPFData.Models.JPFinancial;

namespace JPFData.Interfaces
{
    public interface IBillManager
    {
        List<Bill> GetAllBills();
        Bill GetBill(int? id);
        bool Create(Bill bill);
        bool Edit(Bill bill);
        bool Delete(int billId);

        /// <summary>
        /// Updates database Bills.DueDate if the previous due date has passed
        /// </summary>
        void UpdateBillDueDates();

        /// <summary>
        /// Returns summation of bills within the date range of the begin and end parameters  
        /// </summary>
        /// <param name="begin">start date of the date range</param>
        /// <param name="end">end date of the date range</param>
        /// <param name="onlyMandatory">sumates only mandatory expenses</param>
        /// <returns></returns>
        decimal ExpensesByDateRange(DateTime begin, DateTime end, bool onlyMandatory = false);

        /// <summary>
        /// Returns Dictionary with due dates for all bills for calculating all expenses within a timeframe.  Used for calculating future savings
        /// </summary>
        /// <param name="billsDictionary"></param>
        /// <returns></returns>
        Dictionary<string, string> UpdateBillDueDates(Dictionary<string, string> billsDictionary);

        Dictionary<string, string> UpdateTotalCosts(Dictionary<string, string> billsDictionary);
    }
}
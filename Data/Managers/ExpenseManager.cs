using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using JPFData.DTO;
using JPFData.Interfaces;
using JPFData.Metrics;
using JPFData.Models.JPFinancial;

namespace JPFData.Managers
{
    /// <summary>
    /// Manages all read/write to database Expense Table
    /// </summary>
    public class ExpenseManager : IExpenseManager
    {
        private readonly ApplicationDbContext _db;
        private readonly string _userId;


        public ExpenseManager()
        {
            _db = new ApplicationDbContext();
            _userId = Global.Instance.User != null ? Global.Instance.User.Id : string.Empty;
        }


        public List<KeyValuePair<string, string>> ValidationErrors { get; set; }

        public List<Expense> GetAllExpenses()
        {
            try
            {
                return _db.Expenses.Where(e => e.UserId == _userId).ToList();
            }
            catch (Exception e)
            {
                Logger.Instance.Error(e);
                throw;
            }
        }

        public Expense GetExpense(int? expenseId)
        {
            try
            {
                return _db.Expenses.Find(expenseId);
            }
            catch (Exception e)
            {
                Logger.Instance.Error(e);
                throw;
            }
        }

        public List<Expense> GetAllUnpaidExpenses()
        {
            try
            {
                return _db.Expenses.Where(e => e.UserId == _userId).Where(e => e.BillId > 0).Where(e => e.IsPaid == false).ToList();
            }
            catch (Exception e)
            {
                Logger.Instance.Error(e);
                throw;
            }
        }

        public decimal GetOutstandingExpensesTotal()
        {
            try
            {
                return GetAllUnpaidExpenses().Sum(b => b.Amount);
            }
            catch (Exception e)
            {
                Logger.Instance.Error(e);
                return 0m;
            }
        }

        public bool SetExpenseToPaid(int? expenseId)
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

        public bool SetExpenseToUnpaid(int? expenseId)
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

        public ExpenseMetrics RefreshExpenseMetrics(ExpenseDTO entity)
        {
            throw new NotImplementedException();
        }
    }
}

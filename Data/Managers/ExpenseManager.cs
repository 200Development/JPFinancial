using System;
using System.Collections.Generic;
using System.Linq;
using JPFData.DTO;
using JPFData.Metrics;
using JPFData.Models;

namespace JPFData.Managers
{
    public class ExpenseManager
    {
        private readonly ApplicationDbContext _db;


        public ExpenseManager()
        {
            _db = new ApplicationDbContext();
        }


        public List<KeyValuePair<string, string>> ValidationErrors { get; set; }

        public ExpenseDTO Get(ExpenseDTO entity)
        {
            try
            {
                entity.Expenses = _db.Expenses.ToList();
                entity.Metrics = RefreshExpenseMetrics(entity);
            }
            catch (Exception e)
            {
                Logger.Instance.Error(e);
                throw;
            }


            return entity;
        }

        public ExpenseMetrics RefreshExpenseMetrics(ExpenseDTO entity)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<OutstandingExpense> GetOutstandingBills()
        {
            try
            {
                var ret = new List<OutstandingExpense>();
                Logger.Instance.DataFlow($"Get");
                // Get all bill expenses that have not yet been paid
                var expenses = _db.Expenses.Where(e => e.BillId > 0 && !e.IsPaid).ToList();
                Logger.Instance.DataFlow($"Pulled list of bill expenses from DB that haven't been paid");
                //var outstandingBills = bills.Where(b => b.IsPaid == false).ToList();

                foreach (var expense in expenses)
                {
                    var newExpense = new OutstandingExpense();
                    newExpense.Id = expense.Id;
                    newExpense.Name = $"{expense.Name} - {expense.Amount} Due {expense.Due.ToShortDateString()}";
                    newExpense.DueDate = expense.Due;

                    ret.Add(newExpense);
                }

                return ret;
            }
            catch (Exception e)
            {
                Logger.Instance.Error(e);
                return new List<OutstandingExpense>();
            }
        }
    }
}

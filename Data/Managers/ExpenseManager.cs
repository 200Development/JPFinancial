using System;
using System.Collections.Generic;
using System.Linq;
using JPFData.DTO;
using JPFData.Metrics;
using JPFData.Models.JPFinancial;

namespace JPFData.Managers
{
    public class ExpenseManager
    {
        private readonly ApplicationDbContext _db;
        private readonly string _userId;


        public ExpenseManager()
        {
            _db = new ApplicationDbContext();
            _userId = Global.Instance?.User.Id ?? string.Empty;
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

        private ExpenseMetrics RefreshExpenseMetrics(ExpenseDTO entity)
        {
            throw new NotImplementedException();
        }
       
    }
}

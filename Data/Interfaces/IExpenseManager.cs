using System.Collections.Generic;
using JPFData.DTO;
using JPFData.Metrics;
using JPFData.Models.JPFinancial;

namespace JPFData.Interfaces
{
    public interface IExpenseManager
    {
        List<KeyValuePair<string, string>> ValidationErrors { get; set; }
        List<Expense> GetAllExpenses();
        Expense GetExpense(int? expenseId);
        List<Expense> GetAllUnpaidExpenses();
        decimal GetOutstandingExpensesTotal();
        bool SetExpenseToPaid(int? expenseId);
        bool SetExpenseToUnpaid(int? expenseId);
        ExpenseMetrics RefreshExpenseMetrics(ExpenseDTO entity);
    }
}
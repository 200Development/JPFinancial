using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace JPFData.Metrics
{
    public class TransactionMetrics
    {
        public TransactionMetrics()
        {
            AccountMetrics = new AccountMetrics();
            MandatoryExpenses = 0.0m;;
            DiscretionaryExpenses = 0.0m;;
            Income = 0.0m;;
        }

        public AccountMetrics AccountMetrics { get; set; }

        [Display(Name = "Mandatory Expenses"), DataType(DataType.Currency), DisplayFormat(DataFormatString = "{0:C}", ApplyFormatInEditMode = true)]
        public decimal MandatoryExpenses { get; set; }

        [Display(Name = "Discretionary Expenses"), DataType(DataType.Currency), DisplayFormat(DataFormatString = "{0:C}", ApplyFormatInEditMode = true)]
        public decimal DiscretionaryExpenses { get; set; }

        [Display(Name = "Income"), DataType(DataType.Currency), DisplayFormat(DataFormatString = "{0:C}", ApplyFormatInEditMode = true)]
        public decimal Income { get; set; }

        public Dictionary<string, decimal> MandatoryExpensesByMonth { get; set; }
        public Dictionary<string, decimal> DiscretionaryExpensesByMonth { get; set; }
        public Dictionary<string, decimal> ExpensesByMonth { get; set; }
        public Dictionary<string, decimal> IncomeByMonth { get; set; }
        public Dictionary<string, decimal> TransfersByMonth { get; set; }
        public Dictionary<string, decimal> CreditCardExpensesByMonth { get; set; }
    }
}
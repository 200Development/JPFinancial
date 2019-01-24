using System.ComponentModel.DataAnnotations;

namespace JPFData.Models
{
    public class Expense 
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public decimal Amount { get; set; }
        public int SalaryId { get; set; }
        [Display(Name = "Pre-Taxed?")]
        public bool IsPreTax { get; set; }
    }
}
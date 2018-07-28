using System;
using System.ComponentModel.DataAnnotations;

namespace JPFData.Models
{
    public class Bonus 
    {
        [Key]
        public int Id { get; set; }

        [Required, Display(Name = "Bonus")]
        public string Name { get; set; }

        [Required, DataType(DataType.Currency)]
        public decimal Amount { get; set; }

        [Required, DataType(DataType.Date), Display(Name = "Paid On")]
        public DateTime DatePaid { get; set; }

        [Required]
        public int  SalaryId { get; set; }
    }
}
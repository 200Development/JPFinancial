using System;
using System.ComponentModel.DataAnnotations;
using JPFData.Enumerations;

namespace JPFData.Models
{
    public class Bill 
    {
        [Key]
        public int Id { get; set; }

        [Required, StringLength(255)]
        public string Name { get; set; }

        [Required, DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [Display(Name = "Due Date")]
        public DateTime DueDate { get; set; }

        [Required, DataType(DataType.Currency)]
        [Display(Name = "Amount Due")]
        public decimal AmountDue { get; set; }

        [Required, Display(Name = "Frequency")]
        public FrequencyEnum PaymentFrequency { get; set; }

        public int AccountId { get; set; }

        public Account Account { get; set; }

        [Required]
        [Display(Name = "Late?")]
        public virtual bool IsLate { get; set; }

        [Required]
        [Display(Name = "Mandatory Expense?")]
        public virtual bool IsMandatory { get; set; }

        [Display(Name = "Paid?")]
        public virtual bool IsPaid { get; set; }
    }

    public class OutstandingBill
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
}
using System;
using System.ComponentModel.DataAnnotations;
using JPFData.Enumerations;
using JPFData.Interfaces;

namespace JPFData.Models.JPFinancial
{
    public class Bill : IBill
    {

        public Bill()
        {
            DueDate = Convert.ToDateTime(DateTime.Today.ToString("MMMM dd yyyy"));
            if (Global.Instance.User != null)
                UserId = Global.Instance.User.Id;
        }
        [Key]
        public int Id { get; set; }

        [Required]
        public string UserId { get; set; }

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
    }
}
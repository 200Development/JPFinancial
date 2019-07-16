using System.ComponentModel.DataAnnotations;
using JPFData.Enumerations;

namespace JPFData.Models.JPFinancial
{
    public class Company
    {
        [Key]
        public int Id { get; set; }

        [Required, Display(Name = "Company Name")]
        public string Name { get; set; }

        [Display(Name = "Street Address")]
        public string StreetAddress { get; set; }

        public string City { get; set; }

        public StatesEnum StatesEnum { get; set; }

        [DataType(DataType.PostalCode)]
        public string Zipcode { get; set; }

        [DataType(DataType.PhoneNumber)]
        public string Phone { get; set; }
    }
}
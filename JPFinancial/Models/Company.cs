﻿using JPFinancial.Models.Enumerations;
using System.ComponentModel.DataAnnotations;

namespace JPFinancial.Models
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

        public State State { get; set; }

        [DataType(DataType.PostalCode)]
        public string Zipcode { get; set; }

        [DataType(DataType.PhoneNumber)]
        public string Phone { get; set; }
    }
}
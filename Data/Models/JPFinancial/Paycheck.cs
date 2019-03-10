using System;
using System.ComponentModel.DataAnnotations;

namespace JPFData.Models.JPFinancial
{
    public class Paycheck
    {
        public Paycheck()
        {
            Employer = string.Empty;
            //Regular = decimal.Zero;
            //ElectronicsNontaxable = decimal.Zero;
            //TravelBusinessExpenseNontaxable = decimal.Zero;
            //HolidayPay = decimal.Zero;
            GrossPay = decimal.Zero;
            //FederalTaxableGross = decimal.Zero;
            //FederalWithholding = decimal.Zero;
            //YTDFederalWithholding = decimal.Zero;
            //FederalMedicaidWithholding = decimal.Zero;
            //YTDFederalMedicaidWithholding = decimal.Zero;
            //SocialSecurityWithholding = decimal.Zero;
            //YTDSocialSecurityWithholding = decimal.Zero;
            //StateTaxWithholding = decimal.Zero;
            //YTDStateTaxWithholding = decimal.Zero;
            //CityTaxWithholding = decimal.Zero;
            //YTDCityTaxWithholding = decimal.Zero;
            //IRA401KWithholding = decimal.Zero;
            //YTDIRA401KWithholding = decimal.Zero;
            //DependentCareFSAWithholding = decimal.Zero;
            //YTDDependentCareFSAWithholding = decimal.Zero;
            //HealthInsuranceWithholding = decimal.Zero;
            //YTDHealthInsuranceWithholding = decimal.Zero;
            //DentalInsuranceWithholding = decimal.Zero;
            //YTDDentalInsuranceWithholding = decimal.Zero;
            NetPay = decimal.Zero;
            //ChildSupportWithholding = decimal.Zero;
            //YTDChildSupportWithholding = decimal.Zero;
        }

        [Key]
        public int Id { get; set; }

        [Required]
        public string Employer { get; set; } 

        [Required]
        public DateTime Date { get; set; }

        //[DataType(DataType.Currency)]
        //public decimal? Regular { get; set; }

        //[DataType(DataType.Currency), Display(Name = "Electronics Nontaxable")]
        //public decimal? ElectronicsNontaxable { get; set; }

        //[DataType(DataType.Currency), Display(Name = "Travel Business Expense Nontaxable")]
        //public decimal? TravelBusinessExpenseNontaxable { get; set; }

        //[DataType(DataType.Currency), Display(Name = "Holiday")]
        //public decimal? HolidayPay { get; set; }

        [DataType(DataType.Currency), Display(Name = "Gross Pay")]
        public decimal? GrossPay { get; set; }

        //[DataType(DataType.Currency), Display(Name = "Federal Taxable Gross")]
        //public decimal? FederalTaxableGross { get; set; }

        //[DataType(DataType.Currency), Display(Name = "Federal Withholding")]
        //public decimal? FederalWithholding { get; set; }

        //[DataType(DataType.Currency), Display(Name = "YTD Federal Withholding")]
        //public decimal? YTDFederalWithholding { get; set; }

        //[DataType(DataType.Currency), Display(Name = "Federal Medicaid")]
        //public decimal? FederalMedicaidWithholding { get; set; }

        //[DataType(DataType.Currency), Display(Name = "YTD Federal Medicaid")]
        //public decimal? YTDFederalMedicaidWithholding { get; set; }

        //[DataType(DataType.Currency), Display(Name = "Social Security")]
        //public decimal? SocialSecurityWithholding { get; set; }

        //[DataType(DataType.Currency), Display(Name = "YTD Social Security")]
        //public decimal? YTDSocialSecurityWithholding { get; set; }

        //[DataType(DataType.Currency), Display(Name = "State Withholding")]
        //public decimal? StateTaxWithholding { get; set; }

        //[DataType(DataType.Currency), Display(Name = "YTD State Withholding")]
        //public decimal? YTDStateTaxWithholding { get; set; }

        //[DataType(DataType.Currency), Display(Name = "City Withholding")]
        //public decimal? CityTaxWithholding { get; set; }

        //[DataType(DataType.Currency), Display(Name = "YTD City Withholding")]
        //public decimal? YTDCityTaxWithholding { get; set; }

        //[DataType(DataType.Currency), Display(Name = "401k/Roth Combination")]
        //public decimal? IRA401KWithholding { get; set; }

        //[DataType(DataType.Currency), Display(Name = "YTD 401k/Roth Combination")]
        //public decimal? YTDIRA401KWithholding { get; set; }

        //[DataType(DataType.Currency), Display(Name = "Dependent Care FSA")]
        //public decimal? DependentCareFSAWithholding { get; set; }

        //[DataType(DataType.Currency), Display(Name = "YTD Dependent Care FSA")]
        //public decimal? YTDDependentCareFSAWithholding { get; set; }

        //[DataType(DataType.Currency), Display(Name = "Health Insurance")]
        //public decimal? HealthInsuranceWithholding { get; set; }

        //[DataType(DataType.Currency), Display(Name = "YTD Health Insurance")]
        //public decimal? YTDHealthInsuranceWithholding { get; set; }

        //[DataType(DataType.Currency), Display(Name = "Dental Insurance")]
        //public decimal? DentalInsuranceWithholding { get; set; }

        //[DataType(DataType.Currency), Display(Name = "YTD Dental Insurance")]
        //public decimal? YTDDentalInsuranceWithholding { get; set; }

        //[DataType(DataType.Currency), Display(Name = "Total Before Tax Deductions")]
        //public virtual decimal? TotalBeforeTaxDeductions { get; set; }

        //[DataType(DataType.Currency), Display(Name = "YTD Total Before Tax Deductions")]
        //public virtual decimal? YTDTotalBeforeTaxDeductions { get; set; }

        //[DataType(DataType.Currency), Display(Name = "Child Support")]
        //public decimal? ChildSupportWithholding { get; set; }

        //[DataType(DataType.Currency), Display(Name = "YTD Child Support")]
        //public decimal? YTDChildSupportWithholding { get; set; }

        //[DataType(DataType.Currency), Display(Name = "Total After Tax Deductions")]
        //public virtual decimal? TotalAfterTaxDeductions { get; set; }

        //[DataType(DataType.Currency), Display(Name = "YTD Total After Tax Deductions")]
        //public virtual decimal? YTDTotalAfterTaxDeductions { get; set; }

        //[DataType(DataType.Currency), Display(Name = "Total Deductions")]
        //public virtual decimal? TotalDeductions { get; set; }

        //[DataType(DataType.Currency), Display(Name = "YTD Total Deductions")]
        //public virtual decimal? YTDTotalDeductions { get; set; }

        [DataType(DataType.Currency), Display(Name = "Net Pay")]
        public decimal NetPay { get; set; }

        //public MaritalStatusEnum MaritalStatus { get; set; }

        //public int Allowances { get; set; }
    }
}
namespace JPFData.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdatesPaycheckNullables : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Paychecks", "Regular", c => c.Decimal(precision: 18, scale: 2));
            AlterColumn("dbo.Paychecks", "ElectronicsNontaxable", c => c.Decimal(precision: 18, scale: 2));
            AlterColumn("dbo.Paychecks", "TravelBusinessExpenseNontaxable", c => c.Decimal(precision: 18, scale: 2));
            AlterColumn("dbo.Paychecks", "HolidayPay", c => c.Decimal(precision: 18, scale: 2));
            AlterColumn("dbo.Paychecks", "GrossPay", c => c.Decimal(precision: 18, scale: 2));
            AlterColumn("dbo.Paychecks", "FederalTaxableGross", c => c.Decimal(precision: 18, scale: 2));
            AlterColumn("dbo.Paychecks", "FederalWithholding", c => c.Decimal(precision: 18, scale: 2));
            AlterColumn("dbo.Paychecks", "YTDFederalWithholding", c => c.Decimal(precision: 18, scale: 2));
            AlterColumn("dbo.Paychecks", "FederalMedicaidWithholding", c => c.Decimal(precision: 18, scale: 2));
            AlterColumn("dbo.Paychecks", "YTDFederalMedicaidWithholding", c => c.Decimal(precision: 18, scale: 2));
            AlterColumn("dbo.Paychecks", "SocialSecurityWithholding", c => c.Decimal(precision: 18, scale: 2));
            AlterColumn("dbo.Paychecks", "YTDSocialSecurityWithholding", c => c.Decimal(precision: 18, scale: 2));
            AlterColumn("dbo.Paychecks", "StateTaxWithholding", c => c.Decimal(precision: 18, scale: 2));
            AlterColumn("dbo.Paychecks", "YTDStateTaxWithholding", c => c.Decimal(precision: 18, scale: 2));
            AlterColumn("dbo.Paychecks", "CityTaxWithholding", c => c.Decimal(precision: 18, scale: 2));
            AlterColumn("dbo.Paychecks", "YTDCityTaxWithholding", c => c.Decimal(precision: 18, scale: 2));
            AlterColumn("dbo.Paychecks", "IRA401KWithholding", c => c.Decimal(precision: 18, scale: 2));
            AlterColumn("dbo.Paychecks", "YTDIRA401KWithholding", c => c.Decimal(precision: 18, scale: 2));
            AlterColumn("dbo.Paychecks", "DependentCareFSAWithholding", c => c.Decimal(precision: 18, scale: 2));
            AlterColumn("dbo.Paychecks", "YTDDependentCareFSAWithholding", c => c.Decimal(precision: 18, scale: 2));
            AlterColumn("dbo.Paychecks", "HealthInsuranceWithholding", c => c.Decimal(precision: 18, scale: 2));
            AlterColumn("dbo.Paychecks", "YTDHealthInsuranceWithholding", c => c.Decimal(precision: 18, scale: 2));
            AlterColumn("dbo.Paychecks", "DentalInsuranceWithholding", c => c.Decimal(precision: 18, scale: 2));
            AlterColumn("dbo.Paychecks", "YTDDentalInsuranceWithholding", c => c.Decimal(precision: 18, scale: 2));
            AlterColumn("dbo.Paychecks", "TotalBeforeTaxDeductions", c => c.Decimal(precision: 18, scale: 2));
            AlterColumn("dbo.Paychecks", "YTDTotalBeforeTaxDeductions", c => c.Decimal(precision: 18, scale: 2));
            AlterColumn("dbo.Paychecks", "ChildSupportWithholding", c => c.Decimal(precision: 18, scale: 2));
            AlterColumn("dbo.Paychecks", "YTDChildSupportWithholding", c => c.Decimal(precision: 18, scale: 2));
            AlterColumn("dbo.Paychecks", "TotalAfterTaxDeductions", c => c.Decimal(precision: 18, scale: 2));
            AlterColumn("dbo.Paychecks", "YTDTotalAfterTaxDeductions", c => c.Decimal(precision: 18, scale: 2));
            AlterColumn("dbo.Paychecks", "TotalDeductions", c => c.Decimal(precision: 18, scale: 2));
            AlterColumn("dbo.Paychecks", "YTDTotalDeductions", c => c.Decimal(precision: 18, scale: 2));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Paychecks", "YTDTotalDeductions", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AlterColumn("dbo.Paychecks", "TotalDeductions", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AlterColumn("dbo.Paychecks", "YTDTotalAfterTaxDeductions", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AlterColumn("dbo.Paychecks", "TotalAfterTaxDeductions", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AlterColumn("dbo.Paychecks", "YTDChildSupportWithholding", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AlterColumn("dbo.Paychecks", "ChildSupportWithholding", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AlterColumn("dbo.Paychecks", "YTDTotalBeforeTaxDeductions", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AlterColumn("dbo.Paychecks", "TotalBeforeTaxDeductions", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AlterColumn("dbo.Paychecks", "YTDDentalInsuranceWithholding", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AlterColumn("dbo.Paychecks", "DentalInsuranceWithholding", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AlterColumn("dbo.Paychecks", "YTDHealthInsuranceWithholding", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AlterColumn("dbo.Paychecks", "HealthInsuranceWithholding", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AlterColumn("dbo.Paychecks", "YTDDependentCareFSAWithholding", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AlterColumn("dbo.Paychecks", "DependentCareFSAWithholding", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AlterColumn("dbo.Paychecks", "YTDIRA401KWithholding", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AlterColumn("dbo.Paychecks", "IRA401KWithholding", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AlterColumn("dbo.Paychecks", "YTDCityTaxWithholding", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AlterColumn("dbo.Paychecks", "CityTaxWithholding", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AlterColumn("dbo.Paychecks", "YTDStateTaxWithholding", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AlterColumn("dbo.Paychecks", "StateTaxWithholding", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AlterColumn("dbo.Paychecks", "YTDSocialSecurityWithholding", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AlterColumn("dbo.Paychecks", "SocialSecurityWithholding", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AlterColumn("dbo.Paychecks", "YTDFederalMedicaidWithholding", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AlterColumn("dbo.Paychecks", "FederalMedicaidWithholding", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AlterColumn("dbo.Paychecks", "YTDFederalWithholding", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AlterColumn("dbo.Paychecks", "FederalWithholding", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AlterColumn("dbo.Paychecks", "FederalTaxableGross", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AlterColumn("dbo.Paychecks", "GrossPay", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AlterColumn("dbo.Paychecks", "HolidayPay", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AlterColumn("dbo.Paychecks", "TravelBusinessExpenseNontaxable", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AlterColumn("dbo.Paychecks", "ElectronicsNontaxable", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AlterColumn("dbo.Paychecks", "Regular", c => c.Decimal(nullable: false, precision: 18, scale: 2));
        }
    }
}

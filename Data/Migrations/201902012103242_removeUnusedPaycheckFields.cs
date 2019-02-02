namespace JPFData.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class removeUnusedPaycheckFields : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.Paychecks", "Regular");
            DropColumn("dbo.Paychecks", "ElectronicsNontaxable");
            DropColumn("dbo.Paychecks", "TravelBusinessExpenseNontaxable");
            DropColumn("dbo.Paychecks", "HolidayPay");
            DropColumn("dbo.Paychecks", "FederalTaxableGross");
            DropColumn("dbo.Paychecks", "FederalWithholding");
            DropColumn("dbo.Paychecks", "YTDFederalWithholding");
            DropColumn("dbo.Paychecks", "FederalMedicaidWithholding");
            DropColumn("dbo.Paychecks", "YTDFederalMedicaidWithholding");
            DropColumn("dbo.Paychecks", "SocialSecurityWithholding");
            DropColumn("dbo.Paychecks", "YTDSocialSecurityWithholding");
            DropColumn("dbo.Paychecks", "StateTaxWithholding");
            DropColumn("dbo.Paychecks", "YTDStateTaxWithholding");
            DropColumn("dbo.Paychecks", "CityTaxWithholding");
            DropColumn("dbo.Paychecks", "YTDCityTaxWithholding");
            DropColumn("dbo.Paychecks", "IRA401KWithholding");
            DropColumn("dbo.Paychecks", "YTDIRA401KWithholding");
            DropColumn("dbo.Paychecks", "DependentCareFSAWithholding");
            DropColumn("dbo.Paychecks", "YTDDependentCareFSAWithholding");
            DropColumn("dbo.Paychecks", "HealthInsuranceWithholding");
            DropColumn("dbo.Paychecks", "YTDHealthInsuranceWithholding");
            DropColumn("dbo.Paychecks", "DentalInsuranceWithholding");
            DropColumn("dbo.Paychecks", "YTDDentalInsuranceWithholding");
            DropColumn("dbo.Paychecks", "TotalBeforeTaxDeductions");
            DropColumn("dbo.Paychecks", "YTDTotalBeforeTaxDeductions");
            DropColumn("dbo.Paychecks", "ChildSupportWithholding");
            DropColumn("dbo.Paychecks", "YTDChildSupportWithholding");
            DropColumn("dbo.Paychecks", "TotalAfterTaxDeductions");
            DropColumn("dbo.Paychecks", "YTDTotalAfterTaxDeductions");
            DropColumn("dbo.Paychecks", "TotalDeductions");
            DropColumn("dbo.Paychecks", "YTDTotalDeductions");
            DropColumn("dbo.Paychecks", "MaritalStatus");
            DropColumn("dbo.Paychecks", "Allowances");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Paychecks", "Allowances", c => c.Int(nullable: false));
            AddColumn("dbo.Paychecks", "MaritalStatus", c => c.Int(nullable: false));
            AddColumn("dbo.Paychecks", "YTDTotalDeductions", c => c.Decimal(precision: 18, scale: 2));
            AddColumn("dbo.Paychecks", "TotalDeductions", c => c.Decimal(precision: 18, scale: 2));
            AddColumn("dbo.Paychecks", "YTDTotalAfterTaxDeductions", c => c.Decimal(precision: 18, scale: 2));
            AddColumn("dbo.Paychecks", "TotalAfterTaxDeductions", c => c.Decimal(precision: 18, scale: 2));
            AddColumn("dbo.Paychecks", "YTDChildSupportWithholding", c => c.Decimal(precision: 18, scale: 2));
            AddColumn("dbo.Paychecks", "ChildSupportWithholding", c => c.Decimal(precision: 18, scale: 2));
            AddColumn("dbo.Paychecks", "YTDTotalBeforeTaxDeductions", c => c.Decimal(precision: 18, scale: 2));
            AddColumn("dbo.Paychecks", "TotalBeforeTaxDeductions", c => c.Decimal(precision: 18, scale: 2));
            AddColumn("dbo.Paychecks", "YTDDentalInsuranceWithholding", c => c.Decimal(precision: 18, scale: 2));
            AddColumn("dbo.Paychecks", "DentalInsuranceWithholding", c => c.Decimal(precision: 18, scale: 2));
            AddColumn("dbo.Paychecks", "YTDHealthInsuranceWithholding", c => c.Decimal(precision: 18, scale: 2));
            AddColumn("dbo.Paychecks", "HealthInsuranceWithholding", c => c.Decimal(precision: 18, scale: 2));
            AddColumn("dbo.Paychecks", "YTDDependentCareFSAWithholding", c => c.Decimal(precision: 18, scale: 2));
            AddColumn("dbo.Paychecks", "DependentCareFSAWithholding", c => c.Decimal(precision: 18, scale: 2));
            AddColumn("dbo.Paychecks", "YTDIRA401KWithholding", c => c.Decimal(precision: 18, scale: 2));
            AddColumn("dbo.Paychecks", "IRA401KWithholding", c => c.Decimal(precision: 18, scale: 2));
            AddColumn("dbo.Paychecks", "YTDCityTaxWithholding", c => c.Decimal(precision: 18, scale: 2));
            AddColumn("dbo.Paychecks", "CityTaxWithholding", c => c.Decimal(precision: 18, scale: 2));
            AddColumn("dbo.Paychecks", "YTDStateTaxWithholding", c => c.Decimal(precision: 18, scale: 2));
            AddColumn("dbo.Paychecks", "StateTaxWithholding", c => c.Decimal(precision: 18, scale: 2));
            AddColumn("dbo.Paychecks", "YTDSocialSecurityWithholding", c => c.Decimal(precision: 18, scale: 2));
            AddColumn("dbo.Paychecks", "SocialSecurityWithholding", c => c.Decimal(precision: 18, scale: 2));
            AddColumn("dbo.Paychecks", "YTDFederalMedicaidWithholding", c => c.Decimal(precision: 18, scale: 2));
            AddColumn("dbo.Paychecks", "FederalMedicaidWithholding", c => c.Decimal(precision: 18, scale: 2));
            AddColumn("dbo.Paychecks", "YTDFederalWithholding", c => c.Decimal(precision: 18, scale: 2));
            AddColumn("dbo.Paychecks", "FederalWithholding", c => c.Decimal(precision: 18, scale: 2));
            AddColumn("dbo.Paychecks", "FederalTaxableGross", c => c.Decimal(precision: 18, scale: 2));
            AddColumn("dbo.Paychecks", "HolidayPay", c => c.Decimal(precision: 18, scale: 2));
            AddColumn("dbo.Paychecks", "TravelBusinessExpenseNontaxable", c => c.Decimal(precision: 18, scale: 2));
            AddColumn("dbo.Paychecks", "ElectronicsNontaxable", c => c.Decimal(precision: 18, scale: 2));
            AddColumn("dbo.Paychecks", "Regular", c => c.Decimal(precision: 18, scale: 2));
        }
    }
}

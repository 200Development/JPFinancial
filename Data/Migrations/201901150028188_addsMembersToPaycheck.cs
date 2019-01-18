namespace JPFData.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addsMembersToPaycheck : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Paychecks", "Regular", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("dbo.Paychecks", "ElectronicsNontaxable", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("dbo.Paychecks", "TravelBusinessExpenseNontaxable", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("dbo.Paychecks", "HolidayPay", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("dbo.Paychecks", "GrossPay", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("dbo.Paychecks", "FederalTaxableGross", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("dbo.Paychecks", "FederalWithholding", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("dbo.Paychecks", "YTDFederalWithholding", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("dbo.Paychecks", "FederalMedicaidWithholding", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("dbo.Paychecks", "YTDFederalMedicaidWithholding", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("dbo.Paychecks", "SocialSecurityWithholding", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("dbo.Paychecks", "YTDSocialSecurityWithholding", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("dbo.Paychecks", "StateTaxWithholding", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("dbo.Paychecks", "YTDStateTaxWithholding", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("dbo.Paychecks", "CityTaxWithholding", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("dbo.Paychecks", "YTDCityTaxWithholding", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("dbo.Paychecks", "FedWithholding", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("dbo.Paychecks", "YTDFedWithholding", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("dbo.Paychecks", "IRA401KWithholding", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("dbo.Paychecks", "YTDIRA401KWithholding", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("dbo.Paychecks", "DependentCareFSAWithholding", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("dbo.Paychecks", "YTDDependentCareFSAWithholding", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("dbo.Paychecks", "HealthInsuranceWithholding", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("dbo.Paychecks", "YTDHealthInsuranceWithholding", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("dbo.Paychecks", "DentalInsuranceWithholding", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("dbo.Paychecks", "YTDDentalInsuranceWithholding", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("dbo.Paychecks", "TotalBeforeTaxDeductions", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("dbo.Paychecks", "YTDTotalBeforeTaxDeductions", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("dbo.Paychecks", "ChildSupportWithholding", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("dbo.Paychecks", "YTDChildSupportWithholding", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("dbo.Paychecks", "TotalAfterTaxDeductions", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("dbo.Paychecks", "YTDTotalAfterTaxDeductions", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("dbo.Paychecks", "TotalDeductions", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("dbo.Paychecks", "YTDTotalDeductions", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("dbo.Paychecks", "NetPay", c => c.Decimal(nullable: false, precision: 18, scale: 2));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Paychecks", "NetPay");
            DropColumn("dbo.Paychecks", "YTDTotalDeductions");
            DropColumn("dbo.Paychecks", "TotalDeductions");
            DropColumn("dbo.Paychecks", "YTDTotalAfterTaxDeductions");
            DropColumn("dbo.Paychecks", "TotalAfterTaxDeductions");
            DropColumn("dbo.Paychecks", "YTDChildSupportWithholding");
            DropColumn("dbo.Paychecks", "ChildSupportWithholding");
            DropColumn("dbo.Paychecks", "YTDTotalBeforeTaxDeductions");
            DropColumn("dbo.Paychecks", "TotalBeforeTaxDeductions");
            DropColumn("dbo.Paychecks", "YTDDentalInsuranceWithholding");
            DropColumn("dbo.Paychecks", "DentalInsuranceWithholding");
            DropColumn("dbo.Paychecks", "YTDHealthInsuranceWithholding");
            DropColumn("dbo.Paychecks", "HealthInsuranceWithholding");
            DropColumn("dbo.Paychecks", "YTDDependentCareFSAWithholding");
            DropColumn("dbo.Paychecks", "DependentCareFSAWithholding");
            DropColumn("dbo.Paychecks", "YTDIRA401KWithholding");
            DropColumn("dbo.Paychecks", "IRA401KWithholding");
            DropColumn("dbo.Paychecks", "YTDFedWithholding");
            DropColumn("dbo.Paychecks", "FedWithholding");
            DropColumn("dbo.Paychecks", "YTDCityTaxWithholding");
            DropColumn("dbo.Paychecks", "CityTaxWithholding");
            DropColumn("dbo.Paychecks", "YTDStateTaxWithholding");
            DropColumn("dbo.Paychecks", "StateTaxWithholding");
            DropColumn("dbo.Paychecks", "YTDSocialSecurityWithholding");
            DropColumn("dbo.Paychecks", "SocialSecurityWithholding");
            DropColumn("dbo.Paychecks", "YTDFederalMedicaidWithholding");
            DropColumn("dbo.Paychecks", "FederalMedicaidWithholding");
            DropColumn("dbo.Paychecks", "YTDFederalWithholding");
            DropColumn("dbo.Paychecks", "FederalWithholding");
            DropColumn("dbo.Paychecks", "FederalTaxableGross");
            DropColumn("dbo.Paychecks", "GrossPay");
            DropColumn("dbo.Paychecks", "HolidayPay");
            DropColumn("dbo.Paychecks", "TravelBusinessExpenseNontaxable");
            DropColumn("dbo.Paychecks", "ElectronicsNontaxable");
            DropColumn("dbo.Paychecks", "Regular");
        }
    }
}

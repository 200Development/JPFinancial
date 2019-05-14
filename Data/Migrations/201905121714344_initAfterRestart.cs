namespace JPFData.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class initAfterRestart : DbMigration
    {
        public override void Up()
        {
        }
        
        public override void Down()
        {
            CreateTable(
                "dbo.Taxes",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.Int(nullable: false),
                        Amount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        YTDAmount = c.Decimal(precision: 18, scale: 2),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Paychecks",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Employer = c.String(nullable: false),
                        Date = c.DateTime(nullable: false),
                        Regular = c.Decimal(precision: 18, scale: 2),
                        ElectronicsNontaxable = c.Decimal(precision: 18, scale: 2),
                        TravelBusinessExpenseNontaxable = c.Decimal(precision: 18, scale: 2),
                        HolidayPay = c.Decimal(precision: 18, scale: 2),
                        GrossPay = c.Decimal(precision: 18, scale: 2),
                        FederalTaxableGross = c.Decimal(precision: 18, scale: 2),
                        FederalWithholding = c.Decimal(precision: 18, scale: 2),
                        YTDFederalWithholding = c.Decimal(precision: 18, scale: 2),
                        FederalMedicaidWithholding = c.Decimal(precision: 18, scale: 2),
                        YTDFederalMedicaidWithholding = c.Decimal(precision: 18, scale: 2),
                        SocialSecurityWithholding = c.Decimal(precision: 18, scale: 2),
                        YTDSocialSecurityWithholding = c.Decimal(precision: 18, scale: 2),
                        StateTaxWithholding = c.Decimal(precision: 18, scale: 2),
                        YTDStateTaxWithholding = c.Decimal(precision: 18, scale: 2),
                        CityTaxWithholding = c.Decimal(precision: 18, scale: 2),
                        YTDCityTaxWithholding = c.Decimal(precision: 18, scale: 2),
                        IRA401KWithholding = c.Decimal(precision: 18, scale: 2),
                        YTDIRA401KWithholding = c.Decimal(precision: 18, scale: 2),
                        DependentCareFSAWithholding = c.Decimal(precision: 18, scale: 2),
                        YTDDependentCareFSAWithholding = c.Decimal(precision: 18, scale: 2),
                        HealthInsuranceWithholding = c.Decimal(precision: 18, scale: 2),
                        YTDHealthInsuranceWithholding = c.Decimal(precision: 18, scale: 2),
                        DentalInsuranceWithholding = c.Decimal(precision: 18, scale: 2),
                        YTDDentalInsuranceWithholding = c.Decimal(precision: 18, scale: 2),
                        TotalBeforeTaxDeductions = c.Decimal(precision: 18, scale: 2),
                        YTDTotalBeforeTaxDeductions = c.Decimal(precision: 18, scale: 2),
                        ChildSupportWithholding = c.Decimal(precision: 18, scale: 2),
                        YTDChildSupportWithholding = c.Decimal(precision: 18, scale: 2),
                        TotalAfterTaxDeductions = c.Decimal(precision: 18, scale: 2),
                        YTDTotalAfterTaxDeductions = c.Decimal(precision: 18, scale: 2),
                        TotalDeductions = c.Decimal(precision: 18, scale: 2),
                        YTDTotalDeductions = c.Decimal(precision: 18, scale: 2),
                        NetPay = c.Decimal(nullable: false, precision: 18, scale: 2),
                        MaritalStatus = c.Int(nullable: false),
                        Allowances = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Earnings",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Description = c.Int(nullable: false),
                        Rate = c.Decimal(precision: 18, scale: 2),
                        Hours = c.Double(),
                        Units = c.Double(),
                        Amount = c.Decimal(precision: 18, scale: 2),
                        YTDHours = c.Double(),
                        YTDUnits = c.Double(),
                        YTDAmount = c.Decimal(precision: 18, scale: 2),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Deductions",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.Int(nullable: false),
                        Amount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        YTDAmount = c.Decimal(precision: 18, scale: 2),
                        BeforeTax = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Companies",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false),
                        StreetAddress = c.String(),
                        City = c.String(),
                        StatesEnum = c.Int(nullable: false),
                        Zipcode = c.String(),
                        Phone = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Bonus",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false),
                        Amount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        DatePaid = c.DateTime(nullable: false),
                        SalaryId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Benefits",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false),
                        Amount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        YTDAmount = c.Decimal(precision: 18, scale: 2),
                        Taxable = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            AddColumn("dbo.Transactions", "PaycheckId", c => c.Int());
            AddColumn("dbo.Transactions", "SelectedCreditCardAccount", c => c.Int());
            AddColumn("dbo.Expenses", "IsPreTax", c => c.Boolean(nullable: false));
            AddColumn("dbo.Expenses", "SalaryId", c => c.Int(nullable: false));
            AlterColumn("dbo.Transactions", "Memo", c => c.String());
            AlterColumn("dbo.Transactions", "Payee", c => c.String());
            AlterColumn("dbo.Accounts", "BalanceSurplus", c => c.Decimal(precision: 18, scale: 2));
            AlterColumn("dbo.Accounts", "RequiredSavings", c => c.Decimal(precision: 18, scale: 2));
            AlterColumn("dbo.Accounts", "SuggestedPaycheckContribution", c => c.Decimal(precision: 18, scale: 2));
            AlterColumn("dbo.Accounts", "PaycheckContribution", c => c.Decimal(precision: 18, scale: 2));
            DropColumn("dbo.Transactions", "SelectedExpenseId");
            DropColumn("dbo.Transactions", "SelectedCreditCardAccountId");
            DropColumn("dbo.Transactions", "UserId");
            DropColumn("dbo.Expenses", "IsPaid");
            DropColumn("dbo.Expenses", "CreditAccountId");
            DropColumn("dbo.Expenses", "BillId");
            DropColumn("dbo.Expenses", "Due");
            DropColumn("dbo.Accounts", "BalanceLimit");
            DropColumn("dbo.Accounts", "IsMandatory");
            DropColumn("dbo.Accounts", "UserId");
            DropTable("dbo.Rebalances");
            CreateIndex("dbo.Transactions", "PaycheckId");
            AddForeignKey("dbo.Transactions", "PaycheckId", "dbo.Paychecks", "Id");
        }
    }
}

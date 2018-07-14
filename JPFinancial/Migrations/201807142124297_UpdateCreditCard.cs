namespace JPFinancial.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdateCreditCard : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.CreditCards", "StatementBalance", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("dbo.CreditCards", "CreditLimit", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("dbo.CreditCards", "AvailableCredit", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("dbo.CreditCards", "MinimumPaymentDue", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("dbo.CreditCards", "PurchaseApr", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("dbo.CreditCards", "CashAdvanceApr", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("dbo.CreditCards", "GracePeriodDays", c => c.Int(nullable: false));
            DropColumn("dbo.CreditCards", "PrincipalBalance");
            DropColumn("dbo.CreditCards", "CompoundedInterest");
            DropColumn("dbo.CreditCards", "DueDayOfMonth");
            DropColumn("dbo.CreditCards", "APR");
        }
        
        public override void Down()
        {
            AddColumn("dbo.CreditCards", "APR", c => c.Decimal(precision: 18, scale: 2));
            AddColumn("dbo.CreditCards", "DueDayOfMonth", c => c.Int(nullable: false));
            AddColumn("dbo.CreditCards", "CompoundedInterest", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("dbo.CreditCards", "PrincipalBalance", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            DropColumn("dbo.CreditCards", "GracePeriodDays");
            DropColumn("dbo.CreditCards", "CashAdvanceApr");
            DropColumn("dbo.CreditCards", "PurchaseApr");
            DropColumn("dbo.CreditCards", "MinimumPaymentDue");
            DropColumn("dbo.CreditCards", "AvailableCredit");
            DropColumn("dbo.CreditCards", "CreditLimit");
            DropColumn("dbo.CreditCards", "StatementBalance");
        }
    }
}

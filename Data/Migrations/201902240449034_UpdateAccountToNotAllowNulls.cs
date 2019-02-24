namespace JPFData.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdateAccountToNotAllowNulls : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Accounts", "PaycheckContribution", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AlterColumn("dbo.Accounts", "SuggestedPaycheckContribution", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AlterColumn("dbo.Accounts", "RequiredSavings", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AlterColumn("dbo.Accounts", "BalanceSurplus", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AlterColumn("dbo.Accounts", "BalanceLimit", c => c.Decimal(nullable: false, precision: 18, scale: 2));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Accounts", "BalanceLimit", c => c.Decimal(precision: 18, scale: 2));
            AlterColumn("dbo.Accounts", "BalanceSurplus", c => c.Decimal(precision: 18, scale: 2));
            AlterColumn("dbo.Accounts", "RequiredSavings", c => c.Decimal(precision: 18, scale: 2));
            AlterColumn("dbo.Accounts", "SuggestedPaycheckContribution", c => c.Decimal(precision: 18, scale: 2));
            AlterColumn("dbo.Accounts", "PaycheckContribution", c => c.Decimal(precision: 18, scale: 2));
        }
    }
}

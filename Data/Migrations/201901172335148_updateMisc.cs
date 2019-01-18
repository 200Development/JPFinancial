namespace JPFData.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class updateMisc : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Accounts", "BudgetRequiredContribution", c => c.Decimal(precision: 18, scale: 2));
            DropColumn("dbo.Accounts", "RequiredPaycheckContribution");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Accounts", "RequiredPaycheckContribution", c => c.Decimal(precision: 18, scale: 2));
            DropColumn("dbo.Accounts", "BudgetRequiredContribution");
        }
    }
}

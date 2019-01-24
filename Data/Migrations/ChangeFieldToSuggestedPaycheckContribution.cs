namespace JPFData.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ChangeFieldToSuggestedPaycheckContribution : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Accounts", "SuggestedPaycheckContribution", c => c.Decimal(precision: 18, scale: 2));
            DropColumn("dbo.Accounts", "BudgetRequiredContribution");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Accounts", "BudgetRequiredContribution", c => c.Decimal(precision: 18, scale: 2));
            DropColumn("dbo.Accounts", "SuggestedPaycheckContribution");
        }
    }
}

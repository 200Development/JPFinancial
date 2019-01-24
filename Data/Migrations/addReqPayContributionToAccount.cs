namespace JPFData.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addReqPayContributionToAccount : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Accounts", "RequiredPaycheckContribution", c => c.Decimal(precision: 18, scale: 2));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Accounts", "RequiredPaycheckContribution");
        }
    }
}

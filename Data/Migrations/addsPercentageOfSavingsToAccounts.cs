namespace JPFData.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addsPercentageOfSavingsToAccounts : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Accounts", "PercentageOfSavings", c => c.Decimal(nullable: false, precision: 18, scale: 2));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Accounts", "PercentageOfSavings");
        }
    }
}

namespace JPFData.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddsPropertiesToAccounts : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Accounts", "BalanceLimit", c => c.Decimal(precision: 18, scale: 2));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Accounts", "BalanceLimit", c => c.Decimal(nullable: false, precision: 18, scale: 2));
        }
    }
}

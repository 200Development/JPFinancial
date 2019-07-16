namespace JPFData.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class updateAccountModel : DbMigration
    {
        public override void Up()
        {
        }
        
        public override void Down()
        {
            AddColumn("dbo.Accounts", "PercentageOfSavings", c => c.Decimal(nullable: false, precision: 18, scale: 2));
        }
    }
}

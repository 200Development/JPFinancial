namespace JPFinancial.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class updateTables : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Accounts", "PaycheckContribution", c => c.Decimal(precision: 18, scale: 2));
            DropColumn("dbo.Accounts", "Goal");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Accounts", "Goal", c => c.Decimal(precision: 18, scale: 2));
            DropColumn("dbo.Accounts", "PaycheckContribution");
        }
    }
}

namespace JPFData.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RemoveIsPaidFromBills : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Expenses", "CreditAccountId", c => c.Int(nullable: false));
            DropColumn("dbo.Bills", "IsPaid");
            DropColumn("dbo.Expenses", "CreditAccount");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Expenses", "CreditAccount", c => c.Int(nullable: false));
            AddColumn("dbo.Bills", "IsPaid", c => c.Boolean(nullable: false));
            DropColumn("dbo.Expenses", "CreditAccountId");
        }
    }
}

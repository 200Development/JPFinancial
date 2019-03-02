namespace JPFData.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdatesExpenseTable : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Expenses", "Due", c => c.DateTime(nullable: false));
            AddColumn("dbo.Expenses", "BillId", c => c.Int(nullable: false));
            AddColumn("dbo.Expenses", "CreditAccount", c => c.Int(nullable: false));
            AddColumn("dbo.Expenses", "IsPaid", c => c.Boolean(nullable: false));
            DropColumn("dbo.Expenses", "SalaryId");
            DropColumn("dbo.Expenses", "IsPreTax");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Expenses", "IsPreTax", c => c.Boolean(nullable: false));
            AddColumn("dbo.Expenses", "SalaryId", c => c.Int(nullable: false));
            DropColumn("dbo.Expenses", "IsPaid");
            DropColumn("dbo.Expenses", "CreditAccount");
            DropColumn("dbo.Expenses", "BillId");
            DropColumn("dbo.Expenses", "Due");
        }
    }
}

namespace JPFData.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdateTransactionToGetExpenseIdInsteadOfBillId : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Transactions", "SelectedExpenseId", c => c.Int());
            AddColumn("dbo.Transactions", "SelectedBillDueDate", c => c.DateTime(nullable: false));
            DropColumn("dbo.Transactions", "SelectedBillId");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Transactions", "SelectedBillId", c => c.Int());
            DropColumn("dbo.Transactions", "SelectedBillDueDate");
            DropColumn("dbo.Transactions", "SelectedExpenseId");
        }
    }
}

namespace JPFData.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdateTransactionSelectedBillDueDateToNullable : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Transactions", "SelectedBillDueDate", c => c.DateTime());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Transactions", "SelectedBillDueDate", c => c.DateTime(nullable: false));
        }
    }
}

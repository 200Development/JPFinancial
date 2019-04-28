namespace JPFData.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addUserIdToTransactionUpdateAccount : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Transactions", "UserId", c => c.String(nullable: false));
            AlterColumn("dbo.Transactions", "Payee", c => c.String(nullable: false, maxLength: 255));
            AlterColumn("dbo.Transactions", "Memo", c => c.String(maxLength: 255));
            DropColumn("dbo.Transactions", "SelectedBillDueDate");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Transactions", "SelectedBillDueDate", c => c.DateTime());
            AlterColumn("dbo.Transactions", "Memo", c => c.String());
            AlterColumn("dbo.Transactions", "Payee", c => c.String());
            DropColumn("dbo.Transactions", "UserId");
        }
    }
}

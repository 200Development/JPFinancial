namespace JPFData.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddIdToSelectedCCAndBill : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Transactions", "SelectedCreditCardAccountId", c => c.Int());
            AddColumn("dbo.Transactions", "SelectedBillId", c => c.Int());
            DropColumn("dbo.Transactions", "SelectedCreditCardAccount");
            DropColumn("dbo.Transactions", "SelectedBill");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Transactions", "SelectedBill", c => c.Int());
            AddColumn("dbo.Transactions", "SelectedCreditCardAccount", c => c.Int());
            DropColumn("dbo.Transactions", "SelectedBillId");
            DropColumn("dbo.Transactions", "SelectedCreditCardAccountId");
        }
    }
}

namespace JPFinancial.Migrations
{
    using System;
    using System.Data.Entity.Migrations;

    public partial class updateTransactionModel : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Transactions", "TransferFrom_Id", "dbo.Accounts");
            DropForeignKey("dbo.Transactions", "TransferTo_Id", "dbo.Accounts");
            DropIndex("dbo.Transactions", new[] { "TransferFrom_Id" });
            DropIndex("dbo.Transactions", new[] { "TransferTo_Id" });
            DropColumn("dbo.Transactions", "TransferFrom_Id");
            DropColumn("dbo.Transactions", "TransferTo_Id");
        }

        public override void Down()
        {
            AddColumn("dbo.Transactions", "TransferTo_Id", c => c.Int());
            AddColumn("dbo.Transactions", "TransferFrom_Id", c => c.Int());
            CreateIndex("dbo.Transactions", "TransferTo_Id");
            CreateIndex("dbo.Transactions", "TransferFrom_Id");
            AddForeignKey("dbo.Transactions", "TransferTo_Id", "dbo.Accounts", "Id");
            AddForeignKey("dbo.Transactions", "TransferFrom_Id", "dbo.Accounts", "Id");
        }
    }
}

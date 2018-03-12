namespace JPFinancial.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class updateTransactionTableWithFKs : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Transactions", "TransferFrom_Id", c => c.Int());
            AddColumn("dbo.Transactions", "TransferTo_Id", c => c.Int());
            AlterColumn("dbo.Transactions", "Type", c => c.Int(nullable: false));
            AlterColumn("dbo.Transactions", "Category", c => c.Int(nullable: false));
            CreateIndex("dbo.Transactions", "TransferFrom_Id");
            CreateIndex("dbo.Transactions", "TransferTo_Id");
            AddForeignKey("dbo.Transactions", "TransferFrom_Id", "dbo.Accounts", "Id");
            AddForeignKey("dbo.Transactions", "TransferTo_Id", "dbo.Accounts", "Id");
            DropColumn("dbo.Transactions", "TransferTo");
            DropColumn("dbo.Transactions", "TransferFrom");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Transactions", "TransferFrom", c => c.String());
            AddColumn("dbo.Transactions", "TransferTo", c => c.String());
            DropForeignKey("dbo.Transactions", "TransferTo_Id", "dbo.Accounts");
            DropForeignKey("dbo.Transactions", "TransferFrom_Id", "dbo.Accounts");
            DropIndex("dbo.Transactions", new[] { "TransferTo_Id" });
            DropIndex("dbo.Transactions", new[] { "TransferFrom_Id" });
            AlterColumn("dbo.Transactions", "Category", c => c.String());
            AlterColumn("dbo.Transactions", "Type", c => c.String());
            DropColumn("dbo.Transactions", "TransferTo_Id");
            DropColumn("dbo.Transactions", "TransferFrom_Id");
        }
    }
}

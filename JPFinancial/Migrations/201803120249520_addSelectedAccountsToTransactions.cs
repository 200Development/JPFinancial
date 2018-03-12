namespace JPFinancial.Migrations
{
    using System.Data.Entity.Migrations;

    public partial class addSelectedAccountsToTransactions : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Transactions", "CreditAccount_Id", c => c.Int());
            AddColumn("dbo.Transactions", "DebitAccount_Id", c => c.Int());
            CreateIndex("dbo.Transactions", "CreditAccount_Id");
            CreateIndex("dbo.Transactions", "DebitAccount_Id");
            AddForeignKey("dbo.Transactions", "CreditAccount_Id", "dbo.Accounts", "Id");
            AddForeignKey("dbo.Transactions", "DebitAccount_Id", "dbo.Accounts", "Id");
        }

        public override void Down()
        {
            DropForeignKey("dbo.Transactions", "DebitAccount_Id", "dbo.Accounts");
            DropForeignKey("dbo.Transactions", "CreditAccount_Id", "dbo.Accounts");
            DropIndex("dbo.Transactions", new[] { "DebitAccount_Id" });
            DropIndex("dbo.Transactions", new[] { "CreditAccount_Id" });
            DropColumn("dbo.Transactions", "DebitAccount_Id");
            DropColumn("dbo.Transactions", "CreditAccount_Id");
        }
    }
}

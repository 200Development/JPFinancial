namespace JPFinancial.Migrations
{
    using System.Data.Entity.Migrations;

    public partial class RemovesSpendReceiveFromTransactionsModel : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.Transactions", "Spend");
            DropColumn("dbo.Transactions", "Receive");
        }

        public override void Down()
        {
            AddColumn("dbo.Transactions", "Receive", c => c.Decimal(precision: 18, scale: 2));
            AddColumn("dbo.Transactions", "Spend", c => c.Decimal(precision: 18, scale: 2));
        }
    }
}

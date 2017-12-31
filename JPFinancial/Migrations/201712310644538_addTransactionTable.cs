namespace JPFinancial.Migrations
{
    using System;
    using System.Data.Entity.Migrations;

    public partial class addTransactionTable : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Transactions",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Date = c.DateTime(nullable: false),
                        Payee = c.String(),
                        Memo = c.String(),
                        Type = c.String(),
                        Category = c.String(),
                        TransferTo = c.String(),
                        TransferFrom = c.String(),
                        Spend = c.Decimal(precision: 18, scale: 2),
                        Receive = c.Decimal(precision: 18, scale: 2),
                        Amount = c.Decimal(precision: 18, scale: 2),
                    })
                .PrimaryKey(t => t.Id);

        }

        public override void Down()
        {
            DropTable("dbo.Transactions");
        }
    }
}

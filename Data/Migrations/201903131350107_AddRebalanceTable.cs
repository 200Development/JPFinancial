namespace JPFData.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddRebalanceTable : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Rebalances",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Payee = c.String(),
                        Memo = c.String(),
                        Type = c.Int(nullable: false),
                        Category = c.Int(nullable: false),
                        CreditAccountId = c.Int(),
                        DebitAccountId = c.Int(),
                        Date = c.DateTime(nullable: false),
                        Amount = c.Decimal(nullable: false, precision: 18, scale: 2),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.Rebalances");
        }
    }
}

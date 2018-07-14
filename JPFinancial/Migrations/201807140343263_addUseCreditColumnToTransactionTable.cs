namespace JPFinancial.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addUseCreditColumnToTransactionTable : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Transactions", "UsedCreditCard", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Transactions", "UsedCreditCard");
        }
    }
}

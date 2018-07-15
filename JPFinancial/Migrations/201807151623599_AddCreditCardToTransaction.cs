namespace JPFinancial.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddCreditCardToTransaction : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Transactions", "CreditCardId", c => c.Int());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Transactions", "CreditCardId");
        }
    }
}

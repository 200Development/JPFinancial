namespace JPFinancial.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class updateTransactionModel : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Transactions", "SelectedCreditCardAccount", c => c.Int());
            DropColumn("dbo.Transactions", "CreditCardId");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Transactions", "CreditCardId", c => c.Int());
            DropColumn("dbo.Transactions", "SelectedCreditCardAccount");
        }
    }
}

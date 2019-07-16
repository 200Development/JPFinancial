namespace JPFData.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class removesFieldsFromTransaction : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.Transactions", "SelectedCreditCardAccountId");
            DropColumn("dbo.Transactions", "IsBill");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Transactions", "IsBill", c => c.Boolean(nullable: false));
            AddColumn("dbo.Transactions", "SelectedCreditCardAccountId", c => c.Int());
        }
    }
}

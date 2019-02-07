namespace JPFData.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addIsPaidToBills : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Transactions", "SelectedBill", c => c.Int());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Transactions", "SelectedBill");
        }
    }
}

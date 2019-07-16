namespace JPFData.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addsIsBillToTransaction : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Transactions", "IsBill", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Transactions", "IsBill");
        }
    }
}

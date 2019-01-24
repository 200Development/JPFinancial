namespace JPFData.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddPaycheckIdToTransaction : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Transactions", "PaycheckId", c => c.Int());
            CreateIndex("dbo.Transactions", "PaycheckId");
            AddForeignKey("dbo.Transactions", "PaycheckId", "dbo.Paychecks", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Transactions", "PaycheckId", "dbo.Paychecks");
            DropIndex("dbo.Transactions", new[] { "PaycheckId" });
            DropColumn("dbo.Transactions", "PaycheckId");
        }
    }
}

namespace JPFData.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class removePaycheckFromTransaction : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Transactions", "PaycheckId", "dbo.Paychecks");
            DropIndex("dbo.Transactions", new[] { "PaycheckId" });
            DropColumn("dbo.Transactions", "PaycheckId");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Transactions", "PaycheckId", c => c.Int());
            CreateIndex("dbo.Transactions", "PaycheckId");
            AddForeignKey("dbo.Transactions", "PaycheckId", "dbo.Paychecks", "Id");
        }
    }
}

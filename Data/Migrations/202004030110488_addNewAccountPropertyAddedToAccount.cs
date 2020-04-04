namespace JPFData.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addNewAccountPropertyAddedToAccount : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Accounts", "IsAddNewAccount", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Accounts", "IsAddNewAccount");
        }
    }
}

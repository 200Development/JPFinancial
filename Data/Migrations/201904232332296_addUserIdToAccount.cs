namespace JPFData.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addUserIdToAccount : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Accounts", "UserId", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Accounts", "UserId");
        }
    }
}

namespace JPFData.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class updateAccountUserIdToString : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Accounts", "UserId", c => c.String(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Accounts", "UserId", c => c.Int(nullable: false));
        }
    }
}

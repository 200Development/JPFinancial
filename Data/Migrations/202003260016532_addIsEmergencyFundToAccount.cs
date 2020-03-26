namespace JPFData.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addIsEmergencyFundToAccount : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Accounts", "IsEmergencyFund", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Accounts", "IsEmergencyFund");
        }
    }
}

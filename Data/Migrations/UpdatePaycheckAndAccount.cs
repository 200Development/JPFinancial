namespace JPFData.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdatePaycheckAndAccount : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Accounts", "ExcludeFromSurplus", c => c.Boolean(nullable: false));
            AddColumn("dbo.Paychecks", "Date", c => c.DateTime(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Paychecks", "Date");
            DropColumn("dbo.Accounts", "ExcludeFromSurplus");
        }
    }
}

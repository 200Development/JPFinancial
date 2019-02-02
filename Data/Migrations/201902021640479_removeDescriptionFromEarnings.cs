namespace JPFData.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class removeDescriptionFromEarnings : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.Earnings", "Description");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Earnings", "Description", c => c.Int(nullable: false));
        }
    }
}

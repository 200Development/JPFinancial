namespace JPFData.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class removeMandatoryAndLateFromBills : DbMigration
    {
        public override void Up()
        {
        }
        
        public override void Down()
        {
            AddColumn("dbo.Bills", "IsMandatory", c => c.Boolean(nullable: false));
            AddColumn("dbo.Bills", "IsLate", c => c.Boolean(nullable: false));
        }
    }
}

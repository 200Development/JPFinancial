namespace JPFData.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class removeFedWitholding : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.Paychecks", "FedWithholding");
            DropColumn("dbo.Paychecks", "YTDFedWithholding");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Paychecks", "YTDFedWithholding", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("dbo.Paychecks", "FedWithholding", c => c.Decimal(nullable: false, precision: 18, scale: 2));
        }
    }
}

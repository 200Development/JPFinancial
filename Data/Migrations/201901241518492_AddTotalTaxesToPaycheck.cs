namespace JPFData.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddTotalTaxesToPaycheck : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Paychecks", "TotalTaxes", c => c.Decimal(precision: 18, scale: 2));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Paychecks", "TotalTaxes");
        }
    }
}

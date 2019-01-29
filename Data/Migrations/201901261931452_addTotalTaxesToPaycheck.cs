namespace JPFData.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addTotalTaxesToPaycheck : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.Paychecks", "TotalTaxes");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Paychecks", "TotalTaxes", c => c.Decimal(precision: 18, scale: 2));
        }
    }
}

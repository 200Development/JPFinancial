namespace JPFinancial.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class netIncomeAddedToSalaryModel : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Salaries", "NetIncome", c => c.Decimal(precision: 18, scale: 2));
            AlterColumn("dbo.Salaries", "GrossPay", c => c.Decimal(precision: 18, scale: 2));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Salaries", "GrossPay", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            DropColumn("dbo.Salaries", "NetIncome");
        }
    }
}

namespace JPFinancial.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addPaydayOfWeekToSalaryModel : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Salaries", "PaydayOfWeek", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Salaries", "PaydayOfWeek");
        }
    }
}

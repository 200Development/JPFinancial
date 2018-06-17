namespace JPFinancial.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class updateLoansSalariesModels : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Loans", "DueDayOfMonthEnum", c => c.Int(nullable: false));
            AddColumn("dbo.Salaries", "FirstPayday", c => c.String());
            AddColumn("dbo.Salaries", "LastPayday", c => c.String());
            DropColumn("dbo.Loans", "DueDayOfMonth");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Loans", "DueDayOfMonth", c => c.Int(nullable: false));
            DropColumn("dbo.Salaries", "LastPayday");
            DropColumn("dbo.Salaries", "FirstPayday");
            DropColumn("dbo.Loans", "DueDayOfMonthEnum");
        }
    }
}

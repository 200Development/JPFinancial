namespace JPFinancial.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class updateLoanModel : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Loans", "DueDayOfMonth", c => c.Int(nullable: false));
            DropColumn("dbo.Loans", "Fees");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Loans", "Fees", c => c.Decimal(precision: 18, scale: 2));
            DropColumn("dbo.Loans", "DueDayOfMonth");
        }
    }
}

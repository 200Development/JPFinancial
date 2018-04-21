namespace JPFinancial.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addPropertiesToLoanModel : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Loans", "NextDueDate", c => c.DateTime(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Loans", "NextDueDate");
        }
    }
}

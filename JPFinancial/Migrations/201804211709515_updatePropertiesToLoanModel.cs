namespace JPFinancial.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class updatePropertiesToLoanModel : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Loans", "LoanOriginationDate", c => c.DateTime(nullable: false));
            AddColumn("dbo.Loans", "Term", c => c.Int(nullable: false));
            AddColumn("dbo.Loans", "TermClassification", c => c.Int(nullable: false));
            AddColumn("dbo.Loans", "OriginalLoanAmount", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("dbo.Loans", "PrincipalBalance", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("dbo.Loans", "OutstandingBalance", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("dbo.Loans", "AccruedInterest", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("dbo.Loans", "CapitalizedInterest", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("dbo.Loans", "Fees", c => c.Decimal(precision: 18, scale: 2));
            DropColumn("dbo.Loans", "OriginalBalance");
            DropColumn("dbo.Loans", "CurrentBalance");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Loans", "CurrentBalance", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("dbo.Loans", "OriginalBalance", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            DropColumn("dbo.Loans", "Fees");
            DropColumn("dbo.Loans", "CapitalizedInterest");
            DropColumn("dbo.Loans", "AccruedInterest");
            DropColumn("dbo.Loans", "OutstandingBalance");
            DropColumn("dbo.Loans", "PrincipalBalance");
            DropColumn("dbo.Loans", "OriginalLoanAmount");
            DropColumn("dbo.Loans", "TermClassification");
            DropColumn("dbo.Loans", "Term");
            DropColumn("dbo.Loans", "LoanOriginationDate");
        }
    }
}

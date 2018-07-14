namespace JPFinancial.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddCreditCardTable : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.CreditCards",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        Balance = c.Decimal(nullable: false, precision: 18, scale: 2),
                        PrincipalBalance = c.Decimal(nullable: false, precision: 18, scale: 2),
                        CompoundedInterest = c.Decimal(nullable: false, precision: 18, scale: 2),
                        EndOfCycleDay = c.Int(nullable: false),
                        DueDayOfMonth = c.Int(nullable: false),
                        APR = c.Decimal(precision: 18, scale: 2),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.CreditCards");
        }
    }
}

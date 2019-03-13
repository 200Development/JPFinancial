namespace JPFData.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class dropUnusedTables : DbMigration
    {
        public override void Up()
        {
            DropTable("dbo.Benefits");
            DropTable("dbo.Bonus");
            DropTable("dbo.Companies");
            DropTable("dbo.Deductions");
            DropTable("dbo.Earnings");
            DropTable("dbo.Rebalances");
            DropTable("dbo.Taxes");
        }
        
        public override void Down()
        {
            CreateTable(
                "dbo.Taxes",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.Int(nullable: false),
                        Amount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        YTDAmount = c.Decimal(precision: 18, scale: 2),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Rebalances",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Payee = c.String(),
                        Memo = c.String(),
                        Type = c.Int(nullable: false),
                        Category = c.Int(nullable: false),
                        CreditAccountId = c.Int(),
                        DebitAccountId = c.Int(),
                        Date = c.DateTime(nullable: false),
                        Amount = c.Decimal(nullable: false, precision: 18, scale: 2),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Earnings",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Rate = c.Decimal(precision: 18, scale: 2),
                        Hours = c.Double(),
                        Units = c.Double(),
                        Amount = c.Decimal(precision: 18, scale: 2),
                        YTDHours = c.Double(),
                        YTDUnits = c.Double(),
                        YTDAmount = c.Decimal(precision: 18, scale: 2),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Deductions",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.Int(nullable: false),
                        Amount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        YTDAmount = c.Decimal(precision: 18, scale: 2),
                        BeforeTax = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Companies",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false),
                        StreetAddress = c.String(),
                        City = c.String(),
                        StatesEnum = c.Int(nullable: false),
                        Zipcode = c.String(),
                        Phone = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Bonus",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false),
                        Amount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        DatePaid = c.DateTime(nullable: false),
                        SalaryId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Benefits",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false),
                        Amount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        YTDAmount = c.Decimal(precision: 18, scale: 2),
                        Taxable = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
        }
    }
}

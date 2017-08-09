namespace JPFinancial.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddsSalaryBonusBenefitExpenseAndCompanyModels : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Benefits",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        Amount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        SalaryId = c.Int(nullable: false),
                        Bonus_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Bonus", t => t.Bonus_Id)
                .Index(t => t.Bonus_Id);
            
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
                "dbo.Companies",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false),
                        StreetAddress = c.String(),
                        City = c.String(),
                        State = c.Int(nullable: false),
                        Zipcode = c.String(),
                        Phone = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Expenses",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        Amount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        SalaryId = c.Int(nullable: false),
                        IsPreTax = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Salaries",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        PayType = c.Int(nullable: false),
                        PayFrequency = c.Int(nullable: false),
                        GrossPay = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Company_Id = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Companies", t => t.Company_Id, cascadeDelete: true)
                .Index(t => t.Company_Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Salaries", "Company_Id", "dbo.Companies");
            DropForeignKey("dbo.Benefits", "Bonus_Id", "dbo.Bonus");
            DropIndex("dbo.Salaries", new[] { "Company_Id" });
            DropIndex("dbo.Benefits", new[] { "Bonus_Id" });
            DropTable("dbo.Salaries");
            DropTable("dbo.Expenses");
            DropTable("dbo.Companies");
            DropTable("dbo.Bonus");
            DropTable("dbo.Benefits");
        }
    }
}

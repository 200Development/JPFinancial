namespace JPFData.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddModelsForPaycheck : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Benefits", "Bonus_Id", "dbo.Bonus");
            DropIndex("dbo.Benefits", new[] { "Bonus_Id" });
            CreateTable(
                "dbo.Deductions",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.Int(nullable: false),
                        Amount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        YTDAmount = c.Decimal(precision: 18, scale: 2),
                        BeforeTax = c.Boolean(nullable: false),
                        Paycheck_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Paychecks", t => t.Paycheck_Id)
                .Index(t => t.Paycheck_Id);
            
            CreateTable(
                "dbo.Earnings",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Description = c.Int(nullable: false),
                        Rate = c.Decimal(precision: 18, scale: 2),
                        Hours = c.Double(),
                        Units = c.Double(),
                        Amount = c.Decimal(precision: 18, scale: 2),
                        YTDHours = c.Double(),
                        YTDUnits = c.Double(),
                        YTDAmount = c.Decimal(precision: 18, scale: 2),
                        Paycheck_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Paychecks", t => t.Paycheck_Id)
                .Index(t => t.Paycheck_Id);
            
            CreateTable(
                "dbo.Paychecks",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        MaritalStatus = c.Int(nullable: false),
                        Allowances = c.Int(),
                        Employer_Id = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Companies", t => t.Employer_Id, cascadeDelete: true)
                .Index(t => t.Employer_Id);
            
            CreateTable(
                "dbo.Taxes",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.Int(nullable: false),
                        Amount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        YTDAmount = c.Decimal(precision: 18, scale: 2),
                        Paycheck_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Paychecks", t => t.Paycheck_Id)
                .Index(t => t.Paycheck_Id);
            
            AddColumn("dbo.Benefits", "YTDAmount", c => c.Decimal(precision: 18, scale: 2));
            AddColumn("dbo.Benefits", "Taxable", c => c.Boolean(nullable: false));
            AddColumn("dbo.Benefits", "Paycheck_Id", c => c.Int());
            AlterColumn("dbo.Benefits", "Name", c => c.String(nullable: false));
            CreateIndex("dbo.Benefits", "Paycheck_Id");
            AddForeignKey("dbo.Benefits", "Paycheck_Id", "dbo.Paychecks", "Id");
            DropColumn("dbo.Benefits", "SalaryId");
            DropColumn("dbo.Benefits", "Bonus_Id");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Benefits", "Bonus_Id", c => c.Int());
            AddColumn("dbo.Benefits", "SalaryId", c => c.Int(nullable: false));
            DropForeignKey("dbo.Taxes", "Paycheck_Id", "dbo.Paychecks");
            DropForeignKey("dbo.Paychecks", "Employer_Id", "dbo.Companies");
            DropForeignKey("dbo.Earnings", "Paycheck_Id", "dbo.Paychecks");
            DropForeignKey("dbo.Deductions", "Paycheck_Id", "dbo.Paychecks");
            DropForeignKey("dbo.Benefits", "Paycheck_Id", "dbo.Paychecks");
            DropIndex("dbo.Taxes", new[] { "Paycheck_Id" });
            DropIndex("dbo.Paychecks", new[] { "Employer_Id" });
            DropIndex("dbo.Earnings", new[] { "Paycheck_Id" });
            DropIndex("dbo.Deductions", new[] { "Paycheck_Id" });
            DropIndex("dbo.Benefits", new[] { "Paycheck_Id" });
            AlterColumn("dbo.Benefits", "Name", c => c.String());
            DropColumn("dbo.Benefits", "Paycheck_Id");
            DropColumn("dbo.Benefits", "Taxable");
            DropColumn("dbo.Benefits", "YTDAmount");
            DropTable("dbo.Taxes");
            DropTable("dbo.Paychecks");
            DropTable("dbo.Earnings");
            DropTable("dbo.Deductions");
            CreateIndex("dbo.Benefits", "Bonus_Id");
            AddForeignKey("dbo.Benefits", "Bonus_Id", "dbo.Bonus", "Id");
        }
    }
}

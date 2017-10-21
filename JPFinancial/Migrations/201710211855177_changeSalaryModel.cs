namespace JPFinancial.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class changeSalaryModel : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Salaries", "Company_Id", "dbo.Companies");
            DropIndex("dbo.Salaries", new[] { "Company_Id" });
            AddColumn("dbo.Salaries", "Payee", c => c.String(nullable: false));
            DropColumn("dbo.Salaries", "Company_Id");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Salaries", "Company_Id", c => c.Int(nullable: false));
            DropColumn("dbo.Salaries", "Payee");
            CreateIndex("dbo.Salaries", "Company_Id");
            AddForeignKey("dbo.Salaries", "Company_Id", "dbo.Companies", "Id", cascadeDelete: true);
        }
    }
}

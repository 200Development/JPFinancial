namespace JPFData.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class updatesEmployerToStringForPaycheck : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Paychecks", "Employer_Id", "dbo.Companies");
            DropIndex("dbo.Paychecks", new[] { "Employer_Id" });
            AddColumn("dbo.Paychecks", "Employer", c => c.String(nullable: false));
            DropColumn("dbo.Paychecks", "Employer_Id");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Paychecks", "Employer_Id", c => c.Int(nullable: false));
            DropColumn("dbo.Paychecks", "Employer");
            CreateIndex("dbo.Paychecks", "Employer_Id");
            AddForeignKey("dbo.Paychecks", "Employer_Id", "dbo.Companies", "Id", cascadeDelete: true);
        }
    }
}

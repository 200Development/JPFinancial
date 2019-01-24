namespace JPFData.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class removeListsFromPaycheck : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Benefits", "Paycheck_Id", "dbo.Paychecks");
            DropForeignKey("dbo.Deductions", "Paycheck_Id", "dbo.Paychecks");
            DropForeignKey("dbo.Earnings", "Paycheck_Id", "dbo.Paychecks");
            DropForeignKey("dbo.Taxes", "Paycheck_Id", "dbo.Paychecks");
            DropIndex("dbo.Benefits", new[] { "Paycheck_Id" });
            DropIndex("dbo.Deductions", new[] { "Paycheck_Id" });
            DropIndex("dbo.Earnings", new[] { "Paycheck_Id" });
            DropIndex("dbo.Taxes", new[] { "Paycheck_Id" });
            DropColumn("dbo.Benefits", "Paycheck_Id");
            DropColumn("dbo.Deductions", "Paycheck_Id");
            DropColumn("dbo.Earnings", "Paycheck_Id");
            DropColumn("dbo.Taxes", "Paycheck_Id");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Taxes", "Paycheck_Id", c => c.Int());
            AddColumn("dbo.Earnings", "Paycheck_Id", c => c.Int());
            AddColumn("dbo.Deductions", "Paycheck_Id", c => c.Int());
            AddColumn("dbo.Benefits", "Paycheck_Id", c => c.Int());
            CreateIndex("dbo.Taxes", "Paycheck_Id");
            CreateIndex("dbo.Earnings", "Paycheck_Id");
            CreateIndex("dbo.Deductions", "Paycheck_Id");
            CreateIndex("dbo.Benefits", "Paycheck_Id");
            AddForeignKey("dbo.Taxes", "Paycheck_Id", "dbo.Paychecks", "Id");
            AddForeignKey("dbo.Earnings", "Paycheck_Id", "dbo.Paychecks", "Id");
            AddForeignKey("dbo.Deductions", "Paycheck_Id", "dbo.Paychecks", "Id");
            AddForeignKey("dbo.Benefits", "Paycheck_Id", "dbo.Paychecks", "Id");
        }
    }
}

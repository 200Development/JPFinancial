namespace JPFData.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class madePaycheckAllowancesNullable : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Paychecks", "Allowances", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Paychecks", "Allowances", c => c.Int());
        }
    }
}

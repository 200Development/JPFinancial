namespace JPFinancial.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class TransactionClassAdded : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Companies", "StatesEnum", c => c.Int(nullable: false));
            AddColumn("dbo.Salaries", "PayTypesEnum", c => c.Int(nullable: false));
            DropColumn("dbo.Companies", "State");
            DropColumn("dbo.Salaries", "PayType");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Salaries", "PayType", c => c.Int(nullable: false));
            AddColumn("dbo.Companies", "State", c => c.Int(nullable: false));
            DropColumn("dbo.Salaries", "PayTypesEnum");
            DropColumn("dbo.Companies", "StatesEnum");
        }
    }
}

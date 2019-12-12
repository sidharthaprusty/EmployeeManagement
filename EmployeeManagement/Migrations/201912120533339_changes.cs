namespace EmployeeManagement.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class changes : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.AspNetUsers", "IsReadOnly");
        }
        
        public override void Down()
        {
            AddColumn("dbo.AspNetUsers", "IsReadOnly", c => c.Boolean(nullable: false));
        }
    }
}

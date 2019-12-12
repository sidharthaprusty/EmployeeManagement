namespace EmployeeManagement.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class CountryStateAddInRegForm1 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Countries",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        CountryName = c.String(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.States",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        StateName = c.String(nullable: false),
                        Country_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Countries", t => t.Country_Id)
                .Index(t => t.Country_Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.States", "Country_Id", "dbo.Countries");
            DropIndex("dbo.States", new[] { "Country_Id" });
            DropTable("dbo.States");
            DropTable("dbo.Countries");
        }
    }
}

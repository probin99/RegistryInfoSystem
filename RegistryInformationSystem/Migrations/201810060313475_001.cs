namespace RegistryInformationSystem.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _001 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.ComputerSoftwares",
                c => new
                    {
                        SoftwareID = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        Publisher = c.String(),
                        InstalledOn = c.DateTime(nullable: false),
                        Size = c.Int(nullable: false),
                        Version = c.String(),
                        SystemComponent = c.Int(nullable: false),
                        InstalledLocation = c.String(),
                        Status = c.String(),
                    })
                .PrimaryKey(t => t.SoftwareID);
            
            CreateTable(
                "dbo.ComputerSystems",
                c => new
                    {
                        ComputerSystemId = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        Type = c.String(),
                        IsLocal = c.Boolean(nullable: false),
                        IsArray = c.Boolean(nullable: false),
                        Origin = c.String(),
                        Qualifiers = c.String(),
                    })
                .PrimaryKey(t => t.ComputerSystemId);
            
            CreateTable(
                "dbo.Logins",
                c => new
                    {
                        UserId = c.Int(nullable: false, identity: true),
                        LoginEmail = c.String(nullable: false),
                        Password = c.String(nullable: false),
                    })
                .PrimaryKey(t => t.UserId);
            
            CreateTable(
                "dbo.Registers",
                c => new
                    {
                        RegisterId = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false),
                        Gender = c.String(nullable: false),
                        RegisterEmail = c.String(nullable: false),
                        Password = c.String(nullable: false),
                    })
                .PrimaryKey(t => t.RegisterId);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.Registers");
            DropTable("dbo.Logins");
            DropTable("dbo.ComputerSystems");
            DropTable("dbo.ComputerSoftwares");
        }
    }
}

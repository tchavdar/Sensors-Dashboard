namespace MQTT_WPF_Client.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Initial : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.AggregatedSensors",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Location = c.String(),
                        PublicName = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.SensorDatas",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ReceivedDt = c.DateTime(nullable: false),
                        Value = c.String(),
                        Sensor_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Sensors", t => t.Sensor_Id)
                .Index(t => t.Sensor_Id);
            
            CreateTable(
                "dbo.Sensors",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Type = c.String(),
                        LastValue = c.String(),
                        Unit = c.String(),
                        LastUpdated = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.SensorDatas", "Sensor_Id", "dbo.Sensors");
            DropIndex("dbo.SensorDatas", new[] { "Sensor_Id" });
            DropTable("dbo.Sensors");
            DropTable("dbo.SensorDatas");
            DropTable("dbo.AggregatedSensors");
        }
    }
}

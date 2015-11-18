using System.Data.Entity;
using MQTT_WPF_Client.Data;

namespace MQTT_WPF_Client.Model
{
    class DBDataModel:DbContext
    {
        public DbSet<Sensor> Sensors { get; set; } 
        public DbSet<SensorData> SensorDatas { get; set; }
        public DbSet<AggregatedSensors> ASensors { get; set; } 
    }
}

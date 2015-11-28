using System;
using MQTT_WPF_Client.Data;
using Syncfusion.Windows.Chart;

namespace MQTT_WPF_Client.Design
{
    public class DesignTimeASensor:AggregatedSensors
    {
        //public AggregatedSensors AS { get; set; }

        
        public DesignTimeASensor():base("designLocation","Design Sensor")
        {
            //this.AS = new AggregatedSensors("Designer","Design Sensor");
            
            var temperature = new Sensor("temperature", "F");
            var humidity= new Sensor("humidity", "%");
            Sensors.Add("temperature", temperature);
            Sensors.Add("humidity",humidity);
            humidity.LastValue = "90";
            temperature.LastValue = "90";

            temperature.SensorDatas.Add(new SensorData(DateTime.Now, "10",2000));
            humidity.SensorDatas.Add(new SensorData(DateTime.Now, "80", 2000));

        }

    }
}

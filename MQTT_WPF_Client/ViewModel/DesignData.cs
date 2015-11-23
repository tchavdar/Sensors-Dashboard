using System;
using MQTT_WPF_Client.Data;

namespace MQTT_WPF_Client.ViewModel
{
    public class DesignTimeASensor
    {
        public AggregatedSensors AggregatedSensor { get; set; }
        public DesignTimeASensor()
        {
            var asensor=new AggregatedSensors("Designer","Design Sensor");
            
            
            var temperature = new Sensor("temperature", "C");
            var humidity= new Sensor("humidity", "%");
            asensor.Sensors.Add("temperature", temperature);
            asensor.Sensors.Add("humidity",humidity);

            temperature.SensorDatas.Add(new SensorData(DateTime.Now, "10",2000));
            humidity.SensorDatas.Add(new SensorData(DateTime.Now, "80", 2000));

        }

    }
}

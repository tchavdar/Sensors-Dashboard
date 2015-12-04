using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MQTT_WPF_Client
{
    public class SensorData
    {
        public DateTime ReceivedDt { get; set; }
        public double Value { get; set; }
        public int Id { get; set; }

        public int Voltage { get; set; }


        public SensorData(DateTime receivedDt, double value, int voltage)
        {
            ReceivedDt = receivedDt;
            Value = value;
            Voltage = voltage;
        }
    }
}

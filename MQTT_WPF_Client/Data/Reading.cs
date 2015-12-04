using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MQTT_WPF_Client.Data
{
    public class Reading
    {
        public string Location { get; set; }
        public double Value { get; set; }
        public string SensorType { get; set; }
        public string PublicName { get; set; }
    }
}

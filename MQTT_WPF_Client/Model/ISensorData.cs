using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BrightstarDB.EntityFramework;

namespace MQTT_WPF_Client.Model
{
    [Entity]
    public interface ISensorData
    {
        DateTime ReceivedDt { get; set; }

        string Value { get; set; }

        ISensor ParentSensor { get; set; }
    }
}

using System;
using System.Collections.Generic;
using BrightstarDB.EntityFramework;

namespace MQTT_WPF_Client.Model
{
    [Entity]
    public interface ISensor
    {
        string Id { get;}
        string SensorType { get; set; }

        string Unit { get; set; }

        string LastValue { get; set; }

        DateTime LastUpdated { get; set; }

        IMultiSensor ParentMultiSensor { get; set; }

        [InverseProperty("ParentSensor")]
        ICollection<ISensorData> SensorDatas { get; set; }
    }
}

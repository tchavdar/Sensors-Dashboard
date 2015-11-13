using System.Collections.Generic;
using BrightstarDB.EntityFramework;

namespace MQTT_WPF_Client.Model
{
    [Entity]
    public interface IMultiSensor
    {
        string Id { get; }

        string Location { get; set; }

        string PublicName { get; set; }

        [InverseProperty("ParentMultiSensor")]
        ICollection<ISensor> Sensors { get; set; }

    }
}

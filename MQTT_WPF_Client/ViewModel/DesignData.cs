using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MQTT_WPF_Client.Data;

namespace MQTT_WPF_Client.ViewModel
{
    public class DesignTimeASensor
    {
        public SortedList<string, Sensor> Sensors { get; set; }
        public string PublicName { get; set; }
        public string Location { get; set; }

        public DesignTimeASensor()
        {
            PublicName = "Design sensor";
            Location = "Designer";
            Sensors = new SortedList<string, Sensor>();
            Sensor sensor = new Sensor("temperature", "C")
            {
                UpdateData = new ObservableCollection<UpdateSensorPeriod>
                {
                    new UpdateSensorPeriod {PercentToUpdate = 80, SomeText = "80pct"},
                    new UpdateSensorPeriod {PercentToUpdate = 20, SomeText = "80pct"}
                }
            };
            Sensors.Add("temperature", sensor);
        }

    }
}

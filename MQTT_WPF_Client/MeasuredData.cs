using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MQTT_WPF_Client.Data;

namespace MQTT_WPF_Client
{
    public class MeasuredData
    {
        public ObservableCollection<Reading> MeasuredTemperatures { get; set; }

        public ObservableCollection<Reading> MeasuredHumidities { get; set; }

        public void NewReading(object sender, Reading args)
        {
            if (args.SensorType == "temperature")
            {
                UpdateMeasuredData(args, MeasuredTemperatures);
                return;
            }

            if (args.SensorType == "humidity")
            {
                UpdateMeasuredData(args, MeasuredHumidities);
            }
        }


        private void UpdateMeasuredData(Reading args, ObservableCollection<Reading> measuredCollection )
        {
            var reading =
                measuredCollection.Select(x => x).FirstOrDefault(x => x.PublicName == args.PublicName);
            if (reading != null)
            {
                reading.Value = args.Value;
                Debug.WriteLine($"Updated {args.PublicName} to {args.Value}");
            }
            else
            {
                measuredCollection.Add(args);
            }
        }

        public MeasuredData()
        {
            MeasuredTemperatures = new ObservableCollection<Reading>();
            MeasuredHumidities = new ObservableCollection<Reading>();
        }
    }
}
  


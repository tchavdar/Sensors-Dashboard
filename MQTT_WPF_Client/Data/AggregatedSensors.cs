using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Runtime.CompilerServices;
using Humanizer;
using MQTT_WPF_Client.Annotations;
using MQTT_WPF_Client.Extensions;
using MQTT_WPF_Client.MQTT;

namespace MQTT_WPF_Client.Data
{




 

    public class AggregatedSensors:INotifyPropertyChanged
    {
        public int Id { get; set; }
        public string Location { get; set; }

        public string PublicName { get; set; }

        public SortedList<String,Sensor> Sensors { get; set; }

        public Sensor Temperature { get; set; }

        public AggregatedSensors(string location, string publicName)
        {
            Sensors = new SortedList<string, Sensor>();
            Location = location;
            PublicName = publicName;
        }
        
        

        public void MqttReceivedData(object sender, MQQTDataReceivedEventArgs e)
        {
            if ((NewReading != null) && (e.newData.Location == Location))
            {
                NewReading(this,
                    new Reading
                    {
                        Location = e.newData.Location,
                        PublicName = this.PublicName,
                        SensorType = e.newData.SensorType,
                        Value = e.newData.Value
                    });

                foreach (var sensor in Sensors.Values)
                {
                    if (e.newData.SensorType == sensor.Type)
                    {
                        sensor.MqttReceivedData(sender, e);
                    }
                }
            }


        }

        public void AddSensor(string sensorType, string sensorUnit)
        {
            Sensor sensor = new Sensor(sensorType, sensorUnit, this);

            Sensors.Add(sensorType,sensor);
            if (sensorType=="temperature")
            {
                Temperature = sensor;
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public event OnNewReading NewReading;


        protected virtual void OnNewReading(Reading args)
        {
            NewReading?.Invoke(this, args);
        }
    }
}

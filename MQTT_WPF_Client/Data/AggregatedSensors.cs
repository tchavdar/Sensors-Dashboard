using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Timers;
using System.Windows;
using System.Windows.Media.Animation;
using MQTT_WPF_Client.Annotations;
using MQTT_WPF_Client.Extensions;
using MQTT_WPF_Client.MQTT;
using Syncfusion.Windows.Chart;

namespace MQTT_WPF_Client.Data
{

    public class SensorData
    {
        public DateTime ReceivedDt { get; set; }
        public string Value { get; set; }
        public int Id { get; set; }

        public int Voltage { get; set; }

        
        public SensorData(DateTime receivedDt, string value, int voltage)
        {
            ReceivedDt = receivedDt;
            Value = value;
            Voltage = voltage;
        }

    }





    public class Sensor:INotifyPropertyChanged
    {
        private int _maxElements;
        public int Id { get; set; }
        private string _lastValue;
        private DateTime _lastUpdated;
        private TimeSpan _updatePeriod;


        private double _percentToUpdate;

        public double PercentToUpdate
        {
            get { return _percentToUpdate; }
            set
            {
                _percentToUpdate = value;
                OnPropertyChanged(nameof(PercentToUpdate));
            }
        }


        public string Type { get; set; }
        
        public ObservableCollection<SensorData> SensorDatas { get; set; }

        

        public string LastValue
        {
            get { return _lastValue; }
            set
            {
                _lastValue = value;
                OnPropertyChanged(nameof(LastValue));
            }
        }

        public string Unit { get; set; }


        public DateTime LastUpdated
        {
            get { return _lastUpdated; }
            set
            {
                _lastUpdated = value;
                OnPropertyChanged(nameof(LastUpdated));
            }
        }



        public TimeSpan UpdatePeriod
        {
            get { return _updatePeriod; }
            set
            {
                _updatePeriod = value;
                OnPropertyChanged(nameof(UpdatePeriod));
                OnPropertyChanged(nameof(Duration));
            }
        }

        public string Duration
        {
            get
            {
                return UpdatePeriod.ToAnimatedDuration();
            }
        }

        public Sensor(string sensorType, string sensorUnit)
        {
            Type = sensorType;
            Unit = sensorUnit;
            LastValue = "N/A";
            LastUpdated=DateTime.Now;
 
            SensorDatas = new ObservableCollection<SensorData>();
            PercentToUpdate = 100;
            _maxElements = 100;
            UpdatePeriod = TimeSpan.FromSeconds(10);

        }

        public void MqttReceivedData(object sender, MQQTDataReceivedEventArgs e)
        {
            if (e.newData.SensorType==Type)
            {
                NewDataReceived(e.newData);
                Update(e.newData);
            }
           
        }

        private void Update(MqttDataFormat newData)
        {
            
            UpdatePeriod = (DateTime.Now - LastUpdated);

            PercentToUpdate = 0;
            LastUpdated = newData.ReceivedDt;
            LastValue = newData.Value.ToString(CultureInfo.InvariantCulture);
            Debug.WriteLine($"Period:{UpdatePeriod.ToAnimatedDuration()}");

        }

        private void NewDataReceived(MqttDataFormat newData)
        {

            if ((SensorDatas.Count>0)&&((DateTime.Now-SensorDatas[0].ReceivedDt).Hours > 1))
            {
                SensorDatas.RemoveAt(0);
            }
            SensorDatas.Add(new SensorData(newData.ReceivedDt, newData.Value.ToString(CultureInfo.InvariantCulture),newData.Voltage));
            OnPropertyChanged(nameof(SensorDatas));

        }


        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        

    }

    public class AggregatedSensors
    {
        public int Id { get; set; }
        public string Location { get; set; }

        public string PublicName { get; set; }

        public SortedList<String,Sensor> Sensors { get; set; }


        public AggregatedSensors(string location, string publicName)
        {
            Sensors = new SortedList<string, Sensor>();
            Location = location;
            PublicName = publicName;
        }



        public void MqttReceivedData(object sender, MQQTDataReceivedEventArgs e)
        {
            if (e.newData.Location.Contains(Location))
            {
                foreach (var sensor in Sensors.Values)
                {
                    sensor.MqttReceivedData(sender,e);
                }
            }
        }

        public void AddSensor(string sensorType, string sensorUnit)
        {
            Sensors.Add(sensorType,new Sensor(sensorType,sensorUnit));
        }
    }
}

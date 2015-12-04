using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using MQTT_WPF_Client.Annotations;
using MQTT_WPF_Client.Data;
using MQTT_WPF_Client.Extensions;
using MQTT_WPF_Client.MQTT;

namespace MQTT_WPF_Client
{
    public class Sensor : INotifyPropertyChanged
    {
        private int _maxElements;
        public int Id { get; set; }
        private double _lastValue;
        private DateTime _lastUpdated;
        private TimeSpan _updatePeriod;

        private AggregatedSensors _parentAS;

        public string Type { get; set; }

        public ObservableCollection<SensorData> SensorDatas { get; set; }

        public double LastValue
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

        public string Duration => UpdatePeriod.ToAnimatedDuration();

        private bool _offline;
        public bool Offline
        {
            get { return _offline; }
            set
            {
                _offline = value;
                OnPropertyChanged(nameof(Offline));
            }
        }

        public Sensor(string sensorType, string sensorUnit, AggregatedSensors parentAs)
        {
            Type = sensorType;
            Unit = sensorUnit;
            _parentAS = parentAs;
            LastValue = -100;
            LastUpdated = DateTime.Now;
            Offline = false;

            SensorDatas = new ObservableCollection<SensorData>();
            _maxElements = 100;
            UpdatePeriod = TimeSpan.FromSeconds(10);

        }

        public void MqttReceivedData(object sender, MQQTDataReceivedEventArgs e)
        {
            //the offline info arrives at Location as for now for a location we may have several sensors
            //we put them all offline no matter the type
            if (e.newData.Offline)
            {
                ReceivedOffline(e.newData);
                return;
            }

            if (e.newData.SensorType == Type)
            {
                NewDataReceived(e.newData);
                Update(e.newData);
            }
        }

        private void ReceivedOffline(MqttDataFormat newData)
        {
            Offline = true;
        }

        private void Update(MqttDataFormat newData)
        {

            UpdatePeriod = (DateTime.Now - LastUpdated);
            Offline = false;
            LastUpdated = newData.ReceivedDt;
            LastValue = Convert.ToDouble(newData.Value.ToString(CultureInfo.InvariantCulture));
            Debug.WriteLine($"Period:{UpdatePeriod.ToAnimatedDuration()}");

        }

        private void NewDataReceived(MqttDataFormat newData)
        {
            //if ((SensorDatas.Count>0)&&((DateTime.Now-SensorDatas[0].ReceivedDt).Hours > 1))
            //{
            //    SensorDatas.RemoveAt(0);
            //}
            SensorDatas.Add(new SensorData(newData.ReceivedDt, Convert.ToDouble(newData.Value.ToString(CultureInfo.InvariantCulture)), newData.Voltage));
            if (NewReading != null)
                NewReading(this,
                    new Reading
                    {
                        Location = newData.FullLocation,
                        Value = Convert.ToDouble(newData.Value.ToString(CultureInfo.InvariantCulture)),
                        SensorType = newData.SensorType
                    });
        }


        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public event OnNewReading NewReading;

    }

    public delegate void OnNewReading(object sender, Reading args);


}

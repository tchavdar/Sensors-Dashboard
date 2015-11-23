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
using MQTT_WPF_Client.Annotations;
using MQTT_WPF_Client.MQTT;

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

        private Timer _timer;

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
            }
        }

 
        public Sensor(string sensorType, string sensorUnit)
        {
            Type = sensorType;
            Unit = sensorUnit;
            LastValue = "N/A";
 
            SensorDatas = new ObservableCollection<SensorData>();
            PercentToUpdate = 10;
            _maxElements = 100;
            _timer=new Timer(500);
            _timer.Enabled = false;
            _timer.Elapsed += _timer_Elapsed;
            _timer.AutoReset = true;

        }

        private void _timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            PercentToUpdate = 100-(double)(UpdatePeriod.Seconds - (DateTime.Now - LastUpdated).Seconds)/ (UpdatePeriod.Seconds / 100f);
       //     Debug.WriteLine($"FIRED {PercentToUpdate}");
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
            UpdatePeriod= DateTime.Now-LastUpdated;
            LastUpdated = newData.ReceivedDt;
            LastValue = newData.Value.ToString(CultureInfo.InvariantCulture);
            Debug.WriteLine($"Period:{UpdatePeriod}");
            _timer.Enabled = true;

        }

        private void NewDataReceived(MqttDataFormat newData)
        {

            if ((SensorDatas.Count>0)&&((DateTime.Now-SensorDatas[0].ReceivedDt).Hours > 1))
            {
                SensorDatas.RemoveAt(0);
            }
            SensorDatas.Add(new SensorData(newData.ReceivedDt, newData.Value.ToString(CultureInfo.InvariantCulture),newData.Voltage));
            OnPropertyChanged(nameof(SensorDatas));

            //if (LastValue!=newData.Value.ToString(CultureInfo.InvariantCulture)||((LastUpdated-newData.ReceivedDt).Minutes>15)||(SensorDatas.Count<15))
            //{
            //    if (SensorDatas.Count>_maxElements)
            //    {
            //        SensorDatas.RemoveAt(0);
            //    }
            //    SensorDatas.Add(new SensorData(newData.ReceivedDt, newData.Value.ToString(CultureInfo.InvariantCulture)));
            //    Debug.WriteLine($"Sensor:{Type}:{Unit} received:{newData.Value.ToString(CultureInfo.InvariantCulture)}");
            //    OnPropertyChanged(nameof(SensorDatas));
            //}

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

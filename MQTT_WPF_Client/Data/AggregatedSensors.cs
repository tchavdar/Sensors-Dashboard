using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using MQTT_WPF_Client.Annotations;
using MQTT_WPF_Client.Model;
using MQTT_WPF_Client.MQTT;

namespace MQTT_WPF_Client.Data
{

    public class SensorData
    {
        public DateTime ReceivedDt { get; set; }
        public string Value { get; set; }

        public SensorData(DateTime receivedDt, string value)
        {
            ReceivedDt = receivedDt;
            Value = value;
        }
    }

    public class Sensor:INotifyPropertyChanged
    {
        private string _lastValue;
        private DateTime _lastUpdated;
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

        public Sensor(string sensorType, string sensorUnit)
        {
            Type = sensorType;
            Unit = sensorUnit;
            LastValue = "N/A";
            SensorDatas = new ObservableCollection<SensorData>();
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
            
            LastUpdated = newData.ReceivedDt;
            LastValue = newData.Value.ToString(CultureInfo.InvariantCulture);

        }

        private void NewDataReceived(MqttDataFormat newData)
        {
            if (LastValue!=newData.Value.ToString(CultureInfo.InvariantCulture)||((LastUpdated-newData.ReceivedDt).Minutes>15)||(SensorDatas.Count<15))
            {
                SensorDatas.Add(new SensorData(newData.ReceivedDt, newData.Value.ToString(CultureInfo.InvariantCulture)));
                OnPropertyChanged(nameof(SensorDatas));
            }

        }


        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public ISensor ToISensor()
        {
            ISensor sensor = new Model.Sensor();
            sensor.Unit = Unit;
            sensor.SensorType = Type;
            sensor.LastValue = LastValue;
            sensor.LastUpdated = LastUpdated;

            return sensor;
        }

        public static Sensor CreateFromISensor(ISensor msensor)
        {
            var sensor = new Sensor(msensor.SensorType, msensor.Unit)
            {
                LastUpdated = msensor.LastUpdated,
                LastValue = msensor.LastValue
            };
            return sensor;
        }
    }

    public class AggregatedSensors
    {
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

        public IMultiSensor ToIMultiSensor()
        {
            IMultiSensor multiSensor=new MultiSensor();
            multiSensor.Location = Location;
            multiSensor.PublicName = PublicName;
            multiSensor.Sensors = new List<ISensor>();
            foreach (var sensor in Sensors.Values)
            {
                multiSensor.Sensors.Add(sensor.ToISensor());
            }
            
            return multiSensor;
        }

        public static AggregatedSensors CreateFromIMultiSensor(IMultiSensor multiSensor)
        {
            AggregatedSensors aggregatedSensors = new AggregatedSensors(multiSensor.Location,multiSensor.PublicName);
            
            foreach (var msensor in multiSensor.Sensors)
            {
                aggregatedSensors.Sensors.Add(msensor.Unit,Sensor.CreateFromISensor(msensor));
            }

            return aggregatedSensors;
        }
    }
}

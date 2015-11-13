using System.Windows;
using System.Windows.Threading;
using MQTT_WPF_Client.Data;
using MQTT_WPF_Client.MQTT;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using MahApps.Metro.Controls;
using MQTT_WPF_Client.Annotations;

using Syncfusion.UI.Xaml.Charts;


namespace MQTT_WPF_Client.ViewModel
{
    public class OverviewViewModel:DependencyObject, INotifyPropertyChanged
    {
        
        public MQTTCollection MqttReceivedData { get; set; }


     
        public AggregatedSensors As1
        {
            get { return (AggregatedSensors)GetValue(As1Property); }
            set { SetValue(As1Property, value); }
        }



        public AggregatedSensors As2 
        {
            get { return (AggregatedSensors)GetValue(As2Property); }
            set { SetValue(As2Property, value); }
        }



        public ICommand Settings { get; set; }

        // Using a DependencyProperty as the backing store for As2.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty As2Property =
            DependencyProperty.Register("As2", typeof(AggregatedSensors), typeof(Window), new PropertyMetadata(null));



        // Using a DependencyProperty as the backing store for As1.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty As1Property =
            DependencyProperty.Register("As1", typeof(AggregatedSensors), typeof(Window), new PropertyMetadata(null));




        public DateTimeIntervalType ChartIntervalType
        {
            get { return (DateTimeIntervalType)GetValue(ChartIntervalTypeProperty); }
            set { SetValue(ChartIntervalTypeProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ChartIntervalType.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ChartIntervalTypeProperty =
            DependencyProperty.Register("ChartIntervalType", typeof(DateTimeIntervalType), typeof(Window), new PropertyMetadata(null));




        public bool IsSettingsFlyoutOpen 
        {
            get { return (bool)GetValue(IsSettingsFlyoutOpenProperty); }
            set { SetValue(IsSettingsFlyoutOpenProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsSettingsFlyoutOpen.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsSettingsFlyoutOpenProperty =
            DependencyProperty.Register("IsSettingsFlyoutOpen", typeof(bool), typeof(MetroWindow), new PropertyMetadata(null));





 public MqttDataLayer MqttDl { get; set; }

        public OverviewViewModel(Dispatcher dispatcher)
        {

            MqttReceivedData = new MQTTCollection(this);

            ChartIntervalType=DateTimeIntervalType.Auto;
            //Creating the data layer 
            MqttDl = new MqttDataLayer("WPF Client", "192.168.2.122");
            MqttDl.OnConnected += MqttDL_OnConnected;
            MqttDl.Connect();
            
            As1 = new AggregatedSensors("cabinet", "Cabinet");
            As1.AddSensor("temperature", "C");
            As1.AddSensor("humidity", "%");
            As2 = new AggregatedSensors("cave", "Cave");
            As2.AddSensor("temperature", "C");
            As2.AddSensor("humidity", "%");

            As3 = new AggregatedSensors("outside", "Outside");
            As3.AddSensor("temperature", "C");
            As3.AddSensor("humidity", "%");
            //note that the sensors listen to the collection events not the data layer events
            //the collection takes care to parse the received data so that the agregated sensors can use it directly
            MqttReceivedData.DataReceived += As1.MqttReceivedData;
            MqttReceivedData.DataReceived += As2.MqttReceivedData;
            MqttReceivedData.DataReceived += As3.MqttReceivedData;


            Settings= new SettingsCommand(this);



 
        }

        private void MqttDL_OnConnected(object sender, MqttConnectedArgs args)
        {
            if (MqttDl.Connected)
            {
                MqttDl.SubscribeToTopic("myhome/test/#");
                //subscribe the collection of received data to the receive data event
                //every PublishReceived event will be routed to the collection
                //everything received by the data layer will be processed by the collection
                MqttDl.Client.MqttMsgPublishReceived += MqttReceivedData.MqttMsgPublishReceived;
            }
        }


        public AggregatedSensors As3 { get; set; }


        public void CloseMqtt()
        {
            MqttDl.Disconnect();
        }


        public void SettingsDialogOpen()
        {

            IsSettingsFlyoutOpen = true;
            OnPropertyChanged(nameof(IsSettingsFlyoutOpen));

        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

}

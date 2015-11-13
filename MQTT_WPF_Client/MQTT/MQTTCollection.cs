using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows.Threading;
using MQTT_WPF_Client.ViewModel;
using uPLibrary.Networking.M2Mqtt.Messages;

namespace MQTT_WPF_Client.MQTT
{
    public class MQQTDataReceivedEventArgs : EventArgs
    {
        public readonly MqttDataFormat newData;

        public MQQTDataReceivedEventArgs(MqttDataFormat data)
        {
            newData = data;
        }
    }
    public class MQTTCollection : ObservableCollection<MqttDataFormat>
    {

        private OverviewViewModel _vm;




        public Timer Timer { get; set; }

        public void MqttMsgPublishReceived(object sender, MqttMsgPublishEventArgs e)
        {

            Console.WriteLine("Received = " + Encoding.UTF8.GetString(e.Message) + " on topic " + e.Topic);

            this._vm.Dispatcher.BeginInvoke(
                new Action(() =>
                {
                    MqttDataFormat newData = new MqttDataFormat(e.Topic, Encoding.UTF8.GetString(e.Message));
                    if (newData.ParsableMessage)
                    {
                        this.Add(newData);
                        onDataReceived(new MQQTDataReceivedEventArgs(newData));
                    }
                }));
        }

        public MQTTCollection(OverviewViewModel viewModel)
        {
            _vm = viewModel;
            this.Timer = new Timer(1000);
            Timer.Enabled = false;
            Timer.Elapsed += TimerElapsed;

        }

        private void FakeData(string Topic, string Value)
        {
            this._vm.Dispatcher.BeginInvoke(
                new Action(() =>
                {
                    MqttDataFormat newData = new MqttDataFormat(Topic, $"{Value} id:0000 SID:FAKE_1");
                    if (newData.ParsableMessage)
                    {
                        this.Add(newData);
                        onDataReceived(new MQQTDataReceivedEventArgs(newData));
                    }
                }));

        }

        private void TimerElapsed(object sender, ElapsedEventArgs e)
        {
            Console.WriteLine("Timer Ran");

            FakeData("myhome/cave/temperature","12");
            FakeData("myhome/cabinet/temperature", "20");
            FakeData("myhome/outside/temperature", "15");

        }

        public event EventHandler<MQQTDataReceivedEventArgs> DataReceived;



        protected virtual void onDataReceived(MQQTDataReceivedEventArgs data)
        {
            if (DataReceived != null) DataReceived(this, data);
        }

    }
}

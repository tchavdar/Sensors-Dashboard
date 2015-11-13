using System;
using System.Diagnostics;
using System.Text;
using System.Threading;
using Polly;
using uPLibrary.Networking.M2Mqtt;
using uPLibrary.Networking.M2Mqtt.Messages;

namespace MQTT_WPF_Client.MQTT
{
    public class MqttDataLayer
    {

        /// <summary>
        /// Contains connection status
        /// </summary>
        public bool Connected { get; private set; }
        /// <summary>
        /// Client identifier, can be any string
        /// </summary>
        private string ClientId { get; }


        private string Broker { get; }


        public MqttClient Client { get; private set; }

        /// <summary>
        /// Connect to a broker
        /// </summary>
        public byte Connect()
        {
            //Return Code Return Code Response CONNACK
            //0   Connection Accepted
            //1   Connection Refused, unacceptable protocol version
            //2   Connection Refused, identifier rejected
            //3   Connection Refused, Server unavailable
            //4   Connection Refused, bad user name or password
            //5   Connection Refused, not authorized
            byte connAck = 255;


            var policy = Policy
                .Handle<uPLibrary.Networking.M2Mqtt.Exceptions.MqttConnectionException>()
                .Or<System.Net.Sockets.SocketException>()
                .RetryForever((exception, context) =>
                {
                    Debug.WriteLine("Retrying to connect to broker");
                    Thread.Sleep(5000);
                });

            policy.Execute(() =>
            {
                Client = new MqttClient(Broker);
                connAck = Client.Connect(ClientId);
            });


            if (connAck == 0)
            {
                Connected = true;
                Client.ConnectionClosed += Client_ConnectionClosed;
                Debug.WriteLine("Connection succesfull");
                OnConnected?.Invoke(this, new MqttConnectedArgs());
            }
            else
            {
                Connected = false;
                Debug.WriteLine($"Connection failed with code {connAck}");
            }

            return connAck;
        }

        private void Client_ConnectionClosed(object sender, EventArgs e)
        {
            Debug.WriteLine("Connection to the broker closed");
            Connected = false;
            Connect();
        }


        /// <summary>
        /// Subscribe to a specific topic
        /// </summary>
        /// <param name="topic"> the topic to subscribe to ex:home/automation/room1/temperature</param>
        public void SubscribeToTopic(string topic)
        {
            if (!Connected)
            {
                return;
            }
            Client.Subscribe(new[] { topic }, new[] { MqttMsgBase.QOS_LEVEL_AT_LEAST_ONCE });

            //subsribing for log purposes
            Client.MqttMsgSubscribed += client_MqttMsgSubscribed;
            Client.MqttMsgPublishReceived += client_MqttMsgPublishReceived;

        }

        
        void client_MqttMsgPublishReceived(object sender, MqttMsgPublishEventArgs e)
        {
            Console.WriteLine(@"Received = " + Encoding.UTF8.GetString(e.Message) + @" on topic " + e.Topic);
        }

        void client_MqttMsgSubscribed(object sender, MqttMsgSubscribedEventArgs e)
        {
            Console.WriteLine(@"Subscribed for id = " + e.MessageId);
        }
        void client_MqttMsgPublished(object sender, MqttMsgPublishedEventArgs e)
        {
            Console.WriteLine(@"MessageId = " + e.MessageId + @" Published = " + e.IsPublished);
        }

        public void Disconnect()
        {
            if (Connected)
            {
                Client.ConnectionClosed -= Client_ConnectionClosed;
                Client.Disconnect();
            }
            
        }

        public MqttDataLayer(string clientId, string broker)
        {
            ClientId = clientId;
            Broker = broker;
        }

        public event MqttConnected OnConnected;

        

    }

    public delegate void MqttConnected(object sender, MqttConnectedArgs args);

    public class MqttConnectedArgs
    {

    }
}

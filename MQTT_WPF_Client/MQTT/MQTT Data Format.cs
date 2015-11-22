﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using Humanizer;
using MQTT_WPF_Client.Annotations;

namespace MQTT_WPF_Client.MQTT
{
    public class MqttDataFormat
    {
        private string _sensorId;

        /// <summary>
        /// LastValue as parsed data from the raw data.
        ///  </summary>
        public decimal Value { get; set; }


        /// <summary>
        /// Sequence ID as parsed from the raw data. Note that the sequence restarts after sensor restart/reset so it is usefull only during a single session.
        /// </summary>
        public int SeqId { get; set; }

        /// <summary>
        /// Sensor type is inferred from the last leaf of the rawPath.
        /// </summary>
        public string  SensorType { get; set; }

        /// <summary>
        /// The time when the message has been received
        /// </summary>
        public DateTime ReceivedDt { get; set; }

        public bool ParsableMessage { get; set; }

        public string SensorId
        {
            get { return _sensorId; }
            set { _sensorId = value; }
        }

        public int Voltage { get; set; }
        public string ReceivedDtHumanized => ReceivedDt.Humanize(false);

     /// <summary>
        /// The location is inferred from the rawPath. It is the all the data before the last leaf home/floor1/cabinet/temperature. The part before temperature is the location.
        /// </summary>
     public string Location { get; private set; }

        
        public MqttDataFormat(string rawPath, string rawData)
        {
            ParsableMessage = true;
            GetDataFromMessage(rawData);
            GetDataFromPath(rawPath);
            ReceivedDt=DateTime.Now;
        }

     public int GetSeqFromMessage(string rawData)
     {
         try
         {
                int i1 = rawData.IndexOf("id:", StringComparison.Ordinal) + 3;
               // int i2 = rawData.IndexOf(" ", i1);
                int res = Convert.ToInt32(rawData.Substring(i1));
                return res;
            }
         catch (Exception)
         {
             Console.WriteLine("Exception while reading sequence ID from raw data:{0}",rawData);
             return -1;
         }
     }

        public void  GetDataFromMessage(string rawData)
        {
            try
            {
                string[] data=rawData.Split(' ');
                //Value = Convert.ToDecimal(data[0]);
                //SeqId = Convert.ToInt32(data[1].Substring(3));
                //SensorId = data[2].Substring(4);
                Dictionary<string, string> dict = new Dictionary<string, string>();
                foreach (var s in data)
                {
                    string[] pair = s.Split(':');
                    if (pair.Length > 1)
                    {
                        dict.Add(pair[0], pair[1]);
                    }
                    else
                    {
                        dict.Add("v",pair[0]);
                    }
                }
                Value = Convert.ToDecimal(dict["v"]);
                SeqId = Convert.ToInt32(dict["id"]);
                SensorId = dict["SID"];
                if (dict.ContainsKey("SV"))
                {
                    Voltage = Convert.ToInt32(dict["SV"]);
                }
                

            }
            catch (Exception)
            {
                Console.WriteLine("Exception while converting rawdata:{0}", rawData);
                Value = 0xffff;
                SeqId = -1;
                ParsableMessage = false;
            }
        }

     public void GetDataFromPath(string rawPath)
     {
         try
         {
             string[] data = rawPath.Split('/');
             SensorType = data[data.Length - 1];
             Location = rawPath.Substring(0, rawPath.Length - SensorType.Length-1);

         }
         catch (Exception)
         {
             Console.WriteLine("Exception while parsing rawPath. Format is expected to be root/leaf/leaf/sensor_type. Received:{0}", rawPath);
             SensorType = "UNKNOWN";
             Location = "UNKNOWN";
             ParsableMessage = false;
         }
     }

    }
}

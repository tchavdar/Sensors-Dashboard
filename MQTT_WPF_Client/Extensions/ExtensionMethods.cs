using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MQTT_WPF_Client.Extensions
{
    public static class ExtensionMethods
    {
        public static string ToAnimatedDuration(this TimeSpan span)
        {
            if (span.Milliseconds>500)
            {
                span.Add(TimeSpan.FromSeconds(1));
            }
            return $"{span.Hours}:{span.Minutes}:{span.Seconds}";
        }
    }
}

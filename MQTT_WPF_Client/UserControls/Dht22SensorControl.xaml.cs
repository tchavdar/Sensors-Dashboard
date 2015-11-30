using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using Humanizer;

namespace MQTT_WPF_Client.UserControls
{
    /// <summary>
    /// Interaction logic for Dht22SensorControl.xaml
    /// </summary>
    public partial class Dht22SensorControl : UserControl
    {


        public DateTime LastUpdated
        {
            get { return (DateTime)GetValue(LastUpdatedProperty); }
            set { SetValue(LastUpdatedProperty, value); }
        }

        // Using a DependencyProperty as the backing store for LastUpdated.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty LastUpdatedProperty =
            DependencyProperty.Register("LastUpdated", typeof(DateTime), typeof(Dht22SensorControl), new PropertyMetadata(default(DateTime)));



        public string LastUpdatedHumanized
        {
            get { return (string)GetValue(LastUpdatedHumanizedProperty); }
            set { SetValue(LastUpdatedHumanizedProperty, value); }
        }

        // Using a DependencyProperty as the backing store for LastUpdatedHumanized.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty LastUpdatedHumanizedProperty =
            DependencyProperty.Register("LastUpdatedHumanized", typeof(string), typeof(Dht22SensorControl), new PropertyMetadata(default(string)));




        public Dht22SensorControl()
        {
            InitializeComponent();
            LastUpdatedHumanized = "Updating...";
        }


        private void Dht22SensorControl_OnToolTipOpening(object sender, ToolTipEventArgs e)
        {
            SetValue(LastUpdatedHumanizedProperty,$"Last seen {LastUpdated.Humanize(false)}" );
        }
    }
}

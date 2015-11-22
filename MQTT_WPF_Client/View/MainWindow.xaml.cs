using System;
using System.Windows;
using MahApps.Metro.Controls;
using MQTT_WPF_Client.ViewModel;

namespace MQTT_WPF_Client
{
   
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow: MetroWindow
    {

        public OverviewViewModel ViewModel { get; set; }

        public MainWindow()
        {
            InitializeComponent();
            
            ViewModel=new OverviewViewModel(this.Dispatcher);
            DataContext = ViewModel;
        }

        public void Window_Closed(object sender, EventArgs e)
        {
            // doing this as I'm not sure when to call MQTT.Disconnect()
            ViewModel.CloseMqtt();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.IsSettingsFlyoutOpen=true;
        }
    }
}

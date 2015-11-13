using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using System.Windows;
using BrightstarDB.Client;
using BrightstarDB.Storage;

namespace MQTT_WPF_Client
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {

        private const string  StoreName = "SensorsData";

        private const string BaseConnectionString = @"type=embedded;storesdirectory=l:\BrightstarDB Stores";

        public static string ConnectionString => BaseConnectionString + ";storename=" + StoreName;

        private void InitializeDb()
        {
            var dataObjectContext = BrightstarService.GetDataObjectContext(BaseConnectionString);
            if (!dataObjectContext.DoesStoreExist(StoreName))
            {
                dataObjectContext.CreateStore(StoreName, persistenceType: PersistenceType.AppendOnly);
            }
        }

        //private Application
        private void Application_Startup(object sender, StartupEventArgs e)
        {
            InitializeDb();
        }
    }
}

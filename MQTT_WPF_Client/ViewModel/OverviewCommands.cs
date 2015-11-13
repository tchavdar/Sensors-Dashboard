using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace MQTT_WPF_Client.ViewModel
{
    public  class SettingsCommand:ICommand
    {
        private readonly OverviewViewModel _vm;

        public SettingsCommand(OverviewViewModel vm)
        {
            _vm = vm;
        }
        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            _vm.SettingsDialogOpen();
        }

        public event EventHandler CanExecuteChanged;
    }
}

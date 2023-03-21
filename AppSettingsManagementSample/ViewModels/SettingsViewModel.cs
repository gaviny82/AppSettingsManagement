using AppSettingsManagement;
using AppSettingsManagementSample.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppSettingsManagementSample.ViewModels
{
    public partial class SettingsViewModel : ObservableObject
    {
        [SettingsManager]
        readonly SettingsService Manager;

        public SettingsViewModel(SettingsService manager)
        {
            Manager = manager;

            // Auto generate
            userName = manager.Username;
            //manager.SettingsChanged += () => 
            //{
            //    // Update view model properties when setting is changed.
            //};
            
        }

        // Auto generate using attribute:
        // When set, also change value in SettingsManager
        [ObservableProperty]
        string userName;

        [ObservableProperty]
        int number;

        [ObservableProperty]
        Theme theme;
    }
}

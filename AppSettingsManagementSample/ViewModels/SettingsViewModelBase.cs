using AppSettingsManagement;
using AppSettingsManagement.Mvvm;
using AppSettingsManagementSample.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppSettingsManagementSample.ViewModels;

// Consider moving to AppSettingsManagement package?
abstract class SettingsViewModelBase : ObservableObject
{

    ~SettingsViewModelBase()
    {
        if (this is ISettingsViewModel settingsViewModel)
            settingsViewModel.RemoveSettingsChagnedHandlers();
    }
}


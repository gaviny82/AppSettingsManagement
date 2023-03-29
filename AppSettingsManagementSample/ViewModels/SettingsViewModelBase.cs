using AppSettingsManagement;
using AppSettingsManagementSample.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppSettingsManagementSample.ViewModels;

// Consider moving to AppSettingsManagement package?
public abstract class SettingsViewModelBase : ObservableObject
{
    protected readonly SettingsContainer SettingsContainer;

    protected readonly List<SettingChangedEventHandler> settingChangedEventHandlers = new();

    public SettingsViewModelBase(SettingsContainer settingsContainer)
    {
        SettingsContainer = settingsContainer;
        InitializeSettings();
    }

    ~SettingsViewModelBase()
    {
        RemoveSettingsChagnedHandlers();
    }

    protected abstract void InitializeSettings();
    protected abstract void RemoveSettingsChagnedHandlers();
}


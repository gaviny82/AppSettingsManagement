﻿using AppSettingsManagement;
using AppSettingsManagementSample.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Activation;

namespace AppSettingsManagementSample.ViewModels;

/// <summary>
/// Autogenerated
/// </summary>
internal partial class SettingsViewModel : SettingsViewModelBase
{
    // Initialize view model and register events for updating view model when settings are changed.
    protected override void InitializeSettings()
    {
        Username = SettingsService.ActiveAccount.Username;
        var username_Handler = new SettingChangedEventHandler((_, _) => { Username = SettingsService.ActiveAccount.Username; });
        settingChangedEventHandlers.Add(username_Handler);
        SettingsService.ActiveAccount.UsernameChanged += username_Handler;

        Password = SettingsService.ActiveAccount.Password;
        var password_Handler = new SettingChangedEventHandler((_, _) => { Password = SettingsService.ActiveAccount.Password; });
        settingChangedEventHandlers.Add(password_Handler);
        SettingsService.ActiveAccount.PasswordChanged += password_Handler;
    }

    protected override void RemoveSettingsChagnedHandlers()
    {
        SettingsService.ActiveAccount.UsernameChanged -= settingChangedEventHandlers[0];
        SettingsService.ActiveAccount.PasswordChanged -= settingChangedEventHandlers[1];
    }

    protected override void OnPropertyChanged(PropertyChangedEventArgs e)
    {
        base.OnPropertyChanged(e);

        if (e.PropertyName == nameof(Username))
            SettingsService.ActiveAccount.Username = Username!;
        else if (e.PropertyName == nameof(Password))
            SettingsService.ActiveAccount.Password = Password!;
    }
}


internal partial class SettingsViewModel : SettingsViewModelBase
{
    private readonly SettingsService SettingsService;

    public SettingsViewModel(SettingsService settingsService) : base(settingsService)
    {
        SettingsService = (SettingsService)base.SettingsContainer;
    }


    [ObservableProperty]
    string? username;

    [ObservableProperty]
    string? password;

    [ObservableProperty]
    int number;

    [ObservableProperty]
    Theme theme;
}

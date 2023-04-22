﻿using AppSettingsManagement;
using AppSettingsManagementSample.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Activation;

namespace AppSettingsManagementSample.ViewModels;


class WeakEventManager
{
    static SettingChangedEventHandler? testStringChangedHandler;
    public static void WeakSubscribe(SettingsService settingsService, SettingsViewModel vm, string _event)
    {
        WeakReference<SettingsViewModel> WeakReference = new(vm);
        testStringChangedHandler = (sender, e) =>
        {
            if (WeakReference.TryGetTarget(out var target))
                target.TestString = settingsService.TestString;//TODO: resolve recursive calls
        };
        var e = settingsService.GetType().GetEvent(_event);
        e?.AddEventHandler(settingsService, testStringChangedHandler);
        
        //settingsService.TestStringChanged += SettingsService_TestStringChanged;
        
    }

    private static void SettingsService_TestStringChanged(SettingsContainer sender, SettingChangedEventArgs e)
    {
        testStringChangedHandler?.Invoke(sender, e);
    }

}

// TODO: autogenerate MVVM bindings
/// <summary>
/// Autogenerated
/// </summary>
internal partial class SettingsViewModel
{
    List<SettingChangedEventHandler> settingChangedEventHandlers = new();

    // Initialize view model and register events for updating view model when settings are changed.
    private void InitializeSettings()
    {
        PropertyChanged += SettingsPropertyChanged;
        WeakEventManager.WeakSubscribe(_settingsService, this, "TestStringChanged");

        //WeakReference<SettingsViewModel> WeakReference = new(this);

        //SettingChangedEventHandler testStringChangedHandler = (sender, e) =>
        //{
        //    if (WeakReference.TryGetTarget(out var target))
        //        target.TestString = _settingsService.TestString;
        //};
        //_settingsService.TestStringChanged += testStringChangedHandler;
        //settingChangedEventHandlers.Add(testStringChangedHandler);
    }

    protected void RemoveSettingsChagnedHandlers()
    {
        _settingsService.TestStringChanged += settingChangedEventHandlers[0];
    }

    private void SettingsPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        base.OnPropertyChanged(e);

        if (e.PropertyName == nameof(TestString))
            _settingsService.TestString = TestString;
    }
}


internal partial class SettingsViewModel : ObservableObject
{
    [SettingsProvider]
    private readonly SettingsService _settingsService;

    public SettingsViewModel(SettingsService settingsService)
    {
        _settingsService = settingsService;
        InitializeSettings();
    }

    ~SettingsViewModel()
    {
        Console.WriteLine("Finalized");
        //RemoveSettingsChagnedHandlers();
    }


    // Bind to settings service
    [BindToSetting(Path = nameof(SettingsService.IntList))]
    public ObservableCollection<int> TestList { get; private set; } = null!; // Will be initialized by generated code

    [ObservableProperty]
    [BindToSetting(Path = nameof(SettingsService.TestString))]
    string? testString;

}

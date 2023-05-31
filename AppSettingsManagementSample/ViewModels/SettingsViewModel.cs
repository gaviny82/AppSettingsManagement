using AppSettingsManagement;
using AppSettingsManagement.Mvvm;
using AppSettingsManagementSample.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Activation;

namespace AppSettingsManagementSample.ViewModels;

// Enhancement: build a separate WeakEventManager? use reflection?

internal partial class SettingsViewModel : SettingsViewModelBase, ISettingsViewModel
{
    [SettingsProvider]
    protected readonly SettingsService _settingsService;

    // TODO: Source Generator: also searches [SettingsProvider] in the parent classes to allow initialisation to be placed into a common base class
    // This will allow more simplification in SettingsViewModel when using AppSettingsManagement package.

    public SettingsViewModel(SettingsService settingsService)
    {
        _settingsService = settingsService;
        ((ISettingsViewModel)this).InitializeSettings();
    }


    // Test: Bind to a single value
    [ObservableProperty]
    [BindToSetting(Path = nameof(SettingsService.TestString))]
    string? testString;

    // Test: Bind to an item in a subcontainer
    [ObservableProperty]
    [BindToSetting(Path = $"{nameof(SettingsService.ActiveAccount)}/{nameof(SettingsService.ActiveAccount.Username)}")]
    string username = null!; // Will be initialized by generated code, should not be declared as nullable because it will be bound to a non-nullable setting item.

    // Test: Bind to settings service for collection
    [BindToSetting(Path = nameof(SettingsService.IntList))]
    public ObservableCollection<int> TestList { get; private set; } = null!; // Will be initialized by generated code

    [RelayCommand]
    void AddItem()
    {
        TestList.Add(TestList.Count);
    }

    [RelayCommand]
    void DeleteItem(int index)
    {
        TestList.RemoveAt(index);
    }
}

// Copyright (c) Microsoft Corporation and Contributors.
// Licensed under the MIT License.

using AppSettingsManagementSample.Services;
using AppSettingsManagementSample.ViewModels;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace AppSettingsManagementSample
{


    /// <summary>
    /// An empty window that can be used on its own or navigated to within a Frame.
    /// </summary>
    internal sealed partial class MainWindow : Window
    {
        public SettingsService SettingsManager { get; } = new();
        ApplicationDataContainer LocalSettings = ApplicationData.Current.LocalSettings;

        void ResetSettings()
        {
            LocalSettings.Values.Clear();
            foreach (var c in LocalSettings.Containers.Keys)
                LocalSettings.DeleteContainer(c);
        }

        public MainWindow()
        {
            this.InitializeComponent();

            panel.DataContext = new SettingsViewModel(SettingsManager);
        }

        private void Reset_Click(object sender, RoutedEventArgs e)
        {
            ResetSettings();
        }

        private void AddStr_Click(object sender, RoutedEventArgs e)
        {
            SettingsManager.Names.Add("New item");
        }
    }

}

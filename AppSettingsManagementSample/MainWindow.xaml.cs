// Copyright (c) Microsoft Corporation and Contributors.
// Licensed under the MIT License.

using AppSettingsManagementSample.Services;
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
    public sealed partial class MainWindow : Window
    {
        public string TestString { get; set; }

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

            ResetSettings();
            var num = SettingsManager.Number;
            Theme t = SettingsManager.Theme;
            var a = 1;
            // Cannot be used in unpackaged mode

            // Test: Create child containers
            //var accountsContainer = localSettings.CreateContainer("Accounts", ApplicationDataCreateDisposition.Always);
            //accountsContainer.Values["Username"] = "USERNAME";

            // Test: Storing arrays
            //int[] ints = { 1, 2, 3 };
            //localSettings.Values["ExampleArray"] = ints;

            //string[] strs = { "str1", "str2" };
            //localSettings.Values["StrArray"] = strs;

            //Array a;
        }

        private void setNumber_Click(object sender, RoutedEventArgs e)
        {
            var n = SettingsManager.Number;
            SettingsManager.Number = 3123;
        }

        private void setString_Click(object sender, RoutedEventArgs e)
        {
            SettingsManager.Username = "Name 1";
        }
    }

}

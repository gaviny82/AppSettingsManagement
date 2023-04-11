// Copyright (c) Microsoft Corporation and Contributors.
// Licensed under the MIT License.

using AppSettingsManagement;
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


        public MainWindow()
        {
            this.InitializeComponent();

            // TODO: test view model bindings
            //panel.DataContext = new SettingsViewModel(SettingsManager);
        }

        private void Delete_Click(object sender, RoutedEventArgs e)
            => DeleteAllSettings();
        void DeleteAllSettings()
        {
            LocalSettings.Values.Clear();
            foreach (var c in LocalSettings.Containers.Keys)
                LocalSettings.DeleteContainer(c);
        }

        private void SetStrings_Click(object sender, RoutedEventArgs e)
        {
            SettingsManager.TestString = "Test string";
            SettingsManager.TestStringWithDefault = "Test string with default";
        }

        private void SetInts_Click(object sender, RoutedEventArgs e)
        {
            SettingsManager.TestInt = 100;
            SettingsManager.TestIntWithDefault = 200;
        }

        private void SetEnums_Click(object sender, RoutedEventArgs e)
        {
            SettingsManager.Theme = Theme.Dark;
            SettingsManager.TestEnum = TestEnum.A;
        }

        private void AddItemToList_Click(object sender, RoutedEventArgs e)
        {
            SettingsCollection<int> intList = SettingsManager.IntList;
            intList.Add(intList.LastOrDefault() + 100);
        }

        private void GetItem_Click(object sender, RoutedEventArgs e)
        {
            Button btn = (Button)sender;
            string tag = (string)btn.Tag;

            string value = tag switch
            {
                "string" => $"TestString: {SettingsManager.TestString}; TestStringWithDefault: {SettingsManager.TestStringWithDefault}",
                "int" => $"TestInt: {SettingsManager.TestInt}; TestIntWithDefault: {SettingsManager.TestIntWithDefault}",
                "enum" => $"Theme: {SettingsManager.Theme}; TestEnum: {SettingsManager.TestEnum}",
                "list" => $"IntList: {string.Join(", ", SettingsManager.IntList)}",
                _ => ""
            };

            TestOutput.Text = value;
        }

        private void AddStr_Click(object sender, RoutedEventArgs e)
        {
            //SettingsManager.Names.Add("New item");
        }
    }

}

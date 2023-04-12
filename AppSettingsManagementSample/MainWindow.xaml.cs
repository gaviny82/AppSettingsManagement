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

        private void SetContainer_Click(object sender, RoutedEventArgs e)
        {
            AccountInformation accountInfo = SettingsManager.ActiveAccount;
            accountInfo.Username = "TestUser";
            accountInfo.Password = "TestPassword";
        }

        private void GetAllValues_Click(object sender, RoutedEventArgs e)
        {
            testString.Text = SettingsManager.TestString;
            testStringWithDefault.Text = SettingsManager.TestStringWithDefault;
            testInt.Text = SettingsManager.TestInt.ToString();
            testIntWithDefault.Text = SettingsManager.TestIntWithDefault.ToString();
            testEnumWithDefault.SelectedIndex = (int)SettingsManager.Theme;
            testEnum.SelectedIndex = SettingsManager.TestEnum == null ? -1 : (int)SettingsManager.TestEnum;
            testListContent.Text = string.Join(", ", SettingsManager.IntList);
        }


        private void SetAllValues_Click(object sender, RoutedEventArgs e)
        {
            SettingsManager.TestString = testString.Text;
            SettingsManager.TestStringWithDefault = testStringWithDefault.Text;
            SettingsManager.TestInt = int.Parse(testInt.Text);
            SettingsManager.TestIntWithDefault = int.Parse(testIntWithDefault.Text);
            SettingsManager.TestEnum = (TestEnum)testEnumWithDefault.SelectedIndex;
            SettingsManager.Theme = (Theme)testEnum.SelectedIndex;
            SettingsManager.IntList.Add(Random.Shared.Next());
        }

        private void AddStr_Click(object sender, RoutedEventArgs e)
        {
            //SettingsManager.Names.Add("New item");
        }
    }

}

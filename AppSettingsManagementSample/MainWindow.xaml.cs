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
using System.Text.Json;
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
            SettingsManager.IntList.Clear();
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
            username.Text = SettingsManager.ActiveAccount.Username;
            password.Text = SettingsManager.ActiveAccount.Password;
            studentInfo.Text = JsonSerializer.Serialize(SettingsManager.Student);
            studentInfo.Text += JsonSerializer.Serialize(SettingsManager.Students.ToArray());
        }


        private void SetAllValues_Click(object sender, RoutedEventArgs e)
        {
            SettingsManager.TestString = testString.Text;
            SettingsManager.TestStringWithDefault = testStringWithDefault.Text;
            int? i = int.TryParse(testInt.Text, out int test_int) ? test_int : null;
            SettingsManager.TestInt = i;
            SettingsManager.TestIntWithDefault = int.TryParse(testIntWithDefault.Text, out int num) ? num : -1;
            SettingsManager.Theme = (Theme)testEnumWithDefault.SelectedIndex;
            SettingsManager.TestEnum = (testEnum.SelectedIndex == -1 ? null : (TestEnum)testEnum.SelectedIndex);
            SettingsManager.IntList.Add(Random.Shared.Next());
            SettingsManager.ActiveAccount.Username = username.Text;
            SettingsManager.ActiveAccount.Password = password.Text;
            var s1 = new Student
            {
                Name = "Test",
                Age = 20
            };
            var s2 = new Student
            {
                Name = "Test2",
                Age = 21,
                Year = "Senior",
                Gender = 'F'
            };
            SettingsManager.Student = s1;
            SettingsManager.Students.Add(s1);
            SettingsManager.Students.Add(s2);
        }

    }

}

// Copyright (c) Microsoft Corporation and Contributors.
// Licensed under the MIT License.

using AppSettingsManagement;
using AppSettingsManagementSample.Services;
using AppSettingsManagementSample.ViewModels;
using AppSettingsManagementSample.Views;
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
        public static SettingsService SettingsManager { get; } = new();


        public MainWindow()
        {
            this.InitializeComponent();
            ContentFrame.Navigate(typeof(HomePage));
        }

        private void NavigationView_ItemInvoked(object sender, NavigationViewItemInvokedEventArgs args)
        {
            Type pageType = args switch
            {
                { InvokedItemContainer: NavigationViewItem { Tag: "Home" } }
                    => typeof(HomePage),
                { InvokedItemContainer: NavigationViewItem { Tag: "TestPage1" } }
                    => typeof(TestPage1),

                _ => typeof(HomePage)
            };
            ContentFrame.Navigate(pageType);
            GC.Collect();
        }
    }

}

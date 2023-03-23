using AppSettingsManagement;
using AppSettingsManagementSample.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Activation;

namespace AppSettingsManagementSample.ViewModels
{
    public partial class SettingsViewModel : ObservableObject
    {
        [SettingsManager]
        readonly SettingsService Manager;

        public SettingsViewModel(SettingsService manager)
        {
            Manager = manager;
            InitializeSettings();
        }
        ~SettingsViewModel()
        {
            RemoveSettingsChagnedHandlers();
        }

        // TODO: Autogenerate: 
        // Initialize view model and register events for updating view model when settings are changed.
        private void InitializeSettings()
        {
            Username = Manager.ActiveAccount.Username;
            var username_Handler = new SettingChangedEventHandler((_, _) => { Username = Manager.ActiveAccount.Username; });
            settingChangedEventHandlers.Add(username_Handler);
            Manager.ActiveAccount.UsernameChanged += username_Handler;

            Password = Manager.ActiveAccount.Password;
            var password_Handler = new SettingChangedEventHandler((_, _) => { Password = Manager.ActiveAccount.Password; });
            settingChangedEventHandlers.Add(password_Handler);
            Manager.ActiveAccount.PasswordChanged += password_Handler;
        }

        List<SettingChangedEventHandler> settingChangedEventHandlers = new();

        // TODO: Autogenerate: 
        private void RemoveSettingsChagnedHandlers()
        {
            Manager.ActiveAccount.UsernameChanged -= settingChangedEventHandlers[0];
            Manager.ActiveAccount.PasswordChanged -= settingChangedEventHandlers[1];
        }


        // tmp
        // TODO: Autogenerate: 
        protected override void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            base.OnPropertyChanged(e);

            if (e.PropertyName == nameof(Username))
                Manager.ActiveAccount.Username = Username!;
            else if (e.PropertyName == nameof(Password))
                Manager.ActiveAccount.Password = Password!;
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
}

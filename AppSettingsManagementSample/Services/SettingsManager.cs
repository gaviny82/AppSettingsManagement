using AppSettingsManagement;
using AppSettingsManagement.Windows;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppSettingsManagementSample.Services
{
    public class SettingsManagerService : SettingsManagerBase
    {
        public SettingsManagerService() : base(new WindowsSettingsStorage()) { }

        #region Username

        [SettingItem(nameof(Username))]
        public string Username // Autogenerate:
        {
            get => GetValue<string>(nameof(Username));
            set => SetValue<string>(nameof(Username), value, ref UsernameChanged);
        }

        // Autogenerate:
        public event SettingChangedEventHandler? UsernameChanged;

        #endregion

        #region UsernameWithDefault

        [SettingItem(nameof(UsernameWithDefault), Default = "")]
        public string UsernameWithDefault // Autogenerate:
        {
            get => GetValue<string>(nameof(UsernameWithDefault), "");
            set => SetValue<string>(nameof(UsernameWithDefault), value, ref UsernameWithDefaultChanged);
        }

        // Autogenerate:
        public event SettingChangedEventHandler? UsernameWithDefaultChanged;

        #endregion


        #region Number

        [SettingItem(nameof(Number))]
        public int Number
        {
            get => GetValue<int>(nameof(Number), default);
            set => SetValue<int>(nameof(Number), value, ref NumberChanged);
        }

        public event SettingChangedEventHandler? NumberChanged;

        #endregion

        #region NumberWithDefault

        [SettingItem(nameof(NumberWithDefault), Default = 10)]
        public int NumberWithDefault
        {
            get => GetValue<int>(nameof(NumberWithDefault), 10);
            set => SetValue<int>(nameof(NumberWithDefault), value, ref NumberWithDefaultChanged);
        }

        public event SettingChangedEventHandler? NumberWithDefaultChanged;

        #endregion


        [SettingItem(nameof(Theme))]
        public Theme Theme { get; set; }


        [SettingItem(nameof(ThemeFromInt), Type = typeof(int)) /* TODO: type conversion */]
        public Theme ThemeFromInt { get; set; }

        // TODO: Composite values

        // TODO: Arrays
    }

    public enum Theme
    {
        Bright,
        Dark,
        Default
    }
}

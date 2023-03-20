using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppSettingsManagement
{
    public class SettingChangedEventArgs
    {
        public string Key { get; init; }
        public object? NewValue { get; init; }

        public SettingChangedEventArgs(string key, object? value)
        {
            Key = key;
            NewValue = value;
        }
    }

    public delegate void SettingChangedEventHandler(ISettingsManager sender, SettingChangedEventArgs e);
}

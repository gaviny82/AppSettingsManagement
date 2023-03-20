using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppSettingsManagement;

// TODO: Convert to settings container
public class SettingsManagerBase : ISettingsManager
{
    public event SettingChangedEventHandler? SettingsChanged;

    public ISettingsStorage Storage { get; init; }

    public SettingsManagerBase(ISettingsStorage storage)
    {
        Storage = storage;
    }

    protected T GetValue<T>(string key) where T : notnull
    {
        if (Storage.ContainsKey(key))
        {
            return Storage.GetValue<T>(key);
        }
        else // key is not found
        {
            throw new KeyNotFoundException($"Key {key} is not found and default value is not provided.");
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="key"></param>
    /// <param name="defaultValue">Cannot be null; the setting item is removed if set to null.</param>
    /// <returns></returns>
    protected T GetValue<T>(string key, T defaultValue) where T : notnull
    {
        if (defaultValue is null)
            throw new ArgumentNullException(nameof(defaultValue));

        if (Storage.ContainsKey(key))
        {
            return Storage.GetValue<T>(key);
        }
        else // key is not found
        {
            Storage.SetValue(key, defaultValue);
            return defaultValue;
        }
    }

    protected void SetValue<T>(string key, T value, ref SettingChangedEventHandler? _event) where T: notnull
    {
        object? currentValue = Storage.ContainsKey(key) ? Storage.GetValue<T>(key) : null;

        // Only invoke events when the new value is different
        if (!value.Equals(currentValue))
        {
            Storage.SetValue(key, value);

            _event?.Invoke(this, new SettingChangedEventArgs(key, value));
            SettingsChanged?.Invoke(this, new SettingChangedEventArgs(key, value));
        }
    }

}


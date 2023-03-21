using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppSettingsManagement;

public abstract class SettingsContainer : ISettingsContainer
{
    public static readonly string ROOT_CONTAINER_NAME = "_ROOT_";

    #region ISettingsContainer Members

    public event SettingChangedEventHandler? SettingsChanged;

    public ISettingsStorage Storage { get; init; }

    public string Name { get; init; }

    public ISettingsContainer? Parent { get; init; }

    #endregion ISettingsContainer Members

    /// <summary>
    /// Used to initialize the root container
    /// </summary>
    /// <param name="storage"></param>
    public SettingsContainer(ISettingsStorage storage)
    {
        Storage = storage;
        Name = ROOT_CONTAINER_NAME;
    }

    public SettingsContainer(ISettingsStorage storage, string name, ISettingsContainer parent)
    {
        Storage = storage;
        Name = name;
        Parent = parent;
    }

    #region GetValue<T>


    protected T? GetValue<T>(string key) where T : struct
    {
        if (!Storage.ContainsKey(key))
        {
            return null;
        }

        return (T)Storage.GetValue<T>(key);
    }

    protected T? GetValueReferenceType<T>(string key) where T : class
    {
        if (!Storage.ContainsKey(key))
        {
            return null;
        }

        return (T)Storage.GetValue<T>(key);
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

    #endregion

    #region SetValue<T>

    private void _setValue<T>(string key, T value, ref SettingChangedEventHandler? _event) where T : notnull
    {
        object? currentValue = Storage.ContainsKey(key) ? Storage.GetValue<T>(key) : null;

        var t = typeof(T);

        // Only invoke events when the new value is different
        if (!value.Equals(currentValue))
        {
            Storage.SetValue(key, (T)value);

            _event?.Invoke(this, new SettingChangedEventArgs(key, value));
            SettingsChanged?.Invoke(this, new SettingChangedEventArgs(key, value));
        }
    }

    /// <summary>
    /// Wrap SetValue for value types
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="key"></param>
    /// <param name="value"></param>
    /// <param name="_event"></param>
    protected void SetValue<T>(string key, T? value, ref SettingChangedEventHandler? _event) where T : struct
    {
        if (value is null)
        {
            Storage.DeleteItem(key);
            return;
        }
        _setValue<T>(key, (T)value, ref _event);
    }

    /// <summary>
    /// Wrap SetValue for reference types
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="key"></param>
    /// <param name="value"></param>
    /// <param name="_event"></param>
    protected void SetValue<T>(string key, T? value, ref SettingChangedEventHandler? _event) where T : class
    {
        if (value is null)
        {
            Storage.DeleteItem(key);
            return;
        }
        _setValue<T>(key, value, ref _event);
    }

    #endregion SetValue<T>
}
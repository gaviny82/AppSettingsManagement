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
        InitializeContainers();
    }

    public SettingsContainer(ISettingsStorage storage, string name, ISettingsContainer parent)
    {
        Storage = storage;
        Name = name;
        Parent = parent;
        InitializeContainers();
    }

    /// <summary>
    /// Initialize child containers
    /// </summary>
    protected virtual void InitializeContainers() { }


    /// <summary>
    /// Path to access the item in the hierarchical storage
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    private string GetPathFromKey(string key)
    {
        Stack<string> levels = new();
        ISettingsContainer container = this;
        while (container.Parent is not null)
        {
            levels.Push(container.Name);
            container = container.Parent;
        }

        StringBuilder sb = new();
        foreach (string item in levels)
        {
            sb.Append($"{item}/");
        }
        sb.Append(key);
        return sb.ToString();
    }

    #region GetValue<T>

    protected T? GetValue<T>(string key, IDataTypeConverter? converter = null)
    {
        var path = GetPathFromKey(key);

        if (!Storage.Contains(path))
        {
            return default(T?); // null
        }

        Type type = Nullable.GetUnderlyingType(typeof(T)) ?? typeof(T);

        // If a type converter is given, use it to convert the value; otherwise, return the value as the type stored
        if (converter is not null)
        {
            if (converter.TargetType == type)
            {
                object value = Storage.GetValue(path, type);
                if (value.GetType() == converter.TargetType)
                    return (T?)converter.Convert(value);
            }

            throw new ArgumentException($"Typer converter given cannot convert to {type}", nameof(converter));
        }
        else
        {
            return (T)Storage.GetValue(path, type);
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

        // Try to get the value from the storage
        T? value = GetValue<T>(key);

        if (value is null) // key is not found
        {
            Storage.SetValue(key, defaultValue);
            return defaultValue;
        }

        return value;
    }

    #endregion

    #region SetValue<T>
    // TODO: type convertion

    private void _setValue<T>(string key, T value, ref SettingChangedEventHandler? _event) where T : notnull
    {
        var path = GetPathFromKey(key);

        object? currentValue = Storage.Contains(path) ? Storage.GetValue<T>(path) : null;

        var t = typeof(T);

        // Only invoke events when the new value is different
        if (!value.Equals(currentValue))
        {
            Storage.SetValue(path, (T)value);

            _event?.Invoke(this, new SettingChangedEventArgs(path, value));
            SettingsChanged?.Invoke(this, new SettingChangedEventArgs(path, value));
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
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

    // Overrided by derived classes to initialize containers. Called automatically in the constructors
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

    protected T? GetValue<T>(string key)
    {
        var path = GetPathFromKey(key);

        if (!Storage.ContainsKey(path))
        {
            return default(T?);
        }

        var type = Nullable.GetUnderlyingType(typeof(T)) ?? typeof(T);

        return (T)Storage.GetValue(path, type!);
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

        var path = GetPathFromKey(key);

        if (Storage.ContainsKey(path))
        {
            return Storage.GetValue<T>(path);
        }
        else // key is not found
        {
            Storage.SetValue(path, defaultValue);
            return defaultValue;
        }
    }

    #endregion

    #region SetValue<T>

    private void _setValue<T>(string key, T value, ref SettingChangedEventHandler? _event) where T : notnull
    {
        var path = GetPathFromKey(key);

        object? currentValue = Storage.ContainsKey(path) ? Storage.GetValue<T>(path) : null;

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
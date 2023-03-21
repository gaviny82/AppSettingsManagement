using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppSettingsManagement;


public interface ISettingsContainer
{
    /// <summary>
    /// Name and Parent are null when the container is the root container.
    /// </summary>
    ISettingsContainer? Parent { get; init; }

    string Name { get; init; }

    event SettingChangedEventHandler? SettingsChanged;

    ISettingsStorage Storage { get; }
}

/// <summary>
/// Allows settings to be stored on a device. The implementation should load the settings when instantiated.
/// 
/// A setting item can be any of the following types:
/// - Integers: int, short, byte
/// - Reals: float, double
/// - bool
/// - enum
/// - char, string
/// - Structs
/// - IList<T> where T is one of the types above
/// - Other classes (labelled as SettingsContainer)
/// </summary>
public interface ISettingsStorage
{
    /// <summary>
    /// Delete a setting item in the storage if the key exists .
    /// </summary>
    /// <param name="key"></param>
    void DeleteItem(string key);

    bool ContainsKey(string key);

    /// <summary>
    /// Obtains the value of a setting item from the storage
    /// </summary>
    /// <typeparam name="T">Type of the value</typeparam>
    /// <param name="key">A unique key that refers to the value</param>
    /// <returns>The value in the storage.</returns>
    /// <exception cref="ArgumentException">Throws if the object does not match the type parameter T</exception>
    /// <exception cref="KeyNotFoundException">Throws if the key is not found</exception>
    T GetValue<T>(string key) where T: notnull;

    /// <summary>
    /// Sets a setting item to a new value and save the change in the storage
    /// </summary>
    /// <typeparam name="T">Type of the value</typeparam>
    /// <param name="key">A unique key that refers to the value</param>
    /// <param name="value">The new value. The setting item is removed if value is null</param>
    void SetValue<T>(string key, T value) where T : notnull;
}

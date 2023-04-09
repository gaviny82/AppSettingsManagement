﻿using System;
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
/// Allows settings to be stored on a device. Class implementing this interface should load the settings when instantiated.
/// 
/// </summary>
/// <remarks>
/// The types accepted by the storage provider depends on the implementation.<br/>
/// If the provider does not support a type, a IDataTypeConvert may be used to convert the type to a supported type.
/// </remarks>
public interface ISettingsStorage
{
    /// <summary>
    /// Delete a setting item in the storage if the key exists .
    /// </summary>
    /// <param name="key"></param>
    void DeleteItem(string path);

    bool ContainsKey(string path);

    /// <summary>
    /// Obtains the value of a setting item from the storage
    /// </summary>
    /// <typeparam name="T">Type of the value</typeparam>
    /// <param name="path">A unique path that represents the location of an entry in the settings storage</param>
    /// <param name="converter">An optional converter that allows type convertion between the type accessed in code and the actual type stored.</param>
    /// <returns>The value in the storage.</returns>
    /// <exception cref="ArgumentException">Throws if the object does not match the type parameter T</exception>
    /// <exception cref="KeyNotFoundException">Throws if the key is not found</exception>
    /// <remarks>If a type converter is specified, the storage provider will be responsible for converting the type stored
    /// <br/> into T when the item is accessed, and converting T into the type used in storage.</remarks>
    T GetValue<T>(string path, IDataTypeConverter? converter = null) where T: notnull
    {
        return (T)GetValue(path, typeof(T));
    }

    object GetValue(string path, Type type, IDataTypeConverter? converter = null);


    /// <summary>
    /// Sets a setting item to a new value and save the change in the storage
    /// </summary>
    /// <typeparam name="T">Type of the value</typeparam>
    /// <param name="path">A unique path that represents the location of an entry in the settings storage</param>
    /// <param name="value">The new value. The setting item is removed if value is null</param>
    /// <param name="converter">An optional converter that allows type convertion between the type accessed in code and the actual type stored.</param>
    void SetValue<T>(string path, T value, IDataTypeConverter? converter = null) where T : notnull;
}
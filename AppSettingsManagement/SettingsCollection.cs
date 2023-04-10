using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppSettingsManagement;

public class SettingsCollection<T> : ObservableCollection<T>
{
    private readonly ISettingsStorage _settingsStorage;
    private readonly IDataTypeConverter? _typeConverter;
    private readonly string _storagePath;


    public SettingsCollection(ISettingsStorage settingsStorage, string storagePath, IDataTypeConverter? typeConverter = null)
    {
        _settingsStorage = settingsStorage;
        _storagePath = storagePath;
        
        if (typeConverter is not null)
        {
            if (typeConverter.SourceType == typeof(T))
                _typeConverter = typeConverter;
            else
                throw new ArgumentException($"Typer converter given cannot convert from {typeof(T)}", nameof(typeConverter));
        }

        // Load initial values from storage

        // Init the storage with an empty array if it doesn't exist
        if (!_settingsStorage.Contains(storagePath))
            _settingsStorage.SetValue(storagePath, Array.Empty<T>());

        // Get the array from the storage and apply type conversion
        if (typeConverter is not null)
        {
            Array storedArray = (Array)_settingsStorage.GetValue(storagePath, typeConverter.SourceType);
            
            // Throws if the type converter cannot convert to T
            if (typeConverter.TargetType != typeof(T))
                throw new ArgumentException($"Typer converter given cannot convert to {typeof(T)}", nameof(typeConverter));

            // Convert each element of the array and add it to the collection
            IEnumerator enumerator = storedArray.GetEnumerator();
            while (enumerator.MoveNext())
            {
                T convertedValue = (T)typeConverter.ConvertFrom(enumerator.Current)!; // Type conversion is guaranteed to succeed
                Add(convertedValue);
            }
        }
        else // No type conversion needed
        {
            T[] storedArray = _settingsStorage.GetValue<T[]>(storagePath);

            foreach (T value in storedArray)
                Add(value);
        }

        // Register CollectionChanged event after initialization
        CollectionChanged += SettingsCollection_CollectionChanged;
    }

    private void SettingsCollection_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        // Save the entire list as an array in the storage
        if(_typeConverter is not null) // Convert each element using the type converter
        {
            if (typeof(T) != _typeConverter.TargetType)
                throw new InvalidCastException($"Type converter given does not support convertion from {typeof(T)}");
            
            object?[] convertedArray = this.Select(item => _typeConverter.ConvertFrom(item)).ToArray();
            _settingsStorage.SetValue(_storagePath, convertedArray);
        }
        else // No type conversion needed
        {
            T[] array = this.ToArray();
            _settingsStorage.SetValue<T[]>(_storagePath, array);
        }
    }
}

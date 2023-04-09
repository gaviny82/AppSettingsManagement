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
        if (!_settingsStorage.ContainsKey(storagePath))
            _settingsStorage.SetValue(storagePath, Array.Empty<T>());

        T[] arrayStored = _settingsStorage.GetValue<T[]>(storagePath, typeConverter);

        foreach (T value in arrayStored)
            Add(value);

        // Register CollectionChanged event after initialization
        CollectionChanged += SettingsCollection_CollectionChanged;
    }

    private void SettingsCollection_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        // Save the entire collection as an array to the storage after converting it
        //object convertedValue = _typeConverter.ConvertFrom(this.ToArray());
        _settingsStorage.SetValue(_storagePath, this.ToArray());
    }
}

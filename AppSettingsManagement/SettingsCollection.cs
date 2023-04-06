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
    private readonly string _settingsKey;// TODO: full path


    public SettingsCollection(ISettingsStorage settingsStorage, string settingsKey, IDataTypeConverter? typeConverter = null)
    {
        _settingsStorage = settingsStorage;
        _settingsKey = settingsKey;
        
        if (typeConverter is not null)
        {
            if (typeConverter.SourceType == typeof(T))
                _typeConverter = typeConverter;
            else
                throw new ArgumentException($"Typer converter given cannot convert from {typeof(T)}", nameof(typeConverter));
        }


        // Load initial values from storage

        if(typeConverter is not null)
        {
            throw new NotImplementedException();
            //if (!_settingsStorage.ContainsKey(settingsKey))
            //    _settingsStorage.SetValue(settingsKey, new T[] { default(T) });

            //// Throws if the specified converter cannot convert the stored array
            //Type storedType = typeConverter.TargetType;
            //Array arr = (Array)_settingsStorage.GetValue(settingsKey, storedType.MakeArrayType());

            //if (!typeConverter.CanConvert(elementType))
            //    throw new InvalidCastException($"The type converter given cannot convert the type of the array stored");

            //T[] initialValues = arr
            //    .Cast<object>()
            //    .Select(item => (T)typeConverter.Convert(item))
            //    .ToArray();

            //foreach (T value in arr)
            //    Add(value);
        }
        else // T is the type stored
        {
            if (!_settingsStorage.ContainsKey(settingsKey)) //return;
                _settingsStorage.SetValue(settingsKey, Array.Empty<T>());

            var arrayStored = _settingsStorage.GetValue(settingsKey, typeof(T).MakeArrayType());
            if (arrayStored is not T[] arr)
                return;

            foreach (T value in arr)
                Add(value);
            
        }
        CollectionChanged += SettingsCollection_CollectionChanged;
    }

    private void SettingsCollection_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        // Save the entire collection as an array to the storage after converting it
        //object convertedValue = _typeConverter.ConvertFrom(this.ToArray());
        _settingsStorage.SetValue(_settingsKey, this.ToArray());
    }
}

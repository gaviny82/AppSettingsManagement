using Windows.Storage;
using System.Linq;
using System.Collections;

namespace AppSettingsManagement.Windows;

// Consider rename to WinRTSettingsStorage
public class WindowsSettingsStorage : ISettingsStorage
{
    readonly ApplicationDataContainer container;

    public WindowsSettingsStorage()
    {
        container = ApplicationData.Current.LocalSettings;
    }

    public bool ContainsKey(string path) => container.Values.ContainsKey(path);

    public void DeleteItem(string path)
    {
        container.Values.Remove(path);
    }

    /// <inheritdoc/>
    public T GetValue<T>(string path, IDataTypeConverter? converter = null) where T : notnull
        => (T)GetValue(path, typeof(T), converter);

    public object GetValue(string path, Type type, IDataTypeConverter? converter = null)
    {
        if (type.IsArray)
        {
            Type elementType = type.GetElementType()!;

            // WinRT ApplicationDataContainer cannot store empty arrays.
            if (container.Values.ContainsKey(path))
            {
                // Returns the stored array, which is not empty, if the key exsits.
                object? value = container.Values[path];
                if (value is not Array array)
                    throw new Exception($"Item stored at path\"{path}\" is not an array");

                // Convert the type of array elements
                if (converter is not null && elementType == converter.SourceType)
                {
                    IList list = array;
                    for (int i = 0; i < array.Length; i++)
                    {
                        list[i] = converter.Convert(list[i]);
                    }
                }

                return array;
            }
            else
            {
                // If the array is empty, it is stored as null in the ApplicationDataContainer.
                return Array.CreateInstance(elementType, 0);
            }
        }

        // Single values stored in ApplicationDataContainer
        return GetSingleValue(path, type, converter);
    }

    private object GetSingleValue(string path, Type type, IDataTypeConverter? converter = null)
    {
        object? value = container.Values[path];

        if (!container.Values.ContainsKey(path))
            throw new KeyNotFoundException($"Key {path} not found.");

        
        if (type == typeof(sbyte) ||
            type == typeof(byte) ||
            type == typeof(short) ||
            type == typeof(ushort) ||
            type == typeof(int) ||
            type == typeof(uint) ||
            type == typeof(long) ||
            type == typeof(ulong) ||
            
            type == typeof(float) ||
            type == typeof(double) ||
            type == typeof(decimal) ||
            
            type == typeof(char) ||
            type == typeof(string) || type == typeof(string[]) ||


            type.IsEnum // Enums are stored as integral types, which can be cast to enums automatically
            )
        {
            return value;
        }
        else
        {
            throw new NotSupportedException($"Data type {type} not supported.");
        }
    }

    public void SetValue<T>(string path, T value, IDataTypeConverter? converter = null) where T : notnull
    {
        var type = typeof(T);

        if (type.IsEnum)
        {
            // Store enums as their integral types
            var integralValue = Convert.ChangeType(value, Enum.GetUnderlyingType(type));
            container.Values[path] = integralValue;
        }
        else if (type.IsArray)
        {
            // If array is empty, remove the item, because empty array cannot be stored in ApplicationDataContainer.
            if (value is Array { Length: 0 })
                container.Values[path] = null;
            else if (value is Array)
                container.Values[path] = value;
        }
        else
        {
            // Single values of supported types
            container.Values[path] = value;
        }
    }
}

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

    /// <inheritdoc/>
    public bool Contains(string path) => container.Values.ContainsKey(path);

    /// <inheritdoc/>
    public void DeleteItem(string path)
    {
        container.Values.Remove(path);
    }

    #region GetValue

    /// <inheritdoc/>
    public T GetValue<T>(string path) where T : notnull
        => (T)GetValue(path, typeof(T));

    /// <inheritdoc/>
    public object GetValue(string path, Type type)
    {
        if (type.IsArray)
        {
            Type elementType = type.GetElementType()!;

            // WinRT ApplicationDataContainer cannot store empty arrays.
            if (container.Values.ContainsKey(path))
            {
                // If the key exists, return the stored array, which is never empty.
                object value = container.Values[path];

                // Check that the stored value is an array of the correct type.
                if (value.GetType() != elementType)
                    throw new Exception($"Item stored at path\"{path}\" is not an array of type {elementType}");

                return value;
            }
            else // If the array is empty, it is stored as null in the ApplicationDataContainer.
            {
                // Return an empty array of the correct type.
                return Array.CreateInstance(elementType, 0);
            }
        }

        // Access single values stored in ApplicationDataContainer
        return GetSingleValue(path, type);
    }

    private object GetSingleValue(string path, Type type)
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

    #endregion

    /// <inheritdoc/> // TODO: type convertion
    public void SetValue<T>(string path, T value) where T : notnull
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

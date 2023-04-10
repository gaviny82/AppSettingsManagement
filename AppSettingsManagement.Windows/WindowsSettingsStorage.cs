using Windows.Storage;
using System.Linq;
using System.Collections;
using Windows.Foundation;
using Windows.UI.ViewManagement;

namespace AppSettingsManagement.Windows;

// Consider rename to WinRTSettingsStorage
public class WindowsSettingsStorage : ISettingsStorage
{
    private readonly ApplicationDataContainer container;
    private readonly Type[] supportedTypes =
    {
        //https://learn.microsoft.com/en-us/windows/apps/design/app-settings/store-and-retrieve-app-data
        // Int
        typeof(int),
        typeof(sbyte),
        typeof(byte),
        typeof(short),
        typeof(ushort),
        typeof(int),
        typeof(uint),
        typeof(long),
        typeof(ulong),
        // Floating point
        typeof(float),
        typeof(double),
        // Char16 and string
        typeof(char),
        typeof(string),
        // Boolean
        typeof(bool),
        // Date and time
        typeof(DateTimeOffset),
        typeof(TimeSpan),
        // Other
        typeof(Guid),
        typeof(Point),
        typeof(Rect),

        // Enum is not supported by ApplicationDataContainer, but it can be stored as ints
        typeof(Enum)
    };


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

    // ApplicationDataContainer.Values is IPropertySet, which extends ICollection<KeyValuePair<string, object>>,
    // so there is no need to use generic types.

    /// <inheritdoc/>
    public T GetValue<T>(string path) where T : notnull
        => (T)GetValue(path, typeof(T));

    /// <inheritdoc/>
    public object GetValue(string path, Type type)
    {
        if (type.IsArray)
        {
            Type elementType = type.GetElementType()!;

            if (!supportedTypes.Contains(elementType))
                throw new InvalidOperationException($"Type {elementType} is not supported by {nameof(WindowsSettingsStorage)}");

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

        // Check the type requested
        if (!supportedTypes.Contains(type))
            throw new InvalidOperationException($"Type {type} is not supported by {nameof(WindowsSettingsStorage)}");

        if (!container.Values.ContainsKey(path))
            throw new KeyNotFoundException($"Key {path} not found.");

        return container.Values[path];
    }

    #endregion

    #region SetValue

    /// <inheritdoc/>
    public void SetValue<T>(string path, T value) where T : notnull
    {
        if (value is null)
            throw new ArgumentNullException(nameof(value));

        // ApplicationDataContainer.Values is IPropertySet, which extends ICollection<KeyValuePair<string, object>>,
        // so there is no need to use generic types.
        SetValue(path, value);
    }

    public void SetValue(string path, object value)
    {
        if (value is null)
            throw new ArgumentNullException(nameof(value));

        Type type = value.GetType();
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
            // TODO: check unsupported types
            // Single values of supported types
            container.Values[path] = value;
        }
    }

    #endregion
}

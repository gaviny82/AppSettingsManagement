using Windows.Storage;

namespace AppSettingsManagement.Windows;

public class WindowsSettingsStorage : ISettingsStorage
{
    readonly ApplicationDataContainer container;

    public WindowsSettingsStorage()
    {
        container = ApplicationData.Current.LocalSettings;
    }

    public bool ContainsKey(string key) => container.Values.ContainsKey(key);

    public void DeleteItem(string key)
    {
        container.Values.Remove(key);
    }


    public T GetValue<T>(string key) where T : notnull
    {
        if (!container.Values.ContainsKey(key))
            throw new KeyNotFoundException($"Key {key} not found.");

        var value = container.Values[key];
        var type = typeof(T);
        
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
            type == typeof(string) ||
            
            type.IsEnum // Enums are stored as integral types, which can be cast to enums automatically
            )
        {
            return (T)value;
        }
        else
        {
            throw new NotSupportedException($"Data type {type} not supported.");
        }
    }

    public void SetValue<T>(string key, T value) where T : notnull
    {
        var type = typeof(T);

        if (type.IsEnum)
        {
            var integralValue = Convert.ChangeType(value, Enum.GetUnderlyingType(type));
            container.Values[key] = integralValue;
        }
        else
        {
            container.Values[key] = value;
        }
    }
}

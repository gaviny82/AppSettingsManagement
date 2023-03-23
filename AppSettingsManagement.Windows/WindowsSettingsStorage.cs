using Windows.Storage;

namespace AppSettingsManagement.Windows;

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


    public object GetValue(string path, Type type)
    {
        if (!container.Values.ContainsKey(path))
            throw new KeyNotFoundException($"Key {path} not found.");

        var value = container.Values[path];
        
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
            return value;
        }
        else
        {
            throw new NotSupportedException($"Data type {type} not supported.");
        }
    }

    public void SetValue<T>(string path, T value) where T : notnull
    {
        var type = typeof(T);

        if (type.IsEnum)
        {
            var integralValue = Convert.ChangeType(value, Enum.GetUnderlyingType(type));
            container.Values[path] = integralValue;
        }
        else
        {
            container.Values[path] = value;
        }
    }
}

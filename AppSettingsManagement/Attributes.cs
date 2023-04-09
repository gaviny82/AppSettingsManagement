using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppSettingsManagement
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
    public class SettingsManagerAttribute : Attribute
    {

    }


    /// <summary>
    /// Must be applied to properties of a class which extends SettingsManagerBase
    /// </summary>
    [AttributeUsage(AttributeTargets.Constructor, AllowMultiple = true)]
    public class SettingItemAttribute : Attribute
    {
        public string Key { get; }

        /// <summary>
        /// Type of the property when accessed
        /// </summary>
        public Type Type { get; }

        // TODO: implement type converter
        public Type? Converter { get; init; }

        /// <summary>
        /// Default value of the setting item.<br/>null means default value is not provided.
        /// </summary>
        public object? Default { get; init; }

        /// <summary>
        /// Initialize a new instance of the SettingItemAttribute class
        /// </summary>
        /// <param name="key">Key of the setting item</param>
        /// <param name="type">Data type of the value stored</param>
        /// <param name="converter">Allows conversion between the data type stored and the type of the property. If not specified, type casting will be used.</param>
        public SettingItemAttribute(Type type, string key)
        {
            Key = key;
            Type = type;
        }

    }

    /// <summary>
    /// Indicate that the property is used as a settings container.
    /// </summary>
    [AttributeUsage(AttributeTargets.Constructor, AllowMultiple = true)]
    public class SettingsContainerAttribute : Attribute
    {
        public string ContainerName { get; }
        public Type ContainerType { get; }

        public SettingsContainerAttribute(Type type, string name)
        {
            ContainerType = type;
            ContainerName = name;
        }
    }

    /// <summary>
    /// Defines a collection of settings of the same type
    /// </summary>
    [AttributeUsage(AttributeTargets.Constructor, AllowMultiple = true)]
    public class SettingsCollectionAttribute : Attribute
    {
        public string CollectionName { get; }
        public Type CollectionType { get; }

        // TODO: implement type converter
        public Type? Converter { get; init; }

        public SettingsCollectionAttribute(Type type, string name)
        {
            CollectionName = name;
            CollectionType = type;
        }
    }
}

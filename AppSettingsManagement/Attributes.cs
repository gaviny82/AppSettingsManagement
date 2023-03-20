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
    [AttributeUsage(AttributeTargets.Property)]
    public class SettingItemAttribute : Attribute
    {
        public string Key { get; set; }

        public Type? Type { get; set; }

        public Type? Converter { get; set; }

        /// <summary>
        /// Default value of the setting item.<br/>null means default value is not provided.
        /// </summary>
        public object? Default { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key">Key of the setting item</param>
        /// <param name="type">Data type of the value stored</param>
        /// <param name="converter">Allows conversion between the data type stored and the type of the property. If not specified, type casting will be used.</param>
        public SettingItemAttribute(string key, Type? type = null, Type? converter = null)
        {
            Key = key;
            Type = type;
            
            if (converter != null) 
                throw new NotImplementedException();

            Converter = converter;
        }

    }
}

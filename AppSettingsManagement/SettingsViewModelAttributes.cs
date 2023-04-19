﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppSettingsManagement;

[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
public class SettingsProviderAttribute : Attribute
{

}

[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
public class SettingBinding : Attribute
{
    public required string Path { get; init; }
}
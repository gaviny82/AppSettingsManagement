using AppSettingsManagement;
using AppSettingsManagement.Windows;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppSettingsManagementSample.Services;


internal partial class SettingsService : SettingsContainer
{
    static readonly ISettingsStorage Provider = new WindowsSettingsStorage();

    #region Settings

    // Test strings
    [SettingItem(typeof(string), "TestString")]
    [SettingItem(typeof(string), "TestStringWithDefault", Default = "DEFAULT STRING")]

    // Test ints
    [SettingItem(typeof(int), "TestInt")] // Used to test properties without default
    [SettingItem(typeof(int), "TestIntWithDefault", Default = 100)]

    // Test enums
    [SettingItem(typeof(Theme), "Theme", Default = Theme.Default)]
    [SettingItem(typeof(TestEnum), "TestEnum")]

    // Test lists
    [SettingsCollection(typeof(int), "IntList")]

    // TODO: Test containers
    [SettingsContainer(typeof(AccountInformation), "ActiveAccount")] // Composite values

    // TODO: Test converters

    #endregion

    public SettingsService() : base(Provider) { }

}

public enum Theme
{
    Bright,
    Dark,
    Default
}

enum TestEnum : byte
{
    A,
    B,
    C
}


internal partial class AccountInformation : SettingsContainer
{
    #region Settings

    [SettingItem(typeof(string), "Username", Default = "a")]
    [SettingItem(typeof(string), "Password", Default = "b")]

    #endregion

    public AccountInformation(ISettingsStorage storage, string name, ISettingsContainer parent) : base(storage, name, parent) { }
}
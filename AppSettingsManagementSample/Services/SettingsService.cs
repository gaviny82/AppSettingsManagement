using AppSettingsManagement;
using AppSettingsManagement.Windows;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppSettingsManagementSample.Services;

public partial class SettingsService
{

}

public partial class SettingsService : SettingsContainer
{
    static ISettingsStorage Provider = new WindowsSettingsStorage();

#nullable disable
    public SettingsService() : base(Provider) { }
#nullable enable

    // Autogenerate:
    protected override void InitializeContainers()
    {
        ActiveAccount = new(Storage, nameof(ActiveAccount), this);
    }


    // Used to test properties without default
    #region UsernameWithoutDefault

    [SettingItem(nameof(UsernameWithoutDefault))]
    public string? UsernameWithoutDefault // Autogenerate:
    {
        get => GetValue<string?>(nameof(UsernameWithoutDefault));
        set => SetValue<string>(nameof(UsernameWithoutDefault), value, ref UsernameWithoutDefaultChanged);
    }

    // Autogenerate:
    public event SettingChangedEventHandler? UsernameWithoutDefaultChanged;

    #endregion

    // Used to test properties without default
    #region NumberWithoutDefault

    [SettingItem(nameof(NumberWithoutDefault))]
    public int NumberWithoutDefault
    {
        get => GetValue<int>(nameof(NumberWithoutDefault), 10);
        set => SetValue<int>(nameof(NumberWithoutDefault), value, ref NumberWithDefaultChanged);
    }

    public event SettingChangedEventHandler? NumberWithDefaultChanged;

    #endregion

    #region Number

    [SettingItem(nameof(Number), Default = 10)]
    public int? Number
    {
        get => GetValue<int?>(nameof(Number));
        set => SetValue<int>(nameof(Number), value, ref NumberChanged);
    }

    public event SettingChangedEventHandler? NumberChanged;

    #endregion

    #region Enum

    [SettingItem(nameof(Theme), Default = Theme.Default)]
    public Theme Theme
    {
        get => GetValue<Theme>(nameof(Theme), Theme.Default);
        set => SetValue<Theme>(nameof(Theme), value, ref ThemeChanged);
    }

    public event SettingChangedEventHandler? ThemeChanged;

    #endregion

    // Composite values

    [SettingsContainer(nameof(ActiveAccount))]
    public AccountInformation ActiveAccount { get; private set; }


    // TODO: Arrays

}

public enum Theme
{
    Bright,
    Dark,
    Default
}

public class AccountInformation : SettingsContainer
{
    public AccountInformation(ISettingsStorage storage, string name, ISettingsContainer parent) : base(storage, name, parent) { }

    [SettingItem(nameof(Username), Default = "a")]
    public string Username
    {
        get => GetValue<string>(nameof(Username), "a");
        set => SetValue<string>(nameof(Username), value, ref UsernameChanged);
    }
    public event SettingChangedEventHandler? UsernameChanged;


    [SettingItem(nameof(Password), Default = "b")]
    public string Password
    {
        get => GetValue<string>(nameof(Password), "b");
        set => SetValue<string>(nameof(Password), value, ref PasswordChanged);
    }
    public event SettingChangedEventHandler? PasswordChanged;
}
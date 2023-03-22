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
    public SettingsService() : base(Provider) 
    {
        // Autogenerate:
        ActiveAccount = new(Provider, nameof(ActiveAccount), this);
    }

    #region Username

    [SettingItem(nameof(Username))]
    public string? Username // Autogenerate:
    {
        get => GetValue<string?>(nameof(Username));
        set => SetValue<string>(nameof(Username), value, ref UsernameChanged);
    }

    // Autogenerate:
    public event SettingChangedEventHandler? UsernameChanged;

    #endregion

    #region UsernameWithDefault

    [SettingItem(nameof(UsernameWithDefault), Default = "")]
    public string UsernameWithDefault // Autogenerate:
    {
        get => GetValue<string>(nameof(UsernameWithDefault), "");
        set => SetValue<string>(nameof(UsernameWithDefault), value, ref UsernameWithDefaultChanged);
    }

    // Autogenerate:
    public event SettingChangedEventHandler? UsernameWithDefaultChanged;

    #endregion


    #region Number

    [SettingItem(nameof(Number))]
    public int? Number
    {
        get => GetValue<int?>(nameof(Number));
        set => SetValue<int>(nameof(Number), value, ref NumberChanged);
    }

    public event SettingChangedEventHandler? NumberChanged;

    #endregion

    #region NumberWithDefault

    [SettingItem(nameof(NumberWithDefault), Default = 10)]
    public int NumberWithDefault
    {
        get => GetValue<int>(nameof(NumberWithDefault), 10);
        set => SetValue<int>(nameof(NumberWithDefault), value, ref NumberWithDefaultChanged);
    }

    public event SettingChangedEventHandler? NumberWithDefaultChanged;

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
    public AccountContainer ActiveAccount { get; init; }


    // TODO: Arrays

}

public enum Theme
{
    Bright,
    Dark,
    Default
}

public class AccountContainer : SettingsContainer
{
    public AccountContainer(ISettingsStorage storage, string name, ISettingsContainer parent) : base(storage, name, parent) { }

    [SettingItem(nameof(Username))]
    public string? Username
    {
        get => GetValue<string?>(nameof(Username));
        set => SetValue<string>(nameof(Username), value, ref UsernameChanged);
    }
    public event SettingChangedEventHandler? UsernameChanged;

    [SettingItem(nameof(Password))]
    public string? Password
    {
        get => GetValue<string?>(nameof(Password));
        set => SetValue<string>(nameof(Password), value, ref PasswordChanged);
    }
    public event SettingChangedEventHandler? PasswordChanged;
}
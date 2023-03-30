﻿using AppSettingsManagement;
using AppSettingsManagement.Windows;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppSettingsManagementSample.Services;

// TODO: generate SettingItem
// TODO: generate SettingContainer
/// <summary>
/// Autogenerated
/// </summary>
internal partial class SettingsService
{
    // Whether to use nullable type depends on if a default value is provided.
    public string? UsernameWithoutDefault // Autogenerate:
    {
        get => GetValue<string?>(nameof(UsernameWithoutDefault));
        set => SetValue<string>(nameof(UsernameWithoutDefault), value, ref UsernameWithoutDefaultChanged);
    }

    public event SettingChangedEventHandler? UsernameWithoutDefaultChanged;


    public int? NumberWithoutDefault
    {
        get => GetValue<int?>(nameof(NumberWithoutDefault));
        set => SetValue<int>(nameof(NumberWithoutDefault), value, ref NumberWithDefaultChanged);
    }

    public event SettingChangedEventHandler? NumberWithDefaultChanged;


    public int Number
    {
        get => GetValue<int>(nameof(Number), 10);
        set => SetValue<int>(nameof(Number), value, ref NumberChanged);
    }

    public event SettingChangedEventHandler? NumberChanged;


    public Theme Theme
    {
        get => GetValue<Theme>(nameof(Theme), Theme.Default);
        set => SetValue<Theme>(nameof(Theme), value, ref ThemeChanged);
    }

    public event SettingChangedEventHandler? ThemeChanged;


    public AccountInformation ActiveAccount { get; private set; } = null!;


    protected override void InitializeContainers()
    {
        ActiveAccount = new(Storage, nameof(ActiveAccount), this);
    }

}

internal partial class SettingsService : SettingsContainer
{
    static readonly ISettingsStorage Provider = new WindowsSettingsStorage();

    #region Settings

    [SettingItem(typeof(string), "UsernameWithoutDefault")] // Used to test properties without default

    [SettingItem(typeof(int), "NumberWithoutDefault")] // Used to test properties without default
    [SettingItem(typeof(int), "Number", Default = 10)]

    [SettingItem(typeof(Theme), "Theme", Default = Theme.Default)]

    [SettingsContainer(typeof(AccountInformation), nameof(ActiveAccount))] // Composite values

    #endregion

    public SettingsService() : base(Provider) { }

    // TODO: Arrays
}

public enum Theme
{
    Bright,
    Dark,
    Default
}


internal partial class AccountInformation
{
    public string Username
    {
        get => GetValue<string>(nameof(Username), "a");
        set => SetValue<string>(nameof(Username), value, ref UsernameChanged);
    }

    public event SettingChangedEventHandler? UsernameChanged;


    public string Password
    {
        get => GetValue<string>(nameof(Password), "b");
        set => SetValue<string>(nameof(Password), value, ref PasswordChanged);
    }

    public event SettingChangedEventHandler? PasswordChanged;
}

internal partial class AccountInformation : SettingsContainer
{
    #region Settings

    [SettingItem(typeof(string), "Username", Default = "a")]
    [SettingItem(typeof(string), "Password", Default = "b")]

    #endregion

    public AccountInformation(ISettingsStorage storage, string name, ISettingsContainer parent) : base(storage, name, parent) { }
}
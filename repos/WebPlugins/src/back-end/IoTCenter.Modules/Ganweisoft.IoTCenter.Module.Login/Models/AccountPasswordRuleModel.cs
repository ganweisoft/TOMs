// Copyright (c) 2020-2025 Beijing TOMs Software Technology Co., Ltd
using System.Collections.Generic;

namespace Ganweisoft.IoTCenter.Module.Login;

public class AccountPasswordRuleModel
{
    public Login Login { get; set; }
    public Account Account { get; set; }
    public Password Password { get; set; }
}

public class Login
{
    public bool Enabled { get; set; }
    public int Failures { get; set; }
    public int Lock { get; set; }
}

public class Account
{
    public bool Enabled { get; set; }
    public int Length { get; set; }
    public List<Element> Elements { get; set; }
}

public class Password
{
    public bool Enabled { get; set; }
    public int Failures { get; set; }
    public bool ForceModifyFirstLogin { get; set; }
    public int Length { get; set; }
    public List<Element> Elements { get; set; }
    public int MinCharacters { get; set; }
    public bool AllowedUserName { get; set; }
    public int NpDiffOpAtLeastCharacters { get; set; }
    public int TermOfValidity { get; set; }
    public bool ExpirationReminder { get; set; }
    public int ReminderDaysInAdvance { get; set; }
    public PasswordPolicy AfterExpired { get; set; }
    public int CheckedHistoryPolicy { get; set; }
}

public enum Element
{
    Lowercase,
    Uppercase,
    Number,
    Symbol
}

public enum PasswordPolicy
{
    Disabled = 0,
    EnforcementModify = 1,
    Unlimited = 2
}

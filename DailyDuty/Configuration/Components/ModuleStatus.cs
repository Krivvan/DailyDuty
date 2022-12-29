﻿using System;
using System.Numerics;
using DailyDuty.Localization;
using DailyDuty.Utilities;
using KamiLib.Utilities;

namespace DailyDuty.Configuration.Components;

public enum ModuleStatus
{
    Unknown,
    Incomplete,
    Unavailable,
    Complete
}

public static class ModuleStatusExtensions
{
    public static string GetTranslatedString(this ModuleStatus value)
    {
        return value switch
        {
            ModuleStatus.Unknown => Strings.Common.Unknown,
            ModuleStatus.Incomplete => Strings.Common.Incomplete,
            ModuleStatus.Unavailable => Strings.Common.Unavailable,
            ModuleStatus.Complete => Strings.Common.Complete,
            _ => throw new ArgumentOutOfRangeException(nameof(value), value, null)
        };
    }

    public static Vector4 GetStatusColor(this ModuleStatus value)
    {
        return value switch
        {
            ModuleStatus.Unknown => Colors.Grey,
            ModuleStatus.Incomplete => Colors.Red,
            ModuleStatus.Unavailable => Colors.Orange,
            ModuleStatus.Complete => Colors.Green,
            _ => throw new ArgumentOutOfRangeException(nameof(value), value, null)
        };
    }
}
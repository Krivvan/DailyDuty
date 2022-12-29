﻿using DailyDuty.Interfaces;
using ImGuiNET;
using System;
using DailyDuty.Configuration.Components;
using DailyDuty.Localization;
using KamiLib.Utilities;

namespace DailyDuty.UserInterface.Components;

internal class StatusSelectable : ISelectable
{
    public ModuleName OwnerModuleName { get; }
    public IDrawable Contents { get; }
    public IModule ParentModule { get; }

    private readonly Func<ModuleStatus> status;

    public StatusSelectable(IModule parentModule, IDrawable contents, Func<ModuleStatus> status)
    {
        OwnerModuleName = parentModule.Name;
        ParentModule = parentModule;
        Contents = contents;
        this.status = status;
    }

    public void DrawLabel()
    {
        DrawModuleLabel();
        DrawModuleStatus();
    }

    private void DrawModuleLabel()
    {
        ImGui.Text(OwnerModuleName.GetTranslatedString()[..Math.Min(OwnerModuleName.GetTranslatedString().Length, 28)]);
    }

    private void DrawModuleStatus()
    {
        var region = ImGui.GetContentRegionAvail();
        
        var color = status.Invoke() switch
        {
            ModuleStatus.Unknown => Colors.Grey,
            ModuleStatus.Incomplete => Colors.Red,
            ModuleStatus.Unavailable => Colors.Orange,
            ModuleStatus.Complete => Colors.Green,
            _ => throw new ArgumentOutOfRangeException()
        };

        var text = status.Invoke().GetTranslatedString();

        // Override Status if Module is Disabled
        if (!ParentModule.GenericSettings.Enabled.Value)
        {
            text = Strings.Common.Disabled;
            color = Colors.Grey;
        }

        var textSize = ImGui.CalcTextSize(text);

        ImGui.SameLine(region.X - textSize.X + 3.0f);
        ImGui.TextColored(color, text);
    }
}
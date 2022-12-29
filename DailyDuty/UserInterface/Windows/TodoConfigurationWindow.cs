﻿using System;
using System.Linq;
using System.Numerics;
using DailyDuty.Configuration.Components;
using DailyDuty.Configuration.OverlaySettings;
using DailyDuty.Localization;
using Dalamud.Interface.Windowing;
using ImGuiNET;
using KamiLib.InfoBoxSystem;

namespace DailyDuty.UserInterface.Windows;

internal class TodoConfigurationWindow : Window
{
    public static TodoOverlaySettings Settings => Service.ConfigurationManager.CharacterConfiguration.TodoOverlay;

    public TodoConfigurationWindow() : base("DailyDuty Todo Configuration", ImGuiWindowFlags.AlwaysVerticalScrollbar)
    {
        SizeConstraints = new WindowSizeConstraints
        {
            MinimumSize = new Vector2(350 * (4.0f / 3.0f), 350),
            MaximumSize = new Vector2(9999,9999)
        };
    }

    public override void PreOpenCheck()
    {
        if (!Service.ConfigurationManager.CharacterDataLoaded) IsOpen = false;
        if (Service.ClientState.IsPvP) IsOpen = false;
    }

    public override void Draw()
    {

        InfoBox.Instance
            .AddTitle(Strings.UserInterface.Todo.MainOptions)
            .AddConfigCheckbox(Strings.Common.Enabled, Settings.Enabled)
            .Draw();

        InfoBox.Instance
            .AddTitle(Strings.UserInterface.Todo.TaskSelection)
            .AddConfigCheckbox(Strings.UserInterface.Todo.ShowDailyTasks, Settings.ShowDailyTasks)
            .AddConfigCheckbox(Strings.UserInterface.Todo.ShowWeeklyTasks, Settings.ShowWeeklyTasks)
            .Draw();

        if (Settings.ShowDailyTasks.Value)
        {
            var enabledDailyTasks = Service.ModuleManager.GetTodoComponents(CompletionType.Daily)
                .Where(module => module.ParentModule.GenericSettings.Enabled.Value);

            InfoBox.Instance
                .AddTitle(Strings.UserInterface.Todo.DailyTasks)
                .BeginTable()
                .AddRows(enabledDailyTasks, Strings.UserInterface.Todo.NoTasksEnabled)
                .EndTable()
                .Draw();
        }

        if (Settings.ShowWeeklyTasks.Value)
        {
            var enabledWeeklyTasks = Service.ModuleManager.GetTodoComponents(CompletionType.Weekly)
                .Where(module => module.ParentModule.GenericSettings.Enabled.Value);

            InfoBox.Instance
                .AddTitle(Strings.UserInterface.Todo.WeeklyTasks)
                .BeginTable()
                .AddRows(enabledWeeklyTasks, Strings.UserInterface.Todo.NoTasksEnabled)
                .EndTable()
                .Draw();
        }

        InfoBox.Instance
            .AddTitle(Strings.UserInterface.Todo.TaskDisplay)
            .AddConfigCheckbox(Strings.UserInterface.Todo.HideCompletedTasks, Settings.HideCompletedTasks)
            .AddConfigCheckbox(Strings.UserInterface.Todo.HideUnavailable, Settings.HideUnavailableTasks)
            .AddConfigCheckbox(Strings.UserInterface.Todo.CompleteCategory, Settings.ShowCategoryAsComplete)
            .Draw();

        InfoBox.Instance
            .AddTitle(Strings.UserInterface.Todo.WindowOptions)
            .AddConfigCheckbox(Strings.UserInterface.Todo.HideWindowCompleted, Settings.HideWhenAllTasksComplete)
            .AddConfigCheckbox(Strings.UserInterface.Todo.HideWindowInDuty, Settings.HideWhileInDuty)
            .AddConfigCheckbox(Strings.UserInterface.Todo.LockWindow, Settings.LockWindowPosition)
            .AddConfigCheckbox(Strings.UserInterface.Todo.AutoResize, Settings.AutoResize)
            .AddConfigCombo(Enum.GetValues<WindowAnchor>(), Settings.AnchorCorner, WindowAnchorExtensions.GetTranslatedString, Strings.UserInterface.Todo.AnchorCorner, 200.0f)
            .AddDragFloat(Strings.UserInterface.Todo.Opacity, Settings.Opacity, 0.0f, 1.0f, 200.0f)
            .AddConfigColor(Strings.Common.Header, Settings.TaskColors.HeaderColor)
            .AddConfigColor(Strings.Common.Incomplete, Settings.TaskColors.IncompleteColor)
            .AddConfigColor(Strings.Common.Complete, Settings.TaskColors.CompleteColor)
            .AddConfigColor(Strings.Common.Unavailable, Settings.TaskColors.UnavailableColor)
            .Draw();
    }
    
    public override void OnClose()
    {
        Service.ConfigurationManager.Save();
    }
}
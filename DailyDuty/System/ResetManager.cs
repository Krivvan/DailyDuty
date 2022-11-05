﻿using System;
using System.Threading;
using DailyDuty.Configuration.Components;
using Dalamud.Logging;

namespace DailyDuty.System;

internal class ResetManager : IDisposable
{
    private readonly CancellationTokenSource cancellationToken = new();

    public ResetManager()
    {
        FrameworkOnUpdate();
    }

    private void FrameworkOnUpdate()
    {
        Service.Framework.RunOnTick(FrameworkOnUpdate,TimeSpan.FromSeconds(5), cancellationToken: cancellationToken.Token);

        if (Service.ConfigurationManager.CharacterDataLoaded)
        {
            ResetModules();
        }
    }

    public static void ResetModules()
    {
        foreach (var module in Service.ModuleManager.GetLogicComponents())
        {
            var now = DateTime.UtcNow;

            if (now >= module.ParentModule.GenericSettings.NextReset)
            {
                PluginLog.Debug($"[{module.ParentModule.Name.GetTranslatedString()}] performing reset. Next Reset:[{module.ParentModule.GenericSettings.NextReset}]");

                module.DoReset();
                module.ParentModule.GenericSettings.NextReset = module.GetNextReset();
                Service.ConfigurationManager.Save();
            }
        }
    }

    public void Dispose()
    {
        cancellationToken.Cancel();
    }
}
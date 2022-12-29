﻿using System;
using DailyDuty.DataModels;

namespace DailyDuty.Interfaces;

public interface IModule : IDisposable
{
    GenericSettings GenericSettings { get; }
    ModuleName Name { get; }
    IConfigurationComponent ConfigurationComponent { get; }
    IStatusComponent StatusComponent { get; }
    ILogicComponent LogicComponent { get; }
    ITodoComponent TodoComponent { get; }
    ITimerComponent TimerComponent { get; }
}
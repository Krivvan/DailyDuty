using DailyDuty.Classes;
using DailyDuty.Models;
using Lumina.Excel.GeneratedSheets;
using DailyDuty.Localization;
using ClientStructs = FFXIVClientStructs.FFXIV.Client.Game.UI;
using DailyDuty.Modules.BaseModules;
using Dalamud.Interface.Utility;
using FFXIVClientStructs.FFXIV.Client.Game;
using ImGuiNET;
using System.Drawing;
using FFXIVClientStructs.FFXIV.Client.Game.UI;
using System.Collections.Generic;
using System;
using System.Diagnostics;
using Dalamud.Interface;

namespace DailyDuty.Modules;

public class DiademData : ModuleData
{
    public int MinerPointsTotal = 0;
    public int MinerPointsAccumulated = -1;
    public int MinerPointsGoal;
    public int BotanistPointsTotal = 0;
    public int BotanistPointsAccumulated = -1;
    public int BotanistPointsGoal;

    protected override void DrawModuleData()
    {
        if (MinerPointsAccumulated < 0 | BotanistPointsAccumulated < 0)
        {
            ImGui.TextColored(KnownColor.Orange.Vector(), "Open achievements and click on Skyward Sledgehammer III and Skyward Scythe III to initialize module");
        }
        //else
        //{
            DrawDataTable([
                ("Total Miner Points Accumulated", MinerPointsTotal.ToString()),
                ("Weekly Miner Points Accumulated", MinerPointsAccumulated.ToString()),
                ("Total Botanist Points Accumulated", BotanistPointsTotal.ToString()),
                ("Weekly Botanist Points Accumulated", BotanistPointsAccumulated.ToString())
            ]);
        //}

        ImGuiHelpers.ScaledDummy(5.0f);
    }
}

public class DiademConfig : ModuleConfig
{
    public int MinerPointsGoal = 20000;
    public int MinerPointsWeeklyStart = -1;
    public int BotanistPointsGoal = 20000;
    public int BotanistPointsWeeklyStart = -1;

    protected override bool DrawModuleConfig()
    {
        var configChanged = false;

        ImGui.SetNextItemWidth(ImGui.GetContentRegionAvail().X / 2.0f);
        configChanged |= ImGui.SliderInt("Miner Points Goal", ref MinerPointsGoal, 0, 500000);

        ImGui.SetNextItemWidth(ImGui.GetContentRegionAvail().X / 2.0f);
        configChanged |= ImGui.SliderInt("Botanist Points Goal", ref BotanistPointsGoal, 0, 500000);

        return configChanged;
    }

}

public unsafe class Diadem : Modules.Weekly<DiademData, DiademConfig>
{
    public override ModuleName ModuleName => ModuleName.Diadem;
    public static uint currentAchievement;

    public override void Update()
    {
        //Reset();
        var uiState = UIState.Instance();
        // Get Achievement Data and update if new achievement was opened
        var clickedAchievement = uiState->Achievement.ProgressAchievementId;
        if (currentAchievement != clickedAchievement)
        {
            var achievementState = uiState->Achievement.ProgressCurrent;
            switch (clickedAchievement)
            {
                case 2515:
                    Data.MinerPointsTotal = (int)achievementState;
                    if (Config.MinerPointsWeeklyStart < 0) Config.MinerPointsWeeklyStart = Data.MinerPointsTotal;
                    break;
                case 2518:
                    Data.BotanistPointsTotal = (int)achievementState;
                    if (Config.BotanistPointsWeeklyStart < 0) Config.BotanistPointsWeeklyStart = Data.BotanistPointsTotal;
                    break;
                    //case 2521:
                    //    fsh500K = progress;
                    //    break;
            }
            currentAchievement = clickedAchievement;
        }

        if (Config.MinerPointsWeeklyStart >= 0)
        {
            
            Data.MinerPointsAccumulated = Data.MinerPointsTotal - Config.MinerPointsWeeklyStart;
        }

        if (Config.BotanistPointsWeeklyStart >= 0)
        {
            Config.BotanistPointsWeeklyStart = Data.BotanistPointsTotal;
            Data.BotanistPointsAccumulated = Data.BotanistPointsTotal - Config.BotanistPointsWeeklyStart;
        }

        //if ((DateTime.Now - LastUpdate).TotalMilliseconds > 150)
        //{
        //    DiademResources.CalculatePoints(UpdatePresetIndex);
        //    Data.MinerPointsTotal += DiademResources.MinPoints;
        //    DiademResources.MinPoints = 0;
        //    Data.BotanistPointsTotal += DiademResources.BtnPoints;
        //    DiademResources.BtnPoints = 0;

        //   UpdatePresetIndex++;
        //    if (UpdatePresetIndex > 2)
        //        UpdatePresetIndex = 0;

        //    LastUpdate = DateTime.Now;
        //}

        //var reconstructionBoxData = ReconstructionBoxManager.Instance();

        //if (reconstructionBoxData->Allowance is not 0)
        //{
        //    Data.WeeklyAllowance = TryUpdateData(Data.WeeklyAllowance, reconstructionBoxData->Allowance);
        //    Data.DonatedThisWeek = TryUpdateData(Data.DonatedThisWeek, reconstructionBoxData->Donated);
        //    Data.RemainingAllowance = TryUpdateData(Data.RemainingAllowance, Data.WeeklyAllowance - Data.DonatedThisWeek);
        //}

        base.Update();
    }

    public override void Reset()
    {
        Data.MinerPointsTotal = 0;
        Data.MinerPointsAccumulated = -1;
        Config.MinerPointsWeeklyStart = -1;
        Data.BotanistPointsTotal = 0;
        Data.BotanistPointsAccumulated = -1;
        Config.BotanistPointsWeeklyStart = -1;

        base.Reset();
    }

    protected override ModuleStatus GetModuleStatus()
    {
        if (((Data.MinerPointsAccumulated) >= Config.MinerPointsGoal) & (Data.BotanistPointsAccumulated >= Config.BotanistPointsGoal))
        {
            return ModuleStatus.Complete;
        }
        else 
        { 
            return ModuleStatus.Incomplete;
        }
    }

    protected override StatusMessage GetStatusMessage() => new()
    {
        Message = $"{Config.MinerPointsGoal-Data.MinerPointsTotal} ' Miner Points Remaining; ' {Config.BotanistPointsGoal - Data.BotanistPointsTotal} ' Botanist Points Remaining",
    };
}

//public struct DiademResourceInfo
//{
//    public readonly uint Id;
//    public readonly int Set;
//    public readonly int ScripsReward;
//    public readonly int PointsReward;
//    public readonly int Grade;

//    public DiademResourceInfo(uint id, int set, int scripsReward, int pointsReward, int grade)
//    {
//        Id = id;
//        Set = set;
//        ScripsReward = scripsReward;
//        PointsReward = pointsReward;
//        Grade = grade;
//    }
//}
//public static class DiademResources
//{
//    public static int MinPoints, BtnPoints, FshPoints;
//    public static int MinScrips, BtnScrips, FshScrips;
//    public static int grade2BTN, grade3BTN, grade4BTN = 0;
//    public static int grade2MIN, grade3MIN, grade4MIN = 0;
//    public static int grade2FSH, grade3FSH, grade4FSH = 0;
//    public static uint grade2BTNAch, grade3BTNAch, grade4BTNAch = 0;
//    public static uint grade2MINAch, grade3MINAch, grade4MINAch = 0;
//    public static uint grade2FSHAch, grade3FSHAch, grade4FSHAch = 0;
//    public static uint btn500K, min500K, fsh500K = 0;
//    public static uint currentAchievement;

//    private static readonly List<DiademResourceInfo> MinerPreset = new()
//    {
//        new DiademResourceInfo(29939, 10, 0, 3, 2), //Grade 2 Artisanal Skybuilders' Cloudstone
//        new DiademResourceInfo(29940, 10, 0, 3, 2), //Grade 2 Artisanal Skybuilders' Rock Salt
//        new DiademResourceInfo(29941, 10, 0, 3, 2), //Grade 2 Artisanal Skybuilders' Spring Water
//        new DiademResourceInfo(29942, 10, 0, 3, 2), //Grade 2 Artisanal Skybuilders' Aurum Regis Sand
//        new DiademResourceInfo(29943, 10, 0, 3, 2), //Grade 2 Artisanal Skybuilders' Jade
//        new DiademResourceInfo(29946, 10, 0, 4, 2), //Grade 2 Skybuilders' Umbral Flarestone
//        new DiademResourceInfo(29947, 10, 0, 4, 2), //Grade 2 Skybuilders' Umbral Levinshard

//        new DiademResourceInfo(31311, 5, 0, 2, 3), //Grade 3 Artisanal Skybuilders' Cloudstone
//        new DiademResourceInfo(31312, 5, 0, 2, 3), //Grade 3 Artisanal Skybuilders' Basilisk Egg
//        new DiademResourceInfo(31313, 5, 0, 2, 3), //Grade 3 Artisanal Skybuilders' Alumen
//        new DiademResourceInfo(31314, 5, 0, 2, 3), //Grade 3 Artisanal Skybuilders' Clay
//        new DiademResourceInfo(31315, 5, 0, 2, 3), //Grade 3 Artisanal Skybuilders' Granite
//        new DiademResourceInfo(31318, 5, 0, 2, 3), //Grade 3 Skybuilders' Umbral Magma Shard
//        new DiademResourceInfo(31319, 5, 0, 2, 3), //Grade 3 Skybuilders' Umbral Levinite

//        new DiademResourceInfo(32007, 5, 1, 0, 5),  //Grade 4 Skybuilders' Iron Ore
//        new DiademResourceInfo(32008, 5, 1, 0, 5),  //Grade 4 Skybuilders' Iron Sand
//        new DiademResourceInfo(32012, 5, 1, 0, 5),  //Grade 4 Skybuilders' Ore
//        new DiademResourceInfo(32013, 5, 1, 0, 5),  //Grade 4 Skybuilders' Rock Salt
//        new DiademResourceInfo(32014, 5, 1, 0, 5),  //Grade 4 Skybuilders' Mythrite Sand
//        new DiademResourceInfo(32020, 5, 2, 0, 5),  //Grade 4 Skybuilders' Electrum Ore
//        new DiademResourceInfo(32021, 5, 2, 0, 5),  //Grade 4 Skybuilders' Alumen
//        new DiademResourceInfo(32022, 5, 2, 0, 5),  //Grade 4 Skybuilders' Spring Water
//        new DiademResourceInfo(32023, 5, 2, 0, 5),  //Grade 4 Skybuilders' Gold Sand
//        new DiademResourceInfo(32024, 5, 2, 0, 5),  //Grade 4 Skybuilders' Ragstone
//        new DiademResourceInfo(32030, 5, 3, 13, 5), //Grade 4 Skybuilders' Gold Ore
//        new DiademResourceInfo(32031, 5, 3, 13, 5), //Grade 4 Skybuilders' Finest Rock Salt
//        new DiademResourceInfo(32032, 5, 3, 13, 5), //Grade 4 Skybuilders' Truespring Water
//        new DiademResourceInfo(32033, 5, 3, 13, 5), //Grade 4 Skybuilders' Mineral Sand
//        new DiademResourceInfo(32034, 5, 3, 13, 5), //Grade 4 Skybuilders' Bluespirit Ore
//        new DiademResourceInfo(32040, 5, 3, 13, 4), //Grade 4 Artisanal Skybuilders' Cloudstone
//        new DiademResourceInfo(32041, 5, 3, 13, 4), //Grade 4 Artisanal Skybuilders' Spring Water
//        new DiademResourceInfo(32042, 5, 3, 13, 4), //Grade 4 Artisanal Skybuilders' Ice Stalagmite
//        new DiademResourceInfo(32043, 5, 3, 13, 4), //Grade 4 Artisanal Skybuilders' Silex
//        new DiademResourceInfo(32044, 5, 3, 13, 4), //Grade 4 Artisanal Skybuilders' Prismstone
//        new DiademResourceInfo(32047, 5, 5, 15, 4), //Grade 4 Skybuilders' Umbral Flarerock
//        new DiademResourceInfo(32048, 5, 5, 15, 4)  //Grade 4 Skybuilders' Umbral Levinsand
//    };

//    private static readonly List<DiademResourceInfo> BotanistPreset = new()
//    {
//        new DiademResourceInfo(29934, 10, 0, 3, 2), //Grade 2 Artisanal Skybuilders' Log
//        new DiademResourceInfo(29935, 10, 0, 3, 2), //Grade 2 Artisanal Skybuilders' Hardened Sap
//        new DiademResourceInfo(29936, 10, 0, 3, 2), //Grade 2 Artisanal Skybuilders' Wheat
//        new DiademResourceInfo(29937, 10, 0, 3, 2), //Grade 2 Artisanal Skybuilders' Cotton Boll
//        new DiademResourceInfo(29938, 10, 0, 3, 2), //Grade 2 Artisanal Skybuilders' Dawn Lizard
//        new DiademResourceInfo(29944, 10, 0, 4, 2), //Grade 2 Skybuilders' Umbral Galewood Log
//        new DiademResourceInfo(29945, 10, 0, 4, 2), //Grade 2 Skybuilders' Umbral Earthcap

//        new DiademResourceInfo(31306, 5, 0, 2, 3), //Grade 3 Artisanal Skybuilders' Log
//        new DiademResourceInfo(31307, 5, 0, 2, 3), //Grade 3 Artisanal Skybuilders' Amber
//        new DiademResourceInfo(31308, 5, 0, 2, 3), //Grade 3 Artisanal Skybuilders' Cotton Boll
//        new DiademResourceInfo(31309, 5, 0, 2, 3), //Grade 3 Artisanal Skybuilders' Rice
//        new DiademResourceInfo(31310, 5, 0, 2, 3), //Grade 3 Artisanal Skybuilders' Vine
//        new DiademResourceInfo(31316, 5, 0, 2, 3), //Grade 3 Skybuilders' Umbral Galewood Sap
//        new DiademResourceInfo(31317, 5, 0, 2, 3), //Grade 3 Skybuilders' Umbral Tortoise

//        new DiademResourceInfo(32005, 5, 1, 0, 5),  //Grade 4 Skybuilders' Switch
//        new DiademResourceInfo(32006, 5, 1, 0, 5),  //Grade 4 Skybuilders' Hemp
//        new DiademResourceInfo(32009, 5, 1, 0, 5),  //Grade 4 Skybuilders' Mahogany Log
//        new DiademResourceInfo(32010, 5, 1, 0, 5),  //Grade 4 Skybuilders' Sesame
//        new DiademResourceInfo(32011, 5, 1, 0, 5),  //Grade 4 Skybuilders' Cotton Boll
//        new DiademResourceInfo(32015, 5, 2, 0, 5),  //Grade 4 Skybuilders' Spruce Log
//        new DiademResourceInfo(32016, 5, 2, 0, 5),  //Grade 4 Skybuilders' Mistletoe
//        new DiademResourceInfo(32017, 5, 2, 0, 5),  //Grade 4 Skybuilders' Toad
//        new DiademResourceInfo(32018, 5, 2, 0, 5),  //Grade 4 Skybuilders' Vine
//        new DiademResourceInfo(32019, 5, 2, 0, 5),  //Grade 4 Skybuilders' Tea Leaves
//        new DiademResourceInfo(32025, 5, 3, 13, 5), //Grade 4 Skybuilders' White Cedar Log
//        new DiademResourceInfo(32026, 5, 3, 13, 5), //Grade 4 Skybuilders' Primordial Resin
//        new DiademResourceInfo(32027, 5, 3, 13, 5), //Grade 4 Skybuilders' Wheat
//        new DiademResourceInfo(32028, 5, 3, 13, 5), //Grade 4 Skybuilders' Gossamer Cotton Boll
//        new DiademResourceInfo(32029, 5, 3, 13, 5), //Grade 4 Skybuilders' Tortoise
//        new DiademResourceInfo(32035, 5, 3, 13, 5), //Grade 4 Artisanal Skybuilders' Log
//        new DiademResourceInfo(32036, 5, 3, 13, 4), //Grade 4 Artisanal Skybuilders' Raspberry
//        new DiademResourceInfo(32037, 5, 3, 13, 4), //Grade 4 Artisanal Skybuilders' Caiman
//        new DiademResourceInfo(32038, 5, 3, 13, 4), //Grade 4 Artisanal Skybuilders' Cocoon
//        new DiademResourceInfo(32039, 5, 3, 13, 4), //Grade 4 Artisanal Skybuilders' Barbgrass
//        new DiademResourceInfo(32045, 5, 5, 15, 4), //Grade 4 Skybuilders' Umbral Galewood Branch
//        new DiademResourceInfo(32046, 5, 5, 15, 4)  //Grade 4 Skybuilders' Umbral Dirtleaf
//    };


//    private static readonly List<DiademResourceInfo> FisherPreset = new()
//    {
//        new DiademResourceInfo(30008, 1, 0, 15, 2),  //Grade 2 Artisanal Skybuilders' Pterodactyl
//        new DiademResourceInfo(30009, 1, 0, 15, 2),  //Grade 2 Artisanal Skybuilders' Skyfish
//        new DiademResourceInfo(30006, 1, 0, 158, 2), //Grade 2 Artisanal Skybuilders' Rhomaleosaurus
//        new DiademResourceInfo(30007, 1, 0, 158, 2), //Grade 2 Artisanal Skybuilders' Gobbie Mask
//        new DiademResourceInfo(30010, 1, 0, 15, 2),  //Grade 2 Artisanal Skybuilders' Cometfish
//        new DiademResourceInfo(30011, 1, 0, 15, 2),  //Grade 2 Artisanal Skybuilders' Anomalocaris
//        new DiademResourceInfo(30012, 1, 0, 15, 2),  //Grade 2 Artisanal Skybuilders' Rhamphorhynchus
//        new DiademResourceInfo(30013, 1, 0, 15, 2),  //Grade 2 Artisanal Skybuilders' Dragon's Soul

//        new DiademResourceInfo(31596, 1, 0, 12, 3),  //Grade 3 Artisanal Skybuilders' Oscar
//        new DiademResourceInfo(31597, 1, 0, 22, 3),  //Grade 3 Artisanal Skybuilders' Blind Manta
//        new DiademResourceInfo(31598, 1, 0, 83, 3),  //Grade 3 Artisanal Skybuilders' Mosasaur
//        new DiademResourceInfo(31599, 1, 0, 93, 3),  //Grade 3 Artisanal Skybuilders' Storm Chaser
//        new DiademResourceInfo(31600, 1, 0, 57, 3),  //Grade 3 Artisanal Skybuilders' Archaeopteryx
//        new DiademResourceInfo(31601, 1, 0, 90, 3),  //Grade 3 Artisanal Skybuilders' Wyvern
//        new DiademResourceInfo(31602, 1, 0, 77, 3),  //Grade 3 Artisanal Skybuilders' Cloudshark
//        new DiademResourceInfo(31603, 1, 0, 113, 3), //Grade 3 Artisanal Skybuilders' Helicoprion

//        new DiademResourceInfo(32882, 1, 2, 0, 5),     //Grade 4 Skybuilders' Zagas Khaal
//        new DiademResourceInfo(32883, 1, 2, 0, 5),     //Grade 4 Skybuilders' Goldsmith Crab
//        new DiademResourceInfo(32884, 1, 4, 0, 5),     //Grade 4 Skybuilders' Common Bitterling
//        new DiademResourceInfo(32885, 1, 4, 0, 5),     //Grade 4 Skybuilders' Skyloach
//        new DiademResourceInfo(32886, 1, 4, 0, 5),     //Grade 4 Skybuilders' Glacier Core
//        new DiademResourceInfo(32887, 1, 4, 0, 5),     //Grade 4 Skybuilders' Kissing Fish
//        new DiademResourceInfo(32888, 1, 8, 0, 5),     //Grade 4 Skybuilders' Cavalry Catfish
//        new DiademResourceInfo(32889, 1, 8, 0, 5),     //Grade 4 Skybuilders' Manasail
//        new DiademResourceInfo(32890, 1, 4, 0, 5),     //Grade 4 Skybuilders' Starflower
//        new DiademResourceInfo(32891, 1, 4, 0, 5),     //Grade 4 Skybuilders' Cyan Crab
//        new DiademResourceInfo(32892, 1, 10, 0, 5),    //Grade 4 Skybuilders' Fickle Krait
//        new DiademResourceInfo(32893, 1, 10, 0, 5),    //Grade 4 Skybuilders' Proto-hropken
//        new DiademResourceInfo(32894, 1, 3, 2, 5),     //Grade 4 Skybuilders' Ghost Faerie
//        new DiademResourceInfo(32895, 1, 5, 5, 5),     //Grade 4 Skybuilders' Ashfish
//        new DiademResourceInfo(32896, 1, 10, 8, 5),    //Grade 4 Skybuilders' Whitehorse
//        new DiademResourceInfo(32897, 1, 6, 4, 5),     //Grade 4 Skybuilders' Ocean Cloud
//        new DiademResourceInfo(32898, 1, 12, 10, 5),   //Grade 4 Skybuilders' Black Fanfish
//        new DiademResourceInfo(32899, 1, 12, 10, 5),   //Grade 4 Skybuilders' Sunfish
//        new DiademResourceInfo(32900, 1, 17, 106, 4),  //Grade 4 Artisanal Skybuilders' Sweatfish
//        new DiademResourceInfo(32901, 1, 17, 250, 4),  //Grade 4 Artisanal Skybuilders' Sculptor
//        new DiademResourceInfo(32902, 1, 124, 911, 4), //Grade 4 Artisanal Skybuilders' Little Thalaos
//        new DiademResourceInfo(32903, 1, 64, 996, 4),  //Grade 4 Artisanal Skybuilders' Lightning Chaser
//        new DiademResourceInfo(32904, 1, 77, 512, 4),  //Grade 4 Artisanal Skybuilders' Marrella
//        new DiademResourceInfo(32905, 1, 45, 542, 4),  //Grade 4 Artisanal Skybuilders' Crimson Namitaro
//        new DiademResourceInfo(32906, 1, 153, 982, 4), //Grade 4 Artisanal Skybuilders' Griffin
//        new DiademResourceInfo(32907, 1, 126, 1078, 4) //Grade 4 Artisanal Skybuilders' Meganeura
//    };

//    public static unsafe void CalculatePoints(int presetToUse)
//    {
//        var uiState = UIState.Instance();
//        var manager = InventoryManager.Instance();
//        if (manager == null)
//            return;

//        var preset = presetToUse switch
//        {
//            0 => MinerPreset,
//            1 => BotanistPreset,
//            2 => FisherPreset,
//            _ => new List<DiademResourceInfo>()
//        };

//        var (points, scrips) = (0, 0);
//        int[] grades = [0, 0, 0, 0];
//        foreach (var item in preset)
//        {
//            var count = manager->GetInventoryItemCount(item.Id, false, false, false);
//            points += (count / item.Set) * item.PointsReward;
//            scrips += (count / item.Set) * item.ScripsReward;
//            grades[item.Grade - 2] = grades[item.Grade - 2] + count - (count % item.Set);
//        }

//        _ = presetToUse switch
//        {
//            0 => (MinPoints, MinScrips, grade2MIN, grade3MIN, grade4MIN) = (points, scrips, grades[0], grades[1], grades[2]),
//            1 => (BtnPoints, BtnScrips, grade2BTN, grade3BTN, grade4BTN) = (points, scrips, grades[0], grades[1], grades[2]),
//            2 => (FshPoints, FshScrips, grade2FSH, grade3FSH, grade4FSH) = (points, scrips, grades[0], grades[1], grades[2]),
//            _ => throw new NotImplementedException()
//        };
//    }
//}
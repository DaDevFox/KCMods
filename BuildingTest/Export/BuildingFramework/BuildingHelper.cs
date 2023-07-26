
using Harmony;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using UnityEngine;
using Assets;
using Assets.Code;


public class BuildingHelper
{
    public static KCModHelper helper;
    public static List<BuildingInfo> buildingsToRegister = new List<BuildingInfo>();

    public void Preload(KCModHelper _helper)
    {
        helper = _helper;
    }

    public static void RegisterBuilding(BuildingInfo info)
    {
        Building b = info.building;

        if (b == null)
        {
            b = info.buildingPrefab.AddComponent<Building>();
            info.building = b;
        }

        b.UniqueName = info.uniqueName;
        b.DisplayModel = info.displayModel;
        b.dragPlacementMode = info.dragPlacementMode;
        b.customName = info.customName;
        typeof(Building).GetField("descOverride", BindingFlags.NonPublic | BindingFlags.Instance).SetValue(b, info.descOverride);
        typeof(Building).GetField("Cost", BindingFlags.NonPublic | BindingFlags.Instance).SetValue(b, info.buildingCost);
        b.personPositions = info.personPositions;
        b.Yield = info.yield;
        b.YieldInterval = info.yieldInterval;
        b.WorkersForFullYield = info.workersForFullYield;
        b.YieldIntervalMod = info.yieldIntervalMod;
        b.BuildAllowedWorkers = info.buildAllowedWorkers;
        b.JobCategory = info.jobCategory;
        b.CategoryName = info.categoryName;
        b.skillUsed = info.skillUsed;
        b.SubStructure = info.subStructure;
        b.Stackable = info.stackable;
        b.StackHeight = info.stackHeight;
        b.ignoreRoadCoverageForPlacement = info.ignoreRoadCoverageForPlacement;
        b.showHappinessOverlay = info.showHappinessOverlay;
        b.doBuildAnimation = info.doBuildAnimation;
        b.fireChance = info.fireChance;
        b.BuildersRequiredOnLocation = info.buildersRequiredOnLocation;
        b.allowOverAndUnderAqueducts = info.allowOverAndUnderAqueducts;
        b.MaxLife = info.maxLife;
        b.placementSounds = info.placementSounds;
        b.SelectionSounds = info.selectionSounds;
        typeof(Building).GetField("buildSound", BindingFlags.NonPublic | BindingFlags.Instance).SetValue(b, info.buildSound);
        typeof(Building).GetField("buildEffects", BindingFlags.NonPublic | BindingFlags.Instance).SetValue(b, info.buildEffects);
        b.size = info.buildingSize;

        BuildingHelper.buildingsToRegister.Add(info);
    }

    public static object[] getTabByName(String tab)
    {
        object[] tabs = new object[2];
        switch (tab)
        {
            case "Castle":
                tabs[0] = BuildUI.inst.CastleTab;
                tabs[1] = BuildUI.inst.CastleTabVR;
                return tabs;
            case "Town":
                tabs[0] = BuildUI.inst.TownTab;
                tabs[1] = BuildUI.inst.TownTabVR;
                return tabs;
            case "AdvTown":
                tabs[0] = BuildUI.inst.AdvTownTab;
                tabs[1] = BuildUI.inst.AdvTownTabVR;
                return tabs;
            case "Industry":
                tabs[0] = BuildUI.inst.IndustryTab;
                tabs[1] = BuildUI.inst.IndustryTabVR;
                return tabs;
            case "Maritime":
                tabs[0] = BuildUI.inst.MaritimeTab;
                tabs[1] = BuildUI.inst.MaritimeTabVR;
                return tabs;
            case "Cemetery":
                tabs[0] = BuildUI.inst.CemeteryTab;
                tabs[1] = BuildUI.inst.CemeteryTabVR;
                return tabs;
        }
        tabs[0] = BuildUI.inst.CastleTab;
        tabs[1] = BuildUI.inst.CastleTabVR;
        return tabs;
    }



    [HarmonyPatch(typeof(GameState))]
    [HarmonyPatch("Start")]
    public static class BuildingHelperInternalPrefabsPatch
    {
        static void Postfix(GameState __instance)
        {
            try
            {
                List<Building> internalPrefabs = __instance.internalPrefabs;
                foreach (BuildingInfo info in BuildingHelper.buildingsToRegister)
                {
                    internalPrefabs.Add(info.building);
                }
            }
            catch (Exception err)
            {
                helper.Log(err.ToString());
            }
        }
    }


    [HarmonyPatch(typeof(BuildUI))]
    [HarmonyPatch("Start")]
    public static class BuildingHelperBuildUIPatch
    {
        static void Postfix(BuildUI __instance)
        {
            try
            {
                // Custom Building
                foreach (BuildingInfo info in BuildingHelper.buildingsToRegister)
                {
                    object[] tabs = BuildingHelper.getTabByName(info.tabCategory);
                    BuildUI.inst.AddBuilding((BuildTab)(tabs[0]), (BuildTabVR)(tabs[1]), info.uniqueName, info.preqBuilding, info.buildingButtonSize);
                }

            }
            catch (Exception err)
            {
                helper.Log(err.ToString());
            }
        }
    }


}
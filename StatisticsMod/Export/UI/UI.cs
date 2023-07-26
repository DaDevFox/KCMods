using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using KCModUtils.AssetManagement;

namespace StatisticsMod.UI
{
    public static class UI
    {
        public static GameObject statsWindowPrefab;
        public static GameObject statsWindow;
        public static StatsWindow statsWindowScript;


        public static void Init()
        {
            statsWindowPrefab = AssetBundleManager.GetAsset("JSS_StatsView.prefab") as GameObject;
            statsWindow = GameObject.Instantiate(statsWindowPrefab, GameState.inst.playingMode.GameUIParent.transform);
            statsWindowScript = statsWindow.AddComponent<StatsWindow>();

        }








    }
}

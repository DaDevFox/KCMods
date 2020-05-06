using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace InsaneDifficultyMod
{
    static class UI
    {

        #region UI GameObjects

        public static GameObject modSettingsUIObj;
        public static ModSettingsUI modSettingsUI;
        
        public static GameObject riotDemandsUIObj;
        public static RiotDemandsUI riotDemandsUI;


        #endregion

        public static void Setup() 
        {
            //ModSettingsUI
            GameObject modSettingsPrefab = AssetBundleManager.GetAsset("ModSettings.prefab") as GameObject;
            modSettingsUIObj = GameObject.Instantiate(modSettingsPrefab);
            modSettingsUIObj.transform.SetParent(GameState.inst.mainMenuMode.mainMenuUI.transform, false);
            modSettingsUI = modSettingsUIObj.AddComponent<ModSettingsUI>();

            //RiotDemandsUI
            GameObject riotDemandsPrefab = AssetBundleManager.GetAsset("RiotDemandUI.prefab") as GameObject;
            riotDemandsUIObj = GameObject.Instantiate(riotDemandsPrefab);
            riotDemandsUIObj.transform.SetParent(GameState.inst.transform.Find("KeyboardMouseUICanvas"));
            riotDemandsUI = riotDemandsUIObj.AddComponent<RiotDemandsUI>();

        }



    }
}

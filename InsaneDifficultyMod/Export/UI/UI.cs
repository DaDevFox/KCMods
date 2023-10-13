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
        public static Events.RiotDemandsUI riotDemandsUI;

        public static GameObject riotUIObj;
        public static RiotUI riotUI;


        #endregion

        public static void Setup() 
        {
            GameObject riotUIPrefab = Mod.assets.GetByName<GameObject>("RiotViewer");
            riotUIObj = GameObject.Instantiate(riotUIPrefab, GameState.inst.playingMode.GameUIParent.transform);
            riotUI = riotUIObj.AddComponent<RiotUI>();
            //((RectTransform)riotUIObj.transform).

            ////ModSettingsUI
            //GameObject modSettingsPrefab = Mod.legacyAssets.GetByName<GameObject>("ModSettings");
            //modSettingsUIObj = GameObject.Instantiate(modSettingsPrefab);
            //modSettingsUIObj.transform.SetParent(GameState.inst.mainMenuMode.mainMenuUI.transform, false);
            //modSettingsUI = modSettingsUIObj.AddComponent<ModSettingsUI>();

            ////RiotDemandsUI
            //GameObject riotDemandsPrefab = Mod.legacyAssets.GetByName<GameObject>("RiotDemandUI");
            //riotDemandsUIObj = GameObject.Instantiate(riotDemandsPrefab);
            //riotDemandsUIObj.transform.SetParent(GameState.inst.transform.Find("KeyboardMouseUICanvas"));
            //riotDemandsUI = riotDemandsUIObj.AddComponent<RiotDemandsUI>();

        }



    }
}

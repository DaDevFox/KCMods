using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using InsaneDifficultyMod.Events;

namespace InsaneDifficultyMod
{
    public class RiotUI : MonoBehaviour
    {
        #region References

        // Containers
        public GameObject PreactiveTimerContainer;
        public GameObject ActiveTimerContainer;
        public GameObject DescriptionUIContainer;

        // Interactives
        public Slider PreactiveProgress;
        public Slider ActiveProgress;

        public TextMeshProUGUI YearsText;
        public TextMeshProUGUI ActiveRiotText;
        public Button ViewButton;

        // Description UI
        public TextMeshProUGUI BuildingsDescription;
        public TextMeshProUGUI DemandsDescription;
        public GameObject[] DemandsFields = new GameObject[11];
        public Button SendButton;

        #endregion

        public int target = -1;
        private bool ShowDescriptionUI = false;



        void Awake()
        {
            // Containers
            PreactiveTimerContainer = transform.Find("PreActive").gameObject;
            ActiveTimerContainer = transform.Find("Active").gameObject;
            DescriptionUIContainer = transform.Find("RiotUI").gameObject;

            Mod.dLog(1);

            // Interactives
            PreactiveProgress = transform.Find("PreActive").Find("Slider").GetComponent<Slider>();
            ActiveProgress = transform.Find("Active").Find("Slider").GetComponent<Slider>();

            Mod.dLog(2);

            YearsText = transform.Find("PreActive").Find("Text (2)").GetComponent<TextMeshProUGUI>();
            ActiveRiotText = transform.Find("Active").Find("Text").GetComponent<TextMeshProUGUI>();
            ViewButton = transform.Find("Active").Find("Button").GetComponent<Button>();

            Mod.dLog(3);

            // Description UI
            BuildingsDescription = transform.Find("RiotUI").Find("Buildings").GetComponent<TextMeshProUGUI>();
            DemandsDescription = transform.Find("RiotUI").Find("DemandsText").GetComponent<TextMeshProUGUI>();

            // TODO: sort out riot ui, brewing (preactive), assembling (active) and marching states

            Mod.dLog(4);

            string[] demandsFieldNames = { "gold", "wheat", "apples", "pork", "fish", "wood", "stone", "iron", "charcoal", "tools", "armaments" };
            for(int i = 0; i < demandsFieldNames.Length; i++)
            {
                DemandsFields[i] = transform.Find("RiotUI").Find("Demands").Find(demandsFieldNames[i]).gameObject;
                DemandsFields[i].SetActive(false);
            }

            SendButton = transform.Find("RiotUI").Find("SendButton").GetComponent<Button>();
        }

        void Update()
        {
            Riot riot = RiotSystem.GetRiot(Player.inst.FocusedLandMass);
            if (riot != null)
            {
                PreactiveTimerContainer.SetActive(false);
                ActiveTimerContainer.SetActive(true);

                //DebugExt.Log("test");

                if (riot.stage == Riot.Stage.Collecting)
                {
                    ActiveRiotText.text = "RIOT ASSEMBLING!";
                }
                else
                {
                    ActiveRiotText.text = "ACTIVE RIOT!";
                }

                ActiveProgress.value = Mathf.Clamp((float)riot.count / (float)RiotSystem.popForMarch, 0f, 1f);
            }
            else
            {
                PreactiveTimerContainer.SetActive(true);
                ActiveTimerContainer.SetActive(false);

                PreactiveProgress.value = RiotSystem.averageDissatisfaction;
            }
        }
    }
}

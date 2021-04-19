using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using Assets.Code.UI;
using TMPro;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Fox.Maps.Utils;

namespace Fox.Maps
{
    public class MapSaveUI : MonoBehaviour
    {
        public TextMeshProUGUI mapName;
        private Button saveButton;
        private Button openRegistryButton;
        private Button pasteSaveButton;
        private Confirmation saveConfirmation;
        public Confirmation deleteConfirmation;

        void Start()
        {
            mapName = transform.Find("EditPanel/MapName/Text Area/Text").GetComponent<TextMeshProUGUI>();
            saveButton = transform.Find("EditPanel/Buttons/SaveButton").GetComponent<Button>();
            openRegistryButton = transform.Find("EditPanel/Buttons/Button").GetComponent<Button>();
            pasteSaveButton = transform.Find("EditPanel/Buttons/PasteButton").GetComponent<Button>();
            saveConfirmation = transform.Find("SaveConfirmation").gameObject.AddComponent<Confirmation>();
            deleteConfirmation = transform.Find("DeleteConfirmation").gameObject.AddComponent<Confirmation>();

            saveConfirmation.yesButton = transform.Find("SaveConfirmation/Buttons/Confirm").GetComponent<Button>();
            saveConfirmation.noButton = transform.Find("SaveConfirmation/Buttons/Abort").GetComponent<Button>();

            deleteConfirmation.yesButton = transform.Find("DeleteConfirmation/Buttons/Confirm").GetComponent<Button>();
            deleteConfirmation.noButton = transform.Find("DeleteConfirmation/Buttons/Abort").GetComponent<Button>();

            mapName.alignment = TextAlignmentOptions.MidlineLeft;


            pasteSaveButton.onClick.AddListener(() =>
            {
                try
                {
                    string text = UI.CodeToJson(GUIUtility.systemCopyBuffer);

                    MapSaveLoad.MapSaveData data = MapSaveLoad.CreateFromJson((JObject)JsonConvert.DeserializeObject(text));
                    if (data == null)
                        throw new Exception("data not created");

                    Mod.Log(data == null);
                    MapSaveLoad.LoadMap(data);
                }
                catch (Exception ex)
                {
                    // Log Error loading save
                    Mod.Log("Error loading save: \n" + ex.ToString());
                }

            });

            saveButton.onClick.AddListener(() =>
            {
                if (MapSaveLoad.Contains(Mod.EditingMapName))
                {
                    saveConfirmation.ShowConfirmation(Append, () => { });
                }
                else
                    Append();
            });
            openRegistryButton.onClick.AddListener(UI.MapRegistry.Toggle);
        }

        private void Append()
        {
            MapSaveLoad.Compile();
            MapSaveLoad.editing.name = Mod.EditingMapName;
            MapSaveLoad.Append();
            MapRegistryItem ui = UI.MapRegistry.GetByName(MapSaveLoad.editing.name);
            if (ui)
                ui.UpdateInformation();
        }

        void Update()
        {
            Mod.EditingMapName = mapName.text;
            if (mapName.text != "")
                saveButton.interactable = true;
            else
                saveButton.interactable = false;
        }

        // reset name with map cycle
    }

}

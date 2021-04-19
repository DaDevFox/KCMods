using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Assets.Code;
using Fox.Maps.Utils;
using System.Reflection;

namespace Fox.Maps
{

    public class MapRegistry : MonoBehaviour
    {
        private Dictionary<string, MapRegistryItem> items = new Dictionary<string, MapRegistryItem>();

        public void Toggle()
        {
            gameObject.SetActive(!gameObject.activeSelf);
        }

        void Update()
        {
            foreach (MapSaveLoad.MapSaveData map in MapSaveLoad.registry)
                if (!items.ContainsKey(map.name))
                    items.Add(map.name, CreateMapItem(map));

            List<string> toRemove = new List<string>();

            foreach (string mapName in items.Keys)
            {
                bool contains = false;
                foreach (MapSaveLoad.MapSaveData map in MapSaveLoad.registry)
                    if (map.name == mapName)
                        contains = true;

                if (!contains)
                    toRemove.Add(mapName);
            }

            foreach (string name in toRemove)
            {
                GameObject.Destroy(items[name].gameObject);
                items.Remove(name);
            }
        }

        public MapRegistryItem GetByName(string name) => items.ContainsKey(name) ? items[name] : null;

        private MapRegistryItem CreateMapItem(MapSaveLoad.MapSaveData data)
        {
            MapRegistryItem item = GameObject.Instantiate(UI.MapRegistryItemPrefab, transform.Find("Scroll View/Viewport/Content")).AddComponent<MapRegistryItem>();
            item.data = data;

            return item;
        }
    }

    public class MapRegistryItem : MonoBehaviour
    {
        public static Color noneB;
        public static Color noneF;
        public static Color noneV;

        public static Color treeB;
        public static Color treeF;
        public static Color treeV;

        public static Color unusablestoneB;
        public static Color unusablestoneF;
        public static Color unusablestoneV;

        public static Color stoneB;
        public static Color stoneF;
        public static Color stoneV;

        public static Color irondepositB;
        public static Color irondepositF;
        public static Color irondepositV;

        public static Color witchhut;
        public static Color cave;
        public static Color waterDeep;
        public static Color waterShallow;
        public static Color waterFish;



        public MapSaveLoad.MapSaveData data;

        private RawImage preview;
        private TextMeshProUGUI nameText;
        private TextMeshProUGUI dimensionsText;
        private TextMeshProUGUI treesText;
        private TextMeshProUGUI rocksText;
        private TextMeshProUGUI stonesText;
        private TextMeshProUGUI ironsText;
        private Button copyButton;
        private Button deleteButton;

        void Start()
        {
            preview = transform.Find("Previe").GetComponent<RawImage>();
            nameText = transform.Find("NameText").GetComponent<TextMeshProUGUI>();
            dimensionsText = transform.Find("InfoPanel/Text (TMP)").GetComponent<TextMeshProUGUI>();
            treesText = transform.Find("InfoPanel/InfosContainer/Trees/Text (TMP)").GetComponent<TextMeshProUGUI>();
            rocksText = transform.Find("InfoPanel/InfosContainer/Rocks/Text (TMP)").GetComponent<TextMeshProUGUI>();
            stonesText = transform.Find("InfoPanel/InfosContainer/Stones/Text (TMP)").GetComponent<TextMeshProUGUI>();
            ironsText = transform.Find("InfoPanel/InfosContainer/Irons/Text (TMP)").GetComponent<TextMeshProUGUI>();
            copyButton = transform.Find("CopyButton").GetComponent<Button>();
            deleteButton = transform.Find("InfoPanel/DeleteButton").GetComponent<Button>();

            nameText.Left();
            dimensionsText.Center();
            treesText.Left();
            rocksText.Left();
            stonesText.Left();
            ironsText.Left();
            copyButton.transform.GetChild(0).gameObject.Center();


            UpdateInformation();

            preview.GetComponent<Button>().onClick.AddListener(() =>
            {
                MapSaveLoad.LoadMap(data);
                UI.MapSaveUI.mapName.text = data.name;
            });

            copyButton.onClick.AddListener(() =>
            {
                try
                {
                    UI.JsonToCode(JsonConvert.SerializeObject(data)).CopyToClipboard();
                }
                catch (Exception ex)
                {
                    Mod.Log(ex);
                }
            });

            deleteButton.onClick.AddListener(() =>
            {
                UI.MapSaveUI.deleteConfirmation.ShowConfirmation(() =>
                {
                    MapSaveLoad.registry.Remove(data);
                    MapSaveLoad.SaveRegistry();
                }, () => { });
            });
        }

        public void UpdateInformation()
        {
            Analyze(out int trees, out int rocks, out int stones, out int irons);

            Texture2D texture = Preview(data);
            preview.texture = texture;

            nameText.text = data.name;
            dimensionsText.text = $"{data.terrainData.gridWidth} x {data.terrainData.gridHeight}";
            treesText.text = trees.ToString();
            rocksText.text = rocks.ToString();
            stonesText.text = stones.ToString();
            ironsText.text = irons.ToString();
        }


        private void Analyze(out int treeAmount, out int rockAmount, out int stoneAmount, out int ironAmount)
        {
            treeAmount = 0;
            rockAmount = 0;
            stoneAmount = 0;
            ironAmount = 0;
            for (int i = 0; i < data.terrainData.cellSaveData.Length; i++)
            {
                Cell.CellSaveData cell = data.terrainData.cellSaveData[i];

                treeAmount += cell.amount;
                rockAmount += cell.type == ResourceType.UnusableStone ? 1 : 0;
                stoneAmount += cell.type == ResourceType.Stone ? 1 : 0;
                ironAmount += cell.type == ResourceType.IronDeposit ? 1 : 0;
            }
        }

        public Texture2D Preview(MapSaveLoad.MapSaveData data)
        {
            Texture2D texture = new Texture2D(data.terrainData.gridWidth, data.terrainData.gridHeight);

            for (int i = 0; i < data.terrainData.cellSaveData.Length; i++)
            {
                int x = i % data.terrainData.gridWidth;
                int z = i / data.terrainData.gridWidth;

                Cell.CellSaveData cell = data.terrainData.cellSaveData[i];

                Color color = Color.blue;

                Color water = Water.inst.waterMat.GetColor("_Color");
                Color deepWater = Water.inst.waterMat.GetColor("_DeepColor");
                Color saltWater = Water.inst.waterMat.GetColor("_SaltColor");
                Color saltDeepWater = Water.inst.waterMat.GetColor("_SaltDeepColor");

                if (cell.type == ResourceType.Water)
                {
                    color = water;

                    if (cell.deepWater)
                        color = deepWater;

                    if (cell.saltWater)
                        color = saltWater;

                    if (cell.deepWater && cell.saltWater)
                        color = saltDeepWater;


                    if (data.fishData.fishPerCell[i] > 0)
                        color = waterFish;
                }

                else if (cell.type == ResourceType.WitchHut)
                    color = new Color(0.5490196f, 0.3490196f, 0.01960784f);

                else if (cell.type == ResourceType.EmptyCave || cell.type == ResourceType.WolfDen)
                    color = cave;
                else
                {
                    color = (Color)typeof(MapRegistry).GetField(cell.type.ToString().ToLower() + (cell.fertile == 0 ? "B" : (cell.fertile == 1 ? "F" : "V")), BindingFlags.Static | BindingFlags.Public).GetValue(this);
                }


                //if (cell.type == ResourceType.None)
                //    color = cell.fertile == 0 ? TerrainGen.inst.barrenColor : (cell.fertile == 1 ? TerrainGen.inst.tileColor : TerrainGen.inst.fertileColor);


                //if (cell.type == ResourceType.UnusableStone)
                //    color = Color.black;

                //if (cell.type == ResourceType.Stone)
                //    color = Color.grey;

                //if (cell.type == ResourceType.IronDeposit)
                //    color = new Color(0.6f, 0.4039216f, 0.3647059f);

                //if (cell.type == ResourceType.Wood)
                //    color = new Color(0.00f, 0.30f, 0.00f);

                texture.SetPixel(x, z, color);

            }

            texture.Apply();
            return texture;
        }

    }
}

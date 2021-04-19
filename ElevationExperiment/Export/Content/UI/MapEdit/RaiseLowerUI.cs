using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Harmony;
using DG.Tweening;
using System.Reflection;

namespace Elevation
{
    public class RaiseLowerUI : MonoBehaviour
    {
        public static RaiseLowerUI inst;

        public enum Mode
        {
            None,
            Raise,
            Lower
        }
        public static Mode mode { get; private set; } 

        public Button downButton;
        public Button upButton;

        /// <summary>
        /// BrushModes in which elevation will be deleted on target cells
        /// </summary>
        public static List<MapEdit.BrushMode> deletionModes = new List<MapEdit.BrushMode>()
        {
            MapEdit.BrushMode.DeepWater,
            MapEdit.BrushMode.ShallowWater,
            MapEdit.BrushMode.Fish
        };

        public static float spamRateNormal { get; } = 1f;
        public static float spamRateContiguous { get; } = 0.05f;
        public static float spamRateTransition { get; } = 10f;

        #region UI

        private static float animationTime = 0.3f;

        internal void AnimateButtonActive(GameObject button)
        {
            DOTween.To(
                (value) => button.transform.Find("BG").GetComponent<Image>().fillAmount = value,
                button.transform.Find("BG").GetComponent<Image>().fillAmount,
                1, animationTime);
        }

        internal void AnimateButtonInactive(GameObject button)
        {
            DOTween.To(
                (value) => button.transform.Find("BG").GetComponent<Image>().fillAmount = value,
                button.transform.Find("BG").GetComponent<Image>().fillAmount,
                0f, animationTime);
        }

        void Start()
        {
            AnimateButtonInactive(downButton.gameObject);
            AnimateButtonInactive(upButton.gameObject);
        }

        #endregion

        #region Functionality



        void Awake()
        {
            Mod.dLog("Created Raise/Lower UI");

            inst = this;

            downButton = transform.Find("DownButton").GetComponent<Button>();
            upButton = transform.Find("UpButton").GetComponent<Button>();

            downButton.onClick.AddListener(OnDownButtonClick);
            upButton.onClick.AddListener(OnUpButtonClick);


            Mod.dLog("Setup Raise/Lower UI");
        }

        [HarmonyPatch(typeof(MapEdit), "ApplyBrush")]
        class ApplyBrushPatch
        {
            private static bool stoneCell = false;

            static void Prefix(Cell c) => stoneCell =
                c.Type == ResourceType.Stone ||
                c.Type == ResourceType.UnusableStone ||
                c.Type == ResourceType.IronDeposit;

            static void Postfix(MapEdit.BrushMode brush, Cell c)
            {
                if(mode != Mode.None)
                {
                    CellMeta meta = Grid.Cells.Get(c);
                    if (meta)
                    {
                        meta.elevationTier += mode == Mode.Lower ? -1 : 1;
                        meta.elevationTier = Mathf.Clamp(meta.elevationTier, ElevationManager.minElevation, ElevationManager.maxElevation);
                    }
                    ElevationManager.RefreshTile(c);
                }
                if (deletionModes.Contains(brush))
                {
                    CellMeta meta = Grid.Cells.Get(c);
                    if (meta)
                        meta.elevationTier = 0;
                    ElevationManager.RefreshTile(c);
                }
                if(stoneCell)
                    World.inst.CombineStone();
            }
        }

        void OnDownButtonClick()
        {
            // Brush
            mode = Mode.Lower;
            MapEdit menu = GameState.inst.mainMenuMode.mapEditUI.GetComponent<MapEdit>();

            menu.brushMode = MapEdit.BrushMode.BarrenLand;
            typeof(MapEdit).GetMethod("RefreshButtonImages", BindingFlags.NonPublic | BindingFlags.Instance).Invoke(menu, new object[0]);

            // Animation
            AnimateButtonActive(downButton.gameObject);
            AnimateButtonInactive(upButton.gameObject);
        }

        void OnUpButtonClick()
        {
            // Brush
            mode = Mode.Raise;

            MapEdit menu = GameState.inst.mainMenuMode.mapEditUI.GetComponent<MapEdit>();

            menu.brushMode = MapEdit.BrushMode.BarrenLand;
            typeof(MapEdit).GetMethod("RefreshButtonImages", BindingFlags.NonPublic | BindingFlags.Instance).Invoke(menu, new object[0]);

            // Animation
            AnimateButtonActive(upButton.gameObject);
            AnimateButtonInactive(downButton.gameObject);
        }

        [HarmonyPatch(typeof(MapEdit), "SetBrushMode")]
        class ResetBrushPatch
        {
            static void Postfix()
            {
                mode = Mode.None;

                // Reset Buttons
                RaiseLowerUI.inst.AnimateButtonInactive(RaiseLowerUI.inst.downButton.gameObject);
                RaiseLowerUI.inst.AnimateButtonInactive(RaiseLowerUI.inst.upButton.gameObject);
            }
        }


        [HarmonyPatch(typeof(MapEdit), "ClearMap")]
        class ClearMapPatch
        {
            static void Postfix()
            {
                ElevationManager.RefreshTerrain(true);
            }
        }

        [HarmonyPatch(typeof(MapEdit), "Update")]
        class MultiClickPatch
        {
            private static float time = 0f;
            private static float spamRate = spamRateNormal;

            // This patch modifies the behaviour of the MapEdit cursor only when using one of the Elevation brushes
            static void Prefix(MapEdit __instance)
            {
                if (InputManager.Primary())
                    spamRate = Mathf.SmoothStep(spamRate, spamRateContiguous, spamRateTransition * Time.deltaTime);
                else
                    spamRate = spamRateNormal;

                if (InputManager.PrimaryDown())
                    time = spamRate + 1f;

                if (time > spamRate)
                {
                    // Normally when using a brush, it checks if you're clicking on the same tile repeatedly and doesn't register clicks if you just spam on one tile
                    // The reason for this is that spamming a brush, I.E. barren on an already barren tile won't visibly change anything so there's no need to waste processing on it
                    // With elevation however, spamming a tile can raise/lower it more, so we're just throwing off the way they check tile spamming by saying no tile was clicked a second ago, only if
                    // One of the Elevation brushes are active
                    if (mode != Mode.None)
                        typeof(MapEdit).GetField("lastCell", BindingFlags.Instance | BindingFlags.NonPublic).SetValue(__instance, null);

                    time = 0f;
                }

                time += Time.deltaTime;
            }
        }

        [HarmonyPatch(typeof(MapEdit), "Update")]
        class CenterCellCorrectionPatch
        {
            private static bool clickThisFrame = false;

            static void Prefix(MapEdit __instance)
            {
                Ray ray = PointingSystem.GetPointer().GetRay();
                Plane plane = new Plane(new Vector3(0f, 1f, 0f), new Vector3(0f, 0f, 0f));
                float distance;
                plane.Raycast(ray, out distance);
                Vector3 point = ray.GetPoint(distance);
                Cell hit = World.inst.GetCellData(point);

                Cell last = typeof(MapEdit).GetField("lastCell", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(__instance) as Cell;

                clickThisFrame = last != hit && hit != null;
                clickThisFrame &= InputManager.Primary();
            }

            static void Postfix(MapEdit __instance)
            {
                // If not on a frame where a click has happened, don't execute
                if (!clickThisFrame)
                    return;

                //if ((float)typeof(MapEdit)
                //    .GetField("radius", BindingFlags.Instance | BindingFlags.NonPublic)
                //    .GetValue(__instance) <= 1f)
                //    return;

                // Corrects an issue where, with a brush size greater than 1, the center cell gets applied an elevation increase twice
                if (mode != Mode.None)
                {
                    Ray ray = PointingSystem.GetPointer().GetRay();
                    Plane plane = new Plane(new Vector3(0f, 1f, 0f), new Vector3(0f, 0f, 0f));
                    float distance;
                    plane.Raycast(ray, out distance);
                    Vector3 point = ray.GetPoint(distance);
                    Cell hit = World.inst.GetCellData(point);

                    CellMeta meta = Grid.Cells.Get(hit);
                    if (meta)
                    {
                        // If hit either of the extremes, no change will be created, and no correction will be neccessary
                        if (meta.elevationTier == ElevationManager.minElevation || meta.elevationTier == ElevationManager.maxElevation)
                            return;

                        meta.elevationTier += mode == Mode.Raise ? -1 : 1;
                        meta.elevationTier = Mathf.Clamp(meta.elevationTier, ElevationManager.minElevation, ElevationManager.maxElevation);
                    }
                    ElevationManager.RefreshTile(hit);
                }
            }
        }

        #endregion
    }
}

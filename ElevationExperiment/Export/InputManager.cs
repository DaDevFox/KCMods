using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Harmony;

namespace ElevationExperiment
{
    class InputManager : MonoBehaviour
    {

        

        public static void Update()
        {
            Cell selected = GameUI.inst.GetCellSelected();

            if (Settings.debug)
            {

                if (Input.GetKeyDown(Settings.keycode_raise))
                {
                    try
                    {
                        if (ElevationManager.TryProcessElevationChange(selected, 1))
                        {
                            DebugExt.dLog("Elevation raise succesful");
                        }
                    }
                    catch (Exception ex)
                    {
                        DebugExt.HandleException(ex);
                    }
                }
                else if (Input.GetKeyDown(Settings.keycode_lower))
                {
                    try
                    {
                        if (ElevationManager.TryProcessElevationChange(selected, -1))
                        {
                            DebugExt.dLog("Elevation lower succesful");
                        }
                    }
                    catch (Exception ex)
                    {
                        DebugExt.HandleException(ex);
                    }
                }

                

                if (Input.GetKeyDown(Settings.keycode_sampleCell))
                {
                    string text = "";

                    text += "Cell at " + selected.Center.ToString() + ": ";
                    text += Environment.NewLine;
                    text += "has mark: " + (ElevationManager.GetCellMark(selected) != null).ToString();
                    text += Environment.NewLine;
                    text += (ElevationManager.GetCellMark(selected) != null) ? ("Blockers: " + ElevationManager.GetCellMark(selected).blockers.Count.ToString()) +
                        Environment.NewLine : "";
                    text += BlockedTilePruner.GetTileRegion(selected) != -1 ? BlockedTilePruner.GetTileRegion(selected).ToString() +
                        Environment.NewLine : "";
                    text += BlockedTilePruner.Unreachable.Contains(selected) ? "<color=red> Pruned from pathfinding; unreachable </color>" : "";

                    DebugExt.dLog(text);
                }
                if (Input.GetKeyDown(KeyCode.H))
                    BlockedTilePruner.DoRegionSearch(ElevationManager.GetAll());
            }
            if (Input.GetKeyDown(Settings.keycode_topDownView))
            {
                TopDownModeCamera.ToggleTopDownView();
            }
        }

    }
}

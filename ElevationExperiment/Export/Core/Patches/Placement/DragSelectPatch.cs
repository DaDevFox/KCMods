using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Harmony;
using UnityEngine;
using System.Reflection;

namespace Elevation.Patches
{
    [HarmonyPatch(typeof(GameUI),"UpdateCellSelector")]
    class DragSelectPatch
    {
        static float yOffset = 0.1f;
        static float xzMargin = 0.1f;


        static void Postfix(GameUI __instance)
        {
            try
            {
                bool dragSelecting = (bool)typeof(GameUI)
                    .GetField("dragSelecting", BindingFlags.NonPublic | BindingFlags.Instance)
                    .GetValue(__instance);

                if (!dragSelecting)
                    return;

                Vector3 _dragStart = (Vector3)typeof(GameUI)
                    .GetField("dragStart", BindingFlags.NonPublic | BindingFlags.Instance)
                    .GetValue(__instance);
                Vector3 _dragEnd = (Vector3)typeof(GameUI)
                    .GetField("dragEnd", BindingFlags.NonPublic | BindingFlags.Instance)
                    .GetValue(__instance);


                Vector3 dragStart = new Vector3((float)((int)_dragStart.x) - xzMargin/2f, _dragStart.y, (float)((int)_dragStart.z) - xzMargin/2f);
                Vector3 dragEnd = new Vector3((float)((int)_dragEnd.x) + xzMargin/2f, _dragEnd.y, (float)((int)_dragEnd.z) + xzMargin/2f);

                Vector3 diff = dragStart - dragEnd;
                Vector3 newSize = new Vector3(Mathf.Abs(diff.x) + 1f, 0f, Mathf.Abs(diff.z) + 1f);

                Vector3 center = dragStart - (diff * 0.5f) + new Vector3(0.5f, 0f, 0.5f);

                __instance.CellHighlighter.SetTargetWorldBounds(center, newSize);
                __instance.CellHighlighter.SetFillY(dragEnd.y - dragStart.y + yOffset);
            }
            catch(Exception ex)
            {
                DebugExt.HandleException(ex);
            }
        }


    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Harmony;
using UnityEngine;
using ElevationExperiment.Utils;

namespace ElevationExperiment
{
    public class TopDownModeCamera : MonoBehaviour
    {
        //A Message to anybody reading this code:
        //Don't judge, this code has been butchered and is awful, it contains some terrible practices and 
        //All the class members are public, this is because harmony decided to not work in subclasses specifically for this file, idk why
        //So I had to adjust my code, and it is now awful. 

        public static float maxCamHeight = 100f;
        public static float minCamHeight = 20f;
        public static float startCamHeight = 50f;

        public static float camHeightZoomSpeed = 5f;
        public static float camHeight = startCamHeight;

        public static float camHeightSpeedModifierBuffer = 0.8f;

        public static float normalSpeed 
        {
            get 
            {
                return Settings.inst.c_CameraControls.s_speed.Value;
            } 
        }
        public static float shiftSpeedBoost
        {
            get
            {
                return Settings.inst.c_CameraControls.s_shiftSpeed.Value;
            }
        }

        public static float snap
        {
            get
            {
                return Settings.inst.c_CameraControls.s_snap.Value;
            }
        }

        public static float velocityAcceleration = 4f;
        public static float velocityDeceleration = 5f;
        public static float maxVelocity = 2f;

        public static float contiguousVelocity = 0f;
        


        public static bool topDownModeActive = false;

        public static void ActivateTopDownView()
        {
            topDownModeActive = true;
        }

        public static void DeactivateTopDownView()
        {
            topDownModeActive = false;
        }

        public static void ToggleTopDownView()
        {
            topDownModeActive = !topDownModeActive;
        }

        
    }


    [HarmonyPatch(typeof(Cam), "MoveRight")]
    public static class CamMoveRightPatch
    {
        static bool Prefix()
        {
            return !TopDownModeCamera.topDownModeActive;
        }
    }

    [HarmonyPatch(typeof(Cam), "MoveForward")]
    public static class CamMoveForwardPatch
    {
        static bool Prefix()
        {
            return !TopDownModeCamera.topDownModeActive;
        }
    }


    [HarmonyPatch(typeof(Cam), "Zoom")]
    public static class CamZoomPatch
    {
        static bool Prefix()
        {
            return !TopDownModeCamera.topDownModeActive;
        }
    }

    [HarmonyPatch(typeof(Cam), "Rotate")]
    public static class CamRotatePatch
    {
        static bool Prefix()
        {
            return !TopDownModeCamera.topDownModeActive;
        }
    }


    [HarmonyPatch(typeof(Cam), "Update")]
    public static class MainCamMovementPatch
    {
        static bool Prefix()
        {
            if (TopDownModeCamera.topDownModeActive)
            {
                Camera cam = Cam.inst.cam;

                if (Cam.inst.OverrideTrack != null && !Cam.inst.OverrideTrack.Equals(null))
                    Cam.inst.DesiredTrackingPos = Cam.inst.OverrideTrack.GetDesiredTrackingPos();

                Vector3 pos = new Vector3(Cam.inst.DesiredTrackingPos.x, TopDownModeCamera.camHeight, Cam.inst.DesiredTrackingPos.z);

                if (TopDownModeCamera.snap > 0f)
                {
                    pos.x = Utils.Util.RoundToFactor(pos.x, TopDownModeCamera.snap);
                    pos.z = Utils.Util.RoundToFactor(pos.z, TopDownModeCamera.snap);
                }

                cam.transform.rotation = Quaternion.AngleAxis(90f, new Vector3(1f, 0f, 0f));
                cam.transform.position = pos;

                float camHeightSpeedModifier = (TopDownModeCamera.camHeight - TopDownModeCamera.minCamHeight) / (TopDownModeCamera.maxCamHeight - TopDownModeCamera.minCamHeight) + TopDownModeCamera.camHeightSpeedModifierBuffer;
                camHeightSpeedModifier = Mathf.Clamp(camHeightSpeedModifier, 0f, 1f);

                float speed = TopDownModeCamera.normalSpeed * TopDownModeCamera.contiguousVelocity * camHeightSpeedModifier;

                Vector3 movementNormalized = Vector3.zero;

                bool movedThisFrame = false;

                if (ConfigurableControls.inst.GetInputActionKey(InputActions.CameraMoveForward, true, true, true))
                {
                    movedThisFrame = true;
                    movementNormalized += Vector3.forward;
                }


                if (ConfigurableControls.inst.GetInputActionKey(InputActions.CameraMoveBack, true, true, true))
                {
                    movedThisFrame = true;
                    movementNormalized += Vector3.back;
                }

                if (ConfigurableControls.inst.GetInputActionKey(InputActions.CameraMoveRight, true, true, true))
                {
                    movedThisFrame = true;
                    movementNormalized += Vector3.right;
                }


                if (ConfigurableControls.inst.GetInputActionKey(InputActions.CameraMoveLeft, true, true, true))
                {
                    movedThisFrame = true;
                    movementNormalized += Vector3.left;
                }

                if (ConfigurableControls.inst.GetInputActionKey(InputActions.CameraMoveFast, true, true, true))
                {
                    movedThisFrame = true;
                    speed *= TopDownModeCamera.shiftSpeedBoost;
                }

                if (ConfigurableControls.inst.GetInputActionKey(InputActions.CameraZoomOut, true, true, true))
                    TopDownModeCamera.camHeight += TopDownModeCamera.camHeightZoomSpeed;
                else if (Input.GetAxisRaw("Mouse ScrollWheel") < 0f)
                    TopDownModeCamera.camHeight += TopDownModeCamera.camHeightZoomSpeed;

                if (ConfigurableControls.inst.GetInputActionKey(InputActions.CameraZoomIn, true, true, true))
                    TopDownModeCamera.camHeight -= TopDownModeCamera.camHeightZoomSpeed;
                else if (Input.GetAxisRaw("Mouse ScrollWheel") > 0f)
                    TopDownModeCamera.camHeight -= TopDownModeCamera.camHeightZoomSpeed;
                


                TopDownModeCamera.camHeight = Mathf.Clamp(TopDownModeCamera.camHeight, TopDownModeCamera.minCamHeight, TopDownModeCamera.maxCamHeight);

                Vector3 clampedPos = World.inst.ClampToWorld(Cam.inst.DesiredTrackingPos + (movementNormalized * speed));

                Cam.inst.SetDesiredTrackingPos(clampedPos);


                TopDownModeCamera.contiguousVelocity += movedThisFrame ? (Time.unscaledDeltaTime * TopDownModeCamera.velocityAcceleration) : (-Time.unscaledDeltaTime * TopDownModeCamera.velocityDeceleration);
                TopDownModeCamera.contiguousVelocity = Mathf.Clamp(TopDownModeCamera.contiguousVelocity, 0, TopDownModeCamera.maxVelocity);

            }
            return !TopDownModeCamera.topDownModeActive;
        }
    }

}

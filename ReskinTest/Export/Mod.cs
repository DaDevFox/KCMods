using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using BuildingFramework;

namespace ReskinTest
{
    public class Mod
    {

        public static KCModHelper helper;

        GameObject woodBridge_corner;
        GameObject woodBridge_straight;
        GameObject woodBridge_center;
        GameObject woodBridge_threeway;


        GameObject stoneBridge_corner;
        GameObject stoneBridge_straight;
        GameObject stoneBridge_center;
        GameObject stoneBridge_threeway;


        GameObject woodCastleBlock_corner;
        GameObject woodCastleBlock_flat;
        GameObject woodCastleBlock_fulltop;
        GameObject woodCastleBlock_threesided;
        GameObject woodCastleBlock_twosided;
        GameObject woodCastleBlock_onesided;

        GameObject woodCastleBlock_door;


        GameObject woodGate_base;
        GameObject woodGate_porticulus;

        GameObject stoneCastleBlock_corner;
        GameObject stoneCastleBlock_flat;
        GameObject stoneCastleBlock_fulltop;
        GameObject stoneCastleBlock_threesided;
        GameObject stoneCastleBlock_twosided;
        GameObject stoneCastleBlock_onesided;

        GameObject stoneCastleBlock_door;


        GameObject stoneGate_base;
        GameObject stoneGate_porticulus;

        public void Preload(KCModHelper helper)
        {
            Mod.helper = helper;

            LoadAssets();
            Reskin();
        }

        void LoadAssets()
        {
            AssetBundleManager.UnpackAssetBundle();

            // Wooden Bridge
            woodBridge_corner = AssetBundleManager.GetAsset("Wood_WoodBridge_BridgeCorner.prefab") as GameObject;
            woodBridge_straight = AssetBundleManager.GetAsset("Wood_WoodBridge_BridgeStraight.prefab") as GameObject;
            woodBridge_center = AssetBundleManager.GetAsset("Wood_WoodBridge_BridgeFourway.prefab") as GameObject;
            woodBridge_threeway = AssetBundleManager.GetAsset("Wood_WoodBridge_BridgeTBone.prefab") as GameObject;


            // Stone Bridge
            stoneBridge_corner = AssetBundleManager.GetAsset("Stone_StoneBridge_StoneBridgeCorner.prefab") as GameObject;
            stoneBridge_straight = AssetBundleManager.GetAsset("Stone_StoneBridge_StoneBridgeFourway.prefab") as GameObject;
            stoneBridge_center = AssetBundleManager.GetAsset("Stone_StoneBridge_StoneBridgeStraight.prefab") as GameObject;
            stoneBridge_threeway = AssetBundleManager.GetAsset("Stone_StoneBridge_StoneBridgeTBone.prefab") as GameObject;

            // Wooden Castle Block
            woodCastleBlock_corner = AssetBundleManager.GetAsset("Wood_WoodWall_WoodCorner.prefab") as GameObject;
            woodCastleBlock_flat = AssetBundleManager.GetAsset("Wood_WoodWall_FlatWall.prefab") as GameObject;
            woodCastleBlock_fulltop = AssetBundleManager.GetAsset("Wood_WoodWall_WoodFullTop.prefab") as GameObject;
            woodCastleBlock_threesided = AssetBundleManager.GetAsset("Wood_WoodWall_WoodThreeSided.prefab") as GameObject;
            woodCastleBlock_twosided = AssetBundleManager.GetAsset("Wood_WoodWall_WoodTwoSided.prefab") as GameObject;
            woodCastleBlock_onesided = AssetBundleManager.GetAsset("Wood_WoodWall_WoodOneSided.prefab") as GameObject;

            woodCastleBlock_door = AssetBundleManager.GetAsset("Wood_WoodWall_WoodCastleDoor.prefab") as GameObject;

            // Wooden Gate
            woodGate_base = AssetBundleManager.GetAsset("Wood_WoodWall_WoodGate.prefab") as GameObject;
            woodGate_porticulus = AssetBundleManager.GetAsset("Wood_WoodWall_WoodPortculis.prefab") as GameObject;


            // Stone Castle Block
            stoneCastleBlock_corner = AssetBundleManager.GetAsset("Stone_StoneWall_StoneWallCorner.prefab") as GameObject;
            stoneCastleBlock_flat = AssetBundleManager.GetAsset("Stone_StoneWall_StoneWallFlat.prefab") as GameObject;
            stoneCastleBlock_fulltop = AssetBundleManager.GetAsset("Stone_StoneWall_StoneWallFulltop.prefab") as GameObject;
            stoneCastleBlock_threesided = AssetBundleManager.GetAsset("Stone_StoneWall_StoneWallThreeSided.prefab") as GameObject;
            stoneCastleBlock_twosided = AssetBundleManager.GetAsset("Stone_StoneWall_StoneWallTwoSided.prefab") as GameObject;
            stoneCastleBlock_onesided = AssetBundleManager.GetAsset("Stone_StoneWall_StoneWallOneSided.prefab") as GameObject;

            stoneCastleBlock_door = AssetBundleManager.GetAsset("Stone_StoneWall_StoneCastleDoor.prefab") as GameObject;

            // Stone Gate
            stoneGate_base = AssetBundleManager.GetAsset("Stone_StoneWall_StoneCastleGate.prefab") as GameObject;
            stoneGate_porticulus = AssetBundleManager.GetAsset("Stone_StoneWall_StonePortculis.prefab") as GameObject;


        }




        void Reskin()
        {
            try
            {
                //Wood Castle Block
                WoodCastleBlockBuildingSkin woodBlock = new WoodCastleBlockBuildingSkin();
                woodBlock.doorPrefab = woodCastleBlock_door;
                
                woodBlock.Adjacent = woodCastleBlock_corner;
                woodBlock.Threeside = woodCastleBlock_threesided;
                woodBlock.Opposite = woodCastleBlock_twosided;
                woodBlock.Single = woodCastleBlock_onesided;
                woodBlock.Closed = woodCastleBlock_fulltop;
                woodBlock.Open = woodCastleBlock_flat;

                ModelHelper.ReskinBuildingBase(woodBlock);


                //Stone Castle Block
                StoneCastleBlockBuildingSkin stoneBlock = new StoneCastleBlockBuildingSkin();
                stoneBlock.doorPrefab = stoneCastleBlock_door;

                stoneBlock.Adjacent = stoneCastleBlock_corner;
                stoneBlock.Threeside = stoneCastleBlock_threesided;
                stoneBlock.Opposite = stoneCastleBlock_twosided;
                stoneBlock.Single = stoneCastleBlock_onesided;
                stoneBlock.Closed = stoneCastleBlock_fulltop;
                stoneBlock.Open = stoneCastleBlock_flat;

                ModelHelper.ReskinBuildingBase(stoneBlock);


                //Wooden Gate
                WoodenGateBuildingSkin woodGate = new WoodenGateBuildingSkin();
                woodGate.gate = woodGate_base;
                woodGate.porticulus = woodGate_porticulus;

                ModelHelper.ReskinBuildingBase(woodGate);


                //Stone Gate
                StoneGateBuildingSkin stoneGate = new StoneGateBuildingSkin();
                stoneGate.gate = stoneGate_base;
                stoneGate.porticulus = stoneGate_porticulus;

                ModelHelper.ReskinBuildingBase(stoneGate);


                // Wooden Bridge
                WoodenBridgeBuildingSkin woodenBridge = new WoodenBridgeBuildingSkin();
                woodenBridge.Straight = woodBridge_straight;
                woodenBridge.Elbow = woodBridge_corner;
                woodenBridge.Threeway = woodBridge_threeway;
                woodenBridge.Fourway = woodBridge_center;

                ModelHelper.ReskinBuildingBase(woodenBridge);


                // Stone Bridge
                StoneBridgeBuildingSkin stoneBridge = new StoneBridgeBuildingSkin();
                stoneBridge.Straight = stoneBridge_straight;
                stoneBridge.Elbow = stoneBridge_corner;
                stoneBridge.Threeway = stoneBridge_threeway;
                stoneBridge.Fourway = stoneBridge_center;

                ModelHelper.ReskinBuildingBase(stoneBridge);








            }
            catch(Exception ex)
            {
                helper.Log(ex.Message + "\n" + ex.StackTrace);
            }
        }




    }
}

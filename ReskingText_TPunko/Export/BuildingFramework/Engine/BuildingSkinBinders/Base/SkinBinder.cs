﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Harmony;
using System.Reflection;

namespace ReskinEngine.Engine
{
    public static class GameObjectExtensions
    {
        public static void CopyValues<T>(this T sourceComp, T targetComp)
        {
            FieldInfo[] sourceFields = sourceComp.GetType().GetFields(BindingFlags.Public |
                                                             BindingFlags.NonPublic |
                                                             BindingFlags.Instance);
            int i = 0;
            for (i = 0; i < sourceFields.Length; i++)
            {
                var value = sourceFields[i].GetValue(sourceComp);
                sourceFields[i].SetValue(targetComp, value);
            }
        }
    }

    /// <summary>
    /// The Engine-side receiver of API data that applies a skin's data to the game
    /// </summary>
    public abstract class SkinBinder
    {
        /// <summary>
        /// The index of the skin within its mod
        /// </summary>
        public int Identifier { get; internal set; }

        /// <summary>
        /// Compatability identifier used to detremine which skins can be used with this one
        /// </summary>
        public string CompatabilityIdentifier { get; private set; }
        /// <summary>
        /// Name of the mod the skin is included in
        /// </summary>
        public string ModName { get; private set; }

        /// <summary>
        /// String that the engine uses to identify this binder
        /// </summary>
        public virtual string TypeIdentifier { get; }

        /// <summary>
        /// Creates a SkinBinder from the GameObject based on the information provided by the GameObject's name and returns it. 
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static SkinBinder Unpack(GameObject obj)
        {
            if (obj == null)
                throw new ArgumentNullException("obj");

            string[] info = obj.name.Split(new char[] { ':' }, StringSplitOptions.RemoveEmptyEntries);

            string compatabilityIdentifier = info[0];
            string mod = info[1];
            string skinType = info[2];
            int skinIdentifier = int.Parse(info[3]);


            SkinBinder original = Engine.GetOriginalBinder(skinType);

            if(original == null)
            {
                Engine.dLog($"No skin binder found for type identifier {skinType}");
                return null;
            }


            SkinBinder instance = Activator.CreateInstance(original.GetType()) as SkinBinder;

            instance.Read(obj);
            
            instance.CompatabilityIdentifier = compatabilityIdentifier;
            instance.ModName = mod;
            instance.Identifier = skinIdentifier;

            return instance;
        }

        /// <summary>
        /// Read the GameObject packaged by the API-side and apply it to the fields of this SkinBinder
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public abstract void Read(GameObject obj);


        /// <summary>
        /// Use to apply this skin using any data recieved from the data GameObject
        /// </summary>
        public virtual void Bind()
        {

        }

        protected void ReadModels(GameObject _base, BindingFlags flags = BindingFlags.Public | BindingFlags.Instance, params string[] models)
        {
            foreach (string model in models)
                ReadModel( _base, model, flags);
        }

        /// <summary>
        /// Helper function that reads if the given model is present in the packaged GameObject _base and assigns the skin's model to the packaged model.
        /// <para>requires binding flags to use reflection to modify skin's model</para>
        /// </summary>
        /// <param name="binder"></param>
        /// <param name="_base"></param>
        /// <param name="modelName"></param>
        /// <param name="flags"></param>
        protected void ReadModel(GameObject _base, string modelName, BindingFlags flags = BindingFlags.Public | BindingFlags.Instance)
        {
            if (_base.transform.Find(modelName))
                if (GetType().GetField(modelName) != null)
                    GetType().GetField(modelName, flags).SetValue(this, _base.transform.Find(modelName).gameObject);
        }

        protected bool ReadMaterialFlag(GameObject _base, string materialName) => _base.transform.Find($"{materialName}Flag");

        protected void ReadMaterial(GameObject _base, string materialName, BindingFlags flags = BindingFlags.Public | BindingFlags.Instance)
        {
            if (_base.transform.Find(materialName))
                if (GetType().GetField(materialName) != null)
                    GetType().GetField(materialName, flags).SetValue(this, _base.transform.Find(materialName).GetComponent<MeshRenderer>().material);
        }
    }

    /// <summary>
    /// An implementation of SkinBinder designed specifically for buildings and supports skin variation on building placement
    /// </summary>
    public abstract class BuildingSkinBinder : SkinBinder
    {
        public static Dictionary<string, GameObject> originalModels { get; private set; } = new Dictionary<string, GameObject>();

        /// <summary>
        /// Do not override for BuildingSkinBindres
        /// </summary>
        public sealed override string TypeIdentifier => $"building_{UniqueName}";

        public abstract string UniqueName { get; }

        public Vector3[] personPositions;
        public string[] outlineMeshes;
        public string[] outlineSkinnedMeshes;
        public string[] colliders;

        /// <summary>
        /// Do not override for BuildingSkinBinders
        /// </summary>
        public sealed override void Bind()
        {
            if (UniqueName != "unregistered" && UniqueName != "hidden")
            {
                Engine.helper.Log(UniqueName);
                originalModels.Add(UniqueName, GameState.inst.GetPlaceableByUniqueName(UniqueName).gameObject);
                BindToBuildingBase(GameState.inst.GetPlaceableByUniqueName(UniqueName));
            }
        }

        public override void Read(GameObject obj)
        {
            ReadColliders(obj);
            ReadPersonPositions(obj);
            ReadOutlineMeshes(obj);
            ReadOutlineSkinnedMeshes(obj);
        }

        /// <summary>
        /// Use this to bind the skin to the base building that will be duplicated every time a building is placed with the given unique name
        /// </summary>
        /// <param name="building"></param>
        public virtual void BindToBuildingBase(Building building)
        {
            if (building == null)
            {
                Engine.helper.Log("Requested bind to null object instead of building");
                Engine.helper.Log(StackTraceUtility.ExtractStackTrace());
                return;
            }

            BindColliders(building);
            BindPersonPositions(building);
            BindOutlineMeshes(building);
            BindOutlineSkinnedMeshes(building);

            //Engine.dLog(outlineMeshes.Length);
        }

        /// <summary>
        /// Use this to bind the skin to a single building after it has been placed
        /// </summary>
        /// <param name="building"></param>
        public virtual void BindToBuildingInstance(Building building)
        {

        }

        #region Util Functions

        /// <summary>
        /// Helper function that applies peoplePositions from a GameObject to any BuildingSkinBinder; use in Create()
        /// <para>Only neccessary for buildings with workers</para>
        /// </summary>
        /// <param name="binder"></param>
        /// <param name="_base"></param>
        protected void ReadPersonPositions(GameObject _base)
        {
            Transform container = _base.transform.Find("personPositions");
            if (container)
            {
                List<Vector3> positions = new List<Vector3>();
                for (int i = 0; i < container.transform.childCount; i++)
                    positions.Add(container.GetChild(i).transform.localPosition);

                personPositions = positions.ToArray();
            }
            else
                personPositions = new Vector3[0];
        }

        /// <summary>
        /// Helper function that binds personPositions from a BuildingSkinBinder to a building, preserving their original transforms but changing the positions
        /// </summary>
        /// <param name="building"></param>
        /// <param name="binder"></param>
        protected void BindPersonPositions(Building building)
        {
            if (building.personPositions == null)
                return;
            if (personPositions == null)
                return;

            for (int i = 0; i < building.personPositions.Length; i++)
            {
                if (i < personPositions.Length && building.personPositions[i] != null)
                {
                    building.personPositions[i].localPosition = personPositions[i];
                }
            }
        }

        protected void ReadOutlineMeshes(GameObject _base)
        {
            GameObject info = null;
            for (int i = 0; i < _base.transform.childCount; i++)
                if (_base.transform.GetChild(i).name.Contains("outlineMeshes:"))
                    info = _base.transform.GetChild(i).gameObject;
            if (info)
            {
                string[] name = info.name.Split(':');
                string[] list = name[1].Split(',');
                outlineMeshes = list;
            }
            else
                outlineMeshes = new string[0];
        }

        protected void BindOutlineMeshes(Building building)
        {
            if (outlineMeshes == null)
                return;
            List<MeshRenderer> meshes = new List<MeshRenderer>();
            foreach (string path in outlineMeshes)
                if (building.transform.Find(path) && building.transform.Find(path).GetComponent<MeshRenderer>())
                    meshes.Add(building.transform.Find(path).GetComponent<MeshRenderer>());
            building.meshesRequiringOutline = meshes;
            Engine.dLog(meshes.Count);
        }

        protected void ReadOutlineSkinnedMeshes(GameObject _base)
        {
            GameObject info = null;
            for (int i = 0; i < _base.transform.childCount; i++)
                if (_base.transform.GetChild(i).name.Contains("outlineSkinnedMeshes:"))
                    info = _base.transform.GetChild(i).gameObject;
            if (info)
            {
                string[] name = info.name.Split(':');
                string[] list = name[1].Split(',');
                outlineSkinnedMeshes = list;
            }
            else
                outlineSkinnedMeshes = new string[0];
        }

        protected void BindOutlineSkinnedMeshes(Building building)
        {
            if (outlineSkinnedMeshes == null)
                return;
            List<SkinnedMeshRenderer> meshes = new List<SkinnedMeshRenderer>();
            foreach (string path in outlineSkinnedMeshes)
                if (building.transform.Find(path) && building.transform.Find(path).GetComponent<SkinnedMeshRenderer>())
                    meshes.Add(building.transform.Find(path).GetComponent<SkinnedMeshRenderer>());
            building.skinnedMeshesRequiringOutline = meshes;
        }

        protected void ReadColliders(GameObject _base)
        {
            ReadStringArray(_base, "colliders");
            Engine.dLog(colliders.Length);
        }

        protected void BindColliders(Building building)
        {
            if (colliders == null || colliders.Length == 0)
                return;

            // Delete existing BuildingColliders
            foreach (BuildingCollider collider in Util.ComponentsInNodeAndAllDescendants<BuildingCollider>(building.gameObject))
                GameObject.Destroy(collider.Collider);
            // Attach BuildingColliders to all requested collider paths
            foreach (string path in colliders)
                if (building.transform.Find(path) && building.transform.Find(path).GetComponent<Collider>())
                    building.transform.Find(path).gameObject.AddComponent<BuildingCollider>();
        }

        protected void ReadString(GameObject _base, string fieldName, BindingFlags flags = BindingFlags.Public | BindingFlags.Instance)
        {
            GameObject info = null;
            for (int i = 0; i < _base.transform.childCount; i++)
                if (_base.transform.GetChild(i).name.Contains(fieldName))
                    info = _base.transform.GetChild(i).gameObject;

            string value = "";
            if (info != null)
                value = info.name.Split(':')[1];

            if (GetType().GetField(fieldName) != null)
                GetType().GetField(fieldName, flags).SetValue(this, value);
        }

        protected void ReadStringArray(GameObject _base, string fieldName, BindingFlags flags = BindingFlags.Public | BindingFlags.Instance)
        {
            string[] array;
            GameObject info = null;
            for (int i = 0; i < _base.transform.childCount; i++)
                if (_base.transform.GetChild(i).name.Contains(fieldName))
                    info = _base.transform.GetChild(i).gameObject;
            if (info)
            {
                string[] name = info.name.Split(':');
                string[] list = name[1].Split(',');
                array = list;
            }
            else
                array = new string[0];

            if (GetType().GetField(fieldName) != null)
                GetType().GetField(fieldName, flags).SetValue(this, array);
        }

        #endregion

        [HarmonyPatch(typeof(World), "PlaceInternal")]
        static class OnPlacePatch
        {
            static void Postfix(Building PendingObj)
            {
                SkinBinder s = Engine.GetRandomBinderFromActive("building_" + PendingObj.UniqueName);
                if(s as BuildingSkinBinder != null)
                {

                    if (Engine.GetPriority(s.TypeIdentifier).NumBinders(s.TypeIdentifier) > 1)
                    {
                        GameObject _new = GameObject.Instantiate(originalModels[PendingObj.UniqueName]);

                        if (_new)
                        {
                            _new.transform.position = PendingObj.transform.position;
                            _new.transform.rotation = PendingObj.transform.rotation;
                            _new.transform.localScale = PendingObj.transform.localScale;


                            BuildingSkinBinder binder = s as BuildingSkinBinder;
                            binder.BindToBuildingInstance(_new.GetComponent<Building>());
                            GameObject.Destroy(PendingObj);

                            Engine.helper.Log($"binding building skin [{binder.TypeIdentifier}:{binder.Identifier}] to {binder.UniqueName}");
                        }
                    }
                    else
                    {
                        BuildingSkinBinder binder = s as BuildingSkinBinder;
                        binder.BindToBuildingInstance(PendingObj);

                        Engine.helper.Log($"binding building skin [{binder.TypeIdentifier}:{binder.Identifier}] to {binder.UniqueName}");
                    }
                }
            }
        }

    }

    /// <summary>
    /// Skin Binder that applies generically to most buildings in the game
    /// </summary>
    [Unregistered]
    public abstract class GenericBuildingSkinBinder : BuildingSkinBinder
    {
        public GameObject baseModel;

        // TODO: Remove T generic type param
        protected void Read<T>(GameObject obj) where T : GenericBuildingSkinBinder, new()
        {
            base.Read(obj);

            ReadModel(obj, "baseModel");
            ReadString(obj, "collider");
            //if (obj.transform.Find("baseModel"))
            //    baseModel = obj.transform.Find("baseModel").gameObject;

        }

        public override void BindToBuildingBase(Building building)
        {
            Transform target = building.transform.GetChild(0).GetChild(0);
            if (!target)
            {
                Engine.helper.Log("GenericBuildingSkinBinder bound to building that doesn't follow generic building architechture; aborting");
                return;
            }

            BuildingCollider bCollider = target.GetComponent<BuildingCollider>();

            if (baseModel)
            {
                // Reset Mesh
                target.GetComponent<MeshFilter>().mesh = null;
                GameObject model = GameObject.Instantiate(baseModel, target);
                model.name = "baseModel";

                // Reset Collider
                //if (building.transform.Find(collider) && building.transform.Find(collider).GetComponent<Collider>())
                //{
                //    GameObject.Destroy(bCollider.Collider);

                //    Collider newCollider = building.transform.Find(collider).GetComponent<Collider>();
                //    newCollider.gameObject.AddComponent<BuildingCollider>();
                //}
            }

            base.BindToBuildingBase(building);
        }

        public override void BindToBuildingInstance(Building building)
        {
            this.BindToBuildingBase(building);
        }
    }



    //    public class KeepBuildingSkin : BuildingSkin
    //    {
    //        public KeepBuildingSkin()
    //        {
    //            _removalProcedure = delegate (Building b) { Procedures.rp_keep(b); };
    //            _replaceProcedure = delegate (Building b) { Procedures.rep_keep(b, this); };

    //            _buildingUniqueName = "keep";
    //        }

    //        public GameObject keepUpgrade1;
    //        public GameObject keepUpgrade2;
    //        public GameObject keepUpgrade3;
    //        public GameObject keepUpgrade4;

    //        public GameObject banner1;
    //        public GameObject banner2;

    //        protected override void PackageInternal(Transform target, GameObject _base)
    //        {
    //            base.PackageInternal(target, _base);

    //            if (keepUpgrade1)
    //                GameObject.Instantiate(keepUpgrade1, _base.transform).name = "keepUpgrade1";
    //            if (keepUpgrade2)
    //                GameObject.Instantiate(keepUpgrade2, _base.transform).name = "keepUpgrade2";
    //            if (keepUpgrade3)
    //                GameObject.Instantiate(keepUpgrade3, _base.transform).name = "keepUpgrade3";
    //            if (keepUpgrade4)
    //                GameObject.Instantiate(keepUpgrade4, _base.transform).name = "keepUpgrade4";

    //            if (banner1)
    //                GameObject.Instantiate(banner1, _base.transform).name = "banner1";
    //            if (banner2)
    //                GameObject.Instantiate(banner2, _base.transform).name = "banner2";
    //        }
    //    }

    //    #region Castle Blocks

    //    //Castle Block Base
    //    public class CastleBlockBuildingSkin : BuildingSkin
    //    {
    //        public CastleBlockBuildingSkin()
    //        {
    //            _removalProcedure = delegate (Building b) { Procedures.rp_castleblock(b); };
    //            _replaceProcedure = delegate (Building b) { Procedures.rep_castleblock(b, this); };
    //        }

    //        /// <summary>
    //        /// The flat piece without crenelations for a castle block
    //        /// This is a modular piece. See info.txt for details
    //        /// </summary>
    //        public GameObject Open;
    //        /// <summary>
    //        /// The piece of a castleblock with all crenelations at the top and no connections
    //        /// This is a modular piece. See info.txt for details
    //        /// </summary>
    //        public GameObject Closed;
    //        /// <summary>
    //        /// The piece of a castleblock that only has crenelations on one side
    //        /// This is a modular piece. See info.txt for details
    //        /// </summary>
    //        public GameObject Single;
    //        /// <summary>
    //        /// The straight piece of a castle block
    //        /// This is a modular piece. See info.txt for details
    //        /// </summary>
    //        public GameObject Opposite;
    //        /// <summary>
    //        /// The corner piece for a castle block
    //        /// This is a modular piece. See info.txt for details
    //        /// </summary>
    //        public GameObject Adjacent;
    //        /// <summary>
    //        /// The piece of a castleblock with crenelations on 3 sides
    //        /// This is a modular piece. See info.txt for details
    //        /// </summary>
    //        public GameObject Threeside;

    //        /// <summary>
    //        /// The door that appears on a castleblock when it connects to other castleblocks
    //        /// </summary>
    //        public GameObject doorPrefab;
    //    }

    //    //Wood Castle Block
    //    public class WoodCastleBlockBuildingSkin : CastleBlockBuildingSkin
    //    {
    //        public WoodCastleBlockBuildingSkin()
    //        {
    //            _buildingUniqueName = "woodcastleblock";
    //        }

    //    }

    //    //Stone Castle Block
    //    public class StoneCastleBlockBuildingSkin : CastleBlockBuildingSkin
    //    {
    //        public StoneCastleBlockBuildingSkin()
    //        {
    //            _buildingUniqueName = "castleblock";
    //        }
    //    }

    //    #endregion

    //    #region Gates

    //    //  Gate Base
    //    public class GateBuildingSkin : BuildingSkin
    //    {
    //        public GateBuildingSkin()
    //        {
    //            _removalProcedure = delegate (Building b) { Procedures.rp_gate(b); };
    //            _replaceProcedure = delegate (Building b) { Procedures.rep_gate(b, this); };
    //        }

    //        public GameObject gate;
    //        public GameObject porticulus;
    //    }

    //    //Wooden Gate
    //    public class WoodenGateBuildingSkin : GateBuildingSkin
    //    {
    //        public WoodenGateBuildingSkin()
    //        {
    //            _buildingUniqueName = "woodengate";
    //        }

    //    }

    //    //Stone Gate
    //    public class StoneGateBuildingSkin : GateBuildingSkin
    //    {
    //        public StoneGateBuildingSkin()
    //        {
    //            _buildingUniqueName = "gate";
    //        }

    //    }

    //    #endregion

    //    //Castle Stairs
    //    public class CastleStairsBuildingSkin : BuildingSkin
    //    {
    //        public CastleStairsBuildingSkin()
    //        {
    //            _removalProcedure = delegate (Building b) { Procedures.rp_castlestairs(b); };
    //            _replaceProcedure = delegate (Building b) { Procedures.rep_castlestairs(b, this); };

    //            _buildingUniqueName = "castlestairs";
    //        }

    //        public GameObject stairsFront;
    //        public GameObject stairsRight;
    //        public GameObject stairsDown;
    //        public GameObject stairsLeft;
    //    }

    //    //Archer Tower
    //    public class ArcherTowerBuildingSkin : BuildingSkin
    //    {
    //        public ArcherTowerBuildingSkin()
    //        {
    //            _removalProcedure = delegate (Building b) { Procedures.rp_archer(b); };
    //            _replaceProcedure = delegate (Building b) { Procedures.rep_archer(b, this); };

    //            _buildingUniqueName = "archer";
    //        }


    //        /// <summary>
    //        /// The main model of the Archer Tower
    //        /// </summary>
    //        public GameObject baseModel;
    //        /// <summary>
    //        /// An embelishment added to the archer tower when it achieves the veteran status
    //        /// </summary>
    //        public GameObject veteranModel;
    //    }

    //    //Ballista Tower
    //    public class BallistaTowerBuildingSkin : BuildingSkin
    //    {
    //        public BallistaTowerBuildingSkin()
    //        {
    //            _removalProcedure = delegate (Building b) { Procedures.rp_ballista(b); };
    //            _replaceProcedure = delegate (Building b) { Procedures.rep_ballista(b, this); };

    //            _buildingUniqueName = "ballista";
    //        }

    //        /// <summary>
    //        /// An embelishment added to the ballista tower when it achieves the veteran status
    //        /// </summary>
    //        public GameObject veteranModel;
    //        /// <summary>
    //        /// The main model of the Ballista Tower
    //        /// </summary>
    //        public GameObject baseModel;
    //        /// <summary>
    //        /// The base of the rotational top half of the ballista
    //        /// </summary>
    //        public GameObject topBase;
    //        /// <summary>
    //        /// The right side arm used to animate the ballista's firing movement
    //        /// </summary>
    //        public GameObject armR;
    //        /// <summary>
    //        /// The right end of the right arm of the ballista; used for anchoring the right side of the string in animation
    //        /// </summary>
    //        public Transform armREnd;
    //        /// <summary>
    //        /// The left side arm used to animate the ballista's firing movement
    //        /// </summary>
    //        public GameObject armL;
    //        /// <summary>
    //        /// The lef end of the left arm of the ballista; used for anchoring the left side of the string in animation
    //        /// </summary>
    //        public Transform armLEnd;
    //        /// <summary>
    //        /// The right side of the animated string used to pull back and fire the ballista projectile
    //        /// </summary>
    //        public GameObject stringR;
    //        /// <summary>
    //        /// The left side of the animated string used to pull back and fire the ballista projectile
    //        /// </summary>
    //        public GameObject stringL;
    //        /// <summary>
    //        /// The projectile fired from the ballista
    //        /// </summary>
    //        public GameObject projectile;
    //        public Transform projectileEnd;
    //        /// <summary>
    //        /// A decorative flag on the ballista
    //        /// </summary>
    //        public GameObject flag;
    //    }



    //#endregion

    //    #region Town



    //    #endregion

    //    #region Advanced Town

    //    //Hospital
    //    public class HospitalBuildingSkin : GenericBuildingSkin
    //    {
    //        public HospitalBuildingSkin()
    //        {
    //            _buildingUniqueName = "hospital";
    //        }
    //    }

    //    #endregion

    //    #region Food

    //    public class MarketBuildingSkin : BuildingSkin
    //    {
    //        public MarketBuildingSkin()
    //        {
    //            _removalProcedure = Procedures.rp_market;
    //            _replaceProcedure = delegate (Building b) { Procedures.rep_market(b, this); };

    //            _buildingUniqueName = "market";
    //        }

    //        /// <summary>
    //        /// The gameobject that will replace the base model; will be instantiated at runtime
    //        /// </summary>
    //        public GameObject model;
    //        /// <summary>
    //        /// Optional; creates visual stacks of resources at the specified positions, does not affect actual resource consumption or productions, 
    //        /// but should match the max storage of the building to make it visually accurate. 
    //        /// If left null this field will be set to its default value; 
    //        /// </summary>
    //        public ResourceStacks resourceStacks = null;
    //        /// <summary>
    //        /// Optional; the place resources will be left in most scenarios the resourceStacks are full; 
    //        /// If left null this field will be set to its default value; 
    //        /// </summary>
    //        public Transform resourceDropoff = null;
    //        /// <summary>
    //        /// Optional; the positions peasants stand while working at the building; 
    //        /// If left null this field will be set to its default value; 
    //        /// </summary>
    //        public Transform[] personPositions = null;
    //    }




    //    #endregion

    //    #region Path-Types

    //    #region Generic

    //    public class PathTypeBuildingSkin : BuildingSkin
    //    {
    //        public PathTypeBuildingSkin()
    //        {
    //            _removalProcedure = delegate (Building b) { Procedures.rp_path(b); };
    //            _replaceProcedure = delegate (Building b) { Procedures.rep_path(b, this); };
    //        }

    //        public GameObject Straight;
    //        public GameObject Elbow;
    //        public GameObject Threeway;
    //        public GameObject Fourway;
    //    }

    //    #endregion

    //    #region Roads

    //    public class RoadBuildingSkin : PathTypeBuildingSkin
    //    {
    //        public RoadBuildingSkin()
    //        {
    //            _buildingUniqueName = "road";
    //        }
    //    }

    //    public class StoneRoadBuildingSkin : PathTypeBuildingSkin
    //    {
    //        public StoneRoadBuildingSkin()
    //        {
    //            _buildingUniqueName = "stoneroad";
    //        }
    //    }

    //    #endregion

    //    #region Bridges

    //    public class WoodenBridgeBuildingSkin : PathTypeBuildingSkin
    //    {
    //        public WoodenBridgeBuildingSkin()
    //        {
    //            _buildingUniqueName = "bridge";
    //        }
    //    }

    //    public class StoneBridgeBuildingSkin : PathTypeBuildingSkin
    //    {
    //        public StoneBridgeBuildingSkin()
    //        {
    //            _buildingUniqueName = "stonebridge";
    //        }
    //    }

    //    #endregion

    //    #region Garden

    //    public class GardenBuildingSkin : BuildingSkin
    //    {
    //        public GardenBuildingSkin()
    //        {
    //            _removalProcedure = delegate (Building b) { Procedures.rp_garden(b); };
    //            _replaceProcedure = delegate (Building b) { Procedures.rep_garden(b, this); };

    //            _buildingUniqueName = "garden";
    //        }

    //        public GameObject Straight;
    //        public GameObject Elbow;
    //        public GameObject Threeway;
    //        public GameObject Fourway;
    //        public GameObject Fourway_Special;

    //        public Mesh Straight_flowers;
    //        public Mesh Elbow_flowers;
    //        public Mesh Threeway_flowers;
    //        public Mesh Fourway_flowers;
    //        public Mesh Fourway_Special_flowers;
    //    }


    //    #endregion

    //    #endregion

    //#endregion

}
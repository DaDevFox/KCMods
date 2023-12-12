using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using UnityEngine;
using UnityEngine.UI;
using Harmony;

namespace InsaneDifficultyMod.Events
{
    public class BuildingMeta
    {
        // TODO: use reflection to make fill styles population automated
        private static Dictionary<string, BuildingFillStyle> fillStyles = new Dictionary<string, BuildingFillStyle>()
        {
            { "jobPositions", new JobPositionsFill() },
            { "perimeter", new PerimeterFill() },
            { "boundsGrid", new BoundsGridFill() }
        };

        public string uniqueID;
        public int priority;
        public float weight;
        public int capacity = 5;

        public string[] fillStyle = { "jobPositions" };

        public BuildingMeta(string uniqueID, int priority = 0, float weight = 1f, int capacity = 5, params string[] fillStyle)
        {
            this.uniqueID = uniqueID;
            this.priority = priority;
            this.weight = weight;
            this.capacity = capacity;

            if (fillStyle.Length > 0)
                this.fillStyle = fillStyle;
        }

        public BuildingMeta Clone()
        {
            return new BuildingMeta(uniqueID)
            {
                priority = this.priority,
                weight = this.weight,
                capacity = this.capacity,
                fillStyle = this.fillStyle
            };
        }

        public Vector3 FillNext(Building building, int filled, int max)
        {
            return fillStyles[fillStyle[0]].Fill(building, filled, max);
        }
    }

    #region Filling Styles

    public abstract class BuildingFillStyle
    {
        public abstract string id { get; }

        /// <summary>
        /// Gets the position to fill the next spot in the building, given the amount already filled
        /// </summary>
        /// <param name="building"></param>
        /// <param name="filled"></param>
        /// <returns></returns>
        public abstract Vector3 Fill(Building building, int filled, int max);
    }

    public class JobPositionsFill : BuildingFillStyle
    {
        public override string id => "jobPositions";

        public override Vector3 Fill(Building building, int filled, int max)
        {
            if (building == null)
                return new Vector3(0.5f, 0.5f, 0.5f);

            if (max >= building.personPositions.Length || (building.personPositions.Length > filled && building.personPositions[filled] == null))
            {
                Vector3 result = building.GetBounds() / 2f;
                building.GetBoundingFootprint(out int minX, out int minZ, out int maxX, out int maxZ);

                return new Vector3((float)minX + result.x, 0f, (float)minZ + result.z);
            }

            return building.personPositions[filled].position;
        }
    }

    public class PerimeterFill : BuildingFillStyle
    {
        public override string id => "perimeter";

        // TODO: use grids for special formations, nondivisible by four
        /// <summary>
        /// Special hardcoded exception formations
        /// </summary>
        private static Dictionary<int, Vector3[]> grids = new Dictionary<int, Vector3[]>()
        {
            {1,  new Vector3[] { 
                    new Vector3(0f, 0f, -1f) } },
            {2, new Vector3[] {
                    new Vector3(0.5f, 0f, 0f),
                    new Vector3(0.5f, 0f, 1f) }},
            {3, new Vector3[] {
                    new Vector3(1f, 0f, 1f),
                    new Vector3(0f, 0f, 1f),
                    new Vector3(0.5f, 0f, 0f) } },
            { 4, new Vector3[] {
                    new Vector3(1f, 0f, 1f),
                    new Vector3(0f, 0f, 1f),
                    new Vector3(1f, 0f, 0f),
                    new Vector3(0f, 0f, 0f), } },
            { 5, new Vector3[] {
                    new Vector3(1f, 0f, 1f),
                    new Vector3(0f, 0f, 1f),
                    new Vector3(1f, 0f, 0f),
                    new Vector3(0f, 0f, 0f),
                    new Vector3(0.5f, 0f, 0f) } },

            { 6, new Vector3[] {
                    new Vector3(1f, 0f, 1f),
                    new Vector3(0f, 0f, 1f),
                    new Vector3(1f, 0f, 0f),
                    new Vector3(0f, 0f, 0f),
                    new Vector3(0.5f, 0f, 0f),
                    new Vector3(0.5f, 0f, 1f) } },
            { 7, new Vector3[] {
                    new Vector3(1f, 0f, 1f),
                    new Vector3(0f, 0f, 1f),
                    new Vector3(1f, 0f, 0f),
                    new Vector3(0f, 0f, 0f),
                    new Vector3(0.75f, 0f, 1f),
                    new Vector3(0.25f, 0f, 1f),
                    new Vector3(0.5f, 0f, 0f), } },

            { 8, new Vector3[] {
                    new Vector3(1f, 0f, 1f),
                    new Vector3(0f, 0f, 1f),
                    new Vector3(1f, 0f, 0f),
                    new Vector3(0f, 0f, 0f),
                    new Vector3(0.5f, 0f, 1f),
                    new Vector3(0f, 0f, 0.5f),
                    new Vector3(0.5f, 0f, 0f),
                    new Vector3(1f, 0f, 0.5f), } },

        };

        private static Vector3[] sides = new Vector3[]
        {
            new Vector3(0.5f, 0f, 0f)
        };

        public override Vector3 Fill(Building building, int filled, int max)
        {
            Vector3 result = Vector3.zero;

            if (grids.ContainsKey(max))
            {
                if (grids[max].Length > filled)
                {
                    Vector3 gridLoc = grids[max][filled];

                    float x = building.GetBounds().x * gridLoc.x;
                    float y = building.GetBounds().y * gridLoc.y;
                    float z = building.GetBounds().z * gridLoc.z;
                    return new Vector3(x, y, z);
                }
            }
            else if(max % 4 == 0) // case max = 12, filled = 7
            {
                int numPerSide = max / 4; // 3
                int side = filled / numPerSide; // [integer math] 7 / 3 = 2

                if(side == 0)
                {
                    result.x = 1f * building.GetBounds().x * ((float)(filled - (side * numPerSide)) / (float)numPerSide) + (building.GetBounds().x / (float)numPerSide) / 2f;
                }
                else if (side == 1)
                {
                    result.z = 1f * building.GetBounds().z * ((float)(filled - (side * numPerSide)) / (float)numPerSide) + (building.GetBounds().z / (float)numPerSide) / 2f;
                }
                else if (side == 2) // this executes
                {
                    // case building bounds = (2, 2)
                    result.x = 1f * building.GetBounds().x * ((float)(filled - (side * numPerSide)) / (float)numPerSide) + (building.GetBounds().x / (float)numPerSide) / 2f;
                    // 1f * 2f * ((7 - (2 * 3)) / 3) = 2f * (1 / 3f) = 2f * 0.333 = 0.666667
                    result.z = 1f * building.GetBounds().z;
                }
                else if (side == 3)
                {
                    result.z = 1f * building.GetBounds().z * ((float)(filled - (side * numPerSide)) / (float)numPerSide) + (building.GetBounds().z / (float)numPerSide) / 2f;
                    result.x = 1f * building.GetBounds().z;
                }
            }

            building.GetBoundingFootprint(out int minX, out int minZ, out int maxX, out int maxZ);

            return new Vector3((float)minX + result.x, 0f, (float)minZ + result.z);
        }
    }

    /// <summary>
    /// Fills the positions with a grid inside the buildings bounds
    /// Currently only works for square buildings
    /// </summary>
    public class BoundsGridFill : BuildingFillStyle
    {
        public override string id => "boundsGrid";
        public override Vector3 Fill(Building building, int filled, int max)
        {
            Vector3 result = Vector3.zero;

            float sideLength = Mathf.Sqrt(max);
            if (sideLength % 1 == 0)
            {
                float x = (float)filled % sideLength;
                float z = (float)(filled / (int)sideLength);

                x /= sideLength;
                z /= sideLength;

                result = new Vector3(x * building.GetBounds().x, 0f, z * building.GetBounds().z);
            }

            result += Vector3.one * 0.25f;

            building.GetBoundingFootprint(out int minX, out int minZ, out int maxX, out int maxZ);

            return new Vector3((float)minX + result.x, 0f, (float)minZ + result.z);
        }
    }

    #endregion


    public class RiotSystem : IDModEvent
    {
        public static int happinessThreshold { get; } = 60;
        private int accumTime = 0;
        private int timeThreshold = 3;

        /// <summary>
        /// Priority list of metas to check when finding rally points for a riots
        /// </summary>
        public static List<BuildingMeta> metas { get; } = new List<BuildingMeta>()
        {
            // Middle-class Community Centers
            new BuildingMeta("townsquare",  8, 3f, 25,  "boundsGrid"),
            new BuildingMeta("market",      8, 3f, 12,  "perimeter"),
            new BuildingMeta("smallmarket", 8, 3f, 3),

            new BuildingMeta("tavern",      7, 3f, 4),
            new BuildingMeta("church",      7, 3f, 4),
            new BuildingMeta("firehouse",   7, 3f, 5),
            new BuildingMeta("baker",       7, 3f, 3),

            // Symbols of Authority
            new BuildingMeta("statue_levi",     6, 3f, 20, "perimeter"),
            new BuildingMeta("statue_sam",      6, 3f, 20, "perimeter"),
            new BuildingMeta("statue_barbara",  6, 3f, 20, "perimeter"),

            // Supporting Production Buildings
            new BuildingMeta("blacksmith",      5, 3f, 5),
            new BuildingMeta("butcher",         5, 3f, 7),
            new BuildingMeta("charcoalmaker",   5, 3f, 3),
            new BuildingMeta("dock",            5, 3f, 10),
            new BuildingMeta("mason",           5, 3f, 2),
            new BuildingMeta("fishmonger",      5, 3f, 7),
            new BuildingMeta("windmill",        5, 3f, 2),

            // Storage Buildings
            new BuildingMeta("largestockpile",  4, 3f, 12, "perimeter"),
            new BuildingMeta("smallstockpile",  4, 3f, 2),
            new BuildingMeta("largegranary",    4, 3f, 12, "perimeter"),
            new BuildingMeta("smallgranary",    4, 3f, 2),
            new BuildingMeta("producestand",    4, 3f, 1),

            // Core Production Buildings
            new BuildingMeta("farm",        3, 3f, 1),
            new BuildingMeta("orchard",     3, 3f, 12, "perimeter"),
            new BuildingMeta("swineherd",   3, 3f, 6),
            new BuildingMeta("fishinghut",  3, 3f, 5),
            new BuildingMeta("forester",    3, 3f, 4),
            new BuildingMeta("quarry",      3, 3f, 4),
            new BuildingMeta("ironmine",    3, 3f, 6),
            
            // Cemeteries
            new BuildingMeta("cemeterykeeper",  2, 3f, 1),
            new BuildingMeta("cemeteryDiamond", 2, 3f, 1),
            new BuildingMeta("cemeteryCircle",  2, 3f, 1),
            new BuildingMeta("cemetery44",      2, 3f, 1),
            new BuildingMeta("cemetery",        2, 3f, 1),


            // Upper-class Community Centers
            new BuildingMeta("hospital",    2, 3f, 10),
            new BuildingMeta("clinic",      2, 3f, 2),
            new BuildingMeta("library",     2, 3f, 6),
            new BuildingMeta("bathhouse",   2, 3f, 8),

            // water System
            new BuildingMeta("noria",       2, 3f, 5),
            new BuildingMeta("aqueduct",    2, 3f, 1),
            new BuildingMeta("reservoir",   2, 3f, 1),

            // Castle Defenses
            new BuildingMeta("woodengate",  1, 3f, 4, "perimeter"),
            new BuildingMeta("gate",        1, 3f, 4, "perimeter"),
            new BuildingMeta("castlestairs",1, 3f, 4, "perimeter"),
            new BuildingMeta("archer",      1, 3f, 2),
            new BuildingMeta("ballista",    1, 3f, 4),

            // Castle Buildings
            new BuildingMeta("throneroom",      1, 3f, 5),
            new BuildingMeta("chamberofwar",    1, 3f, 5),
            new BuildingMeta("greathall",       1, 3f, 3),
            new BuildingMeta("barracks",        1, 3f, 8),
            new BuildingMeta("archerschool",    1, 3f, 8),
            new BuildingMeta("greatlibrary",    1, 3f, 20),
            new BuildingMeta("cathedral",       1, 3f, 20),
        };

        private static Dictionary<Guid, float> dissatisfaction = new Dictionary<Guid, float>();
        public static Dictionary<int, Riot> landmassRiots = new Dictionary<int, Riot>();

        public static float fear = 20f;

        public static float averageDissatisfaction => 1f;
        public static int popForMarch { get; private set; } = 100;

        public override void Init()
        {
            base.Init();

            for (int i = 0; i < Player.inst.Residentials.Count; i++)
                dissatisfaction.Add((Player.inst.Residentials.data[i].GetComponent<Building>()).guid, 0f);

            testFrequency = 1;

            saveID = "riot";
            saveObject = typeof(RiotEventSaveData);
        }

        public static void Iterate()
        {

            foreach (Riot riot in landmassRiots.Values)
                riot.Iterate();

            for (int i = 0; i < Player.inst.Residentials.Count; i++)
            {
                IResidence residence = Player.inst.Residentials.data[i];
                Building building = ((MonoBehaviour)residence).GetComponent<Building>();
                Guid guid = building.guid;

                if(!dissatisfaction.ContainsKey(guid))
                    dissatisfaction.Add(guid, 0f);

                if(Player.inst.Residentials.data[i].GetHappiness() < happinessThreshold)
                {
                    List<Villager> residents = residence.GetResidents();
                    //int amount = (int)Util.LinearWeightedRandom(1f, residents.Count);
                    //List<Villager> toAdd = new List<Villager>();

                    //for (int j = 0; j < amount; j++)
                    //    toAdd.Add(residents[i]);

                    foreach(Villager target in residents)
                    {
                        DebugExt.dLog(target.name, true);
                        Riot riot = FindRiot(target);
                        if (riot.Add(target))
                        {
                            SetPersonForRiot(target);
                            DebugExt.dLog("confirmed", true);
                        }
                    }
                }
            }
        }

        public static void EndAll()
        {
            foreach (Riot riot in landmassRiots.Values)
                riot.End();
        }

        public static void Update()
        {
            foreach (Riot riot in landmassRiots.Values)
                riot.Update();
        }

        private static void SetPersonForRiot(Villager person)
        {
            //Player.instance.RemovePersonFromWorld(person);
            //person.paralyzed = true;
            //person.QuitJob(false);
            person.LeaveHome();
            //Player.instance.Workers.Remove(person);
            //Player.instance.Homeless.Remove(person);
        }

        public static BuildingMeta GetMeta(string uniqueName)
        {
            for (int i = 0; i < metas.Count; i++)
                if (metas[i].uniqueID == uniqueName)
                    return metas[i];
            return null;
        }

        public static Riot GetRiot(int landmass) => landmassRiots.ContainsKey(landmass) ? landmassRiots[landmass] : null;


        private static Riot FindRiot(Villager villager)
        {
            if (villager.landMass == -1)
                return null;

            if (!landmassRiots.ContainsKey(villager.landMass))
                landmassRiots.Add(villager.landMass, new Riot(villager.landMass));
            return landmassRiots[villager.landMass];
        }

        /// <summary>
        /// Resets the riot completely after it's <seealso cref="Riot.End"/> method has been called
        /// </summary>
        /// <param name="riot"></param>
        public static void NotifyRiotEnd(Riot riot)
        {
            landmassRiots.Remove(riot.landmass);
            landmassRiots.Add(riot.landmass, new Riot(riot.landmass));
        }


        public override bool Test()
        {
            base.Test();

            if (Player.inst.KingdomHappiness < happinessThreshold)
            {
                accumTime += 1;
            }
            else 
            {
                accumTime = 0;
            }
            if (accumTime > timeThreshold) 
            {
                accumTime = 0;
                return true;
            }
            return false;
        }


        #region Load/Save


        public class RiotEventSaveData
        {
            public int accumTime;
            public List<RiotSpawn.RiotSpawnSaveData> riots;
        }


        //public override object OnSave()
        //{
        //    RiotEventSaveData data = new RiotEventSaveData();

        //    data.accumTime = this.accumTime;
        //    foreach(RiotSpawn riot in riots)
        //    {
        //        data.riots.Add(riot.GetSaveData());
        //    }
        //    return data;
        //}


        //public override void OnLoaded(object saveData)
        //{
        //    base.OnLoaded(saveData);
        //    RiotEventSaveData data = saveData as RiotEventSaveData;

        //    this.accumTime = data.accumTime;
        //    this.riots.Clear();
        //    foreach(RiotSpawn.RiotSpawnSaveData riotData in data.riots)
        //    {
        //        this.riots.Add(new RiotSpawn(riotData));
        //    }

        //}


        #endregion

    }

    public class Riot
    {
        public enum Stage
        {
            Collecting,
            Marching
        }

        public int count => villagers.Count;

        public Assets.Code.ResourceAmount demand;

        public int landmass;
        public Stage stage = Stage.Collecting;
        private List<Villager> villagers = new List<Villager>();
        private List<VillagerMeta> villagerMetas = new List<VillagerMeta>();

        private List<int> idleVillagers = new List<int>();

        public List<RiotBuildingMeta> orderedBuildings = new List<RiotBuildingMeta>();
        public Dictionary<Guid, RiotBuildingMeta> keyedBuildings = new Dictionary<Guid, RiotBuildingMeta>();

        public List<UnitSystem.Army> armies = new List<UnitSystem.Army>();

        public Color color = Color.red;
        public Material peasantMat;

        private static Texture riotImage;

        public RiotRallyMarker marker;

        public Riot(int landmass)
        {
            this.landmass = landmass;
            peasantMat = new Material(Shader.Find("Standard"));
            peasantMat.color = color;

            riotImage = Mod.assets.GetByName<Texture>("tb_riot");
        }

        public void End()
        {
            // TODO: fix 'ghost rioters'
            foreach(Villager villager in villagers)
            {
                Player.inst.ReturnPerson(villager, villager.Pos);
                //villager.body.GetComponent<MeshRenderer>().material = VillagerSystem.instance.bodyColors[SRand.Range(0, VillagerSystem.instance.bodyColors.Length)];
            }
            foreach(RiotBuildingMeta meta in orderedBuildings)
            {
                EnableBuilding(meta.target);
                meta.villagers.Clear();
                meta.indices.Clear();
            }

            villagers.Clear();
            RiotSystem.NotifyRiotEnd(this);
        }

        public void Iterate()
        {
            ReformatBuildings();

            // TODO: Fix riot marching phase
            if (villagers.Count > RiotSystem.popForMarch)
            {
                BeginMarch();
            }


            //for(int i = 0; i < villagers.Count; i++)
            //{
            //    VillagerMeta meta = villagerMetas[i];
            //    if (meta.location != null)
            //    {
            //        int index = meta.location.IndexOf(villagers[i]);

            //        Vector3 loc = meta.location.Reference.FillNext(meta.location.target, index, meta.location.capacity);
            //        villagers[i].MoveToDeferred(loc);
            //    }
            //}

        }

        public void ReformatBuildings(bool force = false)
        {
            ArrayExt<Building> buildings = Player.inst.GetBuildingListForLandMass(landmass);
            List<KeyValuePair<Guid, RiotBuildingMeta>> toRemove = new List<KeyValuePair<Guid, RiotBuildingMeta>>();
            foreach (var meta in keyedBuildings)
                if (!buildings.Contains(meta.Value.target))
                    toRemove.Add(meta);

            foreach (var pair in toRemove)
            {
                keyedBuildings.Remove(pair.Key);
                orderedBuildings.Remove(pair.Value);

                for (int i = 0; i < pair.Value.villagers.Count; i++)
                    villagerMetas[pair.Value.indices[i]].location = null;
                pair.Value.villagers.Clear();
                pair.Value.indices.Clear();
            }

            for (int i = 0; i < buildings.Count; i++)
            {
                if (!keyedBuildings.ContainsKey(buildings.data[i].guid))
                {
                    keyedBuildings.Add(buildings.data[i].guid, new RiotBuildingMeta() { riot = this, target = buildings.data[i] });
                    orderedBuildings.Add(keyedBuildings[buildings.data[i].guid]);
                }
            }

            orderedBuildings.Sort((meta, other) =>
            {
                if (meta == null || other == null)
                    return 0;
                if (meta.Reference != null && other.Reference != null)
                    return other.priority - meta.priority;
                return 0;
            });

            ReformatAssignments(force);
            SetMarker();
        }

        public void SetMarker()
        {
            if (marker == null)
            {
                GameObject markerPrefab = Mod.assets.GetByName<GameObject>("RiotRallyMarker.prefab");
                if (markerPrefab != null)
                {
                    GameObject rallyMarkerGO = GameObject.Instantiate(markerPrefab, Vector3.zero, Quaternion.identity);
                    marker = rallyMarkerGO.AddComponent<RiotRallyMarker>();
                    marker.riot = this;
                }
                else if (Settings.debug)
                    Mod.dLog("marker prefab not loaded");
            }

            RiotBuildingMeta main = null;
            foreach (RiotBuildingMeta building in orderedBuildings)
                if (main == null || building.capacity * building.Reference.weight > main.capacity * main.Reference.weight)
                    main = building;

            float offset = 2f;
            marker.transform.position = main.target.GetPos() + Vector3.up * offset;
            marker.basePosition = main.target.GetPos();

            DebugExt.Log("marker", false, KingdomLog.LogStatus.Neutral, marker.transform.position);
        }

        [HarmonyPatch(typeof(UnitSystem), "InitCategoriesGen")]
        class CategoriesGenPatch
        {
            static void Postfix(UnitSystem __instance)
            {
                for (int i = 0; i < __instance.playerArmySet.Count; i++)
                {
                    UnitSystem.ArmyDef armyDef = __instance.playerArmySet[i];

                    UnitSystem.UnitCategory unitCategory = new UnitSystem.UnitCategory();
                    unitCategory.weaponLocalOffset = armyDef.weaponLocalOffset;
                    unitCategory.mesh = armyDef.mesh;
                    unitCategory.weaponMesh = armyDef.weaponMesh;
                    unitCategory.life = armyDef.unitLife;
                    unitCategory.generalLife = armyDef.generalLife;
                    unitCategory.teamId = 6;
                    unitCategory.armyDef = armyDef;
                    unitCategory.type = armyDef.type;
                    
                    List<UnitSystem.UnitCategory> categories = (List<UnitSystem.UnitCategory>)typeof(UnitSystem).GetField("unitCategoriesGen", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(__instance);
                    categories.Add(unitCategory);
                    typeof(UnitSystem).GetField("unitCategoriesGen", BindingFlags.Instance | BindingFlags.NonPublic).SetValue(__instance, categories);
                }

                List<UnitSystem.UnitCategory> _categories = (List<UnitSystem.UnitCategory>)typeof(UnitSystem).GetField("unitCategoriesGen", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(__instance);

                for (int l = 0; l < _categories.Count; l++)
                {
                    for (int m = 0; m < 1023; m++)
                    {
                        _categories[l].units[m] = new UnitSystem.Unit();
                        _categories[l].units[m].pos = new Vector3(SRand.Range(5f, 15f), 0f, SRand.Range(5f, 15f));

                        typeof(UnitSystem).GetMethod("ResetUnit", BindingFlags.Instance | BindingFlags.NonPublic).Invoke(__instance, new object[] { _categories[l].units[m] });
                    }
                }

                typeof(UnitSystem).GetField("unitCategoriesGen", BindingFlags.Instance | BindingFlags.NonPublic).SetValue(__instance, _categories);
            }
        }

        public void BeginMarch()
        {
            int armyUnit = 10;
            int minBuildingCapacity = 6;

            stage = Stage.Marching;

            //ReformatAssignments(true);

            Mod.dLog(orderedBuildings.Count);

            bool proceed = true;
            int buildingIdx = 0;
            while (proceed)
            {
                Mod.dLog(buildingIdx);

                if (buildingIdx >= orderedBuildings.Count)
                    break;

                RiotBuildingMeta next = orderedBuildings[buildingIdx];
                if (villagers.Count >= Math.Max(armyUnit, next.capacity))
                {
                    if (next.capacity > minBuildingCapacity && next.villagers.Count > 0)
                    {
                        UnitSystem.Army army = UnitSystem.inst.MakeArmy(orderedBuildings[buildingIdx].target.GetPos(), AIBrainsContainer.inst.kingdoms.Count + 2, UnitSystem.ArmyType.Default, true);

                        int count = next.villagers.Count;
                        for (int k = 0; k < count; k++)
                        {
                            RemoveVillager(next.indices[k]);
                        }

                        EnableBuilding(next.target);
                        armies.Add(army);
                    }

                    buildingIdx++;
                }
                else
                    proceed = false;
            }

            Mod.mod.StartCoroutine(MarchCoroutine(true));

            //ReformatAssignments(true);
        }

        public IEnumerator MarchCoroutine(bool stopIfAnyReach = false)
        {
            float beginwait = 1f;
            Building keep = Player.inst.keep.GetComponent<Building>();
            Cell origin = keep.GetCell();

            // Random attack timing
            float[] times = new float[armies.Count];
            for (int i = 0; i < armies.Count; i++)
                times[i] = SRand.Range(0f, 3f);

            // Target Queues
            List<Cell> searchSpace = World.inst.cellsToLandmass[landmass].data.ToList();
            //World.instance.GetNonBusyCellInRadius(origin.Center, 6f, ref searchSpace);

            List<Building>[] targetQueues = new List<Building>[armies.Count];
            for (int i = 0; i < armies.Count; i++) 
            {
                int amount = SRand.Range(1, 4);
                for (int j = 0; j < amount; j++) 
                {
                    Cell best = null;
                    float highest = -1f;

                    for(int k = 0; k < searchSpace.Count; k++)
                    {
                        Cell cell = searchSpace[k];
                        float value = 0f;

                        if (cell.OccupyingStructure.Count > 0)
                        {
                            Building considering = cell.BottomStructure;

                            // TODO: Rioteres target government buildings using weighted dictionary
                            value = 1f + (SRand.Range(0.5f, 1f) * Vector3.Distance(cell.Center, origin.Center)) + considering.ModifiedMaxLife;

                            if (value > highest)
                            {
                                highest = value;
                                best = cell;
                            }
                        }
                    }
                    
                    if(highest != -1f)
                        targetQueues[i].Add(best.BottomStructure);
                }
                targetQueues[i].Add(keep);

                yield return new WaitForEndOfFrame();
            }

            yield return new WaitForSeconds(beginwait);

            float elapsed = 0f;
            bool[] allowMoving = new bool[armies.Count];
            bool[] completed = new bool[armies.Count];
            while (elapsed < 4f)
            {
                for (int i = 0; i < times.Length; i++)
                {
                    if (elapsed > times[i] && !allowMoving[i])
                    {
                        MarchStep(armies[i], targetQueues[i]);
                        allowMoving[i] = true;
                    }
                }
                elapsed += Time.deltaTime;
                yield return new WaitForEndOfFrame();
            }

            bool anyComplete = false;
            bool allComplete = false;
            while ((!anyComplete && stopIfAnyReach) || allComplete)
            {
                allComplete = true;

                for (int i = 0; i < armies.Count; i++)
                {
                    if (MarchStep(armies[i], targetQueues[i]))
                    {
                        completed[i] = true;
                        anyComplete = true;
                    }
                    else
                        allComplete = false;
                }

                yield return new WaitForEndOfFrame();
            }


            KingdomLog.TryLog("marchcomplete", "March to keep has completed", KingdomLog.LogStatus.Important);
        }

        public bool MarchStep(UnitSystem.Army unit, List<Building> queue)
        {
            float threshold = 0.5f;

            if (queue.Count > 0)
            {
                if ((unit.GetPos() - unit.GetDestPos()).sqrMagnitude < threshold * threshold)
                    queue.RemoveAt(0);


                if ((Building)unit.moveTarget != queue[0])
                {
                    OrdersManager.inst.MoveTo(unit, queue[0]);
                }

                return false;
            }
            else
                return true;
        }

        public void Update()
        {
            for (int i = 0; i < villagers.Count; i++)
            {
                //float delta = Time.deltaTime * Player.instance.timeScale;
                Villager villager = villagers[i];

                //villager.textThought = "<color=red>Rioting!</color>";
                //villager.UpdateMind(delta);

                //typeof(Villager).GetMethod("ConsumePath", BindingFlags.Instance | BindingFlags.NonPublic).Invoke(villager, new object[0]);
                //typeof(Villager).GetMethod("WalkPath", BindingFlags.Instance | BindingFlags.NonPublic).Invoke(villager, new object[] { delta });
                //typeof(Villager).GetMethod("UpdateAnim", BindingFlags.Instance | BindingFlags.NonPublic).Invoke(villager, new object[] { delta });

                //float fixedTime = (float)typeof(Villager).GetField("fixedTime", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(villager);
                //typeof(Villager).GetField("fixedTime", BindingFlags.Instance | BindingFlags.NonPublic).SetValue(villager, fixedTime + delta);

                //villager.UpdateSpeed();

                villager.MeshType = VillagerSystem.MeshType.Axeman;
                villager.thought.thought = ThoughtBubbleSystem.Thought.GeneralError;

                //if (fixedTime + delta > 0.5f)
                //{

                //    //MaterialPropertyBlock block = (MaterialPropertyBlock)typeof(ThoughtBubble).GetField("block", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(villager.thought);
                //    //block.SetTexture("_MainTex", riotImage);
                //    //villager.thought.meshRenderer.SetPropertyBlock(block);

                //    typeof(Villager).GetMethod("UpdateSkills", BindingFlags.Instance | BindingFlags.NonPublic).Invoke(villager, new object[0]);
                //    typeof(Villager).GetField("fixedTime", BindingFlags.Instance | BindingFlags.NonPublic).SetValue(villager, (fixedTime + delta) % 0.5f);
                //}
            }
        }

        public bool Add(Villager villager)
        {
            RiotBuildingMeta available = GetNextAvailable();

            if (villagers.Contains(villager))
                DebugExt.Log("Repeat");

            if (available != null && !villagers.Contains(villager)) {
                villagers.Add(villager);
                villagerMetas.Add(new VillagerMeta(villager, available));
                available.Add(villager, villagers.Count - 1);

                //villager.textThought = "<color=red>Rioting!</color>";
                //villager.body.GetComponent<MeshRenderer>().material = peasantMat;

                Vector3 loc = available.Reference.FillNext(available.target, available.villagers.Count, available.capacity);
                villager.MoveToDeferred(loc);

                DebugExt.Log($"Riot size: {villagers.Count}");

                return true;
            }

            return false;
        }

        

        public void ReformatAssignments(bool force = false)
        {
            if (force)
            {
                for (int i = 0; i < villagers.Count; i++)
                {
                    if (villagerMetas[i].location != null)
                    {
                        villagerMetas[i].location.villagers.Remove(villagers[i]);
                        villagerMetas[i].location.indices.Remove(i);
                        villagerMetas[i].location = null;
                    }
                }
            }

            for(int i = 0; i < villagers.Count; i++)
            {
                if (villagerMetas[i].location == null)
                {
                    RiotBuildingMeta available = GetNextAvailable();

                    if (available != null)
                    {
                        if (idleVillagers.Contains(i))
                            idleVillagers.Remove(i);

                        available.Add(villagers[i], villagers.Count - 1);

                        Vector3 loc = available.Reference.FillNext(available.target, available.villagers.Count, available.capacity);
                        villagers[i].MoveToDeferred(loc);
                    }
                    else
                    {
                        idleVillagers.Add(i);
                    }
                }
            }
        }

        public RiotBuildingMeta GetNextAvailable()
        {
            for(int i = 0; i < this.orderedBuildings.Count; i++)
            {
                RiotBuildingMeta meta = this.orderedBuildings[i];

                DebugExt.Log(meta.capacity.ToString());

                if (meta.villagers.Count < meta.capacity)
                    return meta;
            }


            return null;
        }

        public void RemoveVillager(int index, bool returnToPlayer = false)
        {
            RiotBuildingMeta meta = villagerMetas[index].location;
            Villager villager = villagers[index];
            villagers.Remove(villager);

            if (meta != null)
            {
                meta.villagers.Remove(villager);
                meta.indices.Remove(index);
            }

            villagerMetas.RemoveAt(index);

            ShiftIndicesLeft(index);

            if (returnToPlayer)
                Player.inst.ReturnPerson(villager, villager.Pos);
            else
                World.inst.RemoveVillagerFromLandmass(villager);
        }

        public void ShiftIndicesLeft(int displaced)
        {
            foreach (RiotBuildingMeta meta in orderedBuildings)
                for (int i = 0; i < meta.indices.Count; i++)
                    if(meta.indices[i] > displaced)
                        meta.indices[i] = meta.indices[i] - 1;
        }

        public static void DisableBuilding(Building building)
        {
            KingdomLog.TryLog("building_takeover", $"My lord, some rioters have taken over a {building.FriendlyName} and are gathering there", KingdomLog.LogStatus.Warning, 1f, building.transform.position);

            building.Open = false;
            //building.PauseYield = true;
        }

        public static void EnableBuilding(Building building)
        {

            building.Open = true;
            //building.PauseYield = false;
        }

        public class VillagerMeta
        {
            public Villager target;
            public RiotBuildingMeta location;

            public VillagerMeta(Villager target, RiotBuildingMeta location = null)
            {
                this.target = target;
                this.location = location;
            }
        }

        public class RiotBuildingMeta : IEmployer
        {
            public List<Villager> villagers { get; } = new List<Villager>();
            public List<int> indices { get; } = new List<int>();
            public Building target;
            private BuildingMeta reference = null;
            public BuildingMeta Reference
            {
                get
                {
                    if (reference == null)
                        reference = RiotSystem.GetMeta(target.UniqueName);

                    return reference;
                }
            }

            public int priority
            {
                get
                {
                    //bool flag;

                    //Cell tCell = target.GetCell();
                    //Cell nearest = World.instance.FindBestSuitedCell(target.GetCell(), false, 2, (cell) => 100 - RadiusBonus.GetHappinessAt(tCell.x, tCell.z, out flag));
                    //float happinessWeightage = (100f - (float)RadiusBonus.GetHappinessAt(nearest.x, nearest.z, out flag))/100f;
                    return Reference.priority;
                }
            }

            public Riot riot;

            public int capacity => Reference != null ? Reference.capacity : 0;

            public void Add(Villager villager, int index)
            {
                if(villagers.Count == 0)
                    Riot.DisableBuilding(target);
                this.villagers.Add(villager);
                this.indices.Add(index);

                RioterJob job = new RioterJob(this) { index = index };
                JobSystem.inst.AddNewJob(job);
                job.AssignEmployee(villager);

                DebugExt.Log("recruited rioter", true, KingdomLog.LogStatus.Neutral, villager.Pos);
            }

            public int IndexOf(Villager villager)
            {
                for (int i = 0; i < villagers.Count; i++)
                    if (villagers[i] == villager)
                        return i;
                return -1;
            }

            public Vector3 GetPositionForPerson(Villager p)
            {
                int indexInBuilding = villagers.IndexOf(p);
                return reference.FillNext(target, indexInBuilding, capacity);
            }

            public bool IsOpen()
            {
                return true;
            }

            public void OnAssigned(Villager p)
            {
                
            }

            public void OnUnAssigned(Villager p)
            {
                int indexInBuilding = villagers.IndexOf(p);

                riot.RemoveVillager(indices[indexInBuilding]);

                villagers.RemoveAt(indexInBuilding);
                indices.RemoveAt(indexInBuilding);

                Reformat();

                DebugExt.Log($"Villager left riot: {p.name}", true);
            }

            public void Reformat()
            {
                if(villagers.Count == 0)
                {
                    // TODO: procedure for riot in which all people have died or reallocated
                }


                for(int i = 0; i < villagers.Count; i++)
                {
                    villagers[i].SetWorkPosition(GetPositionForPerson(villagers[i]));
                    villagers[i].MoveToDeferred(GetPositionForPerson(villagers[i]));
                }
            }

            public string GetUsedSkill() => "";

            public int LandMass() => target.LandMass();

            public JobCategory GetJobCategory() => JobCategory.Undefined;
        }

        [HarmonyPatch(typeof(OutputUI), "Update")]
        public class BuildingTextPatch
        {
            static void Postfix(OutputUI __instance)
            {
                Building building = GameUI.inst.GetBuildingSelected();
                Riot riot = RiotSystem.GetRiot(building.LandMass());
                if (riot != null && riot.keyedBuildings.ContainsKey(building.guid) && riot.keyedBuildings[building.guid].villagers.Count > 0)
                {
                    __instance.explanation.text = "<color=red>Building controlled by rioters</color>";
                    GameUI.inst.workerUI.openCheckbox.interactable = false;
                }else
                    GameUI.inst.workerUI.openCheckbox.interactable = true;
            }
        }
    }
}

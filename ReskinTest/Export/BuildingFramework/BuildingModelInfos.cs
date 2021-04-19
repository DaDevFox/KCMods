using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace ReskinEngine
{
    #region BuildingSkin Definitions

    #region Base

    //Base
    public class BuildingSkin
    {
        public delegate void RemovalProcedure(Building b);
        public delegate void ReplaceProcedure(Building b);


        internal RemovalProcedure removalProcedure
        {
            get
            {
                return _removalProcedure;
            }
        }
        internal ReplaceProcedure replaceProcedure
        {
            get
            {
                return _replaceProcedure;
            }
        }

        protected RemovalProcedure _removalProcedure;
        protected ReplaceProcedure _replaceProcedure;


        public string buildingUniqueName
        {
            get
            {
                return _buildingUniqueName;
            }
        }

        protected string _buildingUniqueName = "";

    }

    //Generic
    public class GenericBuildingSkin : BuildingSkin
    {
        public GenericBuildingSkin()
        {
            _removalProcedure = delegate (Building b) { Procedures.rp_generic(b); };
            _replaceProcedure = delegate (Building b) { Procedures.rep_generic(b, this); };
        }


        public GameObject model;
    }

    #endregion

    #region Castle

    //Keep
    public class KeepBuildingSkin : BuildingSkin
    {
        public KeepBuildingSkin()
        {
            _removalProcedure = delegate (Building b) { Procedures.rp_keep(b); };
            _replaceProcedure = delegate (Building b) { Procedures.rep_keep(b, this); };

            _buildingUniqueName = "keep";
        }

        public GameObject keepUpgrade1;
        public GameObject keepUpgrade2;
        public GameObject keepUpgrade3;
        public GameObject keepUpgrade4;
    }

    #region Castle Blocks

    //Castle Block Base
    public class CastleBlockBuildingSkin : BuildingSkin
    {
        public CastleBlockBuildingSkin()
        {
            _removalProcedure = delegate (Building b) { Procedures.rp_castleblock(b); };
            _replaceProcedure = delegate (Building b) { Procedures.rep_castleblock(b, this); };
        }

        /// <summary>
        /// The flat piece without crenelations for a castle block
        /// This is a modular piece. See info.txt for details
        /// </summary>
        public GameObject Open;
        /// <summary>
        /// The piece of a castleblock with all crenelations at the top and no connections
        /// This is a modular piece. See info.txt for details
        /// </summary>
        public GameObject Closed;
        /// <summary>
        /// The piece of a castleblock that only has crenelations on one side
        /// This is a modular piece. See info.txt for details
        /// </summary>
        public GameObject Single;
        /// <summary>
        /// The straight piece of a castle block
        /// This is a modular piece. See info.txt for details
        /// </summary>
        public GameObject Opposite;
        /// <summary>
        /// The corner piece for a castle block
        /// This is a modular piece. See info.txt for details
        /// </summary>
        public GameObject Adjacent;
        /// <summary>
        /// The piece of a castleblock with crenelations on 3 sides
        /// This is a modular piece. See info.txt for details
        /// </summary>
        public GameObject Threeside;

        /// <summary>
        /// The door that appears on a castleblock when it connects to other castleblocks
        /// </summary>
        public GameObject doorPrefab;
    }

    //Wood Castle Block
    public class WoodCastleBlockBuildingSkin : CastleBlockBuildingSkin
    {
        public WoodCastleBlockBuildingSkin()
        {
            _buildingUniqueName = "woodcastleblock";
        }

    }

    //Stone Castle Block
    public class StoneCastleBlockBuildingSkin : CastleBlockBuildingSkin
    {
        public StoneCastleBlockBuildingSkin()
        {
            _buildingUniqueName = "castleblock";
        }
    }

    #endregion

    #region Gates

    //  Gate Base
    public class GateBuildingSkin : BuildingSkin
    {
        public GateBuildingSkin()
        {
            _removalProcedure = delegate (Building b) { Procedures.rp_gate(b); };
            _replaceProcedure = delegate (Building b) { Procedures.rep_gate(b, this); };
        }

        public GameObject gate;
        public GameObject porticulus;
    }

    //Wooden Gate
    public class WoodenGateBuildingSkin : GateBuildingSkin
    {
        public WoodenGateBuildingSkin()
        {
            _buildingUniqueName = "woodengate";
        }

    }

    //Stone Gate
    public class StoneGateBuildingSkin : GateBuildingSkin
    {
        public StoneGateBuildingSkin()
        {
            _buildingUniqueName = "gate";
        }

    }

    #endregion

    //Castle Stairs
    public class CastleStairsBuildingSkin : BuildingSkin
    {
        public CastleStairsBuildingSkin()
        {
            _removalProcedure = delegate (Building b) { Procedures.rp_castlestairs(b); };
            _replaceProcedure = delegate (Building b) { Procedures.rep_castlestairs(b, this); };

            _buildingUniqueName = "castlestairs";
        }

        public GameObject stairsFront;
        public GameObject stairsRight;
        public GameObject stairsDown;
        public GameObject stairsLeft;
    }

    //Archer Tower
    public class ArcherTowerBuildingSkin : BuildingSkin
    {
        public ArcherTowerBuildingSkin()
        {
            _removalProcedure = delegate (Building b) { Procedures.rp_archer(b); };
            _replaceProcedure = delegate (Building b) { Procedures.rep_archer(b, this); };

            _buildingUniqueName = "archer";
        }


        /// <summary>
        /// The main model of the Archer Tower
        /// </summary>
        public GameObject baseModel;
        /// <summary>
        /// An embelishment added to the archer tower when it achieves the veteran status
        /// </summary>
        public GameObject veteranModel;
    }

    //Ballista Tower
    public class BallistaTowerBuildingSkin : BuildingSkin
    {
        public BallistaTowerBuildingSkin()
        {
            _removalProcedure = delegate (Building b) { Procedures.rp_ballista(b); };
            _replaceProcedure = delegate (Building b) { Procedures.rep_ballista(b, this); };

            _buildingUniqueName = "ballista";
        }

        /// <summary>
        /// An embelishment added to the ballista tower when it achieves the veteran status
        /// </summary>
        public GameObject veteranModel;
        /// <summary>
        /// The main model of the Ballista Tower
        /// </summary>
        public GameObject baseModel;
        /// <summary>
        /// The base of the rotational top half of the ballista
        /// </summary>
        public GameObject topBase;
        /// <summary>
        /// The right side arm used to animate the ballista's firing movement
        /// </summary>
        public GameObject armR;
        /// <summary>
        /// The right end of the right arm of the ballista; used for anchoring the right side of the string in animation
        /// </summary>
        public Transform armREnd;
        /// <summary>
        /// The left side arm used to animate the ballista's firing movement
        /// </summary>
        public GameObject armL;
        /// <summary>
        /// The lef end of the left arm of the ballista; used for anchoring the left side of the string in animation
        /// </summary>
        public Transform armLEnd;
        /// <summary>
        /// The right side of the animated string used to pull back and fire the ballista projectile
        /// </summary>
        public GameObject stringR;
        /// <summary>
        /// The left side of the animated string used to pull back and fire the ballista projectile
        /// </summary>
        public GameObject stringL;
        /// <summary>
        /// The projectile fired from the ballista
        /// </summary>
        public GameObject projectile;
        public Transform projectileEnd;
        /// <summary>
        /// A decorative flag on the ballista
        /// </summary>
        public GameObject flag;




    }



    #endregion

    #region Advanced Town

    //Hospital
    public class HospitalBuildingSkin : GenericBuildingSkin
    {
        public HospitalBuildingSkin()
        {
            _buildingUniqueName = "hospital";
        }
    }

    #endregion

    #region Path-Types

    #region Generic

    public class PathTypeBuildingSkin : BuildingSkin
    {
        public PathTypeBuildingSkin()
        {
            _removalProcedure = delegate (Building b) { Procedures.rp_path(b); };
            _replaceProcedure = delegate (Building b) { Procedures.rep_path(b, this); };
        }

        public GameObject Straight;
        public GameObject Elbow;
        public GameObject Threeway;
        public GameObject Fourway;
    }

    #endregion

    #region Roads

    public class RoadBuildingSkin : PathTypeBuildingSkin
    {
        public RoadBuildingSkin()
        {
            _buildingUniqueName = "road";
        }
    }

    public class StoneRoadBuildingSkin : PathTypeBuildingSkin
    {
        public StoneRoadBuildingSkin()
        {
            _buildingUniqueName = "stoneroad";
        }
    }

    #endregion

    #region Bridges

    public class WoodenBridgeBuildingSkin : PathTypeBuildingSkin
    {
        public WoodenBridgeBuildingSkin()
        {
            _buildingUniqueName = "bridge";
        }
    }

    public class StoneBridgeBuildingSkin : PathTypeBuildingSkin
    {
        public StoneBridgeBuildingSkin()
        {
            _buildingUniqueName = "stonebridge";
        }
    }

    #endregion

    #region Garden

    public class GardenBuildingSkin : BuildingSkin
    {
        public GardenBuildingSkin()
        {
            _removalProcedure = delegate (Building b) { Procedures.rp_garden(b); };
            _replaceProcedure = delegate (Building b) { Procedures.rep_garden(b, this); };

            _buildingUniqueName = "garden";
        }

        public GameObject Straight;
        public GameObject Elbow;
        public GameObject Threeway;
        public GameObject Fourway;
        public GameObject Fourway_Special;

        public Mesh Straight_flowers;
        public Mesh Elbow_flowers;
        public Mesh Threeway_flowers;
        public Mesh Fourway_flowers;
        public Mesh Fourway_Special_flowers;
    }


    #endregion

    #endregion

    #endregion

}

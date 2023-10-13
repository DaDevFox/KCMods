using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Elevation.Utils;

namespace Elevation.InfiniteWorlds
{

    // IDEA: Map generation modes: finite and infinite
    // IDEA: For finite map generation, instead of using masked infinite perlin and/or other noise, use same algorithm game uses for finite world generation (circle collision rasterizing), but rasterize on very very large map, and then split into regions. 

    public static class WorldMap
    {
        public static Dictionary<Vector2Int, Region> regions;
        public static WorldGenerator generator { get; } = new FiniteWorldGenerator();
        /// <summary>
        /// The current region, if left null, only overviewing world map
        /// </summary>
        public static Vector2Int? region { get; set; }
        /// <summary>
        /// Maximum/minimum bounds of the world map in regions; if left null, world will be continually and infinitely generated
        /// </summary>
        public static Vector2Int? bounds { get; set; } = null;



        /// <summary>
        /// Initially loads many of the regions on the world map
        /// </summary>
        public static void Generate()
        {

        }



        public class Region
        {
            public static Vector2Int size { get; } = new Vector2Int(100, 100);
            public Metadata data { get; } = new Metadata();
        }
    }

    /// <summary>
    /// A single instance of this type 
    /// </summary>
    public abstract class WorldGenerator
    {
        public abstract Interpreter interpreter { get; }

        /// <summary>
        /// Initializes once when the full world map is generated
        /// </summary>
        public virtual void Init()
        {

        }

        /// <summary>
        /// Generates metadata for a single region to be interpreted
        /// </summary>
        public abstract void Generate(WorldMap.Region region);

        /// <summary>
        /// Interprets the metadata properties assigned by a WorldGenerator on a region and generates that region
        /// </summary>
        public abstract class Interpreter
        {
            /// <summary>
            /// Generates the region provided, don't have to worry about destroying any old regions
            /// </summary>
            /// <param name="region"></param>
            public abstract void Generate(WorldMap.Region region);
        }
    }

    public class FiniteWorldGenerator : WorldGenerator
    {
        // IDEA: Biomes, Snowy = very short summer, very very long winter, mostly barren land (rely on fishing and imports)
        public override Interpreter interpreter => new FiniteWorldInterpreter();
        private MapGenTest mapGen = new GameObject("MapGen").AddComponent<MapGenTest>();

        public override void Init()
        {
            WorldMap.bounds = new Vector2Int(7, 5);;

        }

        public override void Generate(WorldMap.Region region)
        {

        }


        public class FiniteWorldInterpreter : Interpreter
        {
            public override void Generate(WorldMap.Region region)
            {
            }
        }
    }
}

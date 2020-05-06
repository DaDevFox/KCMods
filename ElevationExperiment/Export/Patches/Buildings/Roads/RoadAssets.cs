using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace ElevationExperiment.Assets
{
    public static class RoadAssets
    {
        private static Dictionary<string, GameObject> assets = new Dictionary<string, GameObject>();

        //public static void LoadAssets()
        //{
        //    assets = new Dictionary<string, GameObject>();

        //    #region Stone

        //    //Straight
        //    assets.Add("stone/straight/s1", new GameObject());
        //    assets.Add("stone/straight/s2", new GameObject());

        //    assets.Add("stone/straight/s1_2", new GameObject());

        //    //Elbow
        //    assets.Add("stone/elbow/s1", new GameObject());
        //    assets.Add("stone/elbow/s2", new GameObject());

        //    assets.Add("stone/elbow/s1_2", new GameObject());

        //    //Threeway

        //    assets.Add("stone/threeway/s1", new GameObject());
        //    assets.Add("stone/threeway/s2", new GameObject());
        //    assets.Add("stone/threeway/s3", new GameObject());

        //    //assets["stone/threeway/s1_2"] = new GameObject();
        //    //assets["stone/threeway/s1_3"] = new GameObject();
        //    //assets["stone/threeway/s2_3"] = new GameObject();

        //    //assets["stone/threeway/s1_2_3"] = new GameObject();

        //    //Fourway
        //    //assets["stone/fourway/s1"] = new GameObject();
        //    //assets["stone/fourway/s2"] = new GameObject();
        //    //assets["stone/fourway/s3"] = new GameObject();
        //    //assets["stone/fourway/s4"] = new GameObject();

        //    //assets["stone/fourway/s1_2"] = new GameObject();
        //    //assets["stone/fourway/s1_3"] = new GameObject();
        //    //assets["stone/fourway/s1_4"] = new GameObject();
        //    //assets["stone/fourway/s2_3"] = new GameObject();
        //    //assets["stone/fourway/s2_4"] = new GameObject();
        //    //assets["stone/fourway/s3_4"] = new GameObject();

        //    //assets["stone/fourway/s1_2_3"] = new GameObject();
        //    //assets["stone/fourway/s1_2_4"] = new GameObject();
        //    //assets["stone/fourway/s1_3_4"] = new GameObject();
        //    //assets["stone/fourway/s2_3_4"] = new GameObject();

        //    #endregion

        //    #region Normal

        //    //Straight
        //    //assets["normal/straight/s1"] = new GameObject();
        //    //assets["normal/straight/s2"] = new GameObject();

        //    //assets["normal/straight/s1_2"] = new GameObject();


        //    //Elbow
        //    //assets["normal/elbow/s1"] = new GameObject();
        //    //assets["normal/elbow/s2"] = new GameObject();
        //    //assets["normal/elbow/s1_2"] = new GameObject();

        //    //Threeway
        //    //assets["normal/threeway/s1"] = new GameObject();
        //    //assets["normal/threeway/s2"] = new GameObject();
        //    //assets["normal/threeway/s3"] = new GameObject();

        //    //assets["normal/threeway/s1_2"] = new GameObject();
        //    //assets["normal/threeway/s1_3"] = new GameObject();
        //    //assets["normal/threeway/s2_3"] = new GameObject();

        //    //assets["normal/threeway/s1_2_3"] = new GameObject();

        //    //Fourway
        //    //assets["normal/fourway/s1"] = new GameObject();
        //    //assets["normal/fourway/s2"] = new GameObject();
        //    //assets["normal/fourway/s3"] = new GameObject();
        //    //assets["normal/fourway/s4"] = new GameObject();

        //    //assets["normal/fourway/s1_2"] = new GameObject();
        //    //assets["normal/fourway/s1_3"] = new GameObject();
        //    //assets["normal/fourway/s1_4"] = new GameObject();
        //    //assets["normal/fourway/s2_3"] = new GameObject();
        //    //assets["normal/fourway/s2_4"] = new GameObject();
        //    //assets["normal/fourway/s3_4"] = new GameObject();

        //    //assets["normal/fourway/s1_2_3"] = new GameObject();
        //    //assets["normal/fourway/s1_2_4"] = new GameObject();
        //    //assets["normal/fourway/s1_3_4"] = new GameObject();
        //    //assets["normal/fourway/s2_3_4"] = new GameObject();

        //    #endregion

        //}


        //public static GameObject Get(string assetName)
        //{
        //    return assets.ContainsKey(assetName) ? assets[assetName] : null;
        //}
    }
}

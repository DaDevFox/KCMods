using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace BuildingTest
{
    static class AssetBundleManager
    {
        public static AssetBundle assetBundle;
        public static void UnpackAssetBundle()
        {
            assetBundle = KCModHelper.LoadAssetBundle(Mod.Helper.modPath + "/assetbundle/", Mod.AssetBundleName);
            if (assetBundle == null)
            {
                Mod.Helper.Log("AssetBundle Failed to load");
            }
        }

        public static object GetAssetByPath(string path)
        {
            return assetBundle.LoadAsset(path);
        }


        public static object GetAsset(string name)
        {
            if (assetBundle.Contains(name))
            {
                object Asset = null;
                string[] paths = assetBundle.GetAllAssetNames();
                for (int i = 0; i < paths.Length; i++)
                {
                    string[] pathParts = paths[i].Split('/');
                    string assetName = pathParts[pathParts.Length - 1];
                    if (assetName.ToLower() == name.ToLower())
                    {
                        Asset = assetBundle.LoadAsset(paths[i]);
                    }
                }
                return Asset;
            }
            else
            {
                Mod.Helper.Log("Asset not found: " + name);
                return null;
            }
        }

        public static T GetAsset<T>(string name) where T : class
        {
            return GetAsset(name) as T;
        }

    }
}

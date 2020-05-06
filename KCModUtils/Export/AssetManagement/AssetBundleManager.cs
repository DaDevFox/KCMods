using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace KCModUtils.AssetManagement
{
    public static class AssetBundleManager
    {
        private static AssetBundle assetBundle;
        /// <summary>
        /// Loads the AssetBundle in for the first time, must be called before attempting to get an asset. 
        /// </summary>
        public static void UnpackAssetBundle()
        {
            assetBundle = KCModHelper.LoadAssetBundle(ModUtilsMain.Helper.modPath + ModUtilsMain.AssetBundleRelativePath, ModUtilsMain.AssetBundleName);

            if (assetBundle == null)
                ModUtilsMain.Helper.Log("AssetBundle Failed to load");
        }


        /// <summary>
        /// Returns an asset by its scene path. 
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static object GetAssetByPath(string path)
        {
            return assetBundle.LoadAsset(path);
        }

        /// <summary>
        /// Returns the first asset in the assetbundle that is loaded with the given name, including its file extensions. 
        /// </summary>
        /// <param name="name">The name of the asset to look for</param>
        /// <returns>the first asset in the assetbundle that is loaded with the given name</returns>
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
                    if (assetName == name.ToLower())
                    {
                        Asset = assetBundle.LoadAsset(paths[i]);
                    }
                }
                return Asset;
            }
            else
            {
                ModUtilsMain.Helper.Log("Asset not found: " + name);
                return null;
            }
        }

    }
}

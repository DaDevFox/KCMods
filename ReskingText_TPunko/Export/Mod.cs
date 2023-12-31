using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using ReskinEngine.API;

namespace ReskinEngine.Examples.Example1
{
	public class Mod
	{
		public static KCModHelper helper;

		public void SceneLoaded(KCModHelper helper)
		{
			try
			{

				// KCModHelper
				Mod.helper = helper;

				// Setup the ReskinProfile with a name and compatability identifier
				ReskinProfile profile = new ReskinProfile("TestMod", "Fox's Collections");

				//Voxel_Houses
				AssetBundle Voxel_Houses_bundle = KCModHelper.LoadAssetBundle(helper.modPath + "/assetbundle", "testmod_voxel_houses");


				// cottage 
				GameObject building_largehouse_cottage_baseModel = Voxel_Houses_bundle.LoadAsset<GameObject>("Assets/Mod/TPunkoModels/Prefabs/Houses/House2x1_1.prefab");
				CottageSkin cottage = new CottageSkin();
				cottage.baseModel = building_largehouse_cottage_baseModel;

				cottage.personPositions = new Vector3[0];
				profile.Add(cottage);

				// cottage1 
				GameObject building_largehouse_cottage1_baseModel = Voxel_Houses_bundle.LoadAsset<GameObject>("Assets/Mod/TPunkoModels/Prefabs/Houses/House2x1_2.prefab");
				CottageSkin cottage1 = new CottageSkin();
				cottage1.baseModel = building_largehouse_cottage1_baseModel;

				cottage1.personPositions = new Vector3[0];
				profile.Add(cottage1);

				// cottage2 
				GameObject building_largehouse_cottage2_baseModel = Voxel_Houses_bundle.LoadAsset<GameObject>("Assets/Mod/TPunkoModels/Prefabs/Houses/House2x1_3.prefab");
				CottageSkin cottage2 = new CottageSkin();
				cottage2.baseModel = building_largehouse_cottage2_baseModel;

				cottage2.personPositions = new Vector3[0];
				profile.Add(cottage2);

				// cottage3 
				GameObject building_largehouse_cottage3_baseModel = Voxel_Houses_bundle.LoadAsset<GameObject>("Assets/Mod/TPunkoModels/Prefabs/Houses/House2x1_4.prefab");
				CottageSkin cottage3 = new CottageSkin();
				cottage3.baseModel = building_largehouse_cottage3_baseModel;

				cottage3.personPositions = new Vector3[0];
				profile.Add(cottage3);

				// hovel 
				GameObject building_smallhouse_hovel_baseModel = Voxel_Houses_bundle.LoadAsset<GameObject>("Assets/Mod/TPunkoModels/Prefabs/Houses/House1x1.prefab");
				HovelSkin hovel = new HovelSkin();
				hovel.baseModel = building_smallhouse_hovel_baseModel;

				hovel.personPositions = new Vector3[0];
				profile.Add(hovel);

				// manor 
				GameObject building_manorhouse_manor_baseModel = Voxel_Houses_bundle.LoadAsset<GameObject>("Assets/Mod/TPunkoModels/Prefabs/Houses/House2x2.prefab");
				ManorSkin manor = new ManorSkin();
				manor.baseModel = building_manorhouse_manor_baseModel;

				manor.personPositions = new Vector3[0];
				profile.Add(manor);

				profile.Register();

				//Voxel_Castle
				AssetBundle Voxel_Castle_bundle = KCModHelper.LoadAssetBundle(helper.modPath + "/assetbundle", "testmod_voxel_castle");


				// cathedralSkin 
				GameObject building_cathedral_cathedralSkin_baseModel = Voxel_Castle_bundle.LoadAsset<GameObject>("Assets/Mod/TPunkoModels/Models/Cathedral/Cathedral 2.prefab");
				CathedralSkin cathedralSkin = new CathedralSkin();
				cathedralSkin.baseModel = building_cathedral_cathedralSkin_baseModel;

				cathedralSkin.personPositions = new Vector3[0];
				profile.Add(cathedralSkin);
				// churchskin 
				GameObject building_church_churchskin_baseModel = Voxel_Castle_bundle.LoadAsset<GameObject>("Assets/Mod/TPunkoModels/Models/Church/Church.prefab");
				ChurchSkin churchskin = new ChurchSkin();
				churchskin.baseModel = building_church_churchskin_baseModel;

				churchskin.personPositions = new Vector3[0];
				profile.Add(churchskin);
				// keep 
				GameObject building_keep_keep_keepUpgrade1 = Voxel_Castle_bundle.LoadAsset<GameObject>("Assets/Mod/TPunkoModels/Models/Keep/Keep-0.prefab");
				// keep 
				GameObject building_keep_keep_keepUpgrade2 = Voxel_Castle_bundle.LoadAsset<GameObject>("Assets/Mod/TPunkoModels/Models/Keep/Keep-0.prefab");
				// keep 
				GameObject building_keep_keep_keepUpgrade3 = Voxel_Castle_bundle.LoadAsset<GameObject>("Assets/Mod/TPunkoModels/Models/Keep/Keep-0.prefab");
				// keep 
				GameObject building_keep_keep_keepUpgrade4 = Voxel_Castle_bundle.LoadAsset<GameObject>("Assets/Mod/TPunkoModels/Models/Keep/Keep-0.prefab");
				KeepSkin keep = new KeepSkin();
				keep.keepUpgrade1 = building_keep_keep_keepUpgrade1;
				keep.keepUpgrade2 = building_keep_keep_keepUpgrade2;
				keep.keepUpgrade3 = building_keep_keep_keepUpgrade3;
				keep.keepUpgrade4 = building_keep_keep_keepUpgrade4;

				keep.personPositions = new Vector3[3] { new Vector3(0f, 0f, 0f), new Vector3(0f, 0f, 0f), new Vector3(0f, 0f, 0f) };
				profile.Add(keep);

				profile.Register();
			}
			catch (Exception ex)
			{
				helper.Log(ex.ToString());
			}

			helper.Log("Init");
		}
	}
}

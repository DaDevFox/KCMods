using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

namespace InsaneDifficultyMod.Events
{
    public class RiotDemandsUI : MonoBehaviour
    {
        public TextMeshProUGUI description;
        public string descriptionFormat = "%s peasants have begun to riot! They demand that the lord pay resources to appease their wishes. Failiure to meet their demands might result in a march on the keep!";

        public bool visible = false;

        public Dictionary<string, Transform> resourceObjs = new Dictionary<string, Transform>();
        public Button acceptButton;

        private Riot riot;


        public void Start() 
        {
            #region UI setup

            resourceObjs.Add("Gold",transform.Find("ResourceContainer/Resource_Gold"));

            resourceObjs.Add("Wood", transform.Find("ResourceContainer/Resource_Wood"));
            resourceObjs.Add("Stone", transform.Find("ResourceContainer/Resource_Stone"));
            resourceObjs.Add("Charcoal", transform.Find("ResourceContainer/Resource_Charcoal"));

            resourceObjs.Add("IronOre", transform.Find("ResourceContainer/Resource_Iron"));
            resourceObjs.Add("Tools", transform.Find("ResourceContainer/Resource_Tool"));
            resourceObjs.Add("Armament", transform.Find("ResourceContainer/Resource_Armaments"));

            resourceObjs.Add("Wheat", transform.Find("ResourceContainer/Resource_Wheat"));
            resourceObjs.Add("Apples", transform.Find("ResourceContainer/Resource_Apples"));
            resourceObjs.Add("Fish", transform.Find("ResourceContainer/Resource_Fish"));
            resourceObjs.Add("Pork", transform.Find("ResourceContainer/Resource_Pork"));

            #endregion

            acceptButton = transform.Find("AcceptButton").gameObject.GetComponent<Button>();

            acceptButton.onClick.AddListener(delegate { 
                OnAcceptButtonClicked();
            });
        }

        public void Open(Riot riot) 
        {
            this.riot = riot;

            visible = true;
            description.text = string.Concat(descriptionFormat, riot.count);

            #region Resource Display

            if (riot.demand.Get(FreeResourceType.Gold) > 0)
            {
                resourceObjs["Gold"].gameObject.SetActive(true);
                resourceObjs["Gold"].Find("Text").GetComponent<TextMeshProUGUI>().text = riot.demand.Get(FreeResourceType.Gold).ToString();
            }
            else 
            {
                resourceObjs["Gold"].gameObject.SetActive(false);
            }

            if (riot.demand.Get(FreeResourceType.Tree) > 0)
            {
                resourceObjs["Wood"].gameObject.SetActive(true);
                resourceObjs["Wood"].Find("Text").GetComponent<TextMeshProUGUI>().text = riot.demand.Get(FreeResourceType.Tree).ToString();
            }
            else
            {
                resourceObjs["Wood"].gameObject.SetActive(false);
            }
            if (riot.demand.Get(FreeResourceType.Stone) > 0)
            {
                resourceObjs["Stone"].gameObject.SetActive(true);
                resourceObjs["Stone"].Find("Text").GetComponent<TextMeshProUGUI>().text = riot.demand.Get(FreeResourceType.Stone).ToString();
            }
            else
            {
                resourceObjs["Stone"].gameObject.SetActive(false);
            }
            if (riot.demand.Get(FreeResourceType.Charcoal) > 0)
            {
                resourceObjs["Charcoal"].gameObject.SetActive(true);
                resourceObjs["Charcoal"].Find("Text").GetComponent<TextMeshProUGUI>().text = riot.demand.Get(FreeResourceType.Charcoal).ToString();
            }
            else
            {
                resourceObjs["Charcoal"].gameObject.SetActive(false);
            }

            if (riot.demand.Get(FreeResourceType.IronOre) > 0)
            {
                resourceObjs["IronOre"].gameObject.SetActive(true);
                resourceObjs["IronOre"].Find("Text").GetComponent<TextMeshProUGUI>().text = riot.demand.Get(FreeResourceType.IronOre).ToString();
            }
            else
            {
                resourceObjs["IronOre"].gameObject.SetActive(false);
            }
            if (riot.demand.Get(FreeResourceType.Tools) > 0)
            {
                resourceObjs["Tools"].gameObject.SetActive(true);
                resourceObjs["Tools"].Find("Text").GetComponent<TextMeshProUGUI>().text = riot.demand.Get(FreeResourceType.Tools).ToString();
            }
            else
            {
                resourceObjs["Tools"].gameObject.SetActive(false);
            }
            if (riot.demand.Get(FreeResourceType.Armament) > 0)
            {
                resourceObjs["Armament"].gameObject.SetActive(true);
                resourceObjs["Armament"].Find("Text").GetComponent<TextMeshProUGUI>().text = riot.demand.Get(FreeResourceType.Armament).ToString();
            }
            else
            {
                resourceObjs["Armament"].gameObject.SetActive(false);
            }

            if (riot.demand.Get(FreeResourceType.Wheat) > 0)
            {
                resourceObjs["Wheat"].gameObject.SetActive(true);
                resourceObjs["Wheat"].Find("Text").GetComponent<TextMeshProUGUI>().text = riot.demand.Get(FreeResourceType.Wheat).ToString();
            }
            else
            {
                resourceObjs["Wheat"].gameObject.SetActive(false);
            }
            if (riot.demand.Get(FreeResourceType.Apples) > 0)
            {
                resourceObjs["Apples"].gameObject.SetActive(true);
                resourceObjs["Apples"].Find("Text").GetComponent<TextMeshProUGUI>().text = riot.demand.Get(FreeResourceType.Apples).ToString();
            }
            else
            {
                resourceObjs["Apples"].gameObject.SetActive(false);
            }
            if (riot.demand.Get(FreeResourceType.Fish) > 0)
            {
                resourceObjs["Fish"].gameObject.SetActive(true);
                resourceObjs["Fish"].Find("Text").GetComponent<TextMeshProUGUI>().text = riot.demand.Get(FreeResourceType.Fish).ToString();
            }
            else
            {
                resourceObjs["Fish"].gameObject.SetActive(false);
            }
            if (riot.demand.Get(FreeResourceType.Pork) > 0)
            {
                resourceObjs["Pork"].gameObject.SetActive(true);
                resourceObjs["Pork"].Find("Text").GetComponent<TextMeshProUGUI>().text = riot.demand.Get(FreeResourceType.Pork).ToString();
            }
            else
            {
                resourceObjs["Pork"].gameObject.SetActive(false);
            }

            #endregion
        }

        void Update() 
        {
            gameObject.SetActive(visible);
            if (visible)
            {
                Assets.Code.ResourceAmount playerResources = Player.inst.resourcesPerLandmass[riot.landmass];

                bool playerCanAfford = true;
                for (int i = 0; i < (int)FreeResourceType.NumTypes; i++)
                    if (playerResources.Get((FreeResourceType)i) < riot.demand.Get((FreeResourceType)i))
                            playerCanAfford = false;

                if (Player.inst.PlayerLandmassOwner.Gold > riot.demand.Get(FreeResourceType.Gold) && playerCanAfford)
                {
                    acceptButton.interactable = true;
                }
                else
                {
                    acceptButton.interactable = false;
                }
            }
        }

        private void OnAcceptButtonClicked() 
        {
            Assets.Code.ResourceAmount playerResources = Player.inst.resourcesPerLandmass[riot.landmass];
            Assets.Code.ResourceAmount negativeDemand = new Assets.Code.ResourceAmount();

            #region Configuring Negative Demand ResourceAmount

            negativeDemand.Set(FreeResourceType.Gold,-riot.demand.Get(FreeResourceType.Gold));

            negativeDemand.Set(FreeResourceType.Tree, -riot.demand.Get(FreeResourceType.Tree));
            negativeDemand.Set(FreeResourceType.Stone, -riot.demand.Get(FreeResourceType.Stone));
            negativeDemand.Set(FreeResourceType.Charcoal, -riot.demand.Get(FreeResourceType.Charcoal));

            negativeDemand.Set(FreeResourceType.IronOre, -riot.demand.Get(FreeResourceType.IronOre));
            negativeDemand.Set(FreeResourceType.Tools, -riot.demand.Get(FreeResourceType.Tools));
            negativeDemand.Set(FreeResourceType.Armament, -riot.demand.Get(FreeResourceType.Armament));

            negativeDemand.Set(FreeResourceType.Wheat, -riot.demand.Get(FreeResourceType.Wheat));
            negativeDemand.Set(FreeResourceType.Apples, -riot.demand.Get(FreeResourceType.Apples));
            negativeDemand.Set(FreeResourceType.Fish, -riot.demand.Get(FreeResourceType.Fish));
            negativeDemand.Set(FreeResourceType.Pork, -riot.demand.Get(FreeResourceType.Pork));

            #endregion

            bool playerCanAfford = true;
            for (int i = 0; i < (int)FreeResourceType.NumTypes; i++)
                if (playerResources.Get((FreeResourceType)i) < riot.demand.Get((FreeResourceType)i))
                    playerCanAfford = false;

            if (Player.inst.PlayerLandmassOwner.Gold > riot.demand.Get(FreeResourceType.Gold) && playerCanAfford)
            {
                playerResources.Add(negativeDemand);
                Player.inst.PlayerLandmassOwner.Gold -= riot.demand.Get(FreeResourceType.Gold);
            }
            else
            {
                acceptButton.interactable = false;
            }
        }

    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace StatisticsMod.UI
{
    class TabContainer : MonoBehaviour
    {
        private List<string> hookedTabs;

        private Transform buttons;
        private Transform contents;

        void Start()
        {
            buttons = transform.Find("Buttons");
            contents = transform.Find("Content");

            int buttonsChildCount = buttons.childCount;
            for(int i = 0; i < buttonsChildCount; i++)
            {
                string tabName = buttons.GetChild(i).name;
                if(contents.Find(tabName) != null)
                {
                    hookedTabs.Add(tabName);
                }
            }
        }

        void LinkTab(string tabName)
        {
            Transform button = buttons.Find(tabName);
            Transform content = contents.Find(tabName);



            button.GetComponent<Toggle>().onValueChanged.AddListener(delegate
            {
                OnTabSelected(tabName);
            });
        }

        void OnTabSelected(string tabName)
        {
            bool enabled = buttons.Find(tabName).GetComponent<Toggle>().isOn;
            foreach (string tab in hookedTabs)
            {
                if (tab == tabName)
                {
                    contents.Find(tab).gameObject.SetActive(enabled);
                }
                else
                {
                    contents.Find(tab).gameObject.SetActive(false);
                }
            }
        }
    }
}

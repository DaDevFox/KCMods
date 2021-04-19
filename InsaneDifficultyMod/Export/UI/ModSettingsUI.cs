using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

namespace InsaneDifficultyMod
{
    public class ModSettingsUI : MonoBehaviour
    {



        public static GameObject _base;

        private Color mainColor = new Color(191, 0, 0);
        private Color accentColor = new Color(153, 153, 153);

        private Color vanillaColor = Color.white;
        private Color l1Color = new Color(247, 250, 105);
        private Color l2Color = new Color(255, 170, 33);
        private Color l3Color = new Color(245, 145, 105);
        private Color l4Color = new Color(255, 66, 66);

        private GameObject slider;

        void Start()
        {
            _base = gameObject;
            slider = _base.transform.Find("Slider").gameObject;
            Slider sliderInput = slider.GetComponent<Slider>();
            sliderInput.onValueChanged.AddListener(delegate { 
                onDifficultyValueChanged(); 
            });


            onDifficultyValueChanged();



            if (FindObjectOfType<SplashScreen>())
            {
                FindObjectOfType<SplashScreen>().Invoke("EndSplashScreen", 0f);
            }



        }

        void onDifficultyValueChanged() {
            int value = (int)slider.GetComponent<Slider>().value;
            TextMeshProUGUI label = _base.transform.Find("TextMeshPro Text (1)").gameObject.GetComponent<TextMeshProUGUI>();
            switch (value)
            {
                // fontColor = 
                case 0:
                    label.text = "Easy";
                    label.color = vanillaColor;
                    break;
                case 1:
                    label.text = "Difficult";
                    label.color = l1Color;
                    break;
                case 2:
                    label.text = "Challenging";
                    label.color = l2Color;
                    break;
                case 3:
                    label.text = "Expert";
                    label.color = l3Color;
                    break;
                case 4:
                    label.text = "Insane";
                    label.color = l4Color;
                    break;
            }
            Settings.Mode = value;
            Settings.UpdateMode(value);
        }


    }
}

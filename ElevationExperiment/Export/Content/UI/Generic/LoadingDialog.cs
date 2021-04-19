using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using TMPro;
using Fox.Localization;
using Fox.UI;

namespace Elevation
{
    public class LoadingDialog : MonoBehaviour
    {
        private TextMeshProUGUI _title;
        private TextMeshProUGUI _description;

        public static string titleTerm { get; } = "generating_title";
        public static string descriptionTerm { get; } = "generating_description";


        void Awake()
        {
            gameObject.SetActive(false);

            _title = transform.Find("window/Title").GetComponent<TextMeshProUGUI>();
            _description = transform.Find("window/Description").GetComponent<TextMeshProUGUI>();
        }

        /// <summary>
        /// Activates the loading screen immediately in a frame
        /// </summary>
        public void Activate()
        {
            gameObject.SetActive(true);
            Localize();
            // Normally UI is rendered at the end of a frame, however in this case, we want the ui rendered immediately, this method will force render the loading dialog and any other ui. 
            Canvas.ForceUpdateCanvases();
        }

        /// <summary>
        /// Deactivates the loading screen
        /// </summary>
        public void Deactivate()
        {
            gameObject.SetActive(false);
        }

        /// <summary>
        /// Updates the text on the loading screen to match the game language
        /// </summary>
        public void Localize()
        {
            _title.text = Localization.Get(titleTerm);
            _description.text = Localization.Get(descriptionTerm);
        }
    }
}

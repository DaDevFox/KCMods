using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace New_Lands
{
    public class InputManager
    {

        // KeyCode recieved from ScriptEntry to open the menu.
        private KeyCode KeyForMenu = KeyCode.N;
        private KeyCode Exit = KeyCode.Escape;

        // Bool representing the status of the menu.
        // True  --> Menu is opened.
        // False --> Menu is closed.
        public bool MenuIsOpen { set; get; }


        public InputManager()
        {
            MenuIsOpen = false;
        }

       


        // Gets called inside the ScriptEntry.Update() function and
        // thus can be seen as an extention
        public void Update()
        {
            if (Input.GetKeyDown(KeyForMenu) && !MenuIsOpen)
            {
                MenuIsOpen = true;

                //UserInterface.ShowMenu();
            }
            else if (Input.GetKeyDown(KeyForMenu) && MenuIsOpen)
            {
                MenuIsOpen = false;
                //UserInterface.HideMenu();
            }

            if (Input.GetKeyDown(Exit) && MenuIsOpen)
            {
                MenuIsOpen = false;
                //UserInterface.HideMenu();
            }
        }

    }
}

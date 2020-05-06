using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace CrimeMod
{
    public static class Settings
    {
        #region Debug

        public static bool Debug { get; } = true;

        public static KeyCode keycode_checkCrime { get; } = KeyCode.I;


        #endregion


        public enum CriminalAssignmentPriority
        {
            PrioritizeDangerousCriminals,
            PrioritizeHarmlessCriminals
        }

        public static CriminalAssignmentPriority criminalAssignmentPriority;

    }
}

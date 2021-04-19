using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zat.Shared.InterModComm;
using UnityEngine;

namespace Biomes.API
{
    public static class API
    {
        public static string PortObjectName { get; } = "BiomesAPI";
        private static IMCPort _port;

        /*
         * Data Structures:
         * CellType (struct):
            * 
            * 
            * 
            * 
         * 
         *  
         * Commands:
         * 
         * int Register(CellType type)
         * 
         */

        static API()
        {
            Mod.Init += Init;
        }

        private static void Init()
        {
            _port = new GameObject(PortObjectName).AddComponent<IMCPort>();

            _port.RegisterReceiveListener<CellType>("Register", r_Register);
        }

        #region Callbacks

        private static void r_Register(IRequestHandler handler, string source, CellType type)
        {
            int result = Engine.Register(type);
            handler.SendResponse<int>(PortObjectName, result);
        }

        #endregion

    }
}

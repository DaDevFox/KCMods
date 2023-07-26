using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Elevation
{
    public static class GameObjectExtensions
    {
        public static void ClearChildren(this GameObject obj)
        {
            List<Transform> children = new List<Transform>();

            for (int i = 0; i < obj.transform.childCount; i++)
            {
                children.Add(obj.transform.GetChild(i));
            }

            foreach (Transform child in children)
                GameObject.Destroy(child.gameObject);
        }

        public static void ClearChildren(this Transform obj)
        {
            List<Transform> children = new List<Transform>();

            for (int i = 0; i < obj.childCount; i++)
            {
                children.Add(obj.GetChild(i));
            }

            foreach (Transform child in children)
                GameObject.Destroy(child.gameObject);
        }

    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Elevation.Utils
{
    public static class EnumerableExtensions
    {
        public static T[] Add<T>(this T[] array)
        {
            Array.Resize(ref array, array.Length + 1);
            return array;
        }

        public static T[] Add<T>(this T[] array, T item)
        {
            Array.Resize(ref array, array.Length + 1);
            array[array.Length - 1] = item;
            //Debug.Log($"array length {array.Length}");
            return array;
        }

        public static void AddRange<T>(this T[] array, IEnumerable<T> collection)
        {
            List<T> list = array.ToList();
            list.AddRange(collection);
            array = list.ToArray();
        }

        public static T Find<T>(this IEnumerable<T> enumrable, T toFind)
        {
            foreach (T item in enumrable)
                if (item.Equals(toFind))
                    return item;
            return default(T);
        }

        public static void ForEach<T>(this T[] array, Action<T> action)
        {
            for (int i = 0; i < array.Length; i++)
            {
                action(array[i]);
            }
        }

        public static void ForEach<T>(this T[] array, Action<int, T> action)
        {
            for (int i = 0; i < array.Length; i++)
            {
                action(i, array[i]);
            }
        }

        public static void ForEach<T>(this T[] array, Action<int> action)
        {
            for (int i = 0; i < array.Length; i++)
            {
                action(i);
            }
        }

        public static void ForEach<T>(this IEnumerable<T> enumerable, Action<T> action)
        {
            enumerable.ToList().ForEach(action);
        }

        public static int GetNextFreeKey<T>(this Dictionary<int, T> dict)
        {
            int found = -1;
            int i = 0;
            while (found == -1)
            {
                if (!dict.ContainsKey(i))
                    found = i;
                i++;
            }
            return found;
        }
    }
}

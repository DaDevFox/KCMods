using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TotallyAccurateModEngine
{
    public class Singleton<T> where T : class
    {
        public static readonly T _instance = Activator.CreateInstance(typeof(T)) as T;
    }
}

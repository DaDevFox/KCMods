using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elevation.Utils
{
    public class Metadata : IEnumerable
    {
        protected Dictionary<string, object> _data = new Dictionary<string, object>();

        public object this[string key]
        {
            get
            {
                return Get(key);
            }
            set
            {
                Set(key, value);
            }
        }

        public virtual void Set(string key, object value)
        {
            if (_data.ContainsKey(key))
                _data[key] = value;
            else
                _data.Add(key, value);
        }

        public virtual void Add(string key, object value) => _data.Add(key, value);

        public virtual void Clear() => _data.Clear();

        public virtual object Get(string key) => _data.ContainsKey(key) ? _data[key] : null;

        public virtual T Get<T>(string key) => Get(key) != null ? (T)Get(key) : default(T);



        public IEnumerator GetEnumerator()
        {
            return ((IEnumerable)_data).GetEnumerator();
        }
    }
}

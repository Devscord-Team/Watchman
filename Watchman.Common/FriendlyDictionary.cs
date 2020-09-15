using System;
using System.Collections.Generic;
using System.Text;

namespace Watchman.Common
{
    public class FriendlyDictionary<T, V>
        where V : class, new()
    {
        private readonly IDictionary<T, V> _dictionary;

        public FriendlyDictionary()
        {
            this._dictionary = new Dictionary<T, V>();
        }

        public FriendlyDictionary(IDictionary<T, V> dictionary)
        {
            this._dictionary = dictionary;
        }

        public bool ContainsKey(T key)
        {
            return this._dictionary.ContainsKey(key);
        }

        public V GetOrCreate(T key)
        {
            V value;
            if (!this._dictionary.TryGetValue(key, out value))
            {
                this.Set(key, new V());
            }
            return this._dictionary[key];
        }

        public bool TrySet(T key, V value)
        {
            if (this._dictionary.ContainsKey(key))
            {
                return false;
            }
            this.Set(key, value);
            return true;
        }

        public void Set(T key, V value)
        {
            this._dictionary[key] = value;
        }

        public int Count()
        {
            return this._dictionary.Count;
        }

        public V this[T key]
        {
            get
            {
                return this.GetOrCreate(key);
            }
            set
            {
                this.Set(key, value);
            }
        }
    }
}

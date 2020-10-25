using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using Watchman.Common.Interfaces;

namespace Watchman.Common
{
    public class FriendlyDictionary<T, V> : Dictionary<T, V>, IFriendlyDictionary
        where V : class, new()
    {
        public FriendlyDictionary() : base()
        {
        }

        public V GetOrCreate(T key)
        {
            if (!this.TryGetValue(key, out var value))
            {
                this.Set(key, new V());
            }
            return base[key];
        }

        public bool TrySet(T key, V value)
        {
            if (this.ContainsKey(key))
            {
                return false;
            }
            this.Set(key, value);
            return true;
        }

        public void Set(T key, V value)
        {
            base[key] = value;
        }

        public new V this[T key]
        {
            get
            {
                return this.GetOrCreate(key);
            }
            set
            {
                base[key] = value;
            }
        }

        public int CleanEmptyContainers()
        {
            var values = this.Values;
            int deletedCount = 0;
            foreach (var value in values)
            {
                if (IsCollection(value.GetType())) 
                {
                    if (value.GetType().GetInterface(typeof(IFriendlyDictionary).Name) != null)
                    {
                        deletedCount += ((IFriendlyDictionary)value).CleanEmptyContainers();
                    }
                    var keyValuePair = this.FirstOrDefault(x => x.Value == value && ((ICollection)value).Count == 0);
                    this.Remove(keyValuePair.Key);
                }
            }            
            return deletedCount;
        }

        private bool IsCollection(Type type)
        {
            var inf = type.GetInterfaces();
            return type.GetInterfaces().Any(x => x.Name == typeof(ICollection).Name);
        }
    }
}

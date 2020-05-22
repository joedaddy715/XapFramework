using System;
using System.Collections.Generic;

namespace Xap.Infrastructure.Caches {
    [Serializable]
    public class XapCache<TKey, TValue> {

        private readonly Dictionary<TKey, TValue> cachedItems = new Dictionary<TKey, TValue>();

        #region "Constructors"
        public XapCache() { }

        #endregion

        #region "Properties"
        object lockItUp = new object();

        public int Count => cachedItems.Count;
        #endregion

        #region "Methods"
        public void AddItem(TKey key, TValue value) {
            lock (lockItUp) {
                if (!cachedItems.ContainsKey(key)) {
                    cachedItems.Add(key, value);
                }
            }
        }

        public void RemoveItem(TKey key) {
            lock (lockItUp) {
                if (cachedItems.ContainsKey(key)) {
                    cachedItems.Remove(key);
                }
            }
        }

        public void ClearCache() {
            lock (lockItUp) {
                cachedItems.Clear();
            }
        }

        public Dictionary<TKey, TValue> GetItems() => cachedItems;

        public TValue GetItem(TKey key) {
            lock (lockItUp) {
                if (cachedItems.ContainsKey(key)) {
                    return cachedItems[key];
                }
            }
            return default(TValue);
        }
        #endregion
    }
}

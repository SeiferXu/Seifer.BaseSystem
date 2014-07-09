using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Seifer.Common
{
    public class CommonCache
    {
        private Dictionary<string, Dictionary<string, object>> _cache;
        private Dictionary<string, Dictionary<string, DateTime>> _cacheTime;
        private static CommonCache _singleton = null;
        public CommonCache()
        {
            _cache = new Dictionary<string, Dictionary<string, object>>();
            _cacheTime = new Dictionary<string, Dictionary<string, DateTime>>();
        }

        private static CommonCache Singleton
        {
            get
            {
                lock (typeof(CommonCache))
                {
                    if (_singleton == null)
                    {
                        _singleton = new CommonCache();
                    }
                }
                return _singleton;
            }
        }

        public static void SetCache(string name, string key, object data)
        {
            CommonCache cache = Singleton;
            lock (typeof(CommonCache))
            {
                if (!cache._cache.ContainsKey(name))
                {
                    cache._cache.Add(name, new Dictionary<string, object>());
                    cache._cacheTime.Add(name, new Dictionary<string, DateTime>());
                }

                if (!cache._cache[name].ContainsKey(key))
                {
                    cache._cache[name].Add(key, data);
                    cache._cacheTime[name].Add(key, DateTime.Now);
                }
                else
                {
                    cache._cache[name][key] = data;
                    cache._cacheTime[name][key] = DateTime.Now;
                }
            }
        }

        public static object GetCache(string name, string key)
        {
            CommonCache cache = Singleton;
            lock (typeof(CommonCache))
            {
                if (!cache._cache.ContainsKey(name))
                {
                    return null;
                }

                if (!cache._cache[name].ContainsKey(key))
                {
                    return null;
                }
                cache._cacheTime[name][key] = DateTime.Now;
                return cache._cache[name][key];
            }
        }

        public static void RemoveCache(string name, string key)
        {
            CommonCache cache = Singleton;
            lock (typeof(CommonCache))
            {
                if (!cache._cache.ContainsKey(name))
                {
                    return;
                }

                if (!cache._cache[name].ContainsKey(key))
                {
                    return;
                }
                cache._cache[name].Remove(key);
                cache._cacheTime[name].Remove(key);
            }
        }

        public static void RemoveCache(string name)
        {
            CommonCache cache = Singleton;
            lock (typeof(CommonCache))
            {
                if (!cache._cache.ContainsKey(name))
                {
                    return;
                }

                cache._cache.Remove(name);
                cache._cacheTime.Remove(name);
            }
        }

        public static void ReleaseCache()
        {
            
        }
    }
}

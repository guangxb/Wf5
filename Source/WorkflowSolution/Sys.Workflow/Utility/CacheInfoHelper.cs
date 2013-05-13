using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.Threading;

namespace Sys.Workflow.Utility
{
    public class WfIdentity: IEquatable<WfIdentity>
    {
        public bool Equals(WfIdentity other)
        {
            return Equals(other);
        }
    }

    /// <summary>
    /// 工作流XPDL缓存
    /// </summary>
    public class WfCacheInfo
    {
        private int hitCount;
        public int GetHitCount()
        {
            return Interlocked.CompareExchange(ref hitCount, 0, 0);
        }

        public void RecordHit()
        {
            Interlocked.Increment(ref hitCount);
        }
    }

    public class WfCacheInfoHelper
    {
        private static int collect;
        private const int COLLECT_PER_TIMES = 1000, COLLECT_HIT_COUNT_MIN = 0;
        static readonly ConcurrentDictionary<WfIdentity, WfCacheInfo> _wfQueryCache = new ConcurrentDictionary<WfIdentity, WfCacheInfo>();

        public static event EventHandler WfQueryCachedPurged;
        private static void OnWfQueryCachedPurged()
        {
            var handler = WfQueryCachedPurged;
            if (handler != null)
                handler(null, EventArgs.Empty);
        }
             
        private static void SetQueryCache(WfIdentity key, WfCacheInfo value)
        {
            if (Interlocked.Increment(ref collect) == COLLECT_PER_TIMES)
            {
                CollectCacheGarbage();
            }
            _wfQueryCache[key] = value;
        }

        private static void CollectCacheGarbage()
        {
            try
            {
                foreach (var pair in _wfQueryCache)
                {
                    if (pair.Value.GetHitCount() <= COLLECT_HIT_COUNT_MIN)
                    {
                        WfCacheInfo cache;
                        _wfQueryCache.TryRemove(pair.Key, out cache);
                    }
                }
            }
            finally
            {
                Interlocked.Exchange(ref collect, 0);
            }
        }

        private static bool TryGetWfQueryCache(WfIdentity key, out WfCacheInfo value)
        {
            if (_wfQueryCache.TryGetValue(key, out value))
            {
                value.RecordHit();
                return true;
            }
            value = null;
            return false;
        }

        public static void PurgeWfQueryCache()
        {
            _wfQueryCache.Clear();
            OnWfQueryCachedPurged();
        }

        private static void PurgeWfQueryCacheByType(Type type)
        {
            foreach (var entry in _wfQueryCache)
            {
                WfCacheInfo cache;
                if (entry.Key.GetType() == type)
                    _wfQueryCache.TryRemove(entry.Key, out cache);
            }
        }

        public static int GetCachedItemsCount()
        {
            return _wfQueryCache.Count;
        }

        public static IEnumerable<Tuple<WfIdentity, int>> GetCachedWfXpdls(int ignoreHitCountAbove = int.MaxValue)
        {
            var data = _wfQueryCache.Select(pair => Tuple.Create(pair.Key, pair.Value.GetHitCount()));
            if (ignoreHitCountAbove < int.MaxValue)
                data = data.Where(tuple => tuple.Item2 <= ignoreHitCountAbove);
            return data;
        }

        public static IEnumerable<Tuple<int, int>> GetHashCollissions()
        {
            var counts = new Dictionary<int, int>();
            foreach (var key in _wfQueryCache)
            {
                int count;
                if (!counts.TryGetValue(key.GetHashCode(), out count))
                {
                    counts.Add(key.GetHashCode(), 1);
                }
            }

            return from pair in counts
                   where pair.Value > 1
                   select Tuple.Create(pair.Key, pair.Value);
        }
    }
}


/*
static readonly System.Collections.Concurrent.ConcurrentDictionary<Identity, CacheInfo> _queryCache = new System.Collections.Concurrent.ConcurrentDictionary<Identity, CacheInfo>();
        private static void SetQueryCache(Identity key, CacheInfo value)
        {
            if (Interlocked.Increment(ref collect) == COLLECT_PER_ITEMS)
            {
                CollectCacheGarbage();
            }
            _queryCache[key] = value;
        }

        private static void CollectCacheGarbage()
        {
            try
            {
                foreach (var pair in _queryCache)
                {
                    if (pair.Value.GetHitCount() <= COLLECT_HIT_COUNT_MIN)
                    {
                        CacheInfo cache;
                        _queryCache.TryRemove(pair.Key, out cache);
                    }
                }
            }

            finally
            {
                Interlocked.Exchange(ref collect, 0);
            }
        }

        private const int COLLECT_PER_ITEMS = 1000, COLLECT_HIT_COUNT_MIN = 0;
        private static int collect;
        private static bool TryGetQueryCache(Identity key, out CacheInfo value)
        {
            if (_queryCache.TryGetValue(key, out value))
            {
                value.RecordHit();
                return true;
            }
            value = null;
            return false;
        }

        /// <summary>
        /// Purge the query cache 
        /// </summary>
        public static void PurgeQueryCache()
        {
            _queryCache.Clear();
            OnQueryCachePurged();
        }

        private static void PurgeQueryCacheByType(Type type)
        {
            foreach (var entry in _queryCache)
            {
                CacheInfo cache;
                if (entry.Key.type == type)
                    _queryCache.TryRemove(entry.Key, out cache);
            }
        }

        /// <summary>
        /// Return a count of all the cached queries by dapper
        /// </summary>
        /// <returns></returns>
        public static int GetCachedSQLCount()
        {
            return _queryCache.Count;
        }

        /// <summary>
        /// Return a list of all the queries cached by dapper
        /// </summary>
        /// <param name="ignoreHitCountAbove"></param>
        /// <returns></returns>
        public static IEnumerable<Tuple<string, string, int>> GetCachedSQL(int ignoreHitCountAbove = int.MaxValue)
        {
            var data = _queryCache.Select(pair => Tuple.Create(pair.Key.connectionString, pair.Key.sql, pair.Value.GetHitCount()));
            if (ignoreHitCountAbove < int.MaxValue) data = data.Where(tuple => tuple.Item3 <= ignoreHitCountAbove);
            return data;
        }

        /// <summary>
        /// Deep diagnostics only: find any hash collisions in the cache
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<Tuple<int, int>> GetHashCollissions()
        {
            var counts = new Dictionary<int, int>();
            foreach (var key in _queryCache.Keys)
            {
                int count;
                if (!counts.TryGetValue(key.hashCode, out count))
                {
                    counts.Add(key.hashCode, 1);
                }
                else
                {
                    counts[key.hashCode] = count + 1;
                }
            }
            return from pair in counts
                   where pair.Value > 1
                   select Tuple.Create(pair.Key, pair.Value);

        }

*/
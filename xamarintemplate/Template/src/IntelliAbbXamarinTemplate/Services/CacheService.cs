using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using IntelliAbbXamarinTemplate.Models;

namespace IntelliAbbXamarinTemplate.Services
{
    public class CacheService : ICacheService
    {
        public IMemoryCache Memory { get; set; }
        public IDeviceCache Device { get; set; }

        public CacheService(IMemoryCache memoryCache, IDeviceCache deviceCache)
        {
            Memory = memoryCache;
            Device = deviceCache;
        }
    }

    /// <summary>
    /// Memory cache.
    /// </summary>
    public class MemoryCache : IMemoryCache
    {
        static Dictionary<string, CacheEntry> _inMemory;
        static readonly object pass = new object();

        public MemoryCache()
        {
            lock (pass)
            {
                _inMemory = new Dictionary<string, CacheEntry>();
#if DEBUG
                System.Diagnostics.Debug.WriteLine("Memory Cache initialized.");
#endif
            }
        }

        public T GetObject<T>(string key)
        {
            lock (pass)
            {
                try
                {
                    if (Exists(key))
                        return JsonConvert.DeserializeObject<T>(_inMemory[key].Value);
                    return default(T);
                }
                catch (Exception ex)
                {
#if DEBUG
                    System.Diagnostics.Debug.WriteLine($"{(ex.InnerException ?? ex).Message}");
#endif
                    return default(T);
                }
            }
        }

        public byte[] GetObject(string key)
        {
            try
            {
                if (Exists(key))
                {
                    var retVal = _inMemory[key];
                    return retVal.Blob;
                }

                return default(byte[]);
            }
            catch (Exception ex)
            {
#if DEBUG
                System.Diagnostics.Debug.WriteLine($"{(ex.InnerException ?? ex).Message}");
#endif
                return default(byte[]);
            }
        }

        public void AddOrUpdateValue(string key, object value, TimeSpan? expiryPeriod = null)
        {
            lock (pass)
            {
                var ser = JsonConvert.SerializeObject(value);
                _inMemory[key] = new CacheEntry { Key = key, Value = ser, CreatedAt = DateTime.UtcNow, ExpiresIn = expiryPeriod };
#if DEBUG
                if (expiryPeriod == null)
                    System.Diagnostics.Debug.WriteLine($"Cache {key} added.");
                else
                    System.Diagnostics.Debug.WriteLine($"Cache {key} added and will expire in {expiryPeriod.ToString()}.");
#endif
            }
        }

        public void AddOrUpdateValue(string key, byte[] value, TimeSpan? expiryPeriod = null)
        {
            lock (pass)
            {
                _inMemory[key] = new CacheEntry { Key = key, Blob = value, CreatedAt = DateTime.UtcNow, ExpiresIn = expiryPeriod };
            }
        }

        public bool Exists(string key)
        {
            lock (pass)
            {
                if (!_inMemory.ContainsKey(key))
                    return false;

                var entry = _inMemory[key];
                return entry != null && !IsExpired(entry);
            }
        }

        public void Remove(params string[] keys)
        {
            lock (pass)
            {
                foreach (var key in keys)
                {
                    if (_inMemory.ContainsKey(key))
                        _inMemory.Remove(key);
                }
            }
        }

        public void Clear()
        {
            lock (pass)
            {
                _inMemory.Clear();
            }
        }

        public void Dispose()
        {
            lock (pass)
            {
                Clear();
                _inMemory = null;
            }
        }

        public override string ToString()
        {
            var retVal = new StringBuilder();
            retVal.AppendLine($"Count: {_inMemory.Count}");
            foreach (var item in _inMemory)
            {
                retVal.AppendLine($"{item.Key} = {item.Value}");
            }
            return retVal.ToString();
        }

        public string GetValue(string key)
        {
            lock (pass)
            {
                return Exists(key) ? JsonConvert.DeserializeObject<string>(_inMemory[key].Value) : null;
            }
        }

        public async ValueTask<T> GetOrFetch<T>(string key, Func<Task<T>> func, TimeSpan? expiryPeriod = null)
        {
            if (Exists(key))
            {
                var entry = _inMemory[key];
                return JsonConvert.DeserializeObject<T>(entry.Value);
            }

            var retVal = await func.Invoke();
            AddOrUpdateValue(key, retVal, expiryPeriod);
            return retVal;
        }

        internal bool IsExpired(CacheEntry entry)
        {
            lock (pass)
            {
#if DEBUG
                System.Diagnostics.Debug.WriteLine($"Cache {entry.Key} expires {entry.ExpiresIn.ToString()} after creation.");
#endif
                if (entry.ExpiresIn == null)
                    return false;

                var isExpired = DateTime.UtcNow > (entry.CreatedAt.Add(entry.ExpiresIn.Value));

                if (isExpired)
                {
#if DEBUG
                    System.Diagnostics.Debug.WriteLine($"Cache '{entry.Key}' expired {entry.ExpiresIn.ToString()} after creation. Removing.");
#endif
                    Remove(entry.Key);
                }

                return isExpired;
            }
        }
    }

    /// <summary>
    /// Device cache.
    /// </summary>
    public class DeviceCache : IDeviceCache
    {
        ICacheRepository _cacheRepo;

        public DeviceCache(ICacheRepository cacheRepo)
        {
            _cacheRepo = cacheRepo;
        }

        public void AddOrUpdateValue(string key, object value, TimeSpan? expiryPeriod = null)
        {
            if (key.Contains(" "))
                SanitizeKey(ref key);

            var ser = JsonConvert.SerializeObject(value);
#if DEBUG
            if (key.StartsWith("location_info"))
                Console.WriteLine(ser);
#endif
            _cacheRepo.AddOrUpdate(new CacheEntry { Key = key, Value = ser, ExpiresIn = expiryPeriod });
#if DEBUG
            if (expiryPeriod.HasValue)
                System.Diagnostics.Debug.WriteLine($"Cache {key} added and will expire in {expiryPeriod.Value.Duration()}.");
            else
                System.Diagnostics.Debug.WriteLine($"Cache {key} added");
#endif
        }

        public void AddOrUpdateValue(string key, byte[] value, TimeSpan? expiryPeriod = null)
        {
            if (key.Contains(" "))
                SanitizeKey(ref key);

            if (key == null)
            {
#if DEBUG
                System.Diagnostics.Debug.WriteLine($"Key is null. Cannot cache.");
#endif
                return;
            }

            _cacheRepo.AddOrUpdate(new CacheEntry { Key = key, Blob = value, ExpiresIn = expiryPeriod });
#if DEBUG
            if (expiryPeriod.HasValue)
                System.Diagnostics.Debug.WriteLine($"Cache {key} added and will expire in {expiryPeriod.Value.Duration()}.");
            else
                System.Diagnostics.Debug.WriteLine($"Cache {key} added");
#endif
        }

        public void Clear()
        {
            _cacheRepo.Clear();
        }

        public void Dispose()
        {
            _cacheRepo = null;
        }

        public bool Exists(string key)
        {
            if (key.Contains(" "))
                SanitizeKey(ref key);

            return _cacheRepo.Exists(key);
        }

        public string GetValue(string key)
        {
            if (key.Contains(" "))
                SanitizeKey(ref key);

            return Exists(key) ? SanitizedString(_cacheRepo.Get(key).Value) : null;
        }

        string SanitizedString(string text)
        {
            var ret = text.TrimStart('"');
            return ret.TrimEnd('"');
        }

        public T GetObject<T>(string key)
        {
            if (key.Contains(" "))
                SanitizeKey(ref key);

            if (!Exists(key))
            {
                return default(T);
            }

            var retVal = _cacheRepo.Get(key);
            return JsonConvert.DeserializeObject<T>(retVal.Value);
        }

        public byte[] GetObject(string key)
        {
            if (key.Contains(" "))
                SanitizeKey(ref key);

            return Exists(key) ? _cacheRepo.Get(key).Blob : default(byte[]);
        }

        public void Remove(params string[] keys)
        {
            foreach (var key in keys)
            {
                var k = key;
                if (k.Contains(" "))
                    SanitizeKey(ref k);

                if (_cacheRepo.Exists(k))
                    _cacheRepo.Remove(k);
            }
        }

        public async ValueTask<T> GetOrFetch<T>(string key, Func<Task<T>> func, TimeSpan? expiryPeriod = null)
        {
            if (key.Contains(" "))
                SanitizeKey(ref key);

            if (Exists(key))
            {
                var entry = _cacheRepo.Get(key);
                return JsonConvert.DeserializeObject<T>(entry.Value);
            }

            var retVal = await func.Invoke();
            AddOrUpdateValue(key, retVal, expiryPeriod);
            return retVal;
        }

        public override string ToString()
        {
            var items = _cacheRepo.GetAll();
            var retVal = new StringBuilder();
            retVal.AppendLine($"Count: {items.Count}");
            foreach (var item in items)
            {
                retVal.AppendLine($"{item.Key} = {item.Value}");
            }
            return retVal.ToString();
        }

        private void SanitizeKey(ref string key)
        {
            key = key?.Replace(" ", "_");
        }
    }
}

using System.Collections.Generic;

namespace IntelliAbbXamarinTemplate.Models
{
    public interface ICacheRepository
    {
        IList<CacheEntry> GetAll();
        CacheEntry Get(string key);
        int AddOrUpdate(CacheEntry entry);
        bool Exists(string key);
        void Remove(params string[] keys);
        void Clear();
        void Destroy();
    }
}

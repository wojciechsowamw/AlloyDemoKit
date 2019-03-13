using System;
using System.Data.Entity;
using System.Runtime.Caching;
using System.Text;
using EPiServer.Cms.UI.AspNetIdentity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.IdentityModel.Clients.ActiveDirectory;

namespace AlloyDemoKit.Helpers
{
    public class SessionTokenCache : TokenCache
    {
        private static readonly object FileLock = new object();
        private readonly string cacheId;
        private readonly MemoryCache memoryCache;

        public SessionTokenCache(string userId, MemoryCache memoryCache)
        {
            // not object, we want the SUB
            cacheId = userId + "TokenCache";
            this.memoryCache = memoryCache;

            Load();
        }

        public void SaveUserStateValue(string state)
        {
            lock (FileLock)
            {
               // memoryCache.Set(cacheId + "state", Encoding.ASCII.GetBytes(state));
               memoryCache.Set(new CacheItem(cacheId + "state", Encoding.ASCII.GetBytes(state)), new CacheItemPolicy());
            }
        }

        public string ReadUserStateValue()
        {
            string state;
            lock (FileLock)
            {
                state = Encoding.ASCII.GetString(memoryCache.Get(cacheId + "state") as byte[]);
            }

            return state;
        }

        public void Load()
        {
            lock (FileLock)
            {
                Deserialize(memoryCache.Get(cacheId) as byte[]);
            }
        }

        public void Persist()
        {
            lock (FileLock)
            {
                // reflect changes in the persistent store
                memoryCache.Set(new CacheItem(cacheId, Serialize()),new CacheItemPolicy() );
                //memoryCache.Set(cacheId, cache.Serialize());
                // once the write operation took place, restore the HasStateChanged bit to false
                HasStateChanged = false;
            }
        }

        // Empties the persistent store.
        public void Clear()
        {
            lock (FileLock)
            {
                memoryCache.Remove(cacheId);
            }
        }

        // Triggered right before MSAL needs to access the cache.
        // Reload the cache from the persistent store in case it changed since the last access.
        private void BeforeAccessNotification(TokenCacheNotificationArgs args)
        {
            Load();
        }

        // Triggered right after MSAL accessed the cache.
        private void AfterAccessNotification(TokenCacheNotificationArgs args)
        {
            // if the access operation resulted in a cache update
            if (HasStateChanged)
            {
                Persist();
            }
        }
    }
}
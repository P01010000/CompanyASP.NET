using CompanyASP.NET.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CompanyASP.NET.Repository
{
    public class CachingRepository<T> : IRepository<T> where T : IModel
    {
        private IRepository<T> Repository;
        private TimeSpan CacheDuration;
        private IEnumerable<T> CachedObjects;
        private DateTime DataDateTime;
        
        public CachingRepository(IRepository<T> repository, int cacheDuration = 30)
        {
            Repository = repository;
            CacheDuration = TimeSpan.FromSeconds(cacheDuration);
            DataDateTime = new DateTime();
        }

        private bool IsCacheValid { get { return CachedObjects != null && (DateTimeOffset.Now - DataDateTime) < CacheDuration; } }

        private void ValidateCache()
        {
            if(!IsCacheValid)
            {
                Console.WriteLine("Refreshing Cache");
                try
                {
                    CachedObjects = Repository.RetrieveAll();
                    DataDateTime = DateTime.Now;
                } catch (Exception)
                {
                    CachedObjects = new List<T>();
                }
            } else
            {
                Console.WriteLine("Reading from Cache");
            }
        }

        private void InvalidateCache()
        {
            CachedObjects = null;
        }


        public int Create(T obj)
        {
            InvalidateCache();
            return Repository.Create(obj);
        }

        public IEnumerable<int> Create(IEnumerable<T> list)
        {
            InvalidateCache();
            return Repository.Create(list);
        }

        public bool Delete(params int[] ids)
        {
            InvalidateCache();
            return Repository.Delete(ids);
        }

        public T Retrieve(params int[] ids)
        {
            ValidateCache();
            return CachedObjects.FirstOrDefault(info => info.Identity(ids));
        }

        public IEnumerable<T> RetrieveAll(params int[] ids)
        {
            ValidateCache();
            return CachedObjects;
        }

        public bool Update(T obj)
        {
            InvalidateCache();
            return Repository.Update(obj);
        }

        public int UpdateAll(IEnumerable<T> list)
        {
            InvalidateCache();
            return Repository.UpdateAll(list);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CompanyASP.NET.Repository
{
    public class CachingRepository<T> : IRepository<T> where T : class
    {
        private IRepository<T> Repository;
        private int Timeout;
        public CachingRepository(IRepository<T> repository, int timeout)
        {
            Repository = repository;
            Timeout = timeout;
        }

        private void ValidateCache()
        {

        }

        private void InvalidateCache()
        {

        }


        public int Create(T obj)
        {
            throw new NotImplementedException();
        }

        public bool Delete(params int[] ids)
        {
            throw new NotImplementedException();
        }

        public T Retrieve(params int[] ids)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<T> RetrieveAll(params int[] ids)
        {
            throw new NotImplementedException();
        }

        public bool Update(T obj)
        {
            throw new NotImplementedException();
        }

        public int UpdateAll(IEnumerable<T> list)
        {
            throw new NotImplementedException();
        }
    }
}

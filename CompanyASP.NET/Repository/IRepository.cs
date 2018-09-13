using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CompanyASP.NET.Repository
{
    public interface IRepository<T> where T : class
    {
        int Create(T obj);
        T Retrieve(params int[] ids);
        IEnumerable<T> RetrieveAll(params int[] ids);
        bool Update(T obj);
        int UpdateAll(IEnumerable<T> list);
        bool Delete(params int[] ids);
    }
}

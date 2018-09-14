using CompanyASP.NET.Models;
using System.Collections.Generic;

namespace CompanyASP.NET.Repository
{
    public interface IRepository<T> where T : IModel
    {
        int Create(T obj);
        T Retrieve(params int[] ids);
        IEnumerable<T> RetrieveAll(params int[] ids);
        bool Update(T obj);
        int UpdateAll(IEnumerable<T> list);
        bool Delete(params int[] ids);
    }
}

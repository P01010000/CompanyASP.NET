using System.Data;

namespace CompanyASP.NET.Helper
{
    public interface IDbContext
    {
        IDbConnection Connection { get; }
    }
}

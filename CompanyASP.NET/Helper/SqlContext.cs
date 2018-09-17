using CompanyASP.NET.Models;
using Microsoft.Extensions.Options;
using System.Data;
using System.Data.SqlClient;

namespace CompanyASP.NET.Helper
{
    public class SqlContext : IDbContext
    {
        private string connectionString;

        public SqlContext(IOptions<DbSettings> options)
        {
            connectionString = options.Value.DefaultConnection;
        }

        public IDbConnection Connection {
            get {
                var con = new SqlConnection(connectionString);
                con.Open();
                return con;
            }
        }
    }
}

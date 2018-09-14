using Microsoft.Extensions.Configuration;
using System.Data;
using System.Data.SqlClient;

namespace CompanyASP.NET.Helper
{
    public class SqlContext : IDbContext
    {
        private string connectionString;

        public SqlContext(IConfiguration configuration)
        {
            connectionString = configuration["ConnectionString"];
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

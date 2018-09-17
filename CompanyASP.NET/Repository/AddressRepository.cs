using CompanyASP.NET.Helper;
using CompanyASP.NET.Models;
using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace CompanyASP.NET.Repository
{
    public class AddressRepository : IRepository<Address>
    {
        private IDbContext DbContext;

        public AddressRepository(IDbContext dbContext)
        {
            DbContext = dbContext;
        }

        public int Create(Address obj)
        {
            // Check required fields
            if (obj.Street == null) throw new RepositoryException<RepositoryErrorType>(RepositoryErrorType.INVALID_ARGUMENT, "Street is missing");
            if (obj.Zip == null) throw new RepositoryException<RepositoryErrorType>(RepositoryErrorType.INVALID_ARGUMENT, "Zip is missing");
            if (obj.Place == null) throw new RepositoryException<RepositoryErrorType>(RepositoryErrorType.INVALID_ARGUMENT, "Place is missing");
            if (obj.Country == null) throw new RepositoryException<RepositoryErrorType>(RepositoryErrorType.INVALID_ARGUMENT, "Country is missing");

            using (IDbConnection con = DbContext.Connection)
            {
                var param = new DynamicParameters();
                /*foreach(PropertyInfo p in obj.GetType().GetProperties())
                {
                    param.Add(p.Name, p.GetValue(obj));
                }*/
                param.Add("addressId", null);
                param.Add("personId", null);
                param.Add("street", obj.Street);
                param.Add("zip", obj.Zip);
                param.Add("place", obj.Place);
                param.Add("countryCode", obj.Country);
                param.Add("returnValue", null, DbType.Int32, ParameterDirection.ReturnValue);

                try
                {
                    con.Execute("spInsertOrUpdateAddress", param, commandType: CommandType.StoredProcedure);

                    int returnValue = param.Get<Int32>("returnValue");
                    if (returnValue <= 0) throw new RepositoryException<RepositoryErrorType>(RepositoryErrorType.NOT_INSERTED);
                    return returnValue;
                }
                catch (SqlException ex)
                {
                    throw new RepositoryException<RepositoryErrorType>(RepositoryErrorType.SQL_EXCEPTION, ex.Message, ex);
                }
            }
        }

        public IEnumerable<int> Create(IEnumerable<Address> list)
        {
            List<int> result = new List<int>();

            DataTable dataTable = new DataTable();
            dataTable.Columns.Add("Street", typeof(string));
            dataTable.Columns.Add("Zip", typeof(string));
            dataTable.Columns.Add("Place", typeof(string));
            dataTable.Columns.Add("County", typeof(string));

            foreach (Address obj in list)
            {
                dataTable.Rows.Add(
                    obj.Street,
                    obj.Zip,
                    obj.Place,
                    obj.Country
                );
            }

            using (IDbConnection con = DbContext.Connection)
            {
                var cmd = con.CreateCommand();
                cmd.CommandText = "spInsertMultipleAddresses";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter { ParameterName = "Addresses", Value = dataTable, SqlDbType = SqlDbType.Structured });
                try
                {
                    IDataReader rdr = cmd.ExecuteReader();

                    while (rdr.Read())
                    {
                        result.Add(rdr.GetInt32(0));
                    }
                }
                catch (SqlException ex)
                {
                    throw new RepositoryException<RepositoryErrorType>(RepositoryErrorType.SQL_EXCEPTION, ex.Message, ex);
                }
            }

            return result;
        }

        public bool Delete(params int[] ids)
        {
            using (IDbConnection con = DbContext.Connection)
            {
                var param = new DynamicParameters();
                param.Add("Id", ids[0]);

                try
                {
                    return con.Execute("UPDATE Address SET DeletedTime=getDate() WHERE Id = @id", param) > 0;
                }
                catch (SqlException ex)
                {
                    throw new RepositoryException<RepositoryErrorType>(RepositoryErrorType.SQL_EXCEPTION, ex.Message, ex);
                }
            }
        }

        public Address Retrieve(params int[] ids)
        {
            Address result = null;
            using (IDbConnection con = DbContext.Connection)
            {
                var param = new DynamicParameters();
                param.Add("Id", ids[0]);

                try
                {
                    result = con.QueryFirstOrDefault<Address>(
                        @"SELECT [Id]
                          ,[Street]
                          ,[Zip]
                          ,[Place]
                          ,[Country]
                        FROM dbo.viAddress
                        WHERE Id = @Id
                    ", param);
                } catch (SqlException ex)
                {
                    throw new RepositoryException<RepositoryErrorType>(RepositoryErrorType.SQL_EXCEPTION, ex.Message, ex);
                }
            }
            if (result == null) throw new RepositoryException<RepositoryErrorType>(RepositoryErrorType.NOT_FOUND);
            return result;
        }

        public IEnumerable<Address> RetrieveAll(params int[] ids)
        {
            List<Address> result;
            using (IDbConnection con = DbContext.Connection)
            {
                try
                {
                    result = con.Query<Address>(
                        @"SELECT [Id]
                          ,[Street]
                          ,[Zip]
                          ,[Place]
                          ,[Country]
                        FROM viAddress"
                    ).AsList();
                } catch (SqlException ex)
                {
                    throw new RepositoryException<RepositoryErrorType>(RepositoryErrorType.SQL_EXCEPTION, ex.Message, ex);
                }
            }
            if (result.Count == 0) throw new RepositoryException<RepositoryErrorType>(RepositoryErrorType.NOT_FOUND);
            return result;
        }

        public bool Update(Address obj)
        {

            using (IDbConnection con = DbContext.Connection)
            {
                var param = new DynamicParameters();
                param.Add("addressId", obj.Id);
                param.Add("personId", null);
                param.Add("street", obj.Street);
                param.Add("zip", obj.Zip);
                param.Add("place", obj.Place);
                param.Add("countryCode", obj.Country);

                try
                {
                    return con.Execute("spInsertOrUpdateAddress", param, commandType: CommandType.StoredProcedure) > 0;
                }
                catch (SqlException ex)
                {
                    throw new RepositoryException<RepositoryErrorType>(RepositoryErrorType.SQL_EXCEPTION, ex.Message, ex);
                }
            }
        }

        public int UpdateAll(IEnumerable<Address> list)
        {
            int updates = 0;
            foreach (Address obj in list)
            {
                if(Update(obj)) updates++;
            }
            return updates;
        }
    }
}

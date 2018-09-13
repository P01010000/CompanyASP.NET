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
        private static AddressRepository instance = new AddressRepository();

        public static AddressRepository getInstance() { return instance; }

        public int Create(Address obj)
        {
            using (SqlConnection con = new SqlConnection(Startup.ConnectionString))
            {
                try
                {
                    con.Open();
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

                    con.Execute("spInsertOrUpdateAddress", param, commandType: CommandType.StoredProcedure);

                    return param.Get<Int32>("returnValue");
                }
                catch (SqlException ex)
                {
                    return -1;
                }
                finally
                {
                    try
                    {
                        con.Close();
                    }
                    catch (SqlException) { }
                }
            }
        }

        public bool Delete(params int[] ids)
        {
            using (SqlConnection con = new SqlConnection(Startup.ConnectionString))
            {
                var param = new DynamicParameters();
                param.Add("Id", ids[0]);

                try
                {
                    con.Open();
                    return con.Execute("UPDATE Address SET DeletedTime=getDate() WHERE Id = @id", param) > 0;
                }
                catch
                {
                    return false;
                }
                finally
                {
                    try
                    {
                        con.Close();
                    }
                    catch (SqlException ex)
                    {
                        Console.WriteLine(ex.Message);
                    }
                }
            }
        }

        public Address Retrieve(params int[] ids)
        {
            using (SqlConnection con = new SqlConnection(Startup.ConnectionString))
            {
                var param = new DynamicParameters();
                param.Add("Id", ids[0]);

                try
                {
                    con.Open();
                    return con.QueryFirstOrDefault<Address>(
                        @"SELECT [Id]
                          ,[Street]
                          ,[Zip]
                          ,[Place]
                          ,[Country]
                        FROM dbo.viAddress
                        WHERE Id = @Id
                    ", param);
                }
                finally
                {
                    try
                    {
                        con.Close();
                    }
                    catch (SqlException ex)
                    {
                        Console.WriteLine(ex.Message);
                    }
                }
            }
        }

        public IEnumerable<Address> RetrieveAll(params int[] ids)
        {
            using (SqlConnection con = new SqlConnection(Startup.ConnectionString))
            {
                try
                {
                    con.Open();

                    return con.Query<Address>(
                        @"SELECT [Id]
                          ,[Street]
                          ,[Zip]
                          ,[Place]
                          ,[Country]
                        FROM viAddress"
                    );
                }
                finally
                {
                    try
                    {
                        con.Close();
                    }
                    catch (SqlException ex)
                    {
                        Console.WriteLine(ex.Message);
                    }
                }
            }
        }

        public bool Update(Address obj)
        {

            using (SqlConnection con = new SqlConnection(Startup.ConnectionString))
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
                    con.Open();

                    return con.Execute("spInsertOrUpdateAddress", param, commandType: CommandType.StoredProcedure) > 0;
                }
                catch
                {
                    return false;
                }
                finally
                {
                    try
                    {
                        con.Close();
                    }
                    catch (SqlException ex)
                    {
                        Console.WriteLine(ex.Message);
                    }
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

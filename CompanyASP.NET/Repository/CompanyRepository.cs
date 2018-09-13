using CompanyASP.NET.Models;
using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace CompanyASP.NET.Repository
{
    public class CompanyRepository : IRepository<Company>
    {
        private static CompanyRepository instance = new CompanyRepository();

        public static CompanyRepository getInstance() { return instance; }

        public int Create(Company obj)
        {
            using (SqlConnection con = new SqlConnection(Startup.ConnectionString))
            {
                try
                {
                    con.Open();
                    var param = new DynamicParameters();
                    param.Add("cid", null);
                    param.Add("name", obj.Name);
                    param.Add("description", obj.Description);
                    param.Add("foundedAt", obj.FoundedAt);
                    param.Add("branch", obj.Branch);
                    param.Add("returnValue", null, DbType.Int32, ParameterDirection.ReturnValue);

                    con.Execute("spInsertOrUpdateCompany", param, commandType: CommandType.StoredProcedure);

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
                    return con.Execute("UPDATE Person SET DeletedTime=getDate() WHERE CompanyId = @id", param) > 0;
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

        public Company Retrieve(params int[] ids)
        {
            using (SqlConnection con = new SqlConnection(Startup.ConnectionString))
            {
                var param = new DynamicParameters();
                param.Add("Id", ids[0]);

                try
                {
                    con.Open();
                    return con.QueryFirstOrDefault<Company>(
                        @"SELECT [Id]
                          ,[PersonId]
                          ,[Name]
                          ,[Description]
                          ,[FoundedAt]
                          ,[Branch]
                        FROM dbo.viCompany
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

        public IEnumerable<Company> RetrieveAll(params int[] ids)
        {
            using (SqlConnection con = new SqlConnection(Startup.ConnectionString))
            {
                try
                {
                    con.Open();

                    return con.Query<Company>(
                        @"SELECT [Id]
                          ,[PersonId]
                          ,[Name]
                          ,[Description]
                          ,[FoundedAt]
                          ,[Branch]
                        FROM viCompany"
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

        public bool Update(Company obj)
        {

            using (SqlConnection con = new SqlConnection(Startup.ConnectionString))
            {
                var param = new DynamicParameters();
                param.Add("cid", obj.Id);
                param.Add("name", obj.Name);
                param.Add("description", obj.Description);
                param.Add("foundedAt", obj.FoundedAt);
                param.Add("branch", obj.Branch);

                try
                {
                    con.Open();

                    return con.Execute("spInsertOrUpdateCompany", param, commandType: CommandType.StoredProcedure) > 0;
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

        public int UpdateAll(IEnumerable<Company> list)
        {
            int updates = 0;
            foreach (Company obj in list)
            {
                if(Update(obj)) updates++;
            }
            return updates;
        }
    }
}

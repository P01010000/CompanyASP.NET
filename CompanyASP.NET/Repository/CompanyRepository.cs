using CompanyASP.NET.Models;
using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using CompanyASP.NET.Helper;
using Microsoft.EntityFrameworkCore;

namespace CompanyASP.NET.Repository
{
    public class CompanyRepository : IRepository<Company>
    {
        private IDbContext DbContext;
        public CompanyRepository(IDbContext dbContext)
        {
            DbContext = dbContext;
        }

        public int Create(Company obj)
        {
            if (obj.Name == null) throw new RepositoryException<CreateResultType>(CreateResultType.INVALID_ARGUMENT, "Name is missing");
            if (obj.Description == null) throw new RepositoryException<CreateResultType>(CreateResultType.INVALID_ARGUMENT, "Description is missing");
            if (obj.FoundedAt == null) throw new RepositoryException<CreateResultType>(CreateResultType.INVALID_ARGUMENT, "FoundedAt is missing");
            if (obj.Branch == null) throw new RepositoryException<CreateResultType>(CreateResultType.INVALID_ARGUMENT, "Branch is missing");

            using (IDbConnection con = DbContext.Connection)
            {
                var param = new DynamicParameters();
                param.Add("cid", null);
                param.Add("name", obj.Name);
                param.Add("description", obj.Description);
                param.Add("foundedAt", obj.FoundedAt);
                param.Add("branch", obj.Branch);
                param.Add("returnValue", null, DbType.Int32, ParameterDirection.ReturnValue);

                try
                {
                    con.Execute("spInsertOrUpdateCompany", param, commandType: CommandType.StoredProcedure);

                    int returnValue = param.Get<Int32>("returnValue");

                    if (returnValue <= 0) throw new RepositoryException<CreateResultType>(CreateResultType.NOT_INSERTED);
                    return returnValue;
                }
                catch (SqlException ex)
                {
                    throw new RepositoryException<CreateResultType>(CreateResultType.SQL_EXCEPTION, ex.Message);
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
            using (IDbConnection con = DbContext.Connection)
            {
                var param = new DynamicParameters();
                param.Add("Id", ids[0]);

                try
                {
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
            using (IDbConnection con = DbContext.Connection)
            {
                var param = new DynamicParameters();
                param.Add("Id", ids[0]);

                try
                {
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
            using (IDbConnection con = DbContext.Connection)
            {
                try
                {
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

            using (IDbConnection con = DbContext.Connection)
            {
                var param = new DynamicParameters();
                param.Add("cid", obj.Id);
                param.Add("name", obj.Name);
                param.Add("description", obj.Description);
                param.Add("foundedAt", obj.FoundedAt);
                param.Add("branch", obj.Branch);

                try
                {
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

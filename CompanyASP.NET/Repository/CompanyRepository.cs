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
            // Check required fields
            if (obj.Name == null) throw new RepositoryException<RepositoryErrorType>(RepositoryErrorType.INVALID_ARGUMENT, "Name is missing");
            if (obj.FoundedAt == null) throw new RepositoryException<RepositoryErrorType>(RepositoryErrorType.INVALID_ARGUMENT, "FoundedAt is missing");

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

                    if (returnValue <= 0) throw new RepositoryException<RepositoryErrorType>(RepositoryErrorType.NOT_INSERTED);
                    return returnValue;
                }
                catch (SqlException ex)
                {
                    throw new RepositoryException<RepositoryErrorType>(RepositoryErrorType.SQL_EXCEPTION, ex.Message);
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
            }
        }

        public Company Retrieve(params int[] ids)
        {
            Company result = null;
            using (IDbConnection con = DbContext.Connection)
            {
                var param = new DynamicParameters();
                param.Add("Id", ids[0]);

                try
                {
                    result = con.QueryFirstOrDefault<Company>(
                        @"SELECT [Id]
                          ,[PersonId]
                          ,[Name]
                          ,[Description]
                          ,[FoundedAt]
                          ,[Branch]
                        FROM dbo.viCompany
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

        public IEnumerable<Company> RetrieveAll(params int[] ids)
        {
            List<Company> result;
            using (IDbConnection con = DbContext.Connection)
            {
                try
                {
                    result = con.Query<Company>(
                        @"SELECT [Id]
                          ,[PersonId]
                          ,[Name]
                          ,[Description]
                          ,[FoundedAt]
                          ,[Branch]
                        FROM viCompany"
                    ).AsList();
                } catch (SqlException ex)
                {
                    throw new RepositoryException<RepositoryErrorType>(RepositoryErrorType.SQL_EXCEPTION, ex.Message, ex);
                }
            }
            if (result.Count == 0) throw new RepositoryException<RepositoryErrorType>(RepositoryErrorType.NOT_FOUND);
            return result;
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

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

            try
            {
                using (IDbConnection con = DbContext.Connection)
                {
                    var param = new DynamicParameters();
                    param.Add("cid", null);
                    param.Add("name", obj.Name);
                    param.Add("description", obj.Description);
                    param.Add("foundedAt", obj.FoundedAt);
                    param.Add("branch", obj.Branch);
                    param.Add("returnValue", null, DbType.Int32, ParameterDirection.ReturnValue);

                    con.Execute("spInsertOrUpdateCompany", param, commandType: CommandType.StoredProcedure);

                    int returnValue = param.Get<Int32>("returnValue");

                    if (returnValue <= 0) throw new RepositoryException<RepositoryErrorType>(RepositoryErrorType.NOT_INSERTED);
                    return returnValue;
                }
            } catch (SqlException ex)
            {
                throw new RepositoryException<RepositoryErrorType>(RepositoryErrorType.SQL_EXCEPTION, ex.Message);
            }
        }

        public IEnumerable<int> Create(IEnumerable<Company> list)
        {
            List<int> result = new List<int>();

            DataTable dataTable = new DataTable();
            dataTable.Columns.Add("Name", typeof(string));
            dataTable.Columns.Add("Description", typeof(string));
            dataTable.Columns.Add("FoundedAt", typeof(DateTime));
            dataTable.Columns.Add("Branch", typeof(string));

            foreach (Company obj in list)
            {
                dataTable.Rows.Add(
                    obj.Name,
                    obj.Description,
                    obj.FoundedAt,
                    obj.Branch
                );
            }

            try
            {
                using (IDbConnection con = DbContext.Connection)
                {
                    var cmd = con.CreateCommand();
                    cmd.CommandText = "spInsertMultipleCompanies";
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add(new SqlParameter { ParameterName = "Companies", Value = dataTable, SqlDbType = SqlDbType.Structured });
                    
                    IDataReader rdr = cmd.ExecuteReader();

                    while (rdr.Read())
                    {
                        result.Add(rdr.GetInt32(0));
                    }
                }
            } catch (SqlException ex)
            {
                throw new RepositoryException<RepositoryErrorType>(RepositoryErrorType.SQL_EXCEPTION, ex.Message, ex);
            }

            return result;
        }

        public bool Delete(params int[] ids)
        {
            try
            {
                using (IDbConnection con = DbContext.Connection)
                {
                    var param = new DynamicParameters();
                    param.Add("Id", ids[0]);

                    return con.Execute("UPDATE Person SET DeletedTime=getDate() WHERE CompanyId = @id", param) > 0;
                }
            } catch (SqlException ex)
            {
                throw new RepositoryException<RepositoryErrorType>(RepositoryErrorType.SQL_EXCEPTION, ex.Message, ex);
            }
        }

        public Company Retrieve(params int[] ids)
        {
            Company result = null;
            try
            {
                using (IDbConnection con = DbContext.Connection)
                {
                    var param = new DynamicParameters();
                    param.Add("Id", ids[0]);

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
                }
            } catch (SqlException ex)
            {
                throw new RepositoryException<RepositoryErrorType>(RepositoryErrorType.SQL_EXCEPTION, ex.Message, ex);
            }

            if (result == null) throw new RepositoryException<RepositoryErrorType>(RepositoryErrorType.NOT_FOUND);
            return result;
        }

        public IEnumerable<Company> RetrieveAll(params int[] ids)
        {
            List<Company> result;
            try
            {
                using (IDbConnection con = DbContext.Connection)
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
                }
            } catch (SqlException ex)
            {
                throw new RepositoryException<RepositoryErrorType>(RepositoryErrorType.SQL_EXCEPTION, ex.Message, ex);
            }

            if (result.Count == 0) throw new RepositoryException<RepositoryErrorType>(RepositoryErrorType.NOT_FOUND);
            return result;
        }

        public bool Update(Company obj)
        {
            try
            {
                using (IDbConnection con = DbContext.Connection)
                {
                    var param = new DynamicParameters();
                    param.Add("cid", obj.Id);
                    param.Add("name", obj.Name);
                    param.Add("description", obj.Description);
                    param.Add("foundedAt", obj.FoundedAt);
                    param.Add("branch", obj.Branch);

                    return con.Execute("spInsertOrUpdateCompany", param, commandType: CommandType.StoredProcedure) > 0;
                }
            } catch (SqlException ex)
            {
                throw new RepositoryException<RepositoryErrorType>(RepositoryErrorType.SQL_EXCEPTION, ex.Message, ex);
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

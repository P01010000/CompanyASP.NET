using CompanyASP.NET.Helper;
using CompanyASP.NET.Models;
using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace CompanyASP.NET.Repository
{
    public class DepartmentRepository : IRepository<Department>
    {
        private IDbContext DbContext;

        public DepartmentRepository(IDbContext dbContext)
        {
            DbContext = dbContext;
        }

        public int Create(Department obj)
        {
            if (obj.Name == null) throw new RepositoryException<RepositoryErrorType>(RepositoryErrorType.INVALID_ARGUMENT, "Name is missing");
            if (obj.Description == null) throw new RepositoryException<RepositoryErrorType>(RepositoryErrorType.INVALID_ARGUMENT, "Description is missing");
            if (obj.SuperDepartment == null) throw new RepositoryException<RepositoryErrorType>(RepositoryErrorType.INVALID_ARGUMENT, "SuperDepartment is missing");
            if (obj.CompanyId == null) throw new RepositoryException<RepositoryErrorType>(RepositoryErrorType.INVALID_ARGUMENT, "CompanyId is missing");

            using (IDbConnection con = DbContext.Connection)
            {
                var param = new DynamicParameters();
                param.Add("did", null);
                param.Add("name", obj.Name);
                param.Add("description", obj.Description);
                param.Add("supervisor", obj.Supervisor);
                param.Add("superDepartment", obj.SuperDepartment);
                param.Add("companyId", obj.CompanyId);
                param.Add("returnValue", null, DbType.Int32, ParameterDirection.ReturnValue);

                try
                {
                    con.Execute("spInsertOrUpdateDepartment", param, commandType: CommandType.StoredProcedure);

                    int returnValue = param.Get<Int32>("returnValue");
                    if (returnValue <= 0) throw new RepositoryException<RepositoryErrorType>(RepositoryErrorType.NOT_INSERTED);
                    return param.Get<Int32>("returnValue");
                }
                catch (SqlException ex)
                {
                    throw new RepositoryException<RepositoryErrorType>(RepositoryErrorType.SQL_EXCEPTION, ex.Message, ex);
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
                    return con.Execute("UPDATE Department SET DeletedTime=getDate() WHERE Id = @id", param) > 0;
                }
                catch (SqlException ex)
                {
                    throw new RepositoryException<RepositoryErrorType>(RepositoryErrorType.SQL_EXCEPTION, ex.Message, ex);
                }
            }
        }

        public Department Retrieve(params int[] ids)
        {
            Department result = null;
            using (IDbConnection con = DbContext.Connection)
            {
                string cmdText =
                    @"SELECT Id,
		                Name,
		                Description,
		                Supervisor,
		                SuperDepartment,
		                CompanyId,
		                CreationTime
                    FROM viDepartment
                    WHERE Id = @Id
                    ";
                var param = new DynamicParameters();
                param.Add("Id", ids[0]);
                if(ids.Length > 1)
                {
                    cmdText += " AND CompanyId = @CompanyId";
                    param.Add("CompanyId", ids[1]);
                }
                try
                {
                    result = con.QueryFirstOrDefault<Department>(cmdText, param);
                } catch (SqlException ex)
                {
                    throw new RepositoryException<RepositoryErrorType>(RepositoryErrorType.SQL_EXCEPTION, ex.Message, ex);
                }
            }
            if (result == null) throw new RepositoryException<RepositoryErrorType>(RepositoryErrorType.NOT_FOUND);
            return result;
        }

        public IEnumerable<Department> RetrieveAll(params int[] ids)
        {
            List<Department> result;
            using (IDbConnection con = DbContext.Connection)
            {
                var param = new DynamicParameters();
                var commandText = 
                    @"SELECT Id,
		                Name,
		                Description,
		                Supervisor,
	                    SuperDepartment,
	                    CompanyId,
	                    CreationTime
                    FROM viDepartment
                    ";
                if( ids.Length > 0 )
                {
                    commandText = commandText + " WHERE CompanyId = @companyId";
                    param.Add("CompanyId", ids[0]);
                }
                try
                {
                    result = con.Query<Department>(commandText, param).AsList();
                } catch (SqlException ex)
                {
                    throw new RepositoryException<RepositoryErrorType>(RepositoryErrorType.SQL_EXCEPTION, ex.Message, ex);
                }
            }
            if (result.Count == 0) throw new RepositoryException<RepositoryErrorType>(RepositoryErrorType.NOT_FOUND);
            return result;
        }

        public bool Update(Department obj)
        {
            using (IDbConnection con = DbContext.Connection)
            {
                var param = new DynamicParameters();
                param.Add("did", obj.Id);
                param.Add("name", obj.Name);
                param.Add("description", obj.Description);
                param.Add("supervisor", obj.Supervisor);
                param.Add("superDepartment", obj.SuperDepartment);
                param.Add("companyId", obj.CompanyId);
                try
                {
                    return con.Execute("spInsertOrUpdateDepartment", param, commandType: CommandType.StoredProcedure) > 0;
                }
                catch (SqlException ex)
                {
                    throw new RepositoryException<RepositoryErrorType>(RepositoryErrorType.SQL_EXCEPTION, ex.Message, ex);
                }
            }
        }

        public int UpdateAll(IEnumerable<Department> list)
        {
            int updates = 0;
            foreach (Department obj in list)
            {
                if(Update(obj)) updates++;
            }
            return updates;
        }
    }
}

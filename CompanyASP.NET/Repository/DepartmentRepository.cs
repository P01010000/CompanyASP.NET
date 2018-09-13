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
        private static DepartmentRepository instance = new DepartmentRepository();

        public static DepartmentRepository getInstance() { return instance; }

        public int Create(Department obj)
        {
            using (SqlConnection con = new SqlConnection(Startup.ConnectionString))
            {
                try
                {
                    con.Open();
                    var param = new DynamicParameters();
                    param.Add("did", null);
                    param.Add("name", obj.Name);
                    param.Add("description", obj.Description);
                    param.Add("supervisor", obj.Supervisor);
                    param.Add("superDepartment", obj.SuperDepartment);
                    param.Add("companyId", obj.CompanyId);
                    param.Add("returnValue", null, DbType.Int32, ParameterDirection.ReturnValue);

                    con.Execute("spInsertOrUpdateDepartment", param, commandType: CommandType.StoredProcedure);

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
                    return con.Execute("UPDATE Department SET DeletedTime=getDate() WHERE Id = @id", param) > 0;
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

        public Department Retrieve(params int[] ids)
        {
            using (SqlConnection con = new SqlConnection(Startup.ConnectionString))
            {
                var param = new DynamicParameters();
                param.Add("CompanyId", ids[0]);
                param.Add("Id", ids[1]);
                try
                {
                    con.Open();
                    return con.QueryFirstOrDefault<Department>(
                        @"SELECT Id,
		                    Name,
		                    Description,
		                    Supervisor,
		                    SuperDepartment,
		                    CompanyId,
		                    CreationTime
                        FROM viDepartment
                        WHERE Id = @Id
                        AND CompanyId = @CompanyId
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

        public IEnumerable<Department> RetrieveAll(params int[] ids)
        {
            using (SqlConnection con = new SqlConnection(Startup.ConnectionString))
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
                    con.Open();

                    return con.Query<Department>(commandText, param);
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

        public bool Update(Department obj)
        {

            using (SqlConnection con = new SqlConnection(Startup.ConnectionString))
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
                    con.Open();

                    return con.Execute("spInsertOrUpdateDepartment", param, commandType: CommandType.StoredProcedure) > 0;
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

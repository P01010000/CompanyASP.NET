using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text.RegularExpressions;
using CompanyASP.NET.Models;
using Dapper;
using System.Reflection;

namespace CompanyASP.NET.Repository
{
    public class EmployeeRepository : IRepository<Employee>
    {
        private static EmployeeRepository instance = new EmployeeRepository();

        public static EmployeeRepository getInstance() { return instance; }

        public int Create(Employee obj)
        {
            using (SqlConnection con = new SqlConnection(Startup.ConnectionString))
            {
                try
                {
                    con.Open();
                    var param = new DynamicParameters();
                    param.Add("id", null);
                    param.Add("personId", null);
                    param.Add("lastName", obj.LastName);
                    param.Add("firstName", obj.FirstName);
                    param.Add("birthday", obj.Birthday);
                    param.Add("phone", obj.Phone);
                    if(obj.Gender != null && new Regex("[0-9]+").IsMatch(obj.Gender)) param.Add("gender", Convert.ToInt32(obj.Gender));
                    param.Add("employeeSince", obj.EmployeeSince);
                    param.Add("returnValue", null, DbType.Int32, ParameterDirection.ReturnValue);

                    con.Execute("spInsertOrUpdateEmployee", param, commandType: CommandType.StoredProcedure);

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
                    return con.Execute("UPDATE Person SET DeletedTime=getDate() WHERE EmployeeId = @id", param) > 0;
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

        public Employee Retrieve(params int[] ids)
        {
            using (SqlConnection con = new SqlConnection(Startup.ConnectionString))
            {
                var param = new DynamicParameters();
                param.Add("Id", ids[0]);

                try
                {
                    con.Open();
                    return con.QueryFirstOrDefault<Employee>(
                        @"SELECT Id,
                            PersonId,
                            LastName,
                            FirstName,
                            Birthday,
                            Phone,
                            Gender,
                            EmployeeSince
                        FROM dbo.viEmployee
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

        public IEnumerable<Employee> RetrieveAll(params int[] ids)
        {
            using (SqlConnection con = new SqlConnection(Startup.ConnectionString))
            {
                try
                {
                    con.Open();

                    return con.Query<Employee>(
                        @"SELECT Id,
                            PersonId,
                            LastName,
                            FirstName,
                            Birthday,
                            Phone,
                            Gender,
                            EmployeeSince
                        FROM viEmployee"
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

        public bool Update(Employee obj)
        {

            using (SqlConnection con = new SqlConnection(Startup.ConnectionString))
            {
                var param = new DynamicParameters();
                param.Add("eid", obj.Id);
                param.Add("lastName", obj.LastName);
                param.Add("firstName", obj.FirstName);
                param.Add("birthday", obj.Birthday);
                param.Add("phone", obj.Birthday);
                if (obj.Gender != null && new Regex("[0-9]+").IsMatch(obj.Gender)) param.Add("gender", Convert.ToInt32(obj.Gender));
                param.Add("employeeSince", obj.EmployeeSince);

                try
                {
                    con.Open();

                    return con.Execute("spInsertOrUpdateEmployee", param, commandType: CommandType.StoredProcedure) > 0;
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

        public int UpdateAll(IEnumerable<Employee> list)
        {
            int updates = 0;
            foreach (Employee obj in list)
            {
                if(Update(obj)) updates++;
            }
            return updates;
        }
    }
}

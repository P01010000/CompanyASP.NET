using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text.RegularExpressions;
using CompanyASP.NET.Models;
using Dapper;
using CompanyASP.NET.Helper;

namespace CompanyASP.NET.Repository
{
    public class EmployeeRepository : IRepository<Employee>
    {
        private readonly IDbContext DbContext;

        public EmployeeRepository(IDbContext dbContext)
        {
            DbContext = dbContext;
        }

        public int Create(Employee obj)
        {
            // Check required fields and validate if necessary
            if (obj.LastName == null) throw new RepositoryException<RepositoryErrorType>(RepositoryErrorType.INVALID_ARGUMENT, "LastName is missing");
            if (obj.FirstName == null) throw new RepositoryException<RepositoryErrorType>(RepositoryErrorType.INVALID_ARGUMENT, "FirstName is missing");
            if (obj.Birthday == null) throw new RepositoryException<RepositoryErrorType>(RepositoryErrorType.INVALID_ARGUMENT, "Birthday is missing");
            if (obj.Phone == null) throw new RepositoryException<RepositoryErrorType>(RepositoryErrorType.INVALID_ARGUMENT, "Phone is missing");
            if (obj.Gender == null) throw new RepositoryException<RepositoryErrorType>(RepositoryErrorType.INVALID_ARGUMENT, "Gender is missing");
            if (!new Regex("[0-9]+").IsMatch(obj.Gender)) throw new RepositoryException<RepositoryErrorType>(RepositoryErrorType.INVALID_ARGUMENT, "Gender has to be '1' for male, '2' for female any digit for 'other'");
            if (obj.EmployeeSince == null) throw new RepositoryException<RepositoryErrorType>(RepositoryErrorType.INVALID_ARGUMENT, "EmployeeSince is missing");

            using (IDbConnection con = DbContext.Connection)
            {
                var param = new DynamicParameters();
                param.Add("id", null);
                param.Add("personId", null);
                param.Add("lastName", obj.LastName);
                param.Add("firstName", obj.FirstName);
                param.Add("birthday", obj.Birthday);
                param.Add("phone", obj.Phone);
                try
                {
                    param.Add("gender", Convert.ToInt32(obj.Gender));
                } catch (Exception)
                {
                    throw new RepositoryException<RepositoryErrorType>(RepositoryErrorType.INVALID_ARGUMENT, $"Gender '{obj.Gender}' can't be converted to integer");
                }
                param.Add("employeeSince", obj.EmployeeSince);
                param.Add("returnValue", null, DbType.Int32, ParameterDirection.ReturnValue);

                try
                {
                    con.Execute("spInsertOrUpdateEmployee", param, commandType: CommandType.StoredProcedure);

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

        public IEnumerable<int> Create(IEnumerable<Employee> list)
        {
            List<int> result = new List<int>();

            DataTable dataTable = new DataTable();
            dataTable.Columns.Add("LastName", typeof(string));
            dataTable.Columns.Add("FirstName", typeof(string));
            dataTable.Columns.Add("Birthday", typeof(DateTime));
            dataTable.Columns.Add("Phone", typeof(string));
            dataTable.Columns.Add("Gender", typeof(int));
            dataTable.Columns.Add("EmployeeSince", typeof(DateTime));

            foreach(Employee obj in list)
            {
                int gender;
                Int32.TryParse(obj.Gender, out gender);
                dataTable.Rows.Add(
                    obj.LastName,
                    obj.FirstName,
                    obj.Birthday,
                    obj.Phone,
                    gender,
                    obj.EmployeeSince
                );
            }

            using (IDbConnection con = DbContext.Connection)
            {
                var cmd = con.CreateCommand();
                cmd.CommandText = "spInsertMultipleEmployee";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter { ParameterName = "Employees", Value = dataTable, SqlDbType = SqlDbType.Structured });
                try
                {
                    IDataReader rdr = cmd.ExecuteReader();

                    while(rdr.Read())
                    {
                        result.Add(rdr.GetInt32(0));
                    }
                }  catch (SqlException ex)
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
                    return con.Execute("UPDATE Person SET DeletedTime=getDate() WHERE EmployeeId = @id", param) > 0;
                }
                catch (SqlException ex)
                {
                    throw new RepositoryException<RepositoryErrorType>(RepositoryErrorType.SQL_EXCEPTION, ex.Message, ex);
                }
            }
        }

        public Employee Retrieve(params int[] ids)
        {
            Employee result = null;
            using (IDbConnection con = DbContext.Connection)
            {
                var param = new DynamicParameters();
                param.Add("Id", ids[0]);

                try
                {
                    result = con.QueryFirstOrDefault<Employee>(
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
                } catch (SqlException ex)
                {
                    throw new RepositoryException<RepositoryErrorType>(RepositoryErrorType.SQL_EXCEPTION, ex.Message, ex);
                }
            }
            if (result == null) throw new RepositoryException<RepositoryErrorType>(RepositoryErrorType.NOT_FOUND);
            return result;
        }

        public IEnumerable<Employee> RetrieveAll(params int[] ids)
        {
            List<Employee> result;
            using (IDbConnection con = DbContext.Connection)
            {
                try
                {
                    result = con.Query<Employee>(
                        @"SELECT Id,
                            PersonId,
                            LastName,
                            FirstName,
                            Birthday,
                            Phone,
                            Gender,
                            EmployeeSince
                        FROM viEmployee"
                    ).AsList();
                } catch (SqlException ex)
                {
                    throw new RepositoryException<RepositoryErrorType>(RepositoryErrorType.SQL_EXCEPTION, ex.Message, ex);
                }
            }
            if (result.Count == 0) throw new RepositoryException<RepositoryErrorType>(RepositoryErrorType.NOT_FOUND);
            return result;
        }

        public bool Update(Employee obj)
        {
            int? gender = null;
            try
            {
                if (obj.Gender != null) gender = Convert.ToInt32(obj.Gender);
            } catch (Exception)
            {
                throw new RepositoryException<RepositoryErrorType>(RepositoryErrorType.INVALID_ARGUMENT, $"Gender '{obj.Gender}' can't be converted to integer");
            }

            using (IDbConnection con = DbContext.Connection)
            {
                var param = new DynamicParameters();
                param.Add("eid", obj.Id);
                param.Add("lastName", obj.LastName);
                param.Add("firstName", obj.FirstName);
                param.Add("birthday", obj.Birthday);
                param.Add("phone", obj.Birthday);
                param.Add("gender", gender);
                param.Add("employeeSince", obj.EmployeeSince);

                try
                {
                    return con.Execute("spInsertOrUpdateEmployee", param, commandType: CommandType.StoredProcedure) > 0;
                }
                catch (SqlException ex)
                {
                    throw new RepositoryException<RepositoryErrorType>(RepositoryErrorType.SQL_EXCEPTION, ex.Message, ex);
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

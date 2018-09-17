using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text.RegularExpressions;
using CompanyASP.NET.Models;
using Dapper;
using CompanyASP.NET.Helper;
using System.Reflection;
using System.ComponentModel.DataAnnotations;

namespace CompanyASP.NET.Repository
{
    public class SqlRepository<T> : IRepository<T> where T : IModel
    {
        private readonly IDbContext DbContext;

        public SqlRepository(IDbContext dbContext)
        {
            DbContext = dbContext;
        }

        public int Create(T obj)
        {
            // Check required fields and validate if necessary
            
            using (IDbConnection con = DbContext.Connection)
            {
                var param = new DynamicParameters();
                foreach(PropertyInfo prop in obj.GetType().GetProperties())
                {
                    bool required = false;
                    bool write = true;
                    foreach(var attr in Attribute.GetCustomAttributes(prop))
                    {
                        if (attr is RequiredAttribute) required = true;
                        if (attr is KeyAttribute) prop.SetValue(obj, null);
                        if (attr is EditableAttribute) write = ((EditableAttribute)attr).AllowEdit;
                    }
                    if ( required && prop.GetValue(obj) == null)
                        throw new RepositoryException<RepositoryErrorType>(RepositoryErrorType.INVALID_ARGUMENT, $"{prop.Name} is missing");
                    if (write) param.Add(prop.Name, prop.GetValue(obj));
                }
                param.Add("returnValue", null, DbType.Int32, ParameterDirection.ReturnValue);

                try
                {
                    con.Execute($"spInsertOrUpdate{obj.GetType().Name}", param, commandType: CommandType.StoredProcedure);

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

        public IEnumerable<int> Create(IEnumerable<T> list)
        {
            List<int> result = new List<int>();

            //DataTable dataTable = new DataTable();
            //dataTable.Columns.Add("LastName", typeof(string));
            //dataTable.Columns.Add("FirstName", typeof(string));
            //dataTable.Columns.Add("Birthday", typeof(DateTime));
            //dataTable.Columns.Add("Phone", typeof(string));
            //dataTable.Columns.Add("Gender", typeof(int));
            //dataTable.Columns.Add("EmployeeSince", typeof(DateTime));

            //foreach(T obj in list)
            //{
            //    int gender;
            //    Int32.TryParse(obj.Gender, out gender);
            //    dataTable.Rows.Add(
            //        obj.LastName,
            //        obj.FirstName,
            //        obj.Birthday,
            //        obj.Phone,
            //        gender,
            //        obj.EmployeeSince
            //    );
            //}

            //using (IDbConnection con = DbContext.Connection)
            //{
            //    var cmd = con.CreateCommand();
            //    cmd.CommandText = "spInsertMultipleEmployee";
            //    cmd.CommandType = CommandType.StoredProcedure;
            //    cmd.Parameters.Add(new SqlParameter { ParameterName = "Employees", Value = dataTable, SqlDbType = SqlDbType.Structured });
            //    try
            //    {
            //        IDataReader rdr = cmd.ExecuteReader();

            //        while(rdr.Read())
            //        {
            //            result.Add(rdr.GetInt32(0));
            //        }
            //    }  catch (SqlException ex)
            //    {
            //        throw new RepositoryException<RepositoryErrorType>(RepositoryErrorType.SQL_EXCEPTION, ex.Message, ex);
            //    }
            //}

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

        public T Retrieve(params int[] ids)
        {
            T result;
            using (IDbConnection con = DbContext.Connection)
            {
                var param = new DynamicParameters();
                param.Add("Id", ids[0]);

                try
                {
                    result = con.QueryFirstOrDefault<T>(
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

        public IEnumerable<T> RetrieveAll(params int[] ids)
        {
            List<T> result;
            using (IDbConnection con = DbContext.Connection)
            {
                try
                {
                    result = con.Query<T>(
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

        public bool Update(T obj)
        {
            //int? gender = null;
            //try
            //{
            //    if (obj.Gender != null) gender = Convert.ToInt32(obj.Gender);
            //} catch (Exception)
            //{
            //    throw new RepositoryException<RepositoryErrorType>(RepositoryErrorType.INVALID_ARGUMENT, $"Gender '{obj.Gender}' can't be converted to integer");
            //}

            //using (IDbConnection con = DbContext.Connection)
            //{
            //    var param = new DynamicParameters();
            //    param.Add("eid", obj.Id);
            //    param.Add("lastName", obj.LastName);
            //    param.Add("firstName", obj.FirstName);
            //    param.Add("birthday", obj.Birthday);
            //    param.Add("phone", obj.Birthday);
            //    param.Add("gender", gender);
            //    param.Add("employeeSince", obj.EmployeeSince);

            //    try
            //    {
            //        return con.Execute("spInsertOrUpdateEmployee", param, commandType: CommandType.StoredProcedure) > 0;
            //    }
            //    catch (SqlException ex)
            //    {
            //        throw new RepositoryException<RepositoryErrorType>(RepositoryErrorType.SQL_EXCEPTION, ex.Message, ex);
            //    }
            //}
            return false;
        }

        public int UpdateAll(IEnumerable<T> list)
        {
            int updates = 0;
            foreach (T obj in list)
            {
                if(Update(obj)) updates++;
            }
            return updates;
        }
    }
}

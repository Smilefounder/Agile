using Agile.Helpers;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Agile.Data.Helpers
{
    public class DataHelper
    {
        static DataHelper()
        {
            var conn = ConfigurationManager.ConnectionStrings["DefaultConnection"];
            if (conn != null)
            {
                _connStr = conn.ConnectionString;
            }
        }

        private static string _connStr = null;

        public static int ExecuteNonQuery(string sqlstr, params SqlParameter[] sps)
        {
            using (var conn = new SqlConnection(_connStr))
            {
                conn.Open();

                var cmd = conn.CreateCommand();
                cmd.CommandText = sqlstr;
                cmd.Parameters.AddRange(sps);

                try
                {
                    return cmd.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message + "\r\nSQL:" + sqlstr);
                }
            }
        }

        public static int ExecuteNonQuery(string sqlstr, object obj)
        {
            using (var conn = new SqlConnection(_connStr))
            {
                conn.Open();

                var sps = ParameterHelper.CreateSqlParameter(obj);
                var cmd = conn.CreateCommand();
                cmd.CommandText = sqlstr;
                cmd.Parameters.AddRange(sps);

                try
                {
                    return cmd.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message + "\r\nSQL:" + sqlstr);
                }
            }
        }

        public static object ExecuteScalar(string sqlstr, params SqlParameter[] sps)
        {
            using (var conn = new SqlConnection(_connStr))
            {
                conn.Open();

                var cmd = conn.CreateCommand();
                cmd.CommandText = sqlstr;
                cmd.Parameters.AddRange(sps);

                try
                {
                    return cmd.ExecuteScalar();
                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message + "\r\nSQL:" + sqlstr);
                }
            }
        }

        public static object ExecuteScalar(string sqlstr, object obj)
        {
            using (var conn = new SqlConnection(_connStr))
            {
                conn.Open();

                var sps = ParameterHelper.CreateSqlParameter(obj);
                var cmd = conn.CreateCommand();
                cmd.CommandText = sqlstr;
                cmd.Parameters.AddRange(sps);

                try
                {
                    return cmd.ExecuteScalar();
                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message + "\r\nSQL:" + sqlstr);
                }
            }
        }

        public static DataTable ExecuteDataTable(string sqlstr, params SqlParameter[] sps)
        {
            var adapter = new SqlDataAdapter(sqlstr, _connStr);
            adapter.SelectCommand.Parameters.AddRange(sps);

            var table = new DataTable();
            try
            {
                adapter.Fill(table);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message + "\r\nSQL:" + sqlstr);
            }

            return table;
        }

        public static DataTable ExecuteDataTable(string sqlstr, object obj)
        {
            var sps = ParameterHelper.CreateSqlParameter(obj);
            var adapter = new SqlDataAdapter(sqlstr, _connStr);
            adapter.SelectCommand.Parameters.AddRange(sps);

            var table = new DataTable();
            try
            {
                adapter.Fill(table);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message + "\r\nSQL:" + sqlstr);
            }

            return table;
        }

        public static List<T> ExecuteList<T>(string sqlstr, params SqlParameter[] sps)
        {
            var table = ExecuteDataTable(sqlstr, sps);
            var list = ReflectHelper.DataTableToList<T>(table);
            return list;
        }

        public static List<T> ExecuteList<T>(string sqlstr, object obj)
        {
            var table = ExecuteDataTable(sqlstr, obj);
            var list = ReflectHelper.DataTableToList<T>(table);
            return list;
        }

        public static T ExecuteOne<T>(string sqlstr, params SqlParameter[] sps)
        {
            var table = ExecuteDataTable(sqlstr, sps);
            var list = ReflectHelper.DataTableToList<T>(table);
            if (list.Count == 0)
            {
                return Activator.CreateInstance<T>();
            }

            return list.FirstOrDefault();
        }

        public static T ExecuteOne<T>(string sqlstr, object obj)
        {
            var table = ExecuteDataTable(sqlstr, obj);
            var list = ReflectHelper.DataTableToList<T>(table);
            if (list.Count == 0)
            {
                return Activator.CreateInstance<T>();
            }

            return list.FirstOrDefault();
        }
    }
}

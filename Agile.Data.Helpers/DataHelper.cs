using Agile.Helpers;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
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

                return cmd.ExecuteNonQuery();
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

                return cmd.ExecuteScalar();
            }
        }

        public static DataTable ExecuteDataTable(string sqlstr, params SqlParameter[] sps)
        {
            var adapter = new SqlDataAdapter(sqlstr, _connStr);
            adapter.SelectCommand.Parameters.AddRange(sps);

            var table = new DataTable();
            adapter.Fill(table);

            return table;
        }

        public static List<T> ExecuteList<T>(string sqlstr, params SqlParameter[] sps)
        {
            var table = ExecuteDataTable(sqlstr, sps);
            var list = ReflectHelper.DataTableToList<T>(table);
            return list;
        }
    }
}

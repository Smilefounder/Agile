using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Agile.Data.Helpers
{
    public class ParameterHelper
    {
        public static SqlParameter[] CreateSqlParameter(object obj)
        {
            if (obj == null)
            {
                return null;
            }

            var splist = new List<SqlParameter>();
            var ttype = obj.GetType();
            var tps = ttype.GetProperties();
            foreach (var tp in tps)
            {
                splist.Add(CreateSqlParameter(obj, ttype, tp));
            }

            return splist.ToArray();
        }

        public static SqlParameter CreateSqlParameter(object obj, Type ttype, PropertyInfo tp)
        {
            object ivalue = null;
            try
            {
                ivalue = tp.GetValue(obj, null);
            }
            catch
            { }

            var sp = new SqlParameter();
            sp.ParameterName = tp.Name;
            sp.DbType = DbTypeConverter(ttype);

            if (ivalue == null)
            {
                sp.Value = DBNull.Value;
                sp.IsNullable = true;
            }
            else
            {
                sp.Value = ivalue;
            }

            return sp;
        }

        public static SqlParameter CreateSqlParameter<T>(T obj, PropertyInfo p, int maxLength, bool isoutput = false)
        {
            var dbValue = p.GetValue(obj, null);
            var dbType = DbTypeConverter(p.PropertyType);
            if (dbValue == null)
            {
                dbValue = DBNull.Value;
            }

            var direction = ParameterDirection.Input;
            if (isoutput)
            {
                direction = ParameterDirection.Output;
            }

            return new SqlParameter
            {
                Direction = direction,
                ParameterName = "@" + p.Name,
                DbType = dbType,
                Size = maxLength,
                Value = dbValue,
                IsNullable = dbValue == null
            };
        }

        public static DbType DbTypeConverter(Type ttype)
        {
            if (ttype == typeof(bool))
            {
                return DbType.Boolean;
            }

            if (ttype == typeof(decimal))
            {
                return DbType.Decimal;
            }

            if (ttype == typeof(int))
            {
                return DbType.Int32;
            }

            if (ttype == typeof(long))
            {
                return DbType.Int64;
            }

            if (ttype == typeof(DateTime))
            {
                return DbType.DateTime;
            }

            return DbType.String;
        }
    }
}

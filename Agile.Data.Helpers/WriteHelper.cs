using Agile.Attributes;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Agile.Data.Helpers
{
    public class WriteHelper
    {
        public static int Save<T>(params T[] objs)
        {
            var count = 0;
            if (objs != null)
            {
                foreach (var obj in objs)
                {
                    try
                    {
                        count += Save<T>(obj);
                    }
                    catch
                    {
                        continue;
                    }
                }
            }

            return count;
        }

        public static int Save<T>(T obj)
        {
            var ttype = typeof(T);
            var tps = ttype.GetProperties();

            var idattr = default(TableFieldAttribute);
            var idparameter = default(SqlParameter);
            var idproperty = default(PropertyInfo);

            var fields = new List<string>();
            var parameters = new List<SqlParameter>();

            foreach (var tp in tps)
            {
                var attr = tp.GetCustomAttribute(typeof(TableFieldAttribute)) as TableFieldAttribute;
                if (attr == null)
                {
                    continue;
                }

                if (attr.IsPrimaryKey)
                {
                    idattr = attr;
                    idproperty = tp;
                    idparameter = ParameterHelper.CreateSqlParameter<T>(obj, tp, attr.MaxLength, true);
                    parameters.Add(idparameter);
                    continue;
                }

                fields.Add(tp.Name);
                parameters.Add(ParameterHelper.CreateSqlParameter<T>(obj, tp, attr.MaxLength));
            }

            var fieldstr = string.Join(",", fields.Select(o => string.Format("[{0}]", o)));
            var parametestr = string.Join(",", fields.Select(o => string.Format("@{0}", o)));

            var sqlstr = string.Format(" INSERT INTO {0}({1}) VALUES({2});SELECT @{3}=@@IDENTITY;", ttype.Name, fieldstr, parametestr, idproperty.Name);
            var rows = DataHelper.ExecuteNonQuery(sqlstr, parameters.ToArray());
            if (rows > 0)
            {
                try
                {
                    idproperty.SetValue(obj, idparameter.Value);
                }
                catch
                {

                }
            }

            return rows;
        }

        public static int Update<T>(T obj, UpdateOptions option)
        {
            var ttype = typeof(T);
            var tps = ttype.GetProperties();
            var fieldstr = "";
            var parameters = new List<SqlParameter>();

            var whereStr = ExpressionHelper.MakeWhereStr(option.WhereExpList.ToArray());
            if (string.IsNullOrEmpty(whereStr))
            {
                throw new Exception("无法执行不带Where的Update语句");
            }

            var fields = ExpressionHelper.GetFields(option.SelectExp);
            foreach (var f in fields)
            {
                var tp = tps.Where(w => w.Name == f).FirstOrDefault();
                if (tp == null)
                {
                    continue;
                }

                var attr = tp.GetCustomAttribute(typeof(TableFieldAttribute)) as TableFieldAttribute;
                if (attr == null || attr.IsPrimaryKey)
                {
                    continue;
                }

                fieldstr += String.Format("{0}=@{0},", tp.Name);
                parameters.Add(ParameterHelper.CreateSqlParameter<T>(obj, tp, attr.MaxLength));
            }

            var sqlstr = string.Format(" UPDATE {0} SET {1} WHERE {2}", ttype.Name, fieldstr, whereStr);
            var rows = DataHelper.ExecuteNonQuery(sqlstr, parameters.ToArray());
            return rows;
        }

        public static int Update<T>(T obj)
        {
            var ttype = typeof(T);
            var tps = ttype.GetProperties();
            var idproperty = default(PropertyInfo);

            var fields = new List<string>();
            var parameters = new List<SqlParameter>();

            foreach (var tp in tps)
            {
                var attr = tp.GetCustomAttribute(typeof(TableFieldAttribute)) as TableFieldAttribute;
                if (attr == null)
                {
                    continue;
                }

                if (attr.IsPrimaryKey)
                {
                    idproperty = tp;
                    parameters.Add(ParameterHelper.CreateSqlParameter<T>(obj, tp, attr.MaxLength));
                    continue;
                }

                fields.Add(tp.Name);
                parameters.Add(ParameterHelper.CreateSqlParameter<T>(obj, tp, attr.MaxLength));
            }

            if (idproperty == null)
            {
                throw new Exception(string.Format("请先给{0}设置主键", ttype.Name));
            }

            var fieldstr = string.Join(",", fields.Select(o => string.Format("[{0}]=@{0}", o)));
            var sqlstr = string.Format(" UPDATE {0} SET {1} WHERE {2}=@{2}", ttype.Name, fieldstr, idproperty.Name);
            var rows = DataHelper.ExecuteNonQuery(sqlstr, parameters.ToArray());
            return rows;
        }

        public static int Delete<T>(T obj)
        {
            var ttype = typeof(T);
            var idproperty = default(PropertyInfo);
            var parameters = new List<SqlParameter>();
            var tps = ttype.GetProperties();
            foreach (var tp in tps)
            {
                var attr = tp.GetCustomAttribute(typeof(TableFieldAttribute)) as TableFieldAttribute;
                if (attr == null)
                {
                    continue;
                }

                if (attr.IsPrimaryKey)
                {
                    idproperty = tp;
                    parameters.Add(ParameterHelper.CreateSqlParameter<T>(obj, tp, attr.MaxLength));
                    break;
                }
            }

            if (idproperty == null)
            {
                throw new Exception(string.Format("请先给{0}设置主键", ttype.Name));
            }

            var tkey = idproperty.Name;
            var tvalue = idproperty.GetValue(obj, null);

            var sqlstr = string.Format(" DELETE FROM {0} WHERE {1}=@{1}", ttype.Name, tkey);
            var rows = DataHelper.ExecuteNonQuery(sqlstr, parameters.ToArray());
            return rows;
        }

        public static int Delete<T>(Expression<Func<T, bool>> exp)
        {
            var ttype = typeof(T);
            var whereStr = ExpressionHelper.MakeWhereStr(exp);
            if (string.IsNullOrEmpty(whereStr))
            {
                throw new Exception("删除操作必须传入条件");
            }

            var sqlstr = string.Format(" DELETE FROM {0} WHERE {1}", ttype.Name);
            var rows = DataHelper.ExecuteNonQuery(sqlstr);
            return rows;
        }
    }
}

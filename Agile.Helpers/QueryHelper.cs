using Agile.Attributes;
using Agile.Dtos;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Agile.Helpers
{
    public class QueryHelper
    {
        public static void Save<T>(T obj)
        {
            var ttype = typeof(T);
            var tps = ttype.GetProperties();

            var fieldstr = "";
            var parameterstr = "";
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
                    continue;
                }

                fieldstr += String.Format("{0},", tp.Name);
                parameterstr += String.Format("@{0},", tp.Name);
                parameters.Add(CreateSqlParameter<T>(obj, tp, attr));
            }

            fieldstr = fieldstr.TrimEnd(new char[] { ',' });
            parameterstr = parameterstr.TrimEnd(new char[] { ',' });

            var sb = new StringBuilder();
            sb.AppendFormat(" INSERT INTO {0}({1}) VALUES({2});SELECT @@IDENTITY;", ttype.Name, fieldstr, parameterstr);

            var sqlstr = sb.ToString();

            try
            {
                DataHelper.ExecuteScalar(sqlstr, parameters.ToArray());
            }
            catch (Exception ex)
            {
                LogHelper.Write(ex.ToString());
            }
        }

        public static void Update<T>(T obj, UpdateOptions options)
        {
            var ttype = typeof(T);
            var tps = ttype.GetProperties();
            var fieldstr = "";
            var parameters = new List<SqlParameter>();
            var sb = new StringBuilder();
            sb.AppendFormat(" UPDATE {0} ", ttype.Name);

            if (options.SelectExp != null)
            {
                var fields = ExpressionHelper.GetMemberNames(options.SelectExp);
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
                    parameters.Add(CreateSqlParameter<T>(obj, tp, attr));
                }
            }

            fieldstr = fieldstr.TrimEnd(new char[] { ',' });
            sb.AppendFormat(" SET {0}", fieldstr);

            if (options.WhereExpList != null)
            {
                var whereStr = ExpressionHelper.MakeWhereStr(options.WhereExpList.ToArray());
                if (!String.IsNullOrEmpty(whereStr))
                {
                    sb.AppendFormat(" WHERE {0}", whereStr);
                }
            }

            var sqlstr = sb.ToString();
            if (!sqlstr.Contains("WHERE"))
            {
                return;
            }

            try
            {
                DataHelper.ExecuteNonQuery(sqlstr, parameters.ToArray());
            }
            catch (Exception ex)
            {
                LogHelper.Write(ex.ToString());
            }
        }

        public static void Update<T>(T obj)
        {
            var ttype = typeof(T);
            var tps = ttype.GetProperties();
            var idproperty = default(PropertyInfo);
            var fieldstr = "";
            var parameters = new List<SqlParameter>();
            var sb = new StringBuilder();
            sb.AppendFormat(" UPDATE {0} ", ttype.Name);

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
                    continue;
                }

                fieldstr += String.Format("{0}=@{0},", tp.Name);
                parameters.Add(CreateSqlParameter<T>(obj, tp, attr));
            }

            fieldstr = fieldstr.TrimEnd(new char[] { ',' });
            sb.AppendFormat(" SET {0}", fieldstr);

            var tkey = "";
            var tvalue = default(object);
            if (idproperty != null)
            {
                tkey = idproperty.Name;
                tvalue = idproperty.GetValue(obj, null);
            }

            if (!String.IsNullOrEmpty(tkey) && tvalue != null)
            {
                sb.AppendFormat(" WHERE {1}='{2}'", ttype.Name, tkey, tvalue.ToString());
            }

            var sqlstr = sb.ToString();
            if (!sqlstr.Contains("WHERE"))
            {
                return;
            }

            try
            {
                DataHelper.ExecuteNonQuery(sqlstr, parameters.ToArray());
            }
            catch (Exception ex)
            {
                LogHelper.Write(ex.ToString());
            }
        }

        public static int Delete<T>(T obj)
        {
            var ttype = typeof(T);
            var idproperty = default(PropertyInfo);
            var sb = new StringBuilder();
            sb.AppendFormat(" DELETE FROM {0} ", ttype.Name);

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
                    break;
                }
            }

            var tkey = "";
            var tvalue = default(object);
            if (idproperty != null)
            {
                tkey = idproperty.Name;
                tvalue = idproperty.GetValue(obj, null);
            }

            if (!String.IsNullOrEmpty(tkey) && tvalue != null)
            {
                sb.AppendFormat(" WHERE {1}='{2}'", ttype.Name, tkey, tvalue.ToString());
            }

            var sqlstr = sb.ToString();
            if (!sqlstr.Contains("WHERE"))
            {
                return 0;
            }

            try
            {
                return DataHelper.ExecuteNonQuery(sqlstr);
            }
            catch (Exception ex)
            {
                LogHelper.Write(ex.ToString());
                return 0;
            }
        }

        public static int Delete<T>(DeleteOptions options)
        {
            var ttype = typeof(T);
            var sb = new StringBuilder();
            sb.AppendFormat(" DELETE FROM {0} ", ttype.Name);

            if (options.WhereExpList != null)
            {
                var whereStr = ExpressionHelper.MakeWhereStr(options.WhereExpList.ToArray());
                if (!String.IsNullOrEmpty(whereStr))
                {
                    sb.AppendFormat(" WHERE {0}", whereStr);
                }
            }

            var sqlstr = sb.ToString();
            if (!sqlstr.Contains("WHERE"))
            {
                return 0;
            }

            try
            {
                return DataHelper.ExecuteNonQuery(sqlstr);
            }
            catch (Exception ex)
            {
                LogHelper.Write(ex.ToString());
                return 0;
            }
        }

        public static List<T> GetList<T>(QueryOptions options)
        {
            var ttype = typeof(T);
            var sb = new StringBuilder();

            sb.AppendFormat(" SELECT");
            if (options.TopNum.HasValue)
            {
                sb.AppendFormat(" TOP({0})", options.TopNum.Value);
            }

            if (options.SelectExp != null)
            {
                var fields = ExpressionHelper.GetMemberNames(options.SelectExp);
                sb.AppendFormat(" {0}", String.Join(",", fields));
            }
            else
            {
                sb.AppendFormat(" *");
            }

            sb.AppendFormat(" FROM {0}", ttype.Name);
            if (options.WhereExpList != null)
            {
                var whereStr = ExpressionHelper.MakeWhereStr(options.WhereExpList.ToArray());
                if (!String.IsNullOrEmpty(whereStr))
                {
                    sb.AppendFormat(" WHERE {0}", whereStr);
                }
            }

            var orderByStr = GetOrderByStr<T>(options);
            if (!String.IsNullOrEmpty(orderByStr))
            {
                sb.AppendFormat(orderByStr);
            }

            var sqlstr = sb.ToString();

            try
            {
                return DataHelper.ExecuteList<T>(sqlstr);
            }
            catch (Exception ex)
            {
                LogHelper.Write(ex.ToString());
                return new List<T>();
            }
        }

        public static PagedListDto<T> GetPagedList<T>(PagedQueryOptions options)
        {
            var ttype = typeof(T);
            var sb = String.Format(" SELECT ROW_NUMBER() OVER(@OVER) AS RW , @FIELDS FROM {0}", ttype.Name);

            var orderByStr = GetOrderByStr<T>(options);
            if (String.IsNullOrEmpty(orderByStr))
            {
                var tkey = "";
                var tps = ttype.GetProperties();
                foreach (var tp in tps)
                {
                    var attr = tp.GetCustomAttribute(typeof(TableFieldAttribute)) as TableFieldAttribute;
                    if (attr != null && attr.IsPrimaryKey)
                    {
                        tkey = tp.Name;
                        break;
                    }
                }

                if (String.IsNullOrEmpty(tkey))
                {
                    throw new Exception("未给" + ttype.Name + "设置主键");
                }
                else
                {
                    orderByStr = String.Format(" ORDER BY {0} DESC", tkey);
                }
            }

            sb = sb.Replace("@OVER", orderByStr);

            var fields = "*";
            if (options.SelectExp != null)
            {
                var names = ExpressionHelper.GetMemberNames(options.SelectExp);
                if (names.Length > 0)
                {
                    fields = String.Join(",", names);
                }
            }

            sb = sb.Replace("@FIELDS", fields);
            if (options.WhereExpList != null)
            {
                var whereStr = ExpressionHelper.MakeWhereStr(options.WhereExpList.ToArray());
                if (!String.IsNullOrEmpty(whereStr))
                {
                    sb += String.Format(" WHERE {0}", whereStr);
                }
            }

            var dto = new PagedListDto<T>
            {
                Page = options.Page,
                PageSize = options.PageSize
            };

            var sqlstr = String.Format(" SELECT COUNT(1) FROM ({0}) AS Q", sb);

            try
            {
                var recordcount = Convert.ToInt32(DataHelper.ExecuteScalar(sqlstr));
                dto.RecordCount = recordcount;
            }
            catch (Exception ex)
            {
                LogHelper.Write(ex.ToString());
            }

            var begin = dto.PageSize * (dto.Page - 1);
            var end = dto.PageSize * dto.Page;

            sqlstr = String.Format(" SELECT * FROM ({0}) AS Q WHERE Q.RW>{1} AND Q.RW<={2}", sb, begin, end);

            try
            {
                var recordlist = DataHelper.ExecuteList<T>(sqlstr);
                dto.RecordList = recordlist;
            }
            catch (Exception ex)
            {
                LogHelper.Write(ex.ToString());
                dto.RecordList = new List<T>();
            }

            return dto;
        }

        private static string GetOrderByStr<T>(QueryOptions options)
        {
            var sb = new StringBuilder();
            if (options.OrderByAscExpList != null || options.OrderByDescExpList != null)
            {
                sb.AppendFormat(" ORDER BY");

                var orderBys1 = "";
                if (options.OrderByAscExpList != null)
                {
                    var names1 = ExpressionHelper.GetMemberNames(options.OrderByAscExpList.ToArray());
                    if (names1 != null)
                    {
                        orderBys1 = string.Join(",", names1);
                    }
                }

                var orderBys2 = "";
                if (options.OrderByDescExpList != null)
                {
                    var names2 = ExpressionHelper.GetMemberNames(options.OrderByDescExpList.ToArray());
                    if (names2 != null)
                    {
                        foreach (var name in names2)
                        {
                            orderBys2 += String.Format("{0} DESC,", name);
                        }
                    }

                    orderBys2 = orderBys2.TrimEnd(new char[] { ',' });
                }

                var orderBys = "";
                if (!String.IsNullOrEmpty(orderBys1) && !String.IsNullOrEmpty(orderBys2))
                {
                    orderBys = orderBys1 + "," + orderBys2;
                }
                else
                {
                    if (!String.IsNullOrEmpty(orderBys1))
                    {
                        orderBys = orderBys1;
                    }

                    if (!String.IsNullOrEmpty(orderBys2))
                    {
                        orderBys = orderBys2;
                    }
                }

                sb.AppendFormat(" {0}", orderBys);
            }

            return sb.ToString();
        }

        private static SqlParameter CreateSqlParameter<T>(T obj, PropertyInfo p, TableFieldAttribute attr)
        {
            var maxLength = 50;
            var dbValue = p.GetValue(obj, null);
            var dbType = DbTypeConverter(p.PropertyType, dbValue, attr, ref maxLength);
            if (dbValue == null)
            {
                dbValue = DBNull.Value;
            }

            return new SqlParameter
            {
                ParameterName = "@" + p.Name,
                DbType = dbType,
                Size = maxLength,
                Value = dbValue,
                IsNullable = true
            };
        }

        private static DbType DbTypeConverter(Type ttype, object dbvalue, TableFieldAttribute attr, ref int maxLength)
        {
            if (ttype == typeof(string))
            {
                maxLength = attr.MaxLength;
                return DbType.String;
            }

            if (ttype == typeof(int))
            {
                return DbType.Int32;
            }

            if (ttype == typeof(bool))
            {
                return DbType.Boolean;
            }

            if (ttype == typeof(DateTime))
            {
                return DbType.DateTime;
            }

            if (dbvalue != null)
            {
                dbvalue = dbvalue.ToString();
            }

            return DbType.String;
        }
    }

    public class PagedQueryOptions : QueryOptions
    {
        public int Page { get; set; }

        public int PageSize { get; set; }
    }

    public class QueryOptions : UpdateOptions
    {
        public int? TopNum { get; set; }

        public List<Expression> OrderByAscExpList { get; set; }

        public void OrderByAsc<T>(Expression<Func<T, object>> exp)
        {
            if (OrderByAscExpList == null)
            {
                OrderByAscExpList = new List<Expression>();
            }

            OrderByAscExpList.Add(exp);
        }

        public List<Expression> OrderByDescExpList { get; set; }

        public void OrderByDesc<T>(Expression<Func<T, object>> exp)
        {
            if (OrderByDescExpList == null)
            {
                OrderByDescExpList = new List<Expression>();
            }

            OrderByDescExpList.Add(exp);
        }
    }

    public class UpdateOptions : DeleteOptions
    {
        public Expression SelectExp { get; set; }

        public void Select<T>(Expression<Func<T,object>> exp)
        {
            SelectExp = exp;
        }
    }

    public class DeleteOptions
    {
        public List<Expression> WhereExpList { get; set; }

        public void Where<T>(Expression<Func<T, bool>> exp)
        {
            if (WhereExpList == null)
            {
                WhereExpList = new List<Expression>();
            }

            WhereExpList.Add(exp);
        }
    }
}

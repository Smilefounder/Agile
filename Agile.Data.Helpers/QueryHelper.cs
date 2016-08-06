﻿using Agile.Attributes;
using Agile.Dtos;
using Agile.Helpers;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Agile.Data.Helpers
{
    public class QueryHelper
    {
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
                    idparameter = CreateSqlParameter<T>(obj, tp, attr.MaxLength, true);
                    parameters.Add(idparameter);
                    continue;
                }

                fields.Add(tp.Name);
                parameters.Add(CreateSqlParameter<T>(obj, tp, attr.MaxLength));
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

        public static int Update<T>(T obj, UpdateOptions options)
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
                    parameters.Add(CreateSqlParameter<T>(obj, tp, attr.MaxLength));
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
                return -1;
            }

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
                    parameters.Add(CreateSqlParameter<T>(obj, tp, attr.MaxLength));
                    continue;
                }

                fields.Add(tp.Name);
                parameters.Add(CreateSqlParameter<T>(obj, tp, attr.MaxLength));
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
            var idattr = default(TableFieldAttribute);
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
                    idattr = attr;
                    idproperty = tp;
                    break;
                }
            }

            if (idproperty == null || idattr == null)
            {
                throw new Exception(string.Format("请先给{0}设置主键", ttype.Name));
            }

            var tkey = idproperty.Name;
            var tvalue = idproperty.GetValue(obj, null);

            CreateSqlParameter<T>(obj, idproperty, idattr.MaxLength);

            var sqlstr = sb.ToString();
            if (!sqlstr.Contains("WHERE"))
            {
                return -1;
            }

            var rows = DataHelper.ExecuteNonQuery(sqlstr);
            return rows;
        }

        public static int Delete<T>(Expression<Func<T, bool>> exp)
        {
            var ttype = typeof(T);
            var sb = new StringBuilder();
            sb.AppendFormat(" DELETE FROM {0} ", ttype.Name);

            var whereStr = ExpressionHelper.MakeWhereStr(exp);
            if (string.IsNullOrEmpty(whereStr))
            {
                throw new Exception("删除操作必须传入条件");
            }

            sb.AppendFormat(" WHERE {0}", whereStr);

            var sqlstr = sb.ToString();
            var rows = DataHelper.ExecuteNonQuery(sqlstr);
            return rows;
        }

        public static List<T> GetList<T>(QueryOptions options)
        {
            return GetList<T>(new TopQueryOptions
            {
                OrderByAscExpList = options.OrderByAscExpList,
                OrderByDescExpList = options.OrderByDescExpList,
                SelectExp = options.SelectExp,
                TopNum = null,
                WhereExpList = options.WhereExpList
            });
        }

        public static List<T> GetList<T>(TopQueryOptions options)
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

        public static List<T> GetRandomList<T>(RandomQueryOptions options)
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

            sb.AppendFormat(" ORDER BY NEWID();");

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

        private static SqlParameter CreateSqlParameter<T>(T obj, PropertyInfo p, int maxLength, bool isoutput = false)
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
                IsNullable = true
            };
        }

        private static DbType DbTypeConverter(Type ttype)
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

            if (ttype == typeof(DateTime))
            {
                return DbType.DateTime;
            }

            return DbType.String;
        }
    }

    public class PagedQueryOptions : QueryOptions
    {
        public int Page { get; set; }

        public int PageSize { get; set; }
    }

    public class TopQueryOptions : QueryOptions
    {
        public int? TopNum { get; set; }
    }

    public class RandomQueryOptions : UpdateOptions
    {
        public int? TopNum { get; set; }
    }

    public class QueryOptions : UpdateOptions
    {

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

        public void Select<T>(Expression<Func<T, object>> exp)
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

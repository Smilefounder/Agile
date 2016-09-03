using Agile.Attributes;
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
                var fields = ExpressionHelper.GetFields(options.SelectExp);
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
                    sb.AppendFormat(" WHERE 1=1 {0}", whereStr);
                }
            }

            var orderByStr = GetOrderByStr<T>(options);
            if (!String.IsNullOrEmpty(orderByStr))
            {
                sb.AppendFormat(orderByStr);
            }

            var sqlstr = sb.ToString();
            return DataHelper.ExecuteList<T>(sqlstr);
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
                var fields = ExpressionHelper.GetFields(options.SelectExp);
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
                    sb.AppendFormat(" WHERE 1=1 {0}", whereStr);
                }
            }

            sb.AppendFormat(" ORDER BY NEWID();");

            var sqlstr = sb.ToString();
            return DataHelper.ExecuteList<T>(sqlstr);
        }

        public static PagedListDto<T> GetPagedList<T>(int page, int pagesize, string sqlstr)
        {
            var sb1 = string.Format(" SELECT COUNT(1) FROM ({0}) AS Q", sqlstr);
            var recordcount = Convert.ToInt32(DataHelper.ExecuteScalar(sqlstr));

            var begin = pagesize * (page - 1);
            var end = pagesize * page;

            var sb2 = String.Format(" SELECT * FROM ({0}) AS Q WHERE Q.RW>{1} AND Q.RW<={2}", sqlstr, begin, end);
            var recordlist = DataHelper.ExecuteList<T>(sqlstr);

            return new PagedListDto<T>
            {
                Page = page,
                PageSize = pagesize,
                RecordCount = recordcount,
                RecordList = recordlist
            };
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
                var names = ExpressionHelper.GetFields(options.SelectExp);
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
                    sb += String.Format(" WHERE 1=1 {0}", whereStr);
                }
            }

            var dto = new PagedListDto<T>
            {
                Page = options.Page,
                PageSize = options.PageSize
            };

            var sqlstr = String.Format(" SELECT COUNT(1) FROM ({0}) AS Q", sb);
            var recordcount = Convert.ToInt32(DataHelper.ExecuteScalar(sqlstr));
            dto.RecordCount = recordcount;

            var begin = dto.PageSize * (dto.Page - 1);
            var end = dto.PageSize * dto.Page;

            sqlstr = String.Format(" SELECT * FROM ({0}) AS Q WHERE Q.RW>{1} AND Q.RW<={2}", sb, begin, end);
            var recordlist = DataHelper.ExecuteList<T>(sqlstr);
            dto.RecordList = recordlist;

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
                    var names1 = ExpressionHelper.GetFields(options.OrderByAscExpList.ToArray());
                    if (names1 != null)
                    {
                        orderBys1 = string.Join(",", names1);
                    }
                }

                var orderBys2 = "";
                if (options.OrderByDescExpList != null)
                {
                    var names2 = ExpressionHelper.GetFields(options.OrderByDescExpList.ToArray());
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

    public class UpdateOptions
    {
        public Expression SelectExp { get; set; }

        public void Select<T>(Expression<Func<T, object>> exp)
        {
            SelectExp = exp;
        }

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

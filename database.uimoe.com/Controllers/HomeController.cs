using Agile.Helpers;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace database.uimoe.com.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Tables()
        {
            var cmdText = "select name from sysobjects where xtype='u'";
            var table = DataHelper.ExecuteDataTable(cmdText);
            var list = new List<string>();
            foreach (DataRow row in table.Rows)
            {
                var name = "";
                var obj = row["name"];
                if (obj != DBNull.Value)
                {
                    name = Convert.ToString(obj);
                }

                list.Add(name);
            }

            return View(list);
        }

        public ActionResult ItemList(string tablename, int page = 1, int pagesize = 10)
        {
            var sb = String.Format("select row_number() over(order by id desc) as rw,* from {0}", tablename);
            var sqlstr = String.Format("select count(1) from ({0}) as q", sb);
            var obj = DataHelper.ExecuteScalar(sqlstr);
            var recordcount = Convert.ToInt32(obj);

            var begin = (page - 1) * pagesize;
            var end = page * pagesize;

            sqlstr = String.Format("select * from ({0}) as q where q.rw>{1} and q.rw<={2}", sb, begin, end);
            var table = DataHelper.ExecuteDataTable(sqlstr);
            var html = DataTableToHtml(table, tablename, page, pagesize, recordcount, true, true, true);
            return Content(html);
        }

        public string DataTableToHtml(DataTable table, string tablename, int page, int pagesize, int recordcount, bool showInsertButton = false, bool showOperateColumn = false, bool showPager = false)
        {
            if (table == null || table.Rows.Count == 0)
            {
                return "";
            }

            var collist = new List<string>();
            var sb = new StringBuilder();

            if (showInsertButton)
            {
                sb.AppendLine("<div style=\"height:40px;line-height:40px;\">");
                sb.AppendLine("<input class=\"btn btn-success\" type=\"button\" value=\"新增\" onclick=\"tryinsert(this,'" + tablename + "')\"/>");
                sb.AppendLine("</div>");
            }

            sb.AppendLine("<table class=\"table table-bordered table-condensed table-hover\">");
            sb.AppendLine("<tr>");
            foreach (DataColumn col in table.Columns)
            {
                sb.AppendLine("<td>" + col.ColumnName + "</td>");
                collist.Add(col.ColumnName);
            }

            if (showOperateColumn)
            {
                sb.AppendLine("<td>操作</td>");
            }

            sb.AppendLine("</tr>");

            foreach (DataRow row in table.Rows)
            {
                var id = "0";
                sb.AppendLine("<tr>");

                foreach (var col in collist)
                {
                    var val = "";
                    var obj = row[col];
                    if (obj != DBNull.Value)
                    {
                        val = Convert.ToString(obj);
                    }

                    sb.AppendLine("<td>" + val + "</td>");
                    if (col.ToLower() == "id")
                    {
                        id = val;
                    }
                }

                if (showOperateColumn)
                {
                    sb.AppendLine("<td>");
                    sb.AppendLine("<a href=\"javascript:void(0)\" onclick=\"tryupdate(this,'" + tablename + "','" + id + "')\">修改</a>&nbsp;|&nbsp;");
                    sb.AppendLine("<a href=\"javascript:void(0)\" onclick=\"trydelete(this,'" + tablename + "','" + id + "')\">删除</a>");
                    sb.AppendLine("</td>");
                }

                sb.AppendLine("</tr>");
            }

            sb.AppendLine("</table>");

            if (showPager)
            {
                sb.AppendLine("<div style=\"height:40px;line-height:40px;\">");
                sb.AppendLine("<div style=\"float:left;\">");

                var pagecount = Math.Ceiling(1.0 * recordcount / pagesize);
                sb.AppendLine(String.Format("第{0}/{1}页，共{2}条记录", page, pagecount, recordcount));
                sb.AppendLine("</div>");
                sb.AppendLine("<div style=\"float:right;\">");

                if (page > 1)
                {
                    sb.AppendLine("<a href=\"javascript:void(0)\" onclick=\"changepage('" + tablename + "'," + (page - 1) + "," + pagesize + ")\">上一页</a>");
                }

                if (page < pagecount)
                {
                    sb.AppendLine("<a href=\"javascript:void(0)\" onclick=\"changepage('" + tablename + "'," + (page + 1) + "," + pagesize + ")\">下一页</a>");
                }

                sb.AppendLine("</div>");
                sb.AppendLine("<div style=\"clear:both;\">");
                sb.AppendLine("</div>");
                sb.AppendLine("</div>");
            }

            return sb.ToString();
        }

        [HttpPost]
        public ActionResult Delete(string tablename, int id = 0)
        {
            var sb = String.Format("delete from {0} where id={1}", tablename, id);
            var rows = DataHelper.ExecuteNonQuery(sb);
            if (rows < 1)
            {
                return Json(new { error = 1, message = "操作失败，请稍后再试" });
            }
            return Json(new { error = 0 });
        }

        public ActionResult TryInsert(string tablename)
        {
            var collist = GetColumnNameList(tablename, "Id");
            var sb = new StringBuilder();
            sb.AppendLine("<form id=\"insertform\">");
            sb.AppendLine("<input type=\"hidden\" name=\"tablename\" value=\"" + tablename + "\" />");
            foreach (var col in collist)
            {
                sb.AppendLine("<div class=\"form-group\">");
                sb.AppendLine("<label>" + col + "</label>");
                sb.AppendLine("<input class=\"form-control\" name=\"" + col + "\" id=\"" + col + "\" />");
                sb.AppendLine("</div>");
            }
            sb.AppendLine("</form>");
            return Content(sb.ToString());
        }

        public ActionResult TryUpdate(string tablename, int id = 0)
        {
            var sb = new StringBuilder();
            sb.AppendLine("<form id=\"updateform\">");
            sb.AppendLine("<input type=\"hidden\" name=\"tablename\" value=\"" + tablename + "\" />");
            sb.AppendLine("<input type=\"hidden\" name=\"id\" value=\"" + id + "\" />");

            var collist = new List<string>();
            var sqlstr = String.Format("select top 1 * from {0} where id={1}", tablename, id);
            var table = DataHelper.ExecuteDataTable(sqlstr);
            if (table != null)
            {
                foreach (DataColumn col in table.Columns)
                {
                    if (col.ColumnName.ToLower() != "id")
                    {
                        collist.Add(col.ColumnName);
                    }
                }
            }

            foreach (DataRow row in table.Rows)
            {
                foreach (var col in collist)
                {
                    sb.AppendLine("<div class=\"form-group\">");
                    sb.AppendLine("<label>" + col + "</label>");
                    sb.AppendLine("<input class=\"form-control\" name=\"" + col + "\" id=\"" + col + "\" value=\"" + row[col] + "\" />");
                    sb.AppendLine("</div>");
                }
            }

            sb.AppendLine("</form>");
            return Content(sb.ToString());
        }

        [HttpPost]
        public ActionResult Insert()
        {
            var tablename = Request.Form["tablename"];
            if (String.IsNullOrEmpty(tablename))
            {
                return Json(new { error = 1, message = "操作失败，请稍后重试" });
            }

            var collist = GetColumnNameList(tablename, "Id");
            if (collist.Count == 0)
            {
                return Json(new { error = 1, message = "操作失败，请稍后重试" });
            }

            var sb = new StringBuilder();
            sb.AppendLine(String.Format(" INSERT INTO {0}", tablename));
            sb.AppendLine(String.Format(" ({0})", String.Join(",", collist.ToArray())));
            sb.AppendLine(" VALUES");
            sb.AppendLine(" (");
            foreach (var col in collist)
            {
                if (Request.Form.AllKeys.Contains(col))
                {
                    sb.AppendFormat("'{0}',", Request.Form[col]);
                }
                else
                {
                    sb.Append("NULL,");
                }
            }

            var sqlstr = sb.ToString();
            sqlstr = sqlstr.Substring(0, sqlstr.Length - 1);
            sqlstr = sqlstr + ");";

            var rows = DataHelper.ExecuteNonQuery(sqlstr);
            if (rows < 1)
            {
                return Json(new { error = 1, message = "操作失败，请稍后再试" });
            }
            return Json(new { error = 0 });
        }

        [HttpPost]
        public ActionResult Update()
        {
            var tablename = Request.Form["tablename"];
            if (String.IsNullOrEmpty(tablename))
            {
                return Json(new { error = 1, message = "操作失败，请稍后重试" });
            }

            var collist = GetColumnNameList(tablename, "Id");
            if (collist.Count == 0)
            {
                return Json(new { error = 1, message = "操作失败，请稍后重试" });
            }

            var sb = new StringBuilder();
            sb.AppendLine(String.Format(" UPDATE {0}", tablename));
            sb.Append(" SET ");
            foreach (var col in collist)
            {
                if (col.ToLower() == "id")
                {
                    continue;
                }

                if (Request.Form.AllKeys.Contains(col))
                {
                    sb.AppendFormat(" {0}='{1}',", col, Request.Form[col]);
                }
                else
                {
                    sb.AppendFormat(" {0}= NULL,", col);
                }
            }

            var sqlstr = sb.ToString();
            sqlstr = sqlstr.Substring(0, sqlstr.Length - 1);

            var id = 0;
            int.TryParse(Request.Form["id"], out id);
            sqlstr += String.Format(" WHERE id={0}", id);

            var rows = DataHelper.ExecuteNonQuery(sqlstr);
            if (rows < 1)
            {
                return Json(new { error = 1, message = "操作失败，请稍后再试" });
            }
            return Json(new { error = 0 });
        }

        public List<string> GetColumnNameList(string tablename)
        {
            var collist = new List<string>();
            var sqlstr = String.Format("select top 1 * from {0} where 1=0", tablename);
            var table = DataHelper.ExecuteDataTable(sqlstr);
            if (table == null)
            {
                return collist;
            }

            foreach (DataColumn col in table.Columns)
            {
                collist.Add(col.ColumnName);
            }

            return collist;
        }

        public List<string> GetColumnNameList(string tablename, params string[] rejects)
        {
            var collist2 = new List<string>();
            var collist = GetColumnNameList(tablename);
            foreach (var col in collist)
            {
                var count = rejects.Count(w => w.ToLower() == col.ToLower());
                if (count == 0)
                {
                    collist2.Add(col);
                }
            }

            return collist2;
        }

        [HttpGet]
        public ActionResult Execute()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Execute(string sqlstr)
        {
            var responseobj = new { error = 1, message = "操作失败，请稍后再试" };

            try
            {
                if (String.IsNullOrEmpty(sqlstr))
                {
                    return Json(responseobj);
                }

                sqlstr = sqlstr.ToUpper();
                if (sqlstr.Contains("DROP") || sqlstr.Contains("TRUNCATE"))
                {
                    return Json(responseobj);
                }

                if (sqlstr.Contains("DELETE") && sqlstr.Contains("WHERE") == false)
                {
                    return Json(responseobj);
                }

                if (sqlstr.Contains("SELECT") && sqlstr.Contains("TOP") == false && sqlstr.Contains("ROW_NUMBER") == false)
                {
                    return Json(responseobj);
                }

                if (sqlstr.Contains("SELECT") && (sqlstr.Contains("COUNT") || sqlstr.Contains("SUM") || sqlstr.Contains("MAX") || sqlstr.Contains("MIN")))
                {
                    var obj = DataHelper.ExecuteScalar(sqlstr);
                    return Json(new { error = 0, exec = "scalar", data = Convert.ToInt32(obj) });
                }

                if (sqlstr.Contains("INSERT") || sqlstr.Contains("UPDATE") || sqlstr.Contains("DELETE"))
                {
                    var rows = DataHelper.ExecuteNonQuery(sqlstr);
                    return Json(new { error = 0, exec = "nonquery", data = rows });
                }

                var table = DataHelper.ExecuteDataTable(sqlstr);
                if (table == null)
                {
                    return Json(responseobj);
                }

                var str = DataTableToString(table);
                return Json(new { error = 0, exec = "table", data = str });
            }
            catch (Exception ex)
            {
                LogHelper.Write(ex.Message);
                return Json(responseobj);
            }
        }

        private string DataTableToString(DataTable table)
        {
            if (table == null || table.Rows.Count == 0)
            {
                return "没有匹配的记录";
            }

            var collist = new List<string>();
            var sb = new StringBuilder();
            sb.AppendLine("<table class=\"table table-bordered table-condensed table-hover\">");
            sb.AppendLine("<tr>");
            foreach (DataColumn col in table.Columns)
            {
                sb.AppendLine("<td>" + col.ColumnName + "</td>");
                collist.Add(col.ColumnName);
            }
            sb.AppendLine("</tr>");

            foreach (DataRow row in table.Rows)
            {
                sb.AppendLine("<tr>");
                foreach (var col in collist)
                {
                    var val = "";
                    var obj = row[col];
                    if (obj != DBNull.Value)
                    {
                        val = Convert.ToString(obj);
                    }

                    sb.AppendLine("<td>" + val + "</td>");
                }

                sb.AppendLine("</tr>");
            }
            sb.AppendLine("</table>");
            return sb.ToString();
        }
    }
}

﻿using Agile.Dtos;
using Agile.Dtos.API;
using Agile.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Agile.Helpers.API
{
    public class LogicHelper
    {
        /// <summary>
        /// 获取接口列表
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public static HBaseResponse H10000(H10000Request request)
        {
            if (!request.page.HasValue)
            {
                request.page = 1;
            }

            if (!request.pagesize.HasValue)
            {
                request.pagesize = 10;
            }

            var sqlstr = "SELECT COUNT(1) FROM T_interface";
            var obj = DataHelper.ExecuteScalar(sqlstr);
            var recordcount = Convert.ToDouble(obj);

            var begin = new SqlParameter("@begin", SqlDbType.Int, 11);
            begin.Value = request.pagesize * (request.page - 1);

            var end = new SqlParameter("@end", SqlDbType.Int, 11);
            end.Value = request.pagesize * request.page;

            var splist = new List<SqlParameter>();
            splist.Add(begin);
            splist.Add(end);

            sqlstr = "SELECT * FROM (SELECT *,ROW_NUMBER() OVER(ORDER BY ID DESC) AS RW FROM T_interface) AS Q WHERE Q.RW>@begin AND Q.RW<=@end";
            var recordlist = DataHelper.ExecuteList<H10000ResponseListItem>(sqlstr, splist.ToArray());
            return new H10000Response
            {
                error = 0,
                data = new PagedListDto<H10000ResponseListItem>
                {
                    Page = request.page.Value,
                    PageSize = request.pagesize.Value,
                    RecordCount = recordcount > Int64.MaxValue ? Int64.MaxValue : Convert.ToInt64(recordcount),
                    RecordList = recordlist
                }
            };
        }

        /// <summary>
        /// 删除接口
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public static HBaseResponse H10001(H10001Request request)
        {
            if (String.IsNullOrEmpty(request.code))
            {
                return new H10001Response
                {
                    error = 1,
                    message = "接口编码不能为空"
                };
            }

            var sqlstr = "DELETE FROM T_interface WHERE Code=@Code";
            var code = new SqlParameter("@Code", SqlDbType.NVarChar, 50);
            code.Value = request.code;
            var rows = DataHelper.ExecuteNonQuery(sqlstr, code);
            if (rows > 0)
            {
                return new H10001Response
                {
                    error = 0,
                };
            }

            return new HBaseResponse();
        }

        /// <summary>
        /// 新增接口
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public static HBaseResponse H10002(H10002Request request)
        {
            if (String.IsNullOrEmpty(request.code))
            {
                return new H10002Response
                {
                    error = 1,
                    message = "接口编码不能为空"
                };
            }

            if (String.IsNullOrEmpty(request.name))
            {
                return new H10002Response
                {
                    error = 1,
                    message = "接口名称不能为空"
                };
            }

            if (String.IsNullOrEmpty(request.description) == false && request.description.Length > 200)
            {
                return new H10002Response
                {
                    error = 1,
                    message = "接口描述最多输入200字"
                };
            }

            var code = new SqlParameter("@Code", SqlDbType.NVarChar, 50);
            code.Value = request.code;

            var name = new SqlParameter("@Name", SqlDbType.NVarChar, 50);
            name.Value = request.name;

            var description = new SqlParameter("@Description", SqlDbType.NVarChar, 200);
            description.Value = request.description;

            var splist = new List<SqlParameter>();
            splist.Add(code);
            splist.Add(name);
            splist.Add(description);

            var sqlstr = "INSERT INTO T_interface(Code,Name,Description,CreatedAt) VALUES(@Code,@Name,@Description,GETDATE());SELECT @@IDENTITY;";
            var obj = DataHelper.ExecuteScalar(sqlstr, splist.ToArray());
            var id = Convert.ToDouble(obj);
            if (id > 0)
            {
                return new H10002Response
                {
                    error = 0,
                    id = id > Int64.MaxValue ? Int64.MaxValue : Convert.ToInt64(id)
                };
            }

            return new HBaseResponse();
        }

        /// <summary>
        /// 修改接口
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public static HBaseResponse H10003(H10003Request request)
        {
            if (String.IsNullOrEmpty(request.code))
            {
                return new H10003Response
                {
                    error = 1,
                    message = "接口编码不能为空"
                };
            }

            if (String.IsNullOrEmpty(request.name))
            {
                return new H10003Response
                {
                    error = 1,
                    message = "接口名称不能为空"
                };
            }

            if (String.IsNullOrEmpty(request.description) == false && request.description.Length > 200)
            {
                return new H10003Response
                {
                    error = 1,
                    message = "接口描述最多输入200字"
                };
            }

            var id = new SqlParameter("@Id", SqlDbType.Int, 11);
            id.Value = request.id;

            var code = new SqlParameter("@Code", SqlDbType.NVarChar, 50);
            code.Value = request.code;

            var name = new SqlParameter("@Name", SqlDbType.NVarChar, 50);
            name.Value = request.name;

            var description = new SqlParameter("@Description", SqlDbType.NVarChar, 200);
            description.Value = request.description;

            var splist = new List<SqlParameter>();
            splist.Add(id);
            splist.Add(code);
            splist.Add(name);
            splist.Add(description);

            var sqlstr = "UPDATE T_interface SET Code=@Code,Name=@Name,Description=@Description WHERE Id=@Id;";
            var rows = DataHelper.ExecuteNonQuery(sqlstr, splist.ToArray());
            if (rows > 0)
            {
                return new H10003Response
                {
                    error = 0
                };
            }

            return new HBaseResponse();
        }

        /// <summary>
        /// 查询接口
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public static HBaseResponse H10004(H10004Request request)
        {
            if (String.IsNullOrEmpty(request.code) && request.id.HasValue == false)
            {
                return new H10004Response
                {
                    error = 1,
                    message = "接口编码和id必须传一个"
                };
            }

            var splist = new List<SqlParameter>();
            var sqlstr = "SELECT * FROM T_interface WHERE 1=1 ";
            if (request.id.HasValue)
            {
                var id = new SqlParameter("@id", SqlDbType.Int, 11);
                id.Value = request.id.Value;
                splist.Add(id);
                sqlstr += " AND id=@id ";
            }

            if (!String.IsNullOrEmpty(request.code))
            {
                var code = new SqlParameter("@code", SqlDbType.NVarChar, 50);
                code.Value = request.code;
                splist.Add(code);
                sqlstr += " AND code=@code ";
            }

            var list = DataHelper.ExecuteList<H10004ResponseListItem>(sqlstr, splist.ToArray());
            return new H10004Response
            {
                error = 0,
                data = list
            };
        }

        /// <summary>
        /// 获取文章列表（llyn23.com）
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public static HBaseResponse H10005(H10005Request request)
        {
            if (!request.page.HasValue)
            {
                request.page = 1;
            }

            if (!request.pagesize.HasValue)
            {
                request.pagesize = 10;
            }

            var ymdatetime = default(DateTime?);
            if (!String.IsNullOrEmpty(request.yearmonth))
            {
                request.yearmonth += "-1";
            }

            DateTime ymdatetimetemp;
            if (DateTime.TryParse(request.yearmonth, out ymdatetimetemp))
            {
                ymdatetime = ymdatetimetemp;
            }

            var sqlstr = "SELECT COUNT(1) FROM LYN_archive WHERE 1=1 ";
            if (ymdatetime.HasValue)
            {
                sqlstr += " AND DATEDIFF(MONTH,CreatedAt,'@YearMonth')=0".Replace("@YearMonth", ymdatetime.Value.ToString("yyyy-MM-dd"));
            }

            var obj = DataHelper.ExecuteScalar(sqlstr);
            var recordcount = Convert.ToDouble(obj);

            var begin = new SqlParameter("@begin", SqlDbType.Int, 11);
            begin.Value = request.pagesize * (request.page - 1);

            var end = new SqlParameter("@end", SqlDbType.Int, 11);
            end.Value = request.pagesize * request.page;

            var splist = new List<SqlParameter>();
            splist.Add(begin);
            splist.Add(end);

            sqlstr = "SELECT *,ROW_NUMBER() OVER(ORDER BY ID DESC) AS RW FROM LYN_archive  WHERE 1=1 ";
            if (ymdatetime.HasValue)
            {
                sqlstr += " AND DATEDIFF(MONTH,CreatedAt,'@YearMonth')=0".Replace("@YearMonth", ymdatetime.Value.ToString("yyyy-MM-dd"));
            }

            sqlstr = "SELECT * FROM (" + sqlstr + ") AS Q WHERE Q.RW>@begin AND Q.RW<=@end";
            var recordlist = DataHelper.ExecuteList<H10005ResponseListItem>(sqlstr, splist.ToArray());
            return new H10005Response
            {
                error = 0,
                data = new PagedListDto<H10005ResponseListItem>
                {
                    Page = request.page.Value,
                    PageSize = request.pagesize.Value,
                    RecordCount = recordcount > Int64.MaxValue ? Int64.MaxValue : Convert.ToInt64(recordcount),
                    RecordList = recordlist ?? new List<H10005ResponseListItem>()
                }
            };
        }

        /// <summary>
        /// 获取最近的文章的标题（llyn23.com）
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public static HBaseResponse H10006(H10006Request request)
        {
            var take = new SqlParameter("@take", SqlDbType.Int, 11);
            take.Value = request.take;

            var sqlstr = "SELECT TOP (@take) Id,Title FROM LYN_archive ORDER BY ID DESC";
            var recordlist = DataHelper.ExecuteList<H10006ResponseListItem>(sqlstr, take);
            return new H10006Response
            {
                error = 0,
                data = recordlist
            };
        }

        /// <summary>
        /// 获取文章归档（llyn23.com）
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public static HBaseResponse H10007(H10007Request request)
        {
            var sqlstr = "SELECT LEFT(CONVERT(nvarchar(20), CreatedAt, 120 ),7) as name,COUNT(1) as cnt FROM LYN_archive GROUP BY LEFT(CONVERT(nvarchar(20), CreatedAt, 120 ),7) ";
            var recordlist = DataHelper.ExecuteList<H10007ResponseListItem>(sqlstr);
            if (request.take.HasValue)
            {
                recordlist = recordlist.Take(request.take.Value).ToList();
            }

            return new H10007Response
            {
                error = 0,
                data = recordlist ?? new List<H10007ResponseListItem>()
            };
        }

        /// <summary>
        /// 获取文章明细（llyn23.com）
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public static HBaseResponse H10008(H10008Request request)
        {
            if (request.id.HasValue == false || request.id.Value == 0)
            {
                return new H10008Response
                {
                    error = 1,
                    message = "id必须填写"
                };
            }

            var id = new SqlParameter("@id", SqlDbType.Int, 11);
            id.Value = request.id.GetValueOrDefault(0);

            var sqlstr = "SELECT TOP 1 * FROM LYN_archive WHERE id=@id ORDER BY ID DESC";
            var recordlist = DataHelper.ExecuteList<H10008ResponseListItem>(sqlstr, id);

            return new H10008Response
            {
                error = 0,
                data = recordlist == null ? new H10008ResponseListItem() : recordlist.FirstOrDefault()
            };
        }

        /// <summary>
        /// 登录
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public static HBaseResponse H10009(H10009Request request)
        {
            if (String.IsNullOrEmpty(request.username) || String.IsNullOrEmpty(request.userpass))
            {
                return new H10009Response
                {
                    error = 1,
                    message = "用户名和密码不能为空"
                };
            }

            var username = new SqlParameter("@username", SqlDbType.NVarChar, 50);
            username.Value = request.username;

            var sqlstr = "SELECT TOP 1 id,username,userpass,[status] FROM LYN_user WHERE UserName=@username ORDER BY ID DESC";
            var userlist = DataHelper.ExecuteList<H10009Request>(sqlstr, username);
            if (userlist == null || userlist.Count == 0)
            {
                return new H10009Response
                {
                    error = 1,
                    message = "用户名或密码错误"
                };
            }

            var user = userlist.FirstOrDefault();
            if (user == null)
            {
                return new H10009Response
                {
                    error = 1,
                    message = "用户名或密码错误"
                };
            }

            if (user.status.GetValueOrDefault(0) == 1)
            {
                return new H10009Response
                {
                    error = 1,
                    message = "用户需要审核通过后才可以登录"
                };
            }

            var userpass3 = SecurityHelper.GetMD5String(request.userpass);
            if (user.userpass != userpass3)
            {
                return new H10009Response
                {
                    error = 1,
                    message = "用户名或密码错误"
                };
            }

            var userid = user.id.GetValueOrDefault(0);
            var token = Guid.NewGuid().ToString();
            sqlstr = String.Format("INSERT INTO T_token(Token,UserId,CreatedAt) VALUES('{0}',{1},GETDATE())", token, userid);
            var rows = DataHelper.ExecuteNonQuery(sqlstr);
            if (rows > 0)
            {
                return new H10009Response
                {
                    token = token,
                    userid = userid,
                    error = 0
                };
            }

            return new H10009Response
            {
                error = 1
            };
        }

        /// <summary>
        /// 注册
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public static HBaseResponse H10010(H10010Request request)
        {
            if (String.IsNullOrEmpty(request.email) || String.IsNullOrEmpty(request.username) || String.IsNullOrEmpty(request.userpass))
            {
                return new H10010Response
                {
                    error = 1,
                    message = "邮箱，用户名和密码不能为空"
                };
            }

            var sqlstr = "SELECT TOP 1 username,userpass FROM LYN_user WHERE UserName='" + request.username + "' ORDER BY ID DESC";
            var userlist = DataHelper.ExecuteList<H10009Request>(sqlstr);
            if (userlist != null && userlist.Any())
            {
                return new H10010Response
                {
                    error = 1,
                    message = "用户名已存在"
                };
            }

            var username = new SqlParameter("@username", SqlDbType.NVarChar, 50);
            username.Value = request.username;

            var userpass = new SqlParameter("@userpass", SqlDbType.NVarChar, 50);
            userpass.Value = SecurityHelper.GetMD5String(request.userpass);

            var email = new SqlParameter("@email", SqlDbType.NVarChar, 50);
            email.Value = request.email;

            var splist = new List<SqlParameter>();
            splist.Add(username);
            splist.Add(userpass);
            splist.Add(email);

            sqlstr = "INSERT INTO LYN_user(username,userpass,email,createdat,[status]) VALUES(@username,@userpass,@email,GETDATE(),1);";
            var rows = DataHelper.ExecuteNonQuery(sqlstr, splist.ToArray());
            if (rows > 0)
            {
                return new H10010Response
                {
                    error = 0,
                    message = "注册成功，审核通过后将发送邮件通知给您，之后才可以登录"
                };
            }


            return new H10010Response
            {
                error = 1
            };
        }

        /// <summary>
        /// 发表评论
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public static HBaseResponse H10011(H10011Request request)
        {
            var useridstr = "NULL";
            if (request.isanonymous.GetValueOrDefault(0) == 0)
            {
                var sqlstr = "SELECT TOP 1 id FROM LYN_user WHERE UserName='" + request.username + "' ORDER BY ID DESC";
                var userlist = DataHelper.ExecuteList<H10009Request>(sqlstr);
                if (userlist == null || userlist.Count == 0)
                {
                    return new H10011Response
                    {
                        error = 1,
                        message = "未登录"
                    };
                }


                var user = userlist.FirstOrDefault();
                if (user == null)
                {
                    return new H10011Response
                    {
                        error = 1,
                        message = "未登录"
                    };
                }


                useridstr = "'" + user.id.GetValueOrDefault(0).ToString() + "'";
            }

            var archiveid = new SqlParameter("@archiveid", SqlDbType.Int, 50);
            archiveid.IsNullable = true;
            archiveid.Value = request.archiveid;

            var content = new SqlParameter("@content", SqlDbType.NVarChar, 50);
            content.Value = request.content;

            var ipaddress = new SqlParameter("@ipaddress", SqlDbType.NVarChar, 50);
            ipaddress.Value = request.ipaddress;

            var useragent = new SqlParameter("@useragent", SqlDbType.NVarChar, 50);
            useragent.Value = request.useragent;

            var splist = new List<SqlParameter>();
            splist.Add(archiveid);
            splist.Add(content);
            splist.Add(ipaddress);
            splist.Add(useragent);

            var sqlstr2 = "INSERT INTO LYN_comment(archiveid,userid,content,createdat,status,ipaddress,useragent) VALUES(@archiveid," + useridstr + ",@content,GETDATE(),1,@ipaddress,@useragent);";
            var rows = DataHelper.ExecuteNonQuery(sqlstr2, splist.ToArray());
            if (rows > 0)
            {
                return new H10011Response
                {
                    error = 0,
                    message = "评论成功，审核通过后将显示在下方的列表"
                };
            }

            return new H10011Response
            {
                error = 1,
            };
        }

        /// <summary>
        /// 获取评论列表
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public static HBaseResponse H10012(H10012Request request)
        {
            var archiveid = new SqlParameter("@archiveid", SqlDbType.Int, 11);
            archiveid.Value = request.archiveid.GetValueOrDefault(0);

            var sqlstr = "SELECT t1.*,t2.username as username FROM LYN_comment t1 LEFT JOIN LYN_user t2 on t2.Id= t1.UserId WHERE t1.ArchiveId=@archiveid AND (t1.Status IS NULL OR t1.Status=0)";
            var recordlist = DataHelper.ExecuteList<H10012ResponseListItem>(sqlstr, archiveid);
            return new H10012Response
            {
                error = 0,
                data = recordlist
            };
        }

        /// <summary>
        /// 获取应用列表（uimoe.com）
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public static HBaseResponse H10013(H10013Request request)
        {
            var take = new SqlParameter("@take", SqlDbType.Int, 11);
            take.Value = request.take.GetValueOrDefault(1);

            var sqlstr = "SELECT TOP (@take) * FROM UME_app ORDER BY NEWID()";
            var recordlist = DataHelper.ExecuteList<H10013ResponseListItem>(sqlstr, take);
            return new H10013Response
            {
                error = 0,
                data = recordlist ?? new List<H10013ResponseListItem>()
            };
        }

        /// <summary>
        /// 查询粤语
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public static HBaseResponse H10014(H10014Request request)
        {
            var h10015responsebase = H10015(new H10015Request
            {
                input = request.input
            });

            var h10015response = h10015responsebase as H10015Response;
            if (h10015response == null)
            {
                return new HBaseResponse
                {
                    error = 1
                };
            }

            if (h10015response.data != null && h10015response.data.Any())
            {
                return new H10014Response
                {
                    error = 0,
                    isallmatched = true,
                    data = h10015response.data.Select(o => new H10014ResponseListItem
                    {
                        canpronounce = o.canpronounce,
                        cantext = o.cantext,
                        chntext = o.chntext
                    }).ToList()
                };
            }

            var h10016responsebase = H10016(new H10016Request
            {
                input = request.input
            });

            var h10016response = h10016responsebase as H10016Response;
            if (h10016response == null)
            {
                return new HBaseResponse
                {
                    error = 1
                };
            }

            return new H10014Response
            {
                error = 0,
                isallmatched = false,
                data = h10016response.data.Select(o => new H10014ResponseListItem
                {
                    canpronounce = o.canpronounce,
                    cantext = o.cantext,
                    chntext = o.chntext
                }).ToList()
            };
        }

        /// <summary>
        /// 查询粤语(全匹配)
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public static HBaseResponse H10015(H10015Request request)
        {
            var input = new SqlParameter("@input", SqlDbType.NVarChar, 50);
            input.Value = request.input;

            var sqlstr = "SELECT TOP 10 * FROM CAN_vocabulary WHERE ChnText=@input";
            var recordlist = DataHelper.ExecuteList<H10015ResponseListItem>(sqlstr, input);
            return new H10015Response
            {
                error = 0,
                data = recordlist ?? new List<H10015ResponseListItem>()
            };
        }

        /// <summary>
        /// 查询粤语(单个匹配)
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public static HBaseResponse H10016(H10016Request request)
        {
            if (String.IsNullOrEmpty(request.input))
            {
                return new HBaseResponse
                {
                    error = 1
                };
            }

            var input_arr = request.input.ToList().Distinct();
            request.input = String.Join("", input_arr);

            var sqlstr = "";
            foreach (var ch in request.input)
            {
                sqlstr += " SELECT  TOP 1 * FROM CAN_vocabulary WHERE ChnText='" + ch + "' UNION";
            }

            if (sqlstr.Length > 5)
            {
                sqlstr = sqlstr.Substring(0, sqlstr.Length - 5);
            }

            var recordlist = DataHelper.ExecuteList<H10016ResponseListItem>(sqlstr);
            return new H10016Response
            {
                error = 0,
                data = recordlist ?? new List<H10016ResponseListItem>()
            };
        }

        /// <summary>
        /// 粤语词典-反馈
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public static HBaseResponse H10017(H10017Request request)
        {
            var sqlstr = "INSERT INTO CAN_feedback(ChnText,CanText,CreatedAt,CreatedBy) VALUES(N'" + request.chntext + "',N'" + request.cantext + "',GETDATE(),N'" + request.createdby + "')";
            if (request.createdby == "reimu")
            {
                sqlstr = "INSERT INTO CAN_noresult(ChnText,CreatedAt) VALUES('" + request.chntext + "',GETDATE())";
            }

            var rows = DataHelper.ExecuteNonQuery(sqlstr);
            if (rows > 0)
            {
                return new H10017Response
                {
                    error = 0
                };
            }

            return new H10017Response
            {
                error = 1,
            };
        }

        /// <summary>
        /// 获取粤语词汇列表
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public static HBaseResponse H10018(H10018Request request)
        {
            var skip = request.skip.GetValueOrDefault(0);
            var take = request.take.GetValueOrDefault(10);
            var sqlstr = "SELECT ROW_NUMBER() OVER(ORDER BY ID) AS RW,* FROM CAN_vocabulary WHERE 1=1 ";
            var texttype = request.texttype.GetValueOrDefault(1);
            switch (texttype)
            {
                default:
                    {
                        sqlstr += " ";
                        break;
                    }
                case 1:
                    {
                        sqlstr += " AND LEN(ChnText)=1";
                        break;
                    }
                case 2:
                    {
                        sqlstr += " AND LEN(ChnText)=2";
                        break;
                    }
                case 3:
                    {
                        sqlstr += " AND LEN(ChnText)>2";
                        break;
                    }
            }

            sqlstr = String.Format("SELECT * FROM ({0}) AS Q WHERE Q.RW>{1} AND Q.RW<={2}", sqlstr, skip, skip + take);
            var recordlist = DataHelper.ExecuteList<H10018ResponseListItem>(sqlstr);
            return new H10018Response
            {
                error = 0,
                skip = skip,
                take = take,
                data = recordlist ?? new List<H10018ResponseListItem>()
            };
        }

        /// <summary>
        /// 粤语词典-获取问卷列表
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public static HBaseResponse H10019(H10019Request request)
        {
            var take = request.take.GetValueOrDefault(20);
            var sqlstr = "SELECT TOP (" + take + ") Id,Title FROM CAN_testitem ORDER BY NEWID()";
            var items = DataHelper.ExecuteList<H10019ResponseListItem>(sqlstr);
            if (items == null || items.Count == 0)
            {
                return new HBaseResponse
                {
                    error = 1
                };
            }

            var ids = String.Join(",", items.Select(o => o.id));
            var sqlstr2 = "SELECT testitemid,displaytext,innervalue FROM CAN_testoption WHERE testitemid in (" + ids + ") ORDER BY NEWID();";
            var options = DataHelper.ExecuteList<H10019ResponseListItemOption>(sqlstr2);

            return new H10019Response
            {
                error = 0,
                items = items,
                options = options
            };
        }

        /// <summary>
        /// 粤语词典 - 绑定微信用户
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public static HBaseResponse H10020(H10020Request request)
        {
            var h10021responsebase = H10021(new H10021Request
            {
                username = request.username
            });

            if (h10021responsebase.error > 0)
            {
                return new H10020Response
                {
                    error = 1,
                    message = "用户已绑定过"
                };
            }

            var sqlstr = "INSERT INTO CAN_user(UserName,CreatedAt) VALUES(N'" + request.username + "',GETDATE())";
            var rows = DataHelper.ExecuteNonQuery(sqlstr);
            if (rows > 0)
            {
                return new H10020Response
                {
                    error = 0
                };
            }

            return new HBaseResponse
            {
                error = 1
            };
        }

        /// <summary>
        /// 粤语词典 - 是否存在微信用户
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public static HBaseResponse H10021(H10021Request request)
        {
            var sqlstr = "SELECT COUNT(1) FROM CAN_user WHERE UserName=N'" + request.username + "'";
            var obj = DataHelper.ExecuteScalar(sqlstr);
            var count = Convert.ToInt32(obj);
            return new H10021Response
            {
                error = count
            };
        }

        /// <summary>
        /// 粤语词典 - 获取我的反馈
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public static HBaseResponse H10022(H10022Request request)
        {
            if (String.IsNullOrEmpty(request.username))
            {
                return new H10022Response
                {
                    error = 0,
                    data = new List<H10022ResponseListItem>()
                };
            }

            var sqlstr = "SELECT * FROM CAN_feedback WHERE CreatedBy='" + request.username + "'";
            var recordlist = DataHelper.ExecuteList<H10022ResponseListItem>(sqlstr);
            return new H10022Response
            {
                error = 0,
                data = recordlist ?? new List<H10022ResponseListItem>()
            };
        }

        /// <summary>
        /// 我的记账本 - 添加消费记录
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public static HBaseResponse H10023(H10023Request request)
        {
            if (request.money.HasValue == false)
            {
                return new HBaseResponse
                {
                    error = 1,
                    message = "请输入数字金额"
                };
            }

            if (String.IsNullOrEmpty(request.username))
            {
                return new HBaseResponse
                {
                    error = 1,
                    message = "请从微信公众号【UME-Money】发送消息"
                };
            }

            var username = new SqlParameter("@UserName", SqlDbType.NVarChar, 50);
            username.Value = request.username;

            var usercost = new SqlParameter("@UserCost", SqlDbType.Decimal, 18);
            usercost.Value = request.money;

            var splist = new List<SqlParameter>();
            splist.Add(username);
            splist.Add(usercost);

            var sqlstr = "INSERT INTO MNY_daily(UserName,UserCost,CreatedAt) VALUES(@UserName,@UserCost,GETDATE());";
            var rows = DataHelper.ExecuteNonQuery(sqlstr, splist.ToArray());
            if (rows > 0)
            {
                return new H10023Response
                {
                    error = 0
                };
            }

            return new HBaseResponse
            {
                error = 1
            };
        }

        /// <summary>
        /// 我的记账本 - 消费统计
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public static HBaseResponse H10024(H10024Request request)
        {
            if (String.IsNullOrEmpty(request.username))
            {
                return new H10024Response
                {
                    error = 1,
                    message = "username不能为空"
                };
            }

            var ttype = request.ttype.GetValueOrDefault(0);
            if (ttype > 3 || ttype < 0)
            {
                return new H10024Response
                {
                    error = 1,
                    message = "ttype只能是以下几个数字：0[今天],1[本月],2[今年],3[一共]"
                };
            }

            var sqlstr = "";
            switch (ttype)
            {
                default:
                    {
                        sqlstr += " SELECT SUM(UserCost) AS ICount FROM MNY_daily WHERE DATEDIFF(DAY,CreatedAt,GETDATE())=0 AND UserCost<0 AND UserName='" + request.username + "'";
                        sqlstr += " UNION";
                        sqlstr += " SELECT SUM(UserCost) AS ICount FROM MNY_daily WHERE DATEDIFF(DAY,CreatedAt,GETDATE())=0 AND UserCost>0 AND UserName='" + request.username + "'";
                        break;
                    }
                case (int)H10024RequestTypeEnum.ThisMonth:
                    {
                        sqlstr += " SELECT SUM(UserCost) AS ICount FROM MNY_daily WHERE DATEDIFF(DAY,CreatedAt,GETDATE())=0 AND UserCost<0 AND UserName='" + request.username + "'";
                        sqlstr += " UNION";
                        sqlstr += " SELECT SUM(UserCost) AS ICount FROM MNY_daily WHERE DATEDIFF(DAY,CreatedAt,GETDATE())=0 AND UserCost>0 AND UserName='" + request.username + "'";
                        break;
                    }
                case (int)H10024RequestTypeEnum.ThisYear:
                    {
                        sqlstr += " SELECT SUM(UserCost) AS ICount FROM MNY_daily WHERE DATEDIFF(DAY,CreatedAt,GETDATE())=0 AND UserCost<0 AND UserName='" + request.username + "'";
                        sqlstr += " UNION";
                        sqlstr += " SELECT SUM(UserCost) AS ICount FROM MNY_daily WHERE DATEDIFF(DAY,CreatedAt,GETDATE())=0 AND UserCost>0 AND UserName='" + request.username + "'";
                        break;
                    }
                case (int)H10024RequestTypeEnum.Total:
                    {
                        sqlstr += " SELECT SUM(UserCost) AS ICount FROM MNY_daily WHERE UserCost<0 AND UserName='" + request.username + "'"; ;
                        sqlstr += " UNION";
                        sqlstr += " SELECT SUM(UserCost) AS ICount FROM MNY_daily WHERE UserCost>0 AND UserName='" + request.username + "'";
                        break;
                    }
            }

            var recordlist = DataHelper.ExecuteList<GroupItemDto>(sqlstr);
            if (recordlist == null || recordlist.Count < 2)
            {
                return new HBaseResponse
                {
                    error = 1
                };
            }

            var income = recordlist[0].ICount;
            var cost = recordlist[1].ICount;
            return new H10024Response
            {
                error = 0,
                cost = cost,
                income = income
            };
        }

        /// <summary>
        /// 粤语词典-反馈(单字批量)
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public static HBaseResponse H10025(H10025Request request)
        {
            if (String.IsNullOrEmpty(request.chntext))
            {
                return new H10025Response
                {
                    error = 1,
                    message = "chntext必须要填写"
                };
            }

            if (String.IsNullOrEmpty(request.createdby))
            {
                return new H10025Response
                {
                    error = 1,
                    message = "createdby必须要填写"
                };
            }

            if (request.chntext.Length > 20)
            {
                return new H10025Response
                {
                    error = 1,
                    message = "chntext最多能输入20个字符"
                };
            }

            var chars = "";
            var pattern = "[0-9a-zA-Z-_]";
            foreach (var ch in request.chntext)
            {
                if (Regex.IsMatch(ch.ToString(), pattern))
                {
                    continue;
                }

                chars += ch;
            }

            var sb = new StringBuilder();
            foreach (var ch in chars)
            {
                if (request.createdby == "reimu")
                {
                    sb.AppendFormat("INSERT INTO CAN_noresult(ChnText,CreatedAt) VALUES('{0}',GETDATE())", ch);
                }
                else
                {
                    sb.AppendFormat(" INSERT INTO CAN_feedback(ChnText,CreatedAt) VALUES('{0}',GETDATE());", ch);
                }
            }

            var sqlstr = sb.ToString();
            var rows = DataHelper.ExecuteNonQuery(sqlstr);
            if (rows > 0)
            {
                return new H10025Response
                {
                    error = 0
                };
            }

            return new HBaseResponse
            {
                error = 1
            };
        }

        /// <summary>
        /// 柚萌笔记 - 补充日记
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public static HBaseResponse H10026(H10026Request request)
        {
            if (String.IsNullOrEmpty(request.username))
            {
                return new H10026Response
                {
                    error = 1,
                    message = "username不能为空"
                };
            }

            if (String.IsNullOrEmpty(request.content))
            {
                return new H10026Response
                {
                    error = 1,
                    message = "content不能为空"
                };
            }

            if (request.content.Length > 200)
            {
                return new H10026Response
                {
                    error = 1,
                    message = "单次提交笔记长度不能超过200个字符"
                };
            }

            var rows = 0;

            var sqlstr = "SELECT TOP 1 CAST(Id AS NVARCHAR(11)) as IValue FROM NTE_daily WHERE DATEDIFF(DAY,CreatedAt,GETDATE())=0 AND UserName='" + request.username + "' ORDER BY Id DESC";
            var recordlist = DataHelper.ExecuteList<KeyValueDto>(sqlstr);

            //如果今天没记笔记就创建
            //如果已创建就追加
            if (recordlist != null && recordlist.Count > 0)
            {
                var item = recordlist.FirstOrDefault();
                if (item == null)
                {
                    return new H10026Response
                    {
                        error = 1,
                        message = "笔记获取失败"
                    };
                }

                var id = new SqlParameter("@Id", SqlDbType.Int, 11);
                id.Value = Convert.ToInt32(item.IValue);

                var content = new SqlParameter("@Content", SqlDbType.NVarChar, 200);
                content.Value = request.content;

                sqlstr = "UPDATE NTE_daily SET Content+=@Content WHERE Id=@Id";
                rows = DataHelper.ExecuteNonQuery(sqlstr, id, content);
                if (rows > 0)
                {
                    return new H10026Response
                    {
                        error = 0,
                        message = "追加成功"
                    };
                }
                else
                {
                    return new HBaseResponse
                    {
                        error = 1,
                    };
                }
            }

            sqlstr = "INSERT INTO NTE_daily(UserName,Content,CreatedAt) VALUES('" + request.username + "','" + request.content + "',GETDATE())";
            rows = DataHelper.ExecuteNonQuery(sqlstr);
            if (rows > 0)
            {
                return new H10026Response
                {
                    error = 0,
                    message = "创建成功"
                };
            }
            else
            {
                return new HBaseResponse
                {
                    error = 1,
                };
            }
        }

        /// <summary>
        /// 广州地铁 - 查找线路
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public static HBaseResponse H10027(H10027Request request)
        {
            if (String.IsNullOrEmpty(request.name))
            {
                return new H10027Response
                {
                    error = 1,
                    message = "线路名称必须输入"
                };
            }

            var name = new SqlParameter("@Name", SqlDbType.NVarChar, 50);
            name.Value = request.name + "%";

            var sqlstr = "SELECT TOP 1 Id,Name,StartedAt,CompletedAt,OpenedAt,Status FROM MTR_line WHERE Name LIKE @Name";
            var recordlist = DataHelper.ExecuteList<H10027ResponseLine>(sqlstr, name);
            if (recordlist == null || recordlist.Count == 0)
            {
                return new H10027Response
                {
                    error = 1,
                    message = "未找到相关线路"
                };
            }

            var line = recordlist.FirstOrDefault();
            if (line == null)
            {
                return new H10027Response
                {
                    error = 1,
                    message = "获取线路信息失败"
                };
            }

            var response = new H10027Response
            {
                error = 0,
                line = line
            };

            var id = new SqlParameter("@Id", SqlDbType.Int, 11);
            id.Value = line.id;

            sqlstr = @"SELECT 
                       t3.Name,
                       t3.Status 
                       FROM MTR_linestationrelation t1
                       LEFT JOIN MTR_line t2 on t2.Id = t1.LineId
                       LEFT JOIN MTR_station t3 on t3.Id = t1.StationId
                       WHERE t2.Id=@Id";

            var stationlist = DataHelper.ExecuteList<H10027ResponseStation>(sqlstr, id);
            if (stationlist != null && stationlist.Count > 0)
            {
                response.stations = stationlist;
            }

            return response;
        }

        /// <summary>
        /// 广州地铁 - 查找站点
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public static HBaseResponse H10028(H10028Request request)
        {
            if (String.IsNullOrEmpty(request.name))
            {
                return new H10028Response
                {
                    error = 1,
                    message = "线路名称必须输入"
                };
            }

            var name = new SqlParameter("@Name", SqlDbType.NVarChar, 50);
            name.Value = "%" + request.name + "%";

            var sqlstr = @"SELECT Q.LineName,Name as StationName,Status FROM MTR_station t1 
                           OUTER APPLY 
                          (
                               SELECT TOP 1 t3.Name as LineName FROM MTR_linestationrelation t2 
                               LEFT JOIN MTR_line t3 on t3.Id = t2.LineId
                               LEFT JOIN MTR_station t4 on t4.Id = t2.StationId
                               WHERE t4.Id = t1.Id
                           ) AS Q
						   WHERE t1.Name LIKE @Name ";

            var recordlist = DataHelper.ExecuteList<H10028ResponseListItem>(sqlstr, name);
            return new H10028Response
            {
                error = 0,
                data = recordlist ?? new List<H10028ResponseListItem>()
            };
        }

        /// <summary>
        /// 新增或修改文章（llyn23.com）
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public static HBaseResponse H10029(H10029Request request)
        {
            if (String.IsNullOrEmpty(request.title) ||
                String.IsNullOrEmpty(request.content))
            {
                return new H10029Response
                {
                    error = 1,
                    message = "title和content不能为空"
                };
            }

            var rows = 0;
            var sqlstr = "";
            if (!request.id.HasValue)
            {
                sqlstr = "INSERT INTO LYN_archive(Title,Content,CreatedAt) VALUES('" + request.title + "','" + request.content + "',GETDATE())";
                rows = DataHelper.ExecuteNonQuery(sqlstr);
                if (rows > 0)
                {
                    return new H10029Response
                    {
                        error = 0,
                        message = "已创建文章"
                    };
                }

                return new HBaseResponse
                {
                    error = 1
                };
            }

            var id = new SqlParameter("@Id", SqlDbType.Int, 11);
            id.Value = request.id.GetValueOrDefault(0);

            var title = new SqlParameter("@Title", SqlDbType.NVarChar, 11);
            title.Value = request.title;

            var content = new SqlParameter("@Content", SqlDbType.NVarChar, 11);
            content.Value = request.content;

            sqlstr = "UPDATE LYN_archive SET Title=@Title,Content=@Content WHERE Id=@Id;";
            rows = DataHelper.ExecuteNonQuery(sqlstr, id, title, content);
            if (rows > 0)
            {
                return new H10029Response
                {
                    error = 0,
                    message = "已更新文章"
                };
            }

            return new H10029Response
            {
                error = 1,
                message = "文章不存在或者更新时失败了哦"
            };
        }

        /// <summary>
        /// 新增或修改笔记（llyn23.com）
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public static HBaseResponse H10030(H10030Request request)
        {
            if (String.IsNullOrEmpty(request.content))
            {
                return new H10030Response
                {
                    error = 1,
                    message = "content不能为空"
                };
            }

            var content = new SqlParameter("@Content", SqlDbType.NVarChar, 200);
            content.Value = request.content;

            var sqlstr = "";
            var rows = 0;
            if (request.id.HasValue)
            {
                var id = new SqlParameter("@Id", SqlDbType.Int, 11);
                id.Value = request.id.Value;

                sqlstr = "UPDATE LYN_note SET Content=@Content WHERE Id=@Id";
                rows = DataHelper.ExecuteNonQuery(sqlstr, content, id);
                if (rows > 0)
                {
                    return new H10026Response
                    {
                        error = 0,
                        message = "更新成功"
                    };
                }

                return new HBaseResponse
                {
                    error = 1,
                };
            }

            sqlstr = "INSERT INTO LYN_note(Content,CreatedAt) VALUES(@Content,GETDATE())";
            rows = DataHelper.ExecuteNonQuery(sqlstr, content);
            if (rows > 0)
            {
                return new H10026Response
                {
                    error = 0,
                    message = "新增成功"
                };
            }

            return new HBaseResponse
            {
                error = 1,
            };
        }

        /// <summary>
        /// 获取笔记列表（llyn23.com）
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public static HBaseResponse H10031(H10031Request request)
        {
            if (!request.page.HasValue)
            {
                request.page = 1;
            }

            if (!request.pagesize.HasValue)
            {
                request.pagesize = 10;
            }

            var sqlstr = "SELECT ROW_NUMBER() OVER(ORDER BY ID DESC) AS RW,Content,CreatedAt FROM LYN_note";
            var sb = String.Format("SELECT COUNT(1) AS ICount FROM ({0}) AS Q;", sqlstr);

            var obj = DataHelper.ExecuteScalar(sb);
            var recordcount = Convert.ToInt32(obj);

            var begin = new SqlParameter("@begin", SqlDbType.Int, 11);
            begin.Value = request.pagesize * (request.page - 1);

            var end = new SqlParameter("@end", SqlDbType.Int, 11);
            end.Value = request.pagesize * request.page;

            sb = String.Format("SELECT Q.Content,Q.CreatedAt FROM ({0}) AS Q WHERE Q.RW>@begin AND Q.RW<=@end;", sqlstr);
            var recordlist = DataHelper.ExecuteList<H10031ResponseListItem>(sb, begin, end);

            return new H10031Response
            {
                error = 0,
                data = new PagedListDto<H10031ResponseListItem>
                {
                    Page = request.page.Value,
                    PageSize = request.pagesize.Value,
                    RecordCount = recordcount,
                    RecordList = recordlist ?? new List<H10031ResponseListItem>()
                }
            };
        }

        /// <summary>
        /// Token校验
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public static HBaseResponse H10032(H10032Request request)
        {
            if (String.IsNullOrEmpty(request.token))
            {
                return new H10032Response
                {
                    error = 1,
                    message = "token不能为空"
                };
            }

            var token = new SqlParameter("@Token", SqlDbType.NVarChar, 50);
            token.Value = request.token;

            var sqlstr = "SELECT CONVERT(NVARCHAR(20),CreatedAt,120) AS IValue FROM T_token WHERE Token=@Token";
            var list = DataHelper.ExecuteList<KeyValueDto>(sqlstr, token);
            if (list == null || list.Count == 0)
            {
                return new H10032Response
                {
                    error = 1,
                    message = "token无效"
                };
            }

            DateTime dt;
            DateTime.TryParse(list[0].IValue, out dt);

            if (dt.AddMinutes(30) < DateTime.Now)
            {
                return new H10032Response
                {
                    error = 1,
                    message = "token已过期"
                };
            }

            return new H10032Response
            {
                error = 0
            };
        }

        /// <summary>
        /// 获取图片列表（nasa.uimoe.com）
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public static HBaseResponse H10033(H10033Request request)
        {
            if (!request.page.HasValue)
            {
                request.page = 1;
            }

            if (!request.pagesize.HasValue)
            {
                request.pagesize = 5;
            }

            var sqlstr = "SELECT COUNT(1) FROM NASA_img WHERE Status=0";
            var obj = DataHelper.ExecuteScalar(sqlstr);
            var recordcount = Convert.ToDouble(obj);

            var begin = new SqlParameter("@begin", SqlDbType.Int, 11);
            begin.Value = request.pagesize * (request.page - 1);

            var end = new SqlParameter("@end", SqlDbType.Int, 11);
            end.Value = request.pagesize * request.page;

            var splist = new List<SqlParameter>();
            splist.Add(begin);
            splist.Add(end);

            sqlstr = "SELECT * FROM (SELECT *,ROW_NUMBER() OVER(ORDER BY ID DESC) AS RW FROM NASA_img WHERE Status=0) AS Q WHERE Q.RW>@begin AND Q.RW<=@end";
            var recordlist = DataHelper.ExecuteList<H10033ResponseListItem>(sqlstr, splist.ToArray());
            return new H10033Response
            {
                error = 0,
                data = new PagedListDto<H10033ResponseListItem>
                {
                    Page = request.page.Value,
                    PageSize = request.pagesize.Value,
                    RecordCount = recordcount > Int64.MaxValue ? Int64.MaxValue : Convert.ToInt64(recordcount),
                    RecordList = recordlist
                }
            };
        }

        /// <summary>
        /// 保存或更新图片（nasa.uimoe.com）
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public static HBaseResponse H10034(H10034Request request)
        {
            if (String.IsNullOrEmpty(request.title))
            {
                return new H10034Response
                {
                    error = 1,
                    message = "标题不能为空"
                };
            }

            if (String.IsNullOrEmpty(request.filename))
            {
                return new H10034Response
                {
                    error = 1,
                    message = "文件名不能为空"
                };
            }

            var sqlstr = "";
            var rows = 0;

            var title = new SqlParameter("@Title", SqlDbType.NVarChar, 50);
            title.Value = request.title;

            var filename = new SqlParameter("@FileName", SqlDbType.NVarChar, 50);
            filename.Value = request.filename;

            if (request.id.HasValue)
            {
                var id = new SqlParameter("@Id", SqlDbType.Int, 11);
                id.Value = request.id.Value;

                sqlstr = "UPDATE NASA_img SET Title=@Title,FileName=@FileName,Status=1 WHERE Id=@Id;";
                rows = DataHelper.ExecuteNonQuery(sqlstr, title, filename, id);
                if (rows > 0)
                {
                    return new H10034Response
                    {
                        error = 0,
                        message = "更新成功"
                    };
                }

                return new HBaseResponse
                {
                    error = 1
                };
            }

            sqlstr = "INSERT INTO NASA_img(Title,FileName,CreatedAt,Status) VALUES(@Title,@FileName,GETDATE(),1)";
            rows = DataHelper.ExecuteNonQuery(sqlstr, title, filename);
            if (rows > 0)
            {
                return new H10034Response
                {
                    error = 0,
                    message = "新增成功"
                };
            }

            return new HBaseResponse
            {
                error = 1
            };
        }

        /// <summary>
        /// 审核图片（nasa.uimoe.com）
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public static HBaseResponse H10035(H10035Request request)
        {
            if (request.id.HasValue == false && String.IsNullOrEmpty(request.filename))
            {
                return new H10035Response
                {
                    error = 1,
                    message = "id和filename不能同时为空"
                };
            }

            var sqlstr = "UPDATE NASA_img SET Status=0 WHERE 1=1";
            if (request.id.HasValue)
            {
                sqlstr += String.Format(" AND Id=" + request.id.Value);
            }

            if (!String.IsNullOrEmpty(request.filename))
            {
                sqlstr += String.Format(" AND FileName=" + request.id.Value);
            }

            var rows = DataHelper.ExecuteNonQuery(sqlstr);
            if (rows > 0)
            {
                return new H10035Response
                {
                    error = 0
                };
            }

            return new H10035Response
            {
                error = 1
            };
        }

        /// <summary>
        /// 添加访问记录
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public static HBaseResponse H10036(H10036Request request)
        {
            if (String.IsNullOrEmpty(request.rawurl))
            {
                return new H10036Response
                {
                    error = 1,
                    message = "rawurl不能为空"
                };
            }

            if (String.IsNullOrEmpty(request.rawurl))
            {
                return new H10036Response
                {
                    error = 1,
                    message = "userhost不能为空"
                };
            }

            if (String.IsNullOrEmpty(request.useragent))
            {
                return new H10036Response
                {
                    error = 1,
                    message = "useragent不能为空"
                };
            }

            QueryHelper.Save<T_visit>(new T_visit
            {
                CreatedAt = DateTime.Now,
                IPAddress = request.ipaddress,
                RawUrl = request.rawurl,
                UserAgent = request.useragent,
                UserId = request.userid
            });

            return new H10036Response
            {
                error = 0
            };
        }

        /// <summary>
        /// 粤语词典 - 获取情景分类
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public static HBaseResponse H10037(H10037Request request)
        {
            var sqlstr = "SELECT id,name FROM CAN_scene ORDER BY ID DESC";
            if (request.take.HasValue)
            {
                sqlstr = String.Format("SELECT TOP({0}) id,name FROM CAN_scene ORDER BY ID DESC", request.take.Value);
            }

            var recordlist = DataHelper.ExecuteList<H10037ResponseListItem>(sqlstr);
            return new H10037Response
            {
                error = 0,
                data = recordlist ?? new List<H10037ResponseListItem>()
            };
        }

        /// <summary>
        /// 粤语词典 - 获取某个情景的词汇
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public static HBaseResponse H10038(H10038Request request)
        {
            if (!request.sceneid.HasValue)
            {
                return new H10038Response
                {
                    error = 1,
                    message = "sceneid必须填写",
                    data = new PagedListDto<H10038ResponseListItem>
                    {
                        RecordList = new List<H10038ResponseListItem>()
                    }
                };
            }

            var sqlstr = "SELECT t3.ChnText, t3.CanText,t3.CanPronounce FROM CAN_scenewordrelation t1 LEFT JOIN CAN_scene t2 on t2.Id=t1.SceneId LEFT JOIN CAN_vocabulary t3 on t3.Id=t1.VocabularyId WHERE t2.Id=@SceneId;";
            if (request.page.HasValue && request.pagesize.HasValue)
            {
                var skip = request.pagesize.Value * (request.page.Value - 1);
                var take = request.pagesize;
                sqlstr = String.Format("SELECT * FROM (SELECT ROW_NUMBER() OVER(ORDER BY t1.Id DESC) AS RW, t3.ChnText, t3.CanText,t3.CanPronounce FROM CAN_scenewordrelation t1 LEFT JOIN CAN_scene t2 on t2.Id=t1.SceneId LEFT JOIN CAN_vocabulary t3 on t3.Id=t1.VocabularyId WHERE t2.Id=@SceneId) AS Q WHERE Q.RW>{0} AND Q.RW<={1}", skip, skip + take);
            }

            var sceneid = new SqlParameter("@SceneId", SqlDbType.Int, 11);
            sceneid.Value = request.sceneid.Value;
            sceneid.IsNullable = true;

            var recordlist = DataHelper.ExecuteList<H10038ResponseListItem>(sqlstr, sceneid);
            return new H10038Response
            {
                error = 0,
                data = new PagedListDto<H10038ResponseListItem>
                {
                    RecordList = recordlist ?? new List<H10038ResponseListItem>()
                }
            };
        }

        /// <summary>
        /// 粤语词典 - 获取积分增长列表
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public static HBaseResponse H10039(H10039Request request)
        {
            if (request.page.HasValue == false)
            {
                request.page = 1;
            }

            if (request.pagesize.HasValue == false)
            {
                request.pagesize = 10;
            }

            var userid = request.userid.GetValueOrDefault(0);
            if (userid <= 0)
            {
                return new H10039Response
                {
                    error = 1,
                    message = "请先登录",
                    data = new List<H10039ResponseListItem>()
                };
            }

            var begin = new SqlParameter("@begin", SqlDbType.Int, 11);
            begin.Value = request.pagesize.Value * (request.page.Value - 1);

            var end = new SqlParameter("@end", SqlDbType.Int, 11);
            end.Value = request.pagesize.Value * request.page.Value;

            var sqlstr = String.Format(" SELECT * FROM (SELECT ROW_NUMBER() OVER(ORDER BY t1.ID DESC) AS RW,t1.Score,t1.Way,t1.CreatedAt FROM CAN_score t1 WHERE t1.UserId={0}) AS Q WHERE Q.RW>@begin AND Q.RW<=@end;", userid);
            var recordlist = DataHelper.ExecuteList<H10039ResponseListItem>(sqlstr, begin, end);
            return new H10039Response
            {
                error = 0,
                data = recordlist ?? new List<H10039ResponseListItem>()
            };
        }

        /// <summary>
        /// 粤语词典 - 创建积分获取记录
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public static HBaseResponse H10040(H10040Request request)
        {
            var userid = request.userid.GetValueOrDefault(0);
            if (userid <= 0)
            {
                return new H10040Response
                {
                    error = 1,
                    message = "请先登录"
                };
            }

            var score = request.score.GetValueOrDefault(0);
            if (score <= 0)
            {
                return new H10040Response
                {
                    error = 1,
                    message = "score不能为空，且必须为大于0的整数"
                };
            }

            var way = request.way.GetValueOrDefault(0);
            if (way < 0)
            {
                return new H10040Response
                {
                    error = 1,
                    message = "way不能为空，且必须为大于等于0的整数"
                };
            }

            var sqlstr = "";
            var rows = 0;

            var canrepeat = request.canrepeat.GetValueOrDefault(0);
            if (canrepeat == 0)
            {
                sqlstr = "SELECT COUNT(1) FROM CAN_score t1 WHERE DATEDIFF(DAY,t1.CreatedAt,GETDATE())=0 AND t1.UserId='" + userid + "' AND t1.way=" + way;
                var obj = DataHelper.ExecuteScalar(sqlstr);
                var count = Convert.ToInt32(obj);
                if (count > 0)
                {
                    return new H10040Response
                    {
                        error = 1,
                        message = "你今天已经获取过这个积分了哦"
                    };
                }
            }

            sqlstr = String.Format("INSERT INTO CAN_score(Score,Way,UserId,CreatedAt) VALUES({0},{1},{2},GETDATE())", score, way, userid);
            rows = DataHelper.ExecuteNonQuery(sqlstr);

            if (rows > 0)
            {
                return new H10040Response
                {
                    error = 0
                };
            }

            return new H10040Response
            {
                error = 1
            };
        }

        /// <summary>
        /// 粤语词典 - 查询用户积分
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public static HBaseResponse H10041(H10041Request request)
        {
            var userid = request.userid.GetValueOrDefault(0);
            if (userid <= 0)
            {
                return new H10041Response
                {
                    error = 1,
                    message = "请先登录"
                };
            }

            var type = request.type.GetValueOrDefault(0);
            var sqlstr = String.Format("SELECT SUM(Score) FROM CAN_score WHERE UserId={0}", userid);
            switch (type)
            {
                case (int)H10041RequestTypeEnum.Today:
                    {
                        sqlstr += " AND DATEDIFF(DAY,CreatedAt,GETDATE())=0;";
                        break;
                    }
                case (int)H10041RequestTypeEnum.ThisMonth:
                    {
                        sqlstr += " AND DATEDIFF(MONTH,CreatedAt,GETDATE())=0;";
                        break;
                    }
                case (int)H10041RequestTypeEnum.ThisYear:
                    {
                        sqlstr += " AND DATEDIFF(YEAR,CreatedAt,GETDATE())=0;";
                        break;
                    }
            }

            var obj = DataHelper.ExecuteScalar(sqlstr);
            var sum = Convert.ToInt32(obj);

            return new H10041Response
            {
                error = 0,
                score = sum
            };
        }

        /// <summary>
        /// 粤语词典 - 查询用户积分和排行
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public static HBaseResponse H10042(H10042Request request)
        {
            var userid = request.userid.GetValueOrDefault(0);
            if (userid <= 0)
            {
                return new H10041Response
                {
                    error = 1,
                    message = "请先登录"
                };
            }

            var sqlstr = String.Format("SELECT * FROM (SELECT ROW_NUMBER() OVER(ORDER BY Q.Score DESC) AS RW,* FROM( SELECT UserId ,SUM(Score) AS Score FROM CAN_score GROUP BY UserId,Score) AS Q) AS W WHERE W.UserId={0};", request.userid.GetValueOrDefault(0));
            var recordlist = DataHelper.ExecuteList<H10042Response>(sqlstr);
            if (recordlist == null || recordlist.Count == 0)
            {
                return new H10042Response
                {
                    error = 1
                };
            }

            var record = recordlist.FirstOrDefault();
            if (record == null)
            {
                return new H10042Response
                {
                    error = 1
                };
            }

            return new H10042Response
            {
                error = 0,
                score = record.score,
                rw = record.rw
            };
        }

        public static HBaseResponse H10043(H10043Request request)
        {
            var options = new PagedQueryOptions
            {
                Page = request.page.GetValueOrDefault(1),
                PageSize = request.pagesize.GetValueOrDefault(10),
            };

            var pagedlist = QueryHelper.GetPagedList<H10043ResponseListItem>(options);
            return new H10043Response
            {
                error = 0,
                data = pagedlist
            };
        }
    }
}

using Agile.API.Dtos;
using Agile.Cache;
using Agile.Data.Helpers;
using Agile.Dtos;
using Agile.Helpers;
using Agile.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Agile.API.Helpers
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
            var page = request.page.GetValueOrDefault(1);
            var pagesize = request.pagesize.GetValueOrDefault(10);
            var options = new PagedQueryOptions
            {
                Page = page,
                PageSize = pagesize
            };

            var list = QueryHelper.GetPagedList<T_interface>(options);
            return new H10000Response
            {
                error = 0,
                data = new PagedListDto<H10000ResponseListItem>
                {
                    Page = options.Page,
                    PageSize = options.PageSize,
                    RecordCount = list.RecordCount,
                    RecordList = list.RecordList.Select(o => new H10000ResponseListItem
                    {
                        Code = o.Code,
                        CreatedAt = o.CreatedAt,
                        Id = o.Id,
                        Name = o.Name
                    }).ToList()
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

            QueryHelper.Delete<T_interface>(w => w.Code == request.code);
            return new H10001Response
            {
                error = 0
            };
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

            var entity = new T_interface
            {
                Code = request.code,
                CreatedAt = DateTime.Now,
                Description = request.description,
                Name = request.name
            };

            QueryHelper.Save<T_interface>(entity);
            return new H10001Response
            {
                error = 0
            };
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

            var entity = new T_interface
            {
                Code = request.code,
                Description = request.description,
                Name = request.name
            };

            var options = new UpdateOptions();
            options.Where<T_interface>(w => w.Code == request.code);

            QueryHelper.Update<T_interface>(entity, options);
            return new H10003Response
            {
                error = 0
            };
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

            var options = new TopQueryOptions
            {
                TopNum = 10
            };

            if (!string.IsNullOrEmpty(request.code))
            {
                options.Where<T_interface>(w => w.Code == request.code);
            }

            if (request.id.HasValue)
            {
                var id = request.id.Value;
                options.Where<T_interface>(w => w.Id == id);
            }

            var list = QueryHelper.GetList<T_interface>(options);
            return new H10004Response
            {
                error = 0,
                data = list.Select(o => new H10004ResponseListItem
                {
                    code = o.Code,
                    createdat = o.CreatedAt,
                    description = o.Description,
                    id = o.Id,
                    name = o.Name
                }).ToList()
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

            var sqlstr = "SELECT TOP 1 id,username,userpass,[status] FROM T_user WHERE UserName=@username ORDER BY ID DESC";
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

            var userstatus = user.status.GetValueOrDefault(0);
            if (userstatus == (int)H10047UserStatusEnum.Pending)
            {
                return new H10009Response
                {
                    error = 1,
                    message = "用户需要审核通过后才可以登录"
                };
            }

            if (userstatus == (int)H10047UserStatusEnum.Forbidden)
            {
                return new H10009Response
                {
                    error = 1,
                    message = "用户被禁止，无法登录"
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

            var sqlstr = "SELECT TOP 1 username,userpass FROM T_user WHERE UserName='" + request.username + "' ORDER BY ID DESC";
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

            sqlstr = "INSERT INTO T_user(username,userpass,email,createdat,[status]) VALUES(@username,@userpass,@email,GETDATE(),1);";
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
                var sqlstr = "SELECT TOP 1 id FROM T_user WHERE UserName='" + request.username + "' ORDER BY ID DESC";
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

            var sqlstr = "SELECT t1.*,t2.username as username FROM LYN_comment t1 LEFT JOIN T_user t2 on t2.Id= t1.UserId WHERE t1.ArchiveId=@archiveid AND (t1.Status IS NULL OR t1.Status=0)";
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
        public static H10013Response H10013(H10013Request request)
        {
            var take = request.take.GetValueOrDefault(1);

            var sb = new StringBuilder();
            var apptypes = Enum.GetValues(typeof(AppTypeEnum));
            foreach (var apptype in apptypes)
            {
                sb.AppendFormat(" SELECT * FROM (SELECT TOP ({0}) * FROM UME_app WHERE AppType={1} ORDER BY NEWID()) AS t{1}\r\n", take, (int)apptype);
                sb.AppendFormat(" UNION ALL\r\n");
            }

            var sqlstr = sb.ToString();
            if (sqlstr.Length > 12)
            {
                sqlstr = sqlstr.Substring(0, sqlstr.Length - 12);
            }

            var recordlist = DataHelper.ExecuteList<H10013ResponseListItem>(sqlstr);
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
            if (string.IsNullOrEmpty(request.input))
            {
                return new H10014Response
                {
                    error = 1,
                    message = "input不能为空"
                };
            }

            var sqlstr1 = String.Format("SELECT * FROM CAN_vocabulary WHERE ChnText=N'{0}'", request.input);
            var list1 = DataHelper.ExecuteList<Can_vocabulary>(sqlstr1);
            if (list1 != null && list1.Count > 0)
            {
                var response1 = new H10014Response
                {
                    error = 0,
                    isallmatched = true,
                    groups = new List<H10014ResponseGroupItem>()
                };

                response1.groups.Add(new H10014ResponseGroupItem
                {
                    rw = 1,
                    chntext = request.input,
                    items = list1.Select(o => new H10014ResponseListItem
                    {
                        canpronounce = o.CanPronounce,
                        cantext = o.CanText,
                        canvoice = o.CanVoice
                    }).ToList()
                });

                return response1;
            }

            var response = new H10014Response
            {
                error = 0,
                isallmatched = false,
                groups = new List<H10014ResponseGroupItem>(),
                noresult = new List<string>()
            };

            //整体匹配未成功(只记录单个字)
            if (request.input.Length == 1)
            {
                response.noresult.Add(request.input);
            }

            //开始逐个匹配
            var words = new List<string>();
            foreach (var ch in request.input)
            {
                words.Add(ch.ToString());
            }

            var sb = new StringBuilder();
            for (var i = 0; i < words.Count; i++)
            {
                sb.AppendFormat(" SELECT *,{0} as rw FROM CAN_vocabulary WHERE ChnText=N'{1}'\r\n", i, words[i]);
                sb.AppendFormat(" UNION ALL\r\n");
            }

            var sqlstr = sb.ToString();
            if (sqlstr.Length > 11)
            {
                sqlstr = sqlstr.Substring(0, sqlstr.Length - 11);
            }

            var list = DataHelper.ExecuteList<H10014ResponseOrderItem>(sqlstr);
            foreach (var w in words)
            {
                var cnt = list.Count(c => c.chntext == w);
                if (cnt == 0)
                {
                    response.noresult.Add(w);
                }
            }

            var groups = list.GroupBy(g => g.chntext);
            foreach (var g in groups)
            {
                var rg = new H10014ResponseGroupItem
                {
                    rw = g.FirstOrDefault().rw,
                    chntext = g.Key,
                    items = new List<H10014ResponseListItem>()
                };

                var items = g.Select(o => new H10014ResponseListItem
                {
                    canpronounce = o.canpronounce,
                    cantext = o.cantext,
                    canvoice = o.canvoice,
                }).ToList();

                foreach (var i in items)
                {
                    var cnt2 = rg.items.Count(c => c.canpronounce == i.canpronounce);
                    if (cnt2 > 0)
                    {
                        continue;
                    }

                    rg.items.Add(i);
                }

                response.groups.Add(rg);
            }

            response.groups = response.groups.OrderBy(o => o.rw).ToList();
            return response;
        }

        /// <summary>
        /// 粤语词典-反馈
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public static HBaseResponse H10017(H10017Request request)
        {
            var sqlstr = "INSERT INTO CAN_feedback(ChnText,CanText,CreatedAt,CreatedBy,Status) VALUES(@ChnText,@CanText,GETDATE(),@CreatedBy,1);";
            var sp1 = new SqlParameter("@ChnText", SqlDbType.NVarChar, 50);
            sp1.Value = request.chntext;
            sp1.IsNullable = true;

            var sp2 = new SqlParameter("@CanText", SqlDbType.NVarChar, 50);
            sp2.Value = request.cantext;
            sp2.IsNullable = true;

            var sp3 = new SqlParameter("@CreatedBy", SqlDbType.NVarChar, 50);
            sp3.Value = request.createdby;
            sp3.IsNullable = true;

            var rows = DataHelper.ExecuteNonQuery(sqlstr, sp1, sp2, sp3);
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
                data = new SkipTakeListDto<H10018ResponseListItem>
                {
                    Skip = skip,
                    Take = take,
                    RecordList = recordlist
                }
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
        /// 粤语词典 - 获取我的反馈
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public static HBaseResponse H10022(H10022Request request)
        {
            var options = new PagedQueryOptions
            {
                Page = request.page.GetValueOrDefault(1),
                PageSize = request.pagesize.GetValueOrDefault(10)
            };

            if (!string.IsNullOrEmpty(request.username))
            {
                options.Where<Can_feedback>(w => w.CreatedBy == request.username);
            }

            var pagedlist = QueryHelper.GetPagedList<Can_feedback>(options);
            return new H10022Response
            {
                error = 0,
                data = new PagedListDto<H10022ResponseListItem>
                {
                    Page = pagedlist.Page,
                    PageSize = pagedlist.PageSize,
                    RecordCount = pagedlist.RecordCount,
                    RecordList = pagedlist.RecordList.Select(o => new H10022ResponseListItem
                    {
                        cantext = o.CanText,
                        chntext = o.ChnText,
                        createdat = o.CreatedAt,
                        status = o.Status
                    }).ToList()
                }
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
                case (int)H10041RequestTypeEnum.ThisMonth:
                    {
                        sqlstr += " SELECT SUM(UserCost) AS ICount FROM MNY_daily WHERE DATEDIFF(DAY,CreatedAt,GETDATE())=0 AND UserCost<0 AND UserName='" + request.username + "'";
                        sqlstr += " UNION";
                        sqlstr += " SELECT SUM(UserCost) AS ICount FROM MNY_daily WHERE DATEDIFF(DAY,CreatedAt,GETDATE())=0 AND UserCost>0 AND UserName='" + request.username + "'";
                        break;
                    }
                case (int)H10041RequestTypeEnum.ThisYear:
                    {
                        sqlstr += " SELECT SUM(UserCost) AS ICount FROM MNY_daily WHERE DATEDIFF(DAY,CreatedAt,GETDATE())=0 AND UserCost<0 AND UserName='" + request.username + "'";
                        sqlstr += " UNION";
                        sqlstr += " SELECT SUM(UserCost) AS ICount FROM MNY_daily WHERE DATEDIFF(DAY,CreatedAt,GETDATE())=0 AND UserCost>0 AND UserName='" + request.username + "'";
                        break;
                    }
                case (int)H10041RequestTypeEnum.Total:
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
                sb.AppendFormat(" INSERT INTO CAN_feedback(ChnText,CreatedAt) VALUES('{0}',GETDATE());", ch);
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
            if (!request.domain.HasValue)
            {
                return new H10036Response
                {
                    error = 1,
                    message = "domain不能为空"
                };
            }

            if (String.IsNullOrEmpty(request.rawurl))
            {
                return new H10036Response
                {
                    error = 1,
                    message = "rawurl不能为空"
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
                UserId = request.userid,
                Domain = request.domain
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
            var sqlstr = " SELECT";
            if (request.take.HasValue)
            {
                sqlstr = string.Format(" TOP({0})", request.take.Value);
            }

            sqlstr += " id,name,(SELECT COUNT(1) FROM CAN_scenewordrelation WHERE CAN_scenewordrelation.SceneId=CAN_scene.Id) AS total FROM CAN_scene ORDER BY ID DESC";

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

            var sqlstr = "SELECT t3.ChnText, t3.CanText,t3.CanPronounce FROM CAN_scenewordrelation t1 LEFT JOIN CAN_scene t2 on t2.Id=t1.SceneId LEFT JOIN Can_vocabulary t3 on t3.Id=t1.VocabularyId WHERE t2.Id=@SceneId;";
            if (request.page.HasValue && request.pagesize.HasValue)
            {
                var skip = request.pagesize.Value * (request.page.Value - 1);
                var take = request.pagesize;
                sqlstr = String.Format("SELECT * FROM (SELECT ROW_NUMBER() OVER(ORDER BY t1.Id DESC) AS RW, t3.ChnText, t3.CanText,t3.CanPronounce FROM CAN_scenewordrelation t1 LEFT JOIN CAN_scene t2 on t2.Id=t1.SceneId LEFT JOIN Can_vocabulary t3 on t3.Id=t1.VocabularyId WHERE t2.Id=@SceneId) AS Q WHERE Q.RW>{0} AND Q.RW<={1}", skip, skip + take);
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

        /// <summary>
        /// 粤语词典 - 获取粤语常用字
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public static HBaseResponse H10043(H10043Request request)
        {
            var options = new PagedQueryOptions
            {
                Page = request.page.GetValueOrDefault(1),
                PageSize = request.pagesize.GetValueOrDefault(10),
            };

            var pagedlist = QueryHelper.GetPagedList<Can_word>(options);
            return new H10043Response
            {
                error = 0,
                data = new PagedListDto<H10043ResponseListItem>
                {
                    Page = pagedlist.Page,
                    PageSize = pagedlist.PageSize,
                    RecordCount = pagedlist.RecordCount,
                    RecordList = pagedlist.RecordList.Select(o => new H10043ResponseListItem
                    {
                        canpronounce = o.CanPronounce,
                        cantext = o.CanText,
                        description = o.Description
                    }).ToList()
                }
            };
        }

        /// <summary>
        /// 获取用户的权限列表
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public static HBaseResponse H10044(H10044Request request)
        {
            var userid = request.userid.GetValueOrDefault(0);
            if (userid == 0)
            {
                return new H10044Response
                {
                    error = 1,
                    message = "userid必须填写"
                };
            }

            var sqlstr = @"SELECT (SELECT COUNT(1) FROM T_menu WHERE T_menu.PermissionId= t1.Id) AS HasMenu, t1.Domain, t1.Name,t1.RawUrl FROM T_permission t1
                           JOIN T_rolepermissionrelation t2 on t2.PermissionId = t1.Id
                           JOIN T_role t3 on t3.Id = t2.RoleId
                           JOIN T_userrolerelation t4 on t4.RoleId = t3.Id
                           JOIN T_user t5 on t5.Id = t4.UserId
                           WHERE t5.Id=" + userid;

            if (request.domain.HasValue)
            {
                sqlstr += String.Format(" AND t1.Domain={0}", request.domain.Value);
            }

            if (!String.IsNullOrEmpty(request.rawurl))
            {
                sqlstr += String.Format(" AND t1.RawUrl={0}", request.rawurl);
            }

            var list = DataHelper.ExecuteList<H10044ResponseListItem>(sqlstr);

            var sqlstr2 = @"SELECT (SELECT COUNT(1) FROM T_menu WHERE T_menu.PermissionId= t1.Id) AS HasMenu,t1.Domain, t1.Name,t1.RawUrl from T_blacklist t2
                            JOIN T_permission t1 on t1.Id = t2.PermissionId
                            JOIN T_user t3 on t3.Id = t2.UserId
                            WHERE t3.Id=" + userid;

            var blacklist = DataHelper.ExecuteList<H10044ResponseListItem>(sqlstr2);

            var sqlstr3 = @"SELECT (SELECT COUNT(1) FROM T_menu WHERE T_menu.PermissionId= t1.Id) AS HasMenu,t1.Domain, t1.Name,t1.RawUrl from T_whitelist t2
                            JOIN T_permission t1 on t1.Id = t2.PermissionId
                            JOIN T_user t3 on t3.Id = t2.UserId
                            WHERE t3.Id=" + userid;

            var whitelist = DataHelper.ExecuteList<H10044ResponseListItem>(sqlstr3);

            foreach (var item in whitelist)
            {
                list.Add(new H10044ResponseListItem
                {
                    hasmenu = item.hasmenu,
                    name = item.name,
                    rawurl = item.rawurl
                });
            }

            for (var i = list.Count - 1; i >= 0; i--)
            {
                var permission = list[i];
                if (blacklist.Any(a => a.domain == permission.domain && a.rawurl == permission.rawurl))
                {
                    list.Remove(permission);
                }
            }

            return new H10044Response
            {
                error = 0,
                data = list
            };
        }

        /// <summary>
        /// 获取访问记录
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public static HBaseResponse H10045(H10045Request request)
        {
            var options = new PagedQueryOptions
            {
                Page = request.page.GetValueOrDefault(1),
                PageSize = request.pagesize.GetValueOrDefault(10)
            };

            if (request.domain.HasValue)
            {
                var domain = request.domain.Value;
                options.Where<T_visit>(w => w.Domain == domain);
            }

            var pagedlist = QueryHelper.GetPagedList<T_visit>(options);
            return new H10045Response
            {
                error = 0,
                data = new PagedListDto<H10045ResponseListItem>
                {
                    Page = pagedlist.Page,
                    PageSize = pagedlist.PageSize,
                    RecordCount = pagedlist.RecordCount,
                    RecordList = pagedlist.RecordList.Select(o => new H10045ResponseListItem
                    {
                        createdat = o.CreatedAt,
                        ipaddress = o.IPAddress,
                        rawurl = o.RawUrl,
                        useragent = o.UserAgent,
                        userid = o.UserId
                    }).ToList()
                }
            };
        }

        /// <summary>
        /// 粤语词典 - 获取普通话常用字，词
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public static HBaseResponse H10046(H10046Request request)
        {
            var take = request.take.GetValueOrDefault(10);
            var textLength = 1;
            if (request.texttype.GetValueOrDefault(1) == 2)
            {
                textLength = 2;
            }

            var sqlstr = "SELECT TOP(@Take) CanPronounce,CanText,CanVoice,ChnText FROM CAN_vocabulary WHERE LEN(ChnText)=@ChnTextLength ORDER BY NEWID();";
            var sp1 = new SqlParameter("@Take", SqlDbType.Int, 11);
            sp1.Value = take;

            var sp2 = new SqlParameter("@ChnTextLength", SqlDbType.Int, 11);
            sp2.Value = textLength;

            var list = DataHelper.ExecuteList<H10018ResponseListItem>(sqlstr, sp1, sp2);
            return new H10046Response
            {
                error = 0,
                data = list.Select(o => new H10018ResponseListItem
                {
                    canpronounce = o.canpronounce,
                    cantext = o.cantext,
                    canvoice = o.canvoice,
                    chntext = o.chntext
                }).ToList()
            };
        }

        /// <summary>
        /// 获取用户列表
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public static HBaseResponse H10047(H10047Request request)
        {
            var options = new PagedQueryOptions
            {
                Page = request.page.GetValueOrDefault(1),
                PageSize = request.pagesize.GetValueOrDefault(10)
            };

            var pagedlist = QueryHelper.GetPagedList<T_user>(options);
            return new H10047Response
            {
                error = 0,
                data = new PagedListDto<H10047ResponseListItem>
                {
                    Page = pagedlist.Page,
                    PageSize = pagedlist.PageSize,
                    RecordCount = pagedlist.RecordCount,
                    RecordList = pagedlist.RecordList.Select(o => new H10047ResponseListItem
                    {
                        status = o.Status.GetValueOrDefault(0),
                        username = o.UserName
                    }).ToList()
                }
            };
        }

        /// <summary>
        /// 通过一个用户
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public static HBaseResponse H10048(H10048Request request)
        {
            if (String.IsNullOrEmpty(request.username))
            {
                return new H10048Response
                {
                    error = 1,
                    message = "username不能为空"
                };
            }

            var sqlstr = "UPDATE T_user SET Status=0 WHERE UserName=@UserName";
            var sp = new SqlParameter("@UserName", SqlDbType.NVarChar, 50);
            sp.Value = request.username;

            DataHelper.ExecuteNonQuery(sqlstr, sp);
            return new H10048Response
            {
                error = 0
            };
        }

        /// <summary>
        /// 禁止一个用户
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public static HBaseResponse H10049(H10049Request request)
        {
            if (String.IsNullOrEmpty(request.username))
            {
                return new H10049Response
                {
                    error = 1,
                    message = "username不能为空"
                };
            }

            var sqlstr = "UPDATE T_user SET Status=2 WHERE UserName=@UserName";
            var sp = new SqlParameter("@UserName", SqlDbType.NVarChar, 50);
            sp.Value = request.username;

            DataHelper.ExecuteNonQuery(sqlstr, sp);
            return new H10049Response
            {
                error = 0
            };
        }

        /// <summary>
        /// 处理一条反馈
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public static HBaseResponse H10050(H10050Request request)
        {
            if (String.IsNullOrEmpty(request.chntext))
            {
                return new H10049Response
                {
                    error = 1,
                    message = "chntext不能为空"
                };
            }

            var sqlstr = "UPDATE Can_feedback SET Status=0 WHERE ChnText=@ChnText";
            var sp = new SqlParameter("@ChnText", SqlDbType.NVarChar, 50);
            sp.Value = request.chntext;

            DataHelper.ExecuteNonQuery(sqlstr, sp);
            return new H10049Response
            {
                error = 0
            };
        }

        /// <summary>
        /// 粤语词典 - 查询普通话常用字（随机列表）
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public static H10051Response H10051(H10051Request request)
        {
            var options = new PagedQueryOptions
            {
                Page = request.page.GetValueOrDefault(1),
                PageSize = request.pagesize.GetValueOrDefault(10)
            };

            var pagedlist = QueryHelper.GetPagedList<Can_word>(options);
            return new H10051Response
            {
                error = 0,
                data = new PagedListDto<H10051ResponseListItem>
                {
                    Page = pagedlist.Page,
                    PageSize = pagedlist.PageSize,
                    RecordCount = pagedlist.RecordCount,
                    RecordList = pagedlist.RecordList.Select(o => new H10051ResponseListItem
                    {
                        canpronounce = o.CanPronounce,
                        cantext = o.CanText,
                        description = o.Description
                    }).ToList()
                }
            };
        }

        /// <summary>
        /// 粤语词典 - 查询普通话常用词（随机列表）
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public static H10052Response H10052(H10052Request request)
        {
            var options = new PagedQueryOptions
            {
                Page = request.page.GetValueOrDefault(1),
                PageSize = request.pagesize.GetValueOrDefault(10)
            };

            var pagedlist = QueryHelper.GetPagedList<Can_term>(options);
            return new H10052Response
            {
                error = 0,
                data = new PagedListDto<H10052ResponseListItem>
                {
                    Page = pagedlist.Page,
                    PageSize = pagedlist.PageSize,
                    RecordCount = pagedlist.RecordCount,
                    RecordList = pagedlist.RecordList.Select(o => new H10052ResponseListItem
                    {
                        canpronounce = o.CanPronounce,
                        cantext = o.CanText,
                        description = o.Description
                    }).ToList()
                }
            };
        }

        /// <summary>
        /// 粤语词典 - 没有结果的查询管理
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public static H10053Response H10053(H10053Request request)
        {
            var options = new PagedQueryOptions
            {
                Page = request.page.GetValueOrDefault(1),
                PageSize = request.pagesize.GetValueOrDefault(10)
            };

            var pagedlist = QueryHelper.GetPagedList<Can_noresult>(options);
            return new H10053Response
            {
                error = 0,
                data = new PagedListDto<H10053ResponseListItem>
                {
                    Page = pagedlist.Page,
                    PageSize = pagedlist.PageSize,
                    RecordCount = pagedlist.RecordCount,
                    RecordList = pagedlist.RecordList.Select(o => new H10053ResponseListItem
                    {
                        chntext = o.ChnText
                    }).ToList()
                }
            };
        }

        /// <summary>
        /// 粤语词典 - 删除查询无结果的项
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public static H10054Response H10054(H10054Request request)
        {
            QueryHelper.Delete<Can_noresult>(w => w.ChnText == request.chntext);
            return new H10054Response
            {
                error = 0
            };
        }

        /// <summary>
        /// 删除权限
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public static H10055Response H10055(H10055Request request)
        {
            if (!request.permissionid.HasValue)
            {
                return new H10055Response
                {
                    error = 1,
                    message = "permissionid必须填写"
                };
            }

            if (!request.userid.HasValue)
            {
                var sqlstr0 = "DELETE FROM T_permission WHERE ID=" + request.permissionid.Value;
                DataHelper.ExecuteNonQuery(sqlstr0);
                return new H10055Response
                {
                    error = 0
                };
            }

            var sqlstr = "DELETE FROM T_whitelist WHERE PermissionId=@PermissionId1 AND UserId=@UserId1";
            var sp1 = new SqlParameter("@PermissionId1", SqlDbType.Int, 11);
            sp1.Value = request.permissionid.Value;

            var sp2 = new SqlParameter("@UserId1", SqlDbType.Int, 11);
            sp2.Value = request.userid.Value;

            DataHelper.ExecuteNonQuery(sqlstr, sp1, sp2);

            var entity = new T_blacklist
            {
                CreatedAt = DateTime.Now,
                PermissionId = request.permissionid.Value,
                UserId = request.userid.Value
            };

            QueryHelper.Save<T_blacklist>(entity);
            return new H10055Response
            {
                error = 0
            };
        }

        /// <summary>
        /// 增加用户权限
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public static H10056Response H10056(H10056Request request)
        {
            if (!request.permissionid.HasValue)
            {
                return new H10056Response
                {
                    error = 1,
                    message = "permissionid必须填写"
                };
            }

            if (!request.userid.HasValue)
            {
                return new H10056Response
                {
                    error = 1,
                    message = "userid必须填写"
                };
            }

            var sqlstr = "DELETE FROM T_black WHERE PermissionId=@PermissionId1 AND UserId=@UserId1";
            var sp1 = new SqlParameter("@PermissionId1", SqlDbType.Int, 11);
            sp1.Value = request.permissionid.Value;

            var sp2 = new SqlParameter("@UserId1", SqlDbType.Int, 11);
            sp2.Value = request.userid.Value;

            DataHelper.ExecuteNonQuery(sqlstr, sp1, sp2);

            var entity = new T_whitelist
            {
                CreatedAt = DateTime.Now,
                PermissionId = request.permissionid.Value,
                UserId = request.userid.Value
            };

            QueryHelper.Save<T_whitelist>(entity);
            return new H10056Response
            {
                error = 0
            };
        }

        /// <summary>
        /// 新增权限
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public static H10057Response H10057(H10057Request request)
        {
            if (String.IsNullOrEmpty(request.rawurl))
            {
                return new H10057Response
                {
                    error = 1,
                    message = "rawurl必须填写"
                };
            }

            if (String.IsNullOrEmpty(request.name))
            {
                return new H10057Response
                {
                    error = 1,
                    message = "name必须填写"
                };
            }

            if (!request.domain.HasValue)
            {
                return new H10057Response
                {
                    error = 1,
                    message = "domain必须填写"
                };
            }

            var permissionid = 0;
            var sqlstr0 = "SELECT TOP 1 CAST(Id AS NVARCHAR(11)) AS IValue FROM T_permission WHERE Domain=" + request.domain.Value + " AND RawUrl='" + request.rawurl + "'";
            var list = DataHelper.ExecuteList<KeyValueDto>(sqlstr0);
            if (list != null && list.Any())
            {
                return new H10057Response
                {
                    error = 1,
                    message = "此权限已经存在"
                };
            }

            sqlstr0 = "INSERT INTO T_permission(Name,Domain,RawUrl,CreatedAt) VALUES(@Name,@Domain,@RawUrl,GETDATE());SELECT @@IDENTITY;";
            var sp0n = new SqlParameter("@Name", SqlDbType.NVarChar, 50);
            sp0n.Value = request.name;

            var sp0d = new SqlParameter("@Domain", SqlDbType.Int, 11);
            sp0d.Value = request.domain.Value;

            var sp0r = new SqlParameter("@RawUrl", SqlDbType.NVarChar, 50);
            sp0r.Value = request.rawurl;

            var obj = DataHelper.ExecuteScalar(sqlstr0, sp0n, sp0d, sp0r);
            permissionid = Convert.ToInt32(obj);
            return new H10057Response
            {
                error = 0
            };
        }

        /// <summary>
        /// 哈哈MX - 获取笑话列表
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public static H10058Response H10058(H10058Request request)
        {
            var sqlstr = "SELECT ROW_NUMBER() OVER(ORDER BY ID DESC) AS RW,* FROM HAHA_collection";
            var sb = string.Format("SELECT COUNT(1) FROM ({0}) AS Q", sqlstr);
            var obj = DataHelper.ExecuteScalar(sb);
            var recordcount = Convert.ToInt32(obj);

            var page = request.page.GetValueOrDefault(1);
            var pagesize = request.pagesize.GetValueOrDefault(10);

            sb = string.Format("SELECT * FROM ({0}) AS Q WHERE Q.RW>{1} AND Q.RW<={2}", sqlstr, pagesize * (page - 1), pagesize * page);
            var list = DataHelper.ExecuteList<HAHA_collection>(sb);

            return new H10058Response
            {
                error = 0,
                data = new PagedListDto<H10058ResponseListItem>
                {
                    Page = page,
                    PageSize = pagesize,
                    RecordCount = recordcount,
                    RecordList = list.Select(o => new H10058ResponseListItem
                    {
                        content = o.Content,
                        jokeid = o.JokeId,
                        pictureurl = o.PictureUrl
                    }).ToList()
                }
            };
        }

        /// <summary>
        /// 哈哈MX - 获取随机笑话列表
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public static H10059Response H10059(H10059Request request)
        {
            var sqlstr = "SELECT TOP(@Take) Content FROM HAHA_collection ORDER BY NEWID()";
            if (request.nopicture.GetValueOrDefault(0) > 0)
            {
                sqlstr = "SELECT TOP(1) Content FROM HAHA_collection WHERE PictureUrl IS NULL OR LEN(PictureUrl) = 0 ORDER BY NEWID()";
            }

            var sp = new SqlParameter("@Take", SqlDbType.Int, 11);
            sp.Value = request.take.GetValueOrDefault(1);

            var list = DataHelper.ExecuteList<H10059ResponseListItem>(sqlstr, sp);
            return new H10059Response
            {
                error = 0,
                data = list
            };
        }

        /// <summary>
        /// 哈哈MX - 保存哈哈
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public static H10060Response H10060(H10060Request request)
        {
            if (request.data == null || request.data.Count == 0)
            {
                new H10060Response
                {
                    error = 1,
                    count = 0,
                    message = "request.data不能为空"
                };
            }

            var sb = new StringBuilder();
            foreach (var item in request.data)
            {
                if (string.IsNullOrEmpty(item.pictureurl))
                {
                    sb.AppendFormat("INSERT INTO HAHA_collection(Content,JokeId,PictureUrl,CreatedAt) VALUES('{0}','{1}',NULL,GETDATE());\r\n", item.content, item.jokeid);
                }
                else
                {
                    sb.AppendFormat("INSERT INTO HAHA_collection(Content,JokeId,PictureUrl,CreatedAt) VALUES('{0}','{1}','{2}',GETDATE());\r\n", item.content, item.jokeid, item.pictureurl);
                }
            }

            var count = DataHelper.ExecuteNonQuery(sb.ToString());
            return new H10060Response
            {
                error = 0,
                count = count
            };
        }

        /// <summary>
        /// 粤语词典 - 获取我的计划
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public static H10061Response H10061(H10061Request request)
        {
            var userid = request.userid.GetValueOrDefault(0);
            if (userid <= 0)
            {
                return new H10061Response
                {
                    error = 1,
                    message = "userid不能为空"
                };
            }

            var sqlstr = "SELECT Id as sceneid,name,(SELECT COUNT(1) FROM CAN_scenewordrelation WHERE CAN_scenewordrelation.SceneId = CAN_scene.Id) AS total,(SELECT COUNT(1) FROM CAN_plan WHERE CAN_plan.SceneId=CAN_scene.Id AND CAN_plan.UserId=" + userid + ") AS finished FROM CAN_scene";
            var sb1 = String.Format("SELECT COUNT(1) FROM ({0}) AS Q", sqlstr);
            var obj = DataHelper.ExecuteScalar(sb1);
            var recordcount = Convert.ToInt32(obj);

            var page = request.page.GetValueOrDefault(1);
            var pagesize = request.pagesize.GetValueOrDefault(10);
            var begin = pagesize * (page - 1);
            var end = pagesize * page;

            var sb2 = String.Format("SELECT * FROM(SELECT *,ROW_NUMBER() OVER(ORDER BY Q.sceneid DESC) AS RW FROM ({0}) AS Q) AS W WHERE W.RW>{1} AND W.RW<={2}", sqlstr, begin, end);
            var list = DataHelper.ExecuteList<H10061ResponseListItem>(sb2);
            return new H10061Response
            {
                error = 0,
                data = new PagedListDto<H10061ResponseListItem>
                {
                    Page = page,
                    PageSize = pagesize,
                    RecordCount = recordcount,
                    RecordList = list ?? new List<H10061ResponseListItem>()
                }
            };
        }

        public static H10062Response H10062(H10062Request request)
        {
            if (request.sceneid.HasValue == false)
            {
                return new H10062Response
                {
                    error = 1,
                    message = "sceneid不能为空"
                };
            }

            if (request.userid.HasValue == false)
            {
                return new H10062Response
                {
                    error = 1,
                    message = "userid不能为空"
                };
            }

            var sqlstr = @"SELECT TOP 1 
                           t2.Id,
                           t2.ChnText,
                           t2.CanText,
                           t2.CanPronounce,
                           (SELECT COUNT(1) FROM CAN_plan p WHERE p.VocabularyId=t1.VocabularyId AND p.SceneId=t1.SceneId AND p.UserId=@UserId) Finished 
                           FROM CAN_scenewordrelation t1
                           JOIN CAN_sceneword t2 on t2.Id = t1.VocabularyId
                           WHERE t1.SceneId = @SceneId
                           ORDER BY NEWID();";

            var sp1 = new SqlParameter("@UserId", SqlDbType.Int, 11);
            sp1.Value = request.userid.Value;

            var sp2 = new SqlParameter("@SceneId", SqlDbType.Int, 11);
            sp2.Value = request.sceneid.Value;

            var list = DataHelper.ExecuteList<H10062ResponseListItem>(sqlstr, sp1, sp2);
            return new H10062Response
            {
                error = 0,
                data = list == null ? null : list.FirstOrDefault()
            };
        }

        /// <summary>
        /// 粤语词典 - 完成任务
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public static H10063Response H10063(H10063Request request)
        {
            if (request.sceneid.HasValue == false)
            {
                return new H10063Response
                {
                    error = 1,
                    message = "sceneid不能为空"
                };
            }

            if (request.vocabularyid.HasValue == false)
            {
                return new H10063Response
                {
                    error = 1,
                    message = "vocabularyid不能为空"
                };
            }

            if (request.userid.HasValue == false)
            {
                return new H10063Response
                {
                    error = 1,
                    message = "userid不能为空"
                };
            }

            var sqlstr = @"INSERT INTO CAN_plan(SceneId,VocabularyId,UserId,CreatedAt) VALUES(@SceneId,@VocabularyId,@UserId,GETDATE())";

            var sp1 = new SqlParameter("@UserId", SqlDbType.Int, 11);
            sp1.Value = request.userid.Value;

            var sp2 = new SqlParameter("@SceneId", SqlDbType.Int, 11);
            sp2.Value = request.sceneid.Value;

            var sp3 = new SqlParameter("@VocabularyId", SqlDbType.Int, 11);
            sp3.Value = request.vocabularyid.Value;

            DataHelper.ExecuteNonQuery(sqlstr, sp1, sp2, sp3);
            return new H10063Response
            {
                error = 0
            };
        }

        public static HBaseResponse H10064(H10064Request request)
        {
            var sqlstr = "SELECT AppType as IKey,COUNT(1) as IValue FROM UME_app GROUP BY AppType";
            var recordlist = DataHelper.ExecuteList<KeyValueDto>(sqlstr);
            return new H10064Response
            {
                error = 0,
                data = recordlist ?? new List<KeyValueDto>()
            };
        }

        public static List<H10013ResponseListItem> H10065(int? apptype)
        {
            if (!apptype.HasValue)
            {
                throw new Exception("apptype不能为空");
            }

            var sqlstr = String.Format("SELECT * FROM UME_app WHERE AppType={0} ORDER BY Id DESC", apptype.Value);
            var list = DataHelper.ExecuteList<UME_app>(sqlstr);
            if (list != null && list.Any())
            {
                return list.Select(o => new H10013ResponseListItem
                {
                    apptype = o.AppType,
                    description = o.Description,
                    href = o.Href,
                    id = o.Id,
                    title = o.Title
                }).ToList();
            }

            return new List<H10013ResponseListItem>();
        }

        /// <summary>
        /// 粤语词典 - 保存查询无结果的字词
        /// </summary>
        /// <param name="words"></param>
        /// <returns></returns>
        public static int H10066(params string[] words)
        {
            var sb = new StringBuilder();
            foreach (var w in words)
            {
                sb.AppendFormat(" INSERT INTO CAN_noresult(ChnText,CreatedAt) VALUES(N'{0}',GETDATE());\r\n", w);
            }

            var sqlstr = sb.ToString();
            var rows = DataHelper.ExecuteNonQuery(sqlstr);
            return rows;
        }

        /// <summary>
        /// 粤语词典 - 统计
        /// </summary>
        /// <returns></returns>
        public static List<GroupItemDto> H10067()
        {
            var sb = new StringBuilder();
            sb.AppendFormat(" SELECT CAST(COUNT(1) AS DECIMAL(18,2)) AS ICount,'wordcount' AS IName FROM (SELECT DISTINCT ChnText FROM CAN_vocabulary) AS Q WHERE LEN(Q.ChnText)=1\r\n");
            sb.AppendFormat(" UNION\r\n");
            sb.AppendFormat(" SELECT CAST(COUNT(1) AS DECIMAL(18,2)) AS ICount,'termcount' AS IName FROM (SELECT DISTINCT ChnText FROM CAN_vocabulary) AS Q WHERE LEN(Q.ChnText)=2 OR LEN(Q.ChnText)=3\r\n");
            sb.AppendFormat(" UNION\r\n");
            sb.AppendFormat(" SELECT CAST(COUNT(1) AS DECIMAL(18,2)) AS ICount,'sentencecount' AS IName FROM (SELECT DISTINCT ChnText FROM CAN_vocabulary) AS Q WHERE LEN(Q.ChnText)>3\r\n");
            sb.AppendFormat(" UNION\r\n");
            sb.AppendFormat(" SELECT CAST(COUNT(1) AS DECIMAL(18,2)) AS ICount,'usercount' AS IName FROM T_user WHERE Domain={0}\r\n", (int)DomainEnum.cantonesedict);
            sb.AppendFormat(" UNION\r\n");
            sb.AppendFormat(" SELECT CAST(COUNT(1) AS DECIMAL(18,2)) AS ICount,'feedbackcount' AS IName FROM CAN_feedback WHERE DATEDIFF(DAY,GETDATE(),CreatedAt)=0\r\n");
            sb.AppendFormat(" UNION\r\n");
            sb.AppendFormat(" SELECT CAST(COUNT(1) AS DECIMAL(18,2)) AS ICount,'noresultcount' AS IName FROM CAN_noresult\r\n");

            var sqlstr = sb.ToString();
            var list = DataHelper.ExecuteList<GroupItemDto>(sqlstr);
            return list;
        }

        /// <summary>
        /// 粤语词典 - 获取词汇列表
        /// </summary>
        /// <returns></returns>
        public static H10068Response H10068(H10068Request request)
        {
            var sb = new StringBuilder();
            sb.AppendFormat(" SELECT *,ROW_NUMBER() OVER(ORDER BY Id DESC) AS RW FROM CAN_vocabulary WHERE 1=1");

            if (!string.IsNullOrEmpty(request.chntext))
            {
                sb.AppendFormat(" AND ChnText LIKE N'%{0}%'", request.chntext);
            }

            if (!string.IsNullOrEmpty(request.cantext))
            {
                sb.AppendFormat(" AND CanText=N'{0}'", request.cantext);
            }

            if (!string.IsNullOrEmpty(request.canpronounce))
            {
                sb.AppendFormat(" AND CanPronounce=N'{0}'", request.canpronounce);
            }

            if (!string.IsNullOrEmpty(request.canvoice))
            {
                sb.AppendFormat(" AND CanVoice=N'{0}'", request.canvoice);
            }

            if (request.createdat.HasValue)
            {
                sb.AppendFormat(" AND DATEDIFF(DAY,CreatedAt,'{0}')=0", request.createdat.Value.ToString("yyyy-MM-dd"));
            }

            if (request.textlength.HasValue)
            {
                sb.AppendFormat(" AND LEN(ChnText)={0}", request.textlength.Value);
            }

            var sqlstr = sb.ToString();
            var sqlstr2 = string.Format("SELECT COUNT(1) FROM ({0}) AS Q", sqlstr);
            var obj = DataHelper.ExecuteScalar(sqlstr2);
            var count = Convert.ToInt32(obj);

            var page = request.page.GetValueOrDefault(1);
            var pagesize = request.pagesize.GetValueOrDefault(10);
            var begin = pagesize * (page - 1);
            var end = pagesize * page;
            var sqlstr3 = string.Format("SELECT * FROM ({0}) AS Q WHERE Q.RW>{1} AND Q.RW<={2}", sqlstr, begin, end);
            var pagedlist = DataHelper.ExecuteList<H10068ResponseListItem>(sqlstr3);
            return new H10068Response
            {
                error = 0,
                data = new PagedListDto<H10068ResponseListItem>
                {
                    Page = page,
                    PageSize = pagesize,
                    RecordCount = count,
                    RecordList = pagedlist ?? new List<H10068ResponseListItem>()
                }
            };
        }

        /// <summary>
        /// 粤语词典 - 删除词汇
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public static int H10069(int id)
        {
            return QueryHelper.Delete<Can_vocabulary>(new Can_vocabulary
            {
                Id = id
            });
        }

        /// <summary>
        /// 粤语词典 - 添加词汇
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public static int H10070(H10070Request request)
        {
            var model = new Can_vocabulary
            {
                CanPronounce = request.canpronounce,
                CanText = request.cantext,
                CanVoice = request.canvoice,
                ChnText = request.chntext,
                CreatedAt = DateTime.Now
            };

            return QueryHelper.Save<Can_vocabulary>(model);
        }

        /// <summary>
        /// 粤语词典 - 获取词汇
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static H10071Response H10071(int id)
        {
            var sqlstr = "SELECT * FROM CAN_vocabulary WHERE Id=" + id;
            var list = DataHelper.ExecuteList<H10071ResponseListItem>(sqlstr);
            return new H10071Response
            {
                error = 0,
                data = list == null ? null : list[0]
            };
        }

        /// <summary>
        /// 粤语词典 - 更新词汇
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static int H10072(H10071ResponseListItem request)
        {
            return QueryHelper.Update<Can_vocabulary>(new Can_vocabulary
            {
                CanPronounce = request.canpronounce,
                CanText = request.cantext,
                CanVoice = request.canvoice,
                ChnText = request.chntext,
                CreatedAt = request.createdat,
                Id = request.id
            });
        }

        /// <summary>
        /// 粤语词典 - 获取情景列表
        /// </summary>
        /// <returns></returns>
        public static H10073Response H10073(H10073Request request)
        {
            var sb = new StringBuilder();
            sb.AppendFormat(" SELECT *,ROW_NUMBER() OVER(ORDER BY Id DESC) AS RW FROM CAN_scene WHERE 1=1");

            if (!string.IsNullOrEmpty(request.name))
            {
                sb.AppendFormat(" AND Name LIKE N'%{0}%'", request.name);
            }

            if (request.createdat.HasValue)
            {
                sb.AppendFormat(" AND DATEDIFF(DAY,CreatedAt,'{0}')=0", request.createdat.Value.ToString("yyyy-MM-dd"));
            }

            var sqlstr = sb.ToString();
            var sqlstr2 = string.Format("SELECT COUNT(1) FROM ({0}) AS Q", sqlstr);
            var obj = DataHelper.ExecuteScalar(sqlstr2);
            var count = Convert.ToInt32(obj);

            var page = request.page.GetValueOrDefault(1);
            var pagesize = request.pagesize.GetValueOrDefault(10);
            var begin = pagesize * (page - 1);
            var end = pagesize * page;
            var sqlstr3 = string.Format("SELECT * FROM ({0}) AS Q WHERE Q.RW>{1} AND Q.RW<={2}", sqlstr, begin, end);
            var pagedlist = DataHelper.ExecuteList<H10073ResponseListItem>(sqlstr3);
            return new H10073Response
            {
                error = 0,
                data = new PagedListDto<H10073ResponseListItem>
                {
                    Page = page,
                    PageSize = pagesize,
                    RecordCount = count,
                    RecordList = pagedlist ?? new List<H10073ResponseListItem>()
                }
            };
        }

        /// <summary>
        /// 粤语词典 - 删除情景
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public static int H10074(int id)
        {
            return QueryHelper.Delete<Can_scene>(new Can_scene
            {
                Id = id
            });
        }

        /// <summary>
        /// 粤语词典 - 添加情景
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public static int H10075(H10075Request request)
        {
            var model = new Can_scene
            {
                CreatedAt = DateTime.Now,
                Name = request.name
            };

            return QueryHelper.Save<Can_scene>(model);
        }

        /// <summary>
        /// 粤语词典 - 获取情景
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static H10076Response H10076(int id)
        {
            var sqlstr = "SELECT * FROM CAN_scene WHERE Id=" + id;
            var list = DataHelper.ExecuteList<H10076ResponseListItem>(sqlstr);
            return new H10076Response
            {
                error = 0,
                data = list == null ? null : list[0]
            };
        }

        /// <summary>
        /// 粤语词典 - 更新情景
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static int H10077(H10076ResponseListItem request)
        {
            return QueryHelper.Update<Can_scene>(new Can_scene
            {
                Name = request.name,
                CreatedAt = request.createdat,
                Id = request.id
            });
        }

        /// <summary>
        /// 获取用户列表
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public static H10078Response H10078(H10078Request request)
        {
            var sb = new StringBuilder();
            sb.AppendFormat(" SELECT *,ROW_NUMBER() OVER(ORDER BY Id DESC) AS RW FROM T_user WHERE 1=1");

            if (!string.IsNullOrEmpty(request.username))
            {
                sb.AppendFormat(" AND UserName LIKE N'%{0}%'", request.username);
            }

            if (!string.IsNullOrEmpty(request.email))
            {
                sb.AppendFormat(" AND Email = N'{0}'", request.email);
            }

            if (request.domain.HasValue)
            {
                sb.AppendFormat(" AND Domain = {0}", request.domain.Value);
            }

            if (request.status.HasValue)
            {
                sb.AppendFormat(" AND Status = {0}", request.status.Value);
            }

            if (request.createdat.HasValue)
            {
                sb.AppendFormat(" AND DATEDIFF(DAY,CreatedAt,'{0}')=0", request.createdat.Value.ToString("yyyy-MM-dd"));
            }

            var sqlstr = sb.ToString();
            var sqlstr2 = string.Format("SELECT COUNT(1) FROM ({0}) AS Q", sqlstr);
            var obj = DataHelper.ExecuteScalar(sqlstr2);
            var count = Convert.ToInt32(obj);

            var page = request.page.GetValueOrDefault(1);
            var pagesize = request.pagesize.GetValueOrDefault(10);
            var begin = pagesize * (page - 1);
            var end = pagesize * page;
            var sqlstr3 = string.Format("SELECT * FROM ({0}) AS Q WHERE Q.RW>{1} AND Q.RW<={2}", sqlstr, begin, end);
            var pagedlist = DataHelper.ExecuteList<H10078ResponseListItem>(sqlstr3);
            return new H10078Response
            {
                error = 0,
                data = new PagedListDto<H10078ResponseListItem>
                {
                    Page = page,
                    PageSize = pagesize,
                    RecordCount = count,
                    RecordList = pagedlist ?? new List<H10078ResponseListItem>()
                }
            };
        }

        /// <summary>
        /// 粤语词典 - 获取反馈列表
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public static H10079Response H10079(H10079Request request)
        {
            var sb = new StringBuilder();
            sb.AppendFormat(" SELECT *,ROW_NUMBER() OVER(ORDER BY Id DESC) AS RW FROM CAN_feedback WHERE 1=1");

            if (!string.IsNullOrEmpty(request.chntext))
            {
                sb.AppendFormat(" AND ChnText LIKE N'%{0}%'", request.chntext);
            }

            if (!string.IsNullOrEmpty(request.cantext))
            {
                sb.AppendFormat(" AND CanText = N'{0}'", request.cantext);
            }

            if (!string.IsNullOrEmpty(request.createdby))
            {
                sb.AppendFormat(" AND CreatedBy = N'{0}'", request.createdby);
            }

            if (request.status.HasValue)
            {
                sb.AppendFormat(" AND Status = {0}", request.status.Value);
            }

            if (request.createdat.HasValue)
            {
                sb.AppendFormat(" AND DATEDIFF(DAY,CreatedAt,'{0}')=0", request.createdat.Value.ToString("yyyy-MM-dd"));
            }

            var sqlstr = sb.ToString();
            var sqlstr2 = string.Format("SELECT COUNT(1) FROM ({0}) AS Q", sqlstr);
            var obj = DataHelper.ExecuteScalar(sqlstr2);
            var count = Convert.ToInt32(obj);

            var page = request.page.GetValueOrDefault(1);
            var pagesize = request.pagesize.GetValueOrDefault(10);
            var begin = pagesize * (page - 1);
            var end = pagesize * page;
            var sqlstr3 = string.Format("SELECT * FROM ({0}) AS Q WHERE Q.RW>{1} AND Q.RW<={2}", sqlstr, begin, end);
            var pagedlist = DataHelper.ExecuteList<H10079ResponseListItem>(sqlstr3);
            return new H10079Response
            {
                error = 0,
                data = new PagedListDto<H10079ResponseListItem>
                {
                    Page = page,
                    PageSize = pagesize,
                    RecordCount = count,
                    RecordList = pagedlist ?? new List<H10079ResponseListItem>()
                }
            };
        }

        /// <summary>
        /// 粤语词典 - 获取无结果列表
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public static H10080Response H10080(H10080Request request)
        {
            var sb = new StringBuilder();
            sb.AppendFormat(" SELECT *,ROW_NUMBER() OVER(ORDER BY Id DESC) AS RW FROM CAN_noresult WHERE 1=1");

            if (!string.IsNullOrEmpty(request.chntext))
            {
                sb.AppendFormat(" AND ChnText LIKE N'%{0}%'", request.chntext);
            }

            if (request.createdat.HasValue)
            {
                sb.AppendFormat(" AND DATEDIFF(DAY,CreatedAt,'{0}')=0", request.createdat.Value.ToString("yyyy-MM-dd"));
            }

            var sqlstr = sb.ToString();
            var sqlstr2 = string.Format("SELECT COUNT(1) FROM ({0}) AS Q", sqlstr);
            var obj = DataHelper.ExecuteScalar(sqlstr2);
            var count = Convert.ToInt32(obj);

            var page = request.page.GetValueOrDefault(1);
            var pagesize = request.pagesize.GetValueOrDefault(10);
            var begin = pagesize * (page - 1);
            var end = pagesize * page;
            var sqlstr3 = string.Format("SELECT * FROM ({0}) AS Q WHERE Q.RW>{1} AND Q.RW<={2}", sqlstr, begin, end);
            var pagedlist = DataHelper.ExecuteList<H10080ResponseListItem>(sqlstr3);
            return new H10080Response
            {
                error = 0,
                data = new PagedListDto<H10080ResponseListItem>
                {
                    Page = page,
                    PageSize = pagesize,
                    RecordCount = count,
                    RecordList = pagedlist ?? new List<H10080ResponseListItem>()
                }
            };
        }

        /// <summary>
        /// 粤语词典 - 删除反馈
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public static int H10081(int id)
        {
            return QueryHelper.Delete<Can_feedback>(new Can_feedback
            {
                Id = id
            });
        }

        /// <summary>
        /// 粤语词典 - 删除无结果
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public static int H10082(int id)
        {
            return QueryHelper.Delete<Can_noresult>(new Can_noresult
            {
                Id = id
            });
        }

        /// <summary>
        /// 粤语词典 - 添加情景词汇
        /// </summary>
        /// <param name="sceneid"></param>
        /// <param name="vocabularyid"></param>
        /// <returns></returns>
        public static int H10083(int sceneid, int vocabularyid)
        {
            if (sceneid <= 0 || vocabularyid <= 0)
            {
                return -1;
            }

            var sqlstr = string.Format("INSERT INTO CAN_scenewordrelation(SceneId,VocabularyId,CreatedAt) VALUES({0},{1},GETDATE())", sceneid, vocabularyid);
            var rows = DataHelper.ExecuteNonQuery(sqlstr);
            return rows;
        }

        /// <summary>
        /// 粤语词典 - 获取情景词汇列表
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public static H10084Response H10084(H10084Request request)
        {
            var sb = new StringBuilder();
            sb.AppendFormat(" SELECT t1.Id,t1.VocabularyId,t2.ChnText,ROW_NUMBER() OVER(ORDER BY t1.Id DESC) AS RW FROM CAN_scenewordrelation t1 LEFT JOIN CAN_vocabulary t2 on t2.Id=t1.VocabularyId  WHERE 1=1");

            if (request.categoryid.HasValue)
            {
                sb.AppendFormat(" AND t1.sceneid ={0}", request.categoryid.Value);
            }

            if (string.IsNullOrEmpty(request.chntext))
            {
                sb.AppendFormat(" AND t2.ChnText LIKE '%{0}%'", request.chntext);
            }

            var sqlstr = sb.ToString();
            var sqlstr2 = string.Format("SELECT COUNT(1) FROM ({0}) AS Q", sqlstr);
            var obj = DataHelper.ExecuteScalar(sqlstr2);
            var count = Convert.ToInt32(obj);

            var page = request.page.GetValueOrDefault(1);
            var pagesize = request.pagesize.GetValueOrDefault(10);
            var begin = pagesize * (page - 1);
            var end = pagesize * page;
            var sqlstr3 = string.Format("SELECT * FROM ({0}) AS Q WHERE Q.RW>{1} AND Q.RW<={2}", sqlstr, begin, end);
            var pagedlist = DataHelper.ExecuteList<H10084ResponseListItem>(sqlstr3);
            return new H10084Response
            {
                error = 0,
                data = new PagedListDto<H10084ResponseListItem>
                {
                    Page = page,
                    PageSize = pagesize,
                    RecordCount = count,
                    RecordList = pagedlist ?? new List<H10084ResponseListItem>()
                }
            };
        }

        /// <summary>
        /// 粤语词典 - 获取情景词汇详细
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static H10084ResponseListItem H10085(int id)
        {
            var sqlstr = "SELECT t1.Id,t1.VocabularyId,t2.ChnText FROM CAN_scenewordrelation t1 LEFT JOIN CAN_vocabulary t2 on t2.Id=t1.VocabularyId WHERE t1.Id=" + id;
            var list = DataHelper.ExecuteList<H10084ResponseListItem>(sqlstr);
            return list.FirstOrDefault();
        }

        /// <summary>
        /// 粤语词典 - 修改情景词汇
        /// </summary>
        /// <param name="vocabularyid"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public static int H10086(int vocabularyid, int id)
        {
            if (id <= 0 || vocabularyid <= 0)
            {
                return -1;
            }

            var sqlstr = string.Format("UPDATE CAN_scenewordrelation SET VocabularyId={1} WHERE Id={0}", id, vocabularyid);
            var rows = DataHelper.ExecuteNonQuery(sqlstr);
            return rows;
        }

        /// <summary>
        /// 粤语词典 - 删除情景词汇
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public static int H10087(int id)
        {
            return QueryHelper.Delete<Can_scenewordrelation>(new Can_scenewordrelation
            {
                Id = id
            });
        }

        /// <summary>
        /// 获取词汇id
        /// </summary>
        /// <param name="chntext"></param>
        /// <returns></returns>
        public static GroupItemDto H10088(string chntext)
        {
            var sqlstr = "SELECT CAST(Id AS NVARCHAR(50)) AS IName,1 AS ICount FROM CAN_vocabulary WHERE ChnText=@ChnText";
            var sp = new SqlParameter("@ChnText", SqlDbType.NVarChar, 50);
            sp.IsNullable = true;
            if (string.IsNullOrEmpty(chntext))
            {
                sp.Value = DBNull.Value;
            }
            else
            {
                sp.Value = chntext;
            }

            var list = DataHelper.ExecuteList<GroupItemDto>(sqlstr, sp);
            return list.FirstOrDefault();
        }
    }
}

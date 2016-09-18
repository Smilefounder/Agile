using Agile.API.Dtos;
using Agile.API.Helpers;
using Agile.Data.Helpers;
using Agile.Dtos;
using Agile.Helpers;
using Agile.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace buy.uimoe.com.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult GetIndex(int? page, int? pagesize)
        {
            var vm = new PagedListDto<H10100ResponseListItem>
            {
                RecordList = new List<H10100ResponseListItem>()
            };

            try
            {
                var options = new PagedQueryOptions
                {
                    Page = page.GetValueOrDefault(1),
                    PageSize = pagesize.GetValueOrDefault(10),
                };

                var pagedlist = QueryHelper.GetPagedList<UME_ad>(options);
                vm = new PagedListDto<H10100ResponseListItem>
                {
                    Page = pagedlist.Page,
                    PageSize = pagedlist.PageSize,
                    RecordCount = pagedlist.RecordCount,
                    RecordList = pagedlist.RecordList.Select(o => new H10100ResponseListItem
                    {
                        Cover = o.Cover,
                        Price = o.Price.GetValueOrDefault(),
                        Rate = o.Rate.GetValueOrDefault(),
                        Title = o.Title,
                        Url = o.Url
                    }).ToList()
                };
            }
            catch (Exception ex)
            {
                LogHelper.WriteAsync(ex.ToString());
            }

            return View(vm);
        }

        [HttpGet]
        public ActionResult NewBuy()
        {
            return View();
        }

        [HttpPost]
        public ActionResult NewBuy(string title, string url, string imgfile, decimal? price, decimal? rate)
        {
            try
            {
                var folder1 = Server.MapPath("~/Uploads");
                var file1 = System.IO.Path.Combine(new string[] { folder1, imgfile });
                if (!System.IO.File.Exists(file1))
                {
                    return Json(new { error = 1, message = "找不到文件：" + imgfile });
                }

                //复制到广告目录下
                var folder2 = Server.MapPath("~/Content/ad");
                var file2 = System.IO.Path.Combine(new string[] { folder2, imgfile });
                System.IO.File.Copy(file1, file2, true);

                var response = LogicHelper.H10101(title, url, imgfile, price, rate);
                return Json(response);
            }
            catch (Exception ex)
            {
                LogHelper.WriteAsync(ex.ToString());
            }

            return Json(new { error = 1, message = "操作失败，请稍后再试" });
        }

        [HttpPost]
        public ActionResult UpdateFile()
        {
            var file1 = Request.Files["file1"];
            if (file1 == null)
            {
                return Json(new { error = 1, message = "未接收到文件" });
            }

            var extlist = new string[] { ".jpg", ".png" };
            var ext = System.IO.Path.GetExtension(file1.FileName);
            if (!extlist.Contains(ext.ToLower()))
            {
                return Json(new { error = 1, message = String.Format("不支持的文件格式：{0},请上传以下格式的文件：{1}", ext, String.Join(",", extlist)) });
            }

            var kbsize = 1.0 * file1.ContentLength / 1024;
            if (kbsize > 320)
            {
                return Json(new { error = 1, message = String.Format("最多只能上传320KB大小的图片,当前文件大小：{0}", kbsize) });
            }

            var folder = Server.MapPath("~/Uploads");
            if (!System.IO.Directory.Exists(folder))
            {
                try
                {
                    System.IO.Directory.CreateDirectory(folder);
                }
                catch
                {
                    return Json(new { error = 1, message = "保存文件失败了" });
                }
            }

            var filename = String.Format("{0}{1}", Guid.NewGuid().ToString(), ext);
            var fullname = System.IO.Path.Combine(new string[] { folder, filename });

            file1.SaveAs(fullname);
            return Json(new { error = 0, data = filename });
        }
    }
}
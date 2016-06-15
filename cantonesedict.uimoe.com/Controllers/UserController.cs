using Agile.Dtos.API;
using Agile.Helpers;
using Agile.Helpers.API;
using cantonesedict.uimoe.com.ViewModels.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace cantonesedict.uimoe.com.Controllers
{
    public class UserController : Controller
    {
        public ActionResult ScoreList()
        {
            var userid = default(int);
            var useridstr = Session["userid"] as string;
            if (int.TryParse(useridstr, out userid) == false)
            {
                return View(new List<ScoreListItemVM>());
            }

            try
            {
                var request = ReflectHelper.ParseFromRequest<H10039Request>();
                request.userid = userid;

                var responsebase = LogicHelper.H10039(request);
                var response = responsebase as H10039Response;
                if (response != null &&
                    response.error == 0 &&
                    response.data != null &&
                    response.data.Count > 0)
                {
                    var recordlist = response.data.Select(o => new ScoreListItemVM
                    {
                        createdat = o.createdat,
                        score = o.score,
                        way = o.way
                    }).ToList();

                    return View(recordlist);
                }
            }
            catch (Exception ex)
            {
                LogHelper.Write(ex.ToString());
            }

            return View(new List<ScoreListItemVM>());
        }
    }
}

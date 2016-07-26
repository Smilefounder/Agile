using Agile.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace cantonesedict.uimoe.com.Controllers
{
    [FreeAccess]
    public class ReimuController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Statistics()
        {
            return View();
        }

        public ActionResult Vocabulary()
        {
            return View();
        }

        public ActionResult AddVocabulary()
        {
            return View();
        }

        public ActionResult Category()
        {
            return View();
        }

        public ActionResult AddCategory()
        {
            return View();
        }

        public ActionResult UserList()
        {
            return View();
        }
    }
}

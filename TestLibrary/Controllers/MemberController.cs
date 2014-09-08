using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace TestLibrary.Controllers
{
    public class MemberController : Controller
    {
        [Authorize]
        public ActionResult Index()
        {
            Session["LoginUser"] = HttpContext.User.Identity.Name;
            if (HttpContext.User.Identity.Name.ToString().Substring(0, 2) == "M_")
                return View();
            else
                return RedirectToAction("Index", "Manage");
        }

        [Authorize]
        public ActionResult Detail()
        {
            Session["LoginUser"] = HttpContext.User.Identity.Name;
            if (HttpContext.User.Identity.Name.ToString().Substring(0, 2) == "M_")
                return View();
            else
                return RedirectToAction("Index", "Manage");
        }



    }
}

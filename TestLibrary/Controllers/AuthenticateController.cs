using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
namespace TestLibrary.Controllers
{
    public class AuthenticateController : Controller
    {
        
        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            Session["LoginUser"] = null;
            return RedirectToAction("Index","Home");
        }

    }
}

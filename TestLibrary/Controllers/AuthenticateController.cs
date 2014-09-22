using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using System.Data.Common;
using TestLibrary.Models;
using TestLibrary.DataAccess;
namespace TestLibrary.Controllers
{
    public class AuthenticateController : Controller
    {
        LibraryContext db = new LibraryContext();
        public ActionResult Login()
        {
            if (HttpContext.User.Identity.IsAuthenticated)
            {
                Session["LoginUser"] = HttpContext.User.Identity.Name;
                return RedirectToAction("Index", "Home");
            }
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(string UserName, string Password, bool remember)
        {
            
                Person loginUser;
                loginUser = db.Members.SingleOrDefault(target => target.UserName == UserName && target.Password == Password);
                if (loginUser != null)
                {
                    FormsAuthentication.SetAuthCookie("M_" + UserName, remember);
                    Session["LoginUser"] = "M_" + UserName;
                    return RedirectToAction("Index", "Account");
                }
                else
                {
                    loginUser = db.Admins.SingleOrDefault(target => target.UserName == UserName && target.Password == Password);
                    if (loginUser != null)
                    {
                        FormsAuthentication.SetAuthCookie("A_" + UserName, remember);
                        Session["LoginUser"] = "A_" + UserName;
                        return RedirectToAction("Index", "Account");
                    }
                }
                return View();
        }

        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            Session["LoginUser"] = null;
            return RedirectToAction("Index","Home");
        }

        public ActionResult Register()
        {
                if (HttpContext.User.Identity.IsAuthenticated)
                {
                    Session["LoginUser"] = HttpContext.User.Identity.Name;
                    return RedirectToAction("Index","Home");
                }
                return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Register(Member regist, string ConfirmPassword)
        {
            if (ModelState.IsValid)
            {
                if ((db.Members.Where(target => target.UserName == (regist.UserName)).SingleOrDefault() == null) &&
                    (db.Admins.Where(target => target.UserName == (regist.UserName)).SingleOrDefault() == null))
                {
                    if (regist.Password == ConfirmPassword)
                    {
                        db.Members.Add(regist);
                        db.SaveChanges();
                        TempData["Notification"] = "Register successful,please login for first use.";
                        return RedirectToAction("Login");
                    }
                    else
                    {
                        TempData["Notification"] = "Password did not match.";
                        return View(regist);
                    }
                }
                else
                {
                    TempData["Notification"] = "This user name is already exists.";
                    return View(regist);
                }
            }
            else
                return View(regist);
        }

        public ActionResult ForgotPassword()
        {
            if (HttpContext.User.Identity.IsAuthenticated)
            {
                Session["LoginUser"] = HttpContext.User.Identity.Name;
                return RedirectToAction("Index", "Home");
            }
            else
                return View();
        }


        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}

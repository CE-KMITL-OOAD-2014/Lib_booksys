using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using TestLibrary.DataAccess;
using TestLibrary.Models;
namespace TestLibrary.Controllers
{
    public class HomeController : Controller
    {
        LibraryContext db = new LibraryContext();
        public ActionResult Index()
        {
            if (HttpContext.User.Identity.IsAuthenticated)
                Session["LoginUser"] = HttpContext.User.Identity.Name;
            return View(db.NewsList.ToList().OrderByDescending(news => news.PostTime));
        }

        public ActionResult About()
        {
            if (HttpContext.User.Identity.IsAuthenticated)
                Session["LoginUser"] = HttpContext.User.Identity.Name;
            return View();
        }

        public ActionResult Contact()
        {
            if (HttpContext.User.Identity.IsAuthenticated)
                Session["LoginUser"] = HttpContext.User.Identity.Name;
            return View();
        }
        public ActionResult ChangeLog()
        {
            if (HttpContext.User.Identity.IsAuthenticated)
                Session["LoginUser"] = HttpContext.User.Identity.Name;
            return View();
        }

        public ActionResult Login()
        {

            if (Session["LoginUser"] == null)
            {
                if (HttpContext.User.Identity.IsAuthenticated)
                {
                    Session["LoginUser"] = HttpContext.User.Identity.Name;
                    return RedirectToAction("Index");
                }
                return View();
            }


            return RedirectToAction("Index");
        }

      


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(string UserName,string Password, bool remember)
        {
            Person loginUser;
            loginUser = db.Members.SingleOrDefault(target => target.UserName == UserName && target.Password == Password);
                if(loginUser != null){
                FormsAuthentication.SetAuthCookie("M_" + UserName, remember);
                Session["LoginUser"] = "M_" + UserName;
                }
                else { 
                loginUser = db.Admins.SingleOrDefault(target => target.UserName == UserName && target.Password == Password);
                FormsAuthentication.SetAuthCookie("A_" + UserName, remember);
                Session["LoginUser"] = "A_" + UserName;
            }
                return RedirectToAction("Index", "Account");
        }

        

        public ActionResult Register()
        {
            if (Session["LoginUser"] == null)
            {
                if (HttpContext.User.Identity.IsAuthenticated)
                {
                    Session["LoginUser"] = HttpContext.User.Identity.Name;
                    return RedirectToAction("Index");
                }
                return View();
            }
            return RedirectToAction("Index");
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
        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TestLibrary.Models;
using System.Data.Entity;
using TestLibrary.DataAccess;
using System.Web.Security;
namespace TestLibrary.Controllers
{
    public class AccountController : Controller
    {
       
        LibraryContext db = new LibraryContext();
        public ActionResult Login()
        {
            if (Session["LoginUser"] == null)
            {
                if (HttpContext.User.Identity.IsAuthenticated)
                {
                    Session["LoginUser"] = HttpContext.User.Identity.Name;
                    return RedirectToAction("Index");
                }
                /*Admin a = new Admin();
                a.Email = "benchbb01@hotmail.com";
                a.password = "std22360";
                a.UserName = "ParatabAdmin";
                a.Name = "ParatabAdmin";
                db.Admins.Add(a);
                db.SaveChanges();*/
                return View();
            }
            

            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(Admin admin,bool remember)
        {
            ModelState.Remove("Name");
            ModelState.Remove("Email");
            if(ModelState.IsValid){
                Admin a = db.Admins.Where(s => s.UserName == admin.UserName).SingleOrDefault();
            if(a != null)
            {
                if (a.password == admin.password)
                {
                    
                    FormsAuthentication.SetAuthCookie(a.UserName,remember);
                    Session["LoginUser"] = a.UserName;
                    return RedirectToAction("Index");
                }
            }
            }
            return View(); 
        }
        public ActionResult Register()
        {
            if (Session["LoginUser"] == null){
                if(HttpContext.User.Identity.IsAuthenticated){
                    Session["LoginUser"] = HttpContext.User.Identity.Name;
                    return RedirectToAction("Index");
                }
                return View();
            }
            return RedirectToAction("Index");
        }
        

        [Authorize]
        public ActionResult Index()
        {
            return View();
        }

        [Authorize]
        public ActionResult ChangePassword()
        {
            Admin a = db.Admins.Where(s => s.UserName == HttpContext.User.Identity.Name).SingleOrDefault();
            return View(a);
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ChangePassword(Admin admin,string oldpass,string confirm)
        {
            if (ModelState.IsValid)
            {
                Admin a = db.Admins.Where(s => s.Id == admin.Id).SingleOrDefault();
               if (a.password == oldpass && admin.password == confirm)
                {
                    a.password = admin.password;
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
                else
                    return View();
            }
            return View();
        }
        public ActionResult Logout()
        {
            
            FormsAuthentication.SignOut();
            Session["LoginUser"] = null;
            return RedirectToAction("Login");
            
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}

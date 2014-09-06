using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TestLibrary.Models;
using System.Data.Entity;
using TestLibrary.DataAccess;
using System.Web.Security;
using System.Data.Entity.Validation;
using System.ComponentModel;
namespace TestLibrary.Controllers
{
    public class ManageController : Controller
    {
       
        LibraryContext db = new LibraryContext();

        [Authorize]
        public ActionResult Index()
        {
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
               /* Admin a = new Admin();
                a.Name = "Paratab";
                a.UserName = "ParatabAdmin";
                a.Password = "surawit";
                a.Email = "b@hotmail.com";
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
                if (a.Password == admin.Password)
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
        public ActionResult Detail()
        {
            Session["LoginUser"] = HttpContext.User.Identity.Name;
            Admin a = db.Admins.Where(s => s.UserName == HttpContext.User.Identity.Name).SingleOrDefault();
            return View(a);
        }

        [Authorize]
        public ActionResult Edit()
        {
            Session["LoginUser"] = HttpContext.User.Identity.Name;
            Admin a = db.Admins.Where(s => s.UserName == HttpContext.User.Identity.Name).SingleOrDefault();
            return View(a);
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Admin admin)
        {
            if (ModelState.IsValid)
            {
                db.Entry(admin).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Detail");
            }
            return View(admin);
        }
        [Authorize]
        public ActionResult ChangePassword()
        {
            Admin a = db.Admins.Where(s => s.UserName == HttpContext.User.Identity.Name).SingleOrDefault();
            return View(a);
        }

        [Authorize]
        public ActionResult AddAdmin()
        {
            Session["LoginUser"] = HttpContext.User.Identity.Name;
            return View();
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddAdmin(Admin admin)
        {
            
            if (ModelState.IsValid)
            {
                if (db.Admins.Where(s => s.UserName == admin.UserName).SingleOrDefault() == null)
                {
                    db.Entry(admin).State = EntityState.Added;
                    db.SaveChanges();
                    TempData["Notification"] = "Add admin " + admin.UserName + " successfully.";
                    return RedirectToAction("AdminList");
                }
                ModelState.AddModelError("UserName","This username is already exists.");
            }
            return View(admin);
        }

        [Authorize]
        public ActionResult Delete([DefaultValue(0)] int id)
        {
            Session["LoginUser"] = HttpContext.User.Identity.Name;
            Admin target = db.Admins.SingleOrDefault(targetadmin => targetadmin.Id == id);
            if (target == null)
                return HttpNotFound();
            return View(target);
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id,string answer)
        {
            if (answer == "Yes")
            {
                Admin admintodelete = db.Admins.Find(id);
                TempData["Notification"] = "Delete " + admintodelete.UserName + " successfully.";
                db.Admins.Remove(admintodelete);
                db.SaveChanges();
            }
                return RedirectToAction("AdminList");
                
        }

        [Authorize]
        public ActionResult AdminList()
        {
            Session["loginUser"] = HttpContext.User.Identity.Name;
            return View(db.Admins.ToList());
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ChangePassword(Admin admin,string oldpass,string confirm)
        {
            if (ModelState.IsValid)
            {
                Admin a = db.Admins.Where(s => s.Id == admin.Id).SingleOrDefault();
               if (a.Password == oldpass && admin.Password == confirm)
                {
                    a.Password = admin.Password;
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

        [Authorize]
        public ActionResult AddBook()
        {
            Session["LoginUser"] = HttpContext.User.Identity.Name;
            return View();
        }

        [Authorize]
        [ValidateAntiForgeryToken]
        [HttpPost]
        public ActionResult AddBook(Book bookToAdd)
        {
            if (ModelState.IsValid)
            {
                db.Entry(bookToAdd).State = EntityState.Added;
                db.SaveChanges();
                TempData["Notification"] = "Add new book successfully.";
                return RedirectToAction("BookList");
            }
            return View(bookToAdd);
        }

        [Authorize]
        public ActionResult BookList()
        {
            Session["LoginUser"] = HttpContext.User.Identity.Name;
            return View(db.Books.ToList());
        }
        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}

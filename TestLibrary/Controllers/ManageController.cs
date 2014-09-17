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
            Session["LoginUser"] = HttpContext.User.Identity.Name;
            if (HttpContext.User.Identity.Name.ToString().Substring(0, 2) != "A_")
                return RedirectToAction("Index", "Member");
            return View();
        }

        [Authorize]
        public ActionResult Detail()
        {
            Session["LoginUser"] = HttpContext.User.Identity.Name;
            if (HttpContext.User.Identity.Name.ToString().Substring(0, 2) != "A_")
                return RedirectToAction("Index", "Member");
            Admin a = db.Admins.Where(s => s.UserName == HttpContext.User.Identity.Name.ToString().Substring(2)).SingleOrDefault();
            return View(a);
        }

        [Authorize]
        public ActionResult Edit()
        {
            Session["LoginUser"] = HttpContext.User.Identity.Name;
            if (HttpContext.User.Identity.Name.ToString().Substring(0, 2) != "A_")
                return RedirectToAction("Index", "Member");
            Admin a = db.Admins.Where(s => s.UserName == HttpContext.User.Identity.Name.ToString().Substring(2)).SingleOrDefault();
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
            Session["LoginUser"] = HttpContext.User.Identity.Name;
            if (HttpContext.User.Identity.Name.ToString().Substring(0, 2) != "A_")
                return RedirectToAction("Index", "Member");
            Admin a = db.Admins.Where(s => s.UserName == HttpContext.User.Identity.Name.ToString().Substring(2)).SingleOrDefault();
            return View(a);
        }

        [Authorize]
        public ActionResult AddAdmin()
        {
            Session["LoginUser"] = HttpContext.User.Identity.Name;
            if (HttpContext.User.Identity.Name.ToString().Substring(0, 2) != "A_")
                return RedirectToAction("Index", "Member");
            return View();
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddAdmin(Admin admin)
        {
            
            if (ModelState.IsValid)
            {
                if ((db.Admins.Where(s => s.UserName == admin.UserName).SingleOrDefault() == null)&&
                    (db.Members.Where(m => m.UserName == admin.UserName).SingleOrDefault() == null))
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
            if (HttpContext.User.Identity.Name.ToString().Substring(0, 2) != "A_")
                return RedirectToAction("Index", "Member");
            Admin target = db.Admins.SingleOrDefault(targetadmin => targetadmin.UserID == id);
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
            if (HttpContext.User.Identity.Name.ToString().Substring(0, 2) != "A_")
                return RedirectToAction("Index", "Member");
            return View(db.Admins.ToList());
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ChangePassword(Admin admin,string oldpass,string confirm)
        {
            if (ModelState.IsValid)
            {
                Admin a = db.Admins.Where(s => s.UserID == admin.UserID).SingleOrDefault();
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
        

        [Authorize]
        public ActionResult BorrowTracking()
        {
            Session["LoginUser"] = HttpContext.User.Identity.Name;
            if (HttpContext.User.Identity.Name.ToString().Substring(0, 2) != "A_")
                return RedirectToAction("Index", "Member");
            return View(db.BorrowList.Include(list => list.BorrowBook).Include(list => list.Borrower).ToList());
        }


        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}

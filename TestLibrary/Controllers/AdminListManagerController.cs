using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.ComponentModel;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using TestLibrary.DataAccess;
using TestLibrary.Models;
namespace TestLibrary.Controllers
{
    public class AdminListManagerController : Controller
    {
        LibraryContext db = new LibraryContext();
        
        [Authorize]
        public ActionResult Index()
        {
            Session["LoginUser"] = HttpContext.User.Identity.Name;
            if (HttpContext.User.Identity.Name.ToString().Substring(0, 2) != "A_")
                return RedirectToAction("Index", "Account");
            return View(db.Admins.ToList());
        }

        [Authorize]
        public ActionResult AddAdmin()
        {
            Session["LoginUser"] = HttpContext.User.Identity.Name;
            if (HttpContext.User.Identity.Name.ToString().Substring(0, 2) != "A_")
                return RedirectToAction("Index", "Account");
            return View();
        }

        [Authorize]
        [ValidateAntiForgeryToken]
        [HttpPost]
        public ActionResult AddAdmin(Admin newAdmin)
        {
            if (ModelState.IsValid)
            {
                if((db.Members.SingleOrDefault(target => target.UserName == newAdmin.UserName)==null)&&
                (db.Admins.SingleOrDefault(target => target.UserName == newAdmin.UserName) == null))
                {
                    db.Admins.Add(newAdmin);
                    db.SaveChanges();
                    TempData["Notification"] = "Add admin " + newAdmin.UserName + " successfully.";
                    return RedirectToAction("Index");
                }
                else
                {
                    TempData["Notification"] = "This username is already exists.";
                }
            }
            return View(newAdmin);
        }
        [Authorize]
        public ActionResult View([DefaultValue(0)]int id)
        {
                Session["LoginUser"] = HttpContext.User.Identity.Name;
                if (HttpContext.User.Identity.Name.ToString().Substring(0, 2) != "A_")
                    return RedirectToAction("Index", "Account");
                Admin target = db.Admins.Find(id);
                if (target != null)
                    return View(target);
                TempData["Notification"] = "Please specified correct Admin ID.";
                return RedirectToAction("Index");
        }

        [Authorize]
        public ActionResult Delete([DefaultValue(0)]int id)
        {
            Session["LoginUser"] = HttpContext.User.Identity.Name;
            if (HttpContext.User.Identity.Name.ToString().Substring(0, 2) != "A_")
                return RedirectToAction("Index", "Account");
            Admin target = db.Admins.Find(id);
            if (target != null)
                return View(target);
            TempData["Notification"] = "Please specified correct Admin ID.";
            return RedirectToAction("Index");
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(Admin target,string answer)
        {
            if (answer == "Yes" && ModelState.IsValid)
            {
                try
                {
                    db.Entry(target).State = EntityState.Deleted;
                    db.SaveChanges();
                    TempData["Notification"] = "Delete admin " + target.UserName + " successfully.";
                    return RedirectToAction("Index");
                }
                catch (DbUpdateException)
                {
                    TempData["Notification"] = "DbUpdateException!!!";
                    return RedirectToAction("Index");
                }
            }
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }

    }
}

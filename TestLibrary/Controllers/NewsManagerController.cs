using System;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity;
using TestLibrary.Models;
using TestLibrary.DataAccess;

namespace TestLibrary.Controllers
{
    public class NewsManagerController : Controller
    {
        LibraryContext db = new LibraryContext();

        [Authorize]
        public ActionResult NewsList()
        {
            Session["LoginUser"] = HttpContext.User.Identity.Name;
            if (HttpContext.User.Identity.Name.ToString().Substring(0, 2) != "A_")
                return RedirectToAction("Index", "Account");
            return View(db.NewsList.ToList());
        }
        
        [Authorize]
        public ActionResult AddNews()
        {
            Session["LoginUser"] = HttpContext.User.Identity.Name;
            if (HttpContext.User.Identity.Name.ToString().Substring(0, 2) != "A_")
                return RedirectToAction("Index", "Account");
            return View();
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddNews(News newstoadd)
        {
            if (ModelState.IsValid)
            {
                newstoadd.PostTime = DateTime.Now;
                db.Entry(newstoadd).State = EntityState.Added;
                db.SaveChanges();
                TempData["Notification"] = "Add news successfully.";
                return RedirectToAction("NewsList");
            }
            return View(newstoadd);
        }


        [Authorize]
        public ActionResult EditNews([DefaultValue(0)]int id)
        {
            Session["LoginUser"] = HttpContext.User.Identity.Name;
            if (HttpContext.User.Identity.Name.ToString().Substring(0, 2) != "A_")
                return RedirectToAction("Index", "Account");
            News newstoedit = db.NewsList.Find(id);
            if (newstoedit != null)
                return View(newstoedit);
            return HttpNotFound();

        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditNews(News newstoedit)
        {
            if (ModelState.IsValid)
            {
                db.Entry(newstoedit).State = EntityState.Modified;
                db.SaveChanges();
                TempData["Notification"] = "Edit news successfully.";
                return RedirectToAction("NewsList");
            }
            return View(newstoedit);
        }


        [Authorize]
        public ActionResult DeleteNews(int id)
        {
            Session["LoginUser"] = HttpContext.User.Identity.Name;
            if (HttpContext.User.Identity.Name.ToString().Substring(0, 2) != "A_")
                return RedirectToAction("Index", "Account");
            News newstodelete = db.NewsList.Find(id);
            if (newstodelete != null)
                return View(newstodelete);
            return HttpNotFound();
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteNews(News newstodelete, string answer)
        {
            if (answer == "Yes")
            {
                db.Entry(newstodelete).State = EntityState.Deleted;
                TempData["Notification"] = "Delete news successfully.";
                db.SaveChanges();
                return RedirectToAction("NewsList");
            }
            return RedirectToAction("NewsList");
        }


        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}

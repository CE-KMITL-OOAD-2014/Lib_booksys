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

        [Authorize]
        public ActionResult Detail()
        {
            Session["LoginUser"] = HttpContext.User.Identity.Name;
            Admin a = db.Admins.Where(s => s.UserName == HttpContext.User.Identity.Name.ToString().Substring(2)).SingleOrDefault();
            return View(a);
        }

        [Authorize]
        public ActionResult Edit()
        {
            Session["LoginUser"] = HttpContext.User.Identity.Name;
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
            Admin a = db.Admins.Where(s => s.UserName == HttpContext.User.Identity.Name.ToString().Substring(2)).SingleOrDefault();
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

        [Authorize]
        public ActionResult ViewBook([DefaultValue(0)]int id)
        {
            Session["LoginUser"] = HttpContext.User.Identity.Name;
            Book booktoview = db.Books.Find(id);
            if (booktoview == null)
                return HttpNotFound();
            else
                return View(booktoview);
        }

        [Authorize]
        public ActionResult EditBook([DefaultValue(0)]int id)
        {
            Session["LoginUser"] = HttpContext.User.Identity.Name;
            Book booktoedit = db.Books.Find(id);
            if (booktoedit == null)
                return HttpNotFound();
            return View(booktoedit);
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Editbook(Book booktoedit)
        {
            if (ModelState.IsValid)
            {
                db.Entry(booktoedit).State = EntityState.Modified;
                TempData["Notification"] = "Edit book successfully.";
                db.SaveChanges();
                return RedirectToAction("BookList");
            }
            return View(booktoedit);
        }



        [Authorize]
        public ActionResult DeleteBook([DefaultValue(0)]int id){
            Session["LoginUser"] = HttpContext.User.Identity.Name;
            Book booktodelete = db.Books.Find(id);
            if (booktodelete == null)
                return HttpNotFound();
            return View(booktodelete);
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteBook(Book booktodelete,string answer)
        {
            if (answer == "Yes")
            {
                TempData["Notification"] = "Delete " + booktodelete.BookName + " successfully.";
                db.Entry(booktodelete).State = EntityState.Deleted;
                db.SaveChanges();
                return RedirectToAction("BookList");
            }
            return RedirectToAction("BookList");
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}

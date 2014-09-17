using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TestLibrary.Models;
using TestLibrary.DataAccess;
using System.Data.Entity;
using System.ComponentModel;
namespace TestLibrary.Controllers
{
    public class BookManagerController : Controller
    {
        LibraryContext db = new LibraryContext();

        [Authorize]
        public ActionResult BookList()
        {
            Session["LoginUser"] = HttpContext.User.Identity.Name;
            if (HttpContext.User.Identity.Name.ToString().Substring(0, 2) != "A_")
                return RedirectToAction("Index", "Member");
            return View(db.Books.ToList());
        }


        //Will edit later
        [Authorize]
        public ActionResult ViewBook([DefaultValue(0)]int id)
        {
            Session["LoginUser"] = HttpContext.User.Identity.Name;
            if (HttpContext.User.Identity.Name.ToString().Substring(0, 2) != "A_")
                return RedirectToAction("Index", "Member");
            Book booktoview = db.Books.Find(id);
            if (booktoview == null)
                return HttpNotFound();
            else
                return View(booktoview);
        }


        [Authorize]
        public ActionResult AddBook()
        {
            Session["LoginUser"] = HttpContext.User.Identity.Name;
            if (HttpContext.User.Identity.Name.ToString().Substring(0, 2) != "A_")
                return RedirectToAction("Index", "Member");
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
        public ActionResult EditBook([DefaultValue(0)]int id)
        {
            Session["LoginUser"] = HttpContext.User.Identity.Name;
            if (HttpContext.User.Identity.Name.ToString().Substring(0, 2) != "A_")
                return RedirectToAction("Index", "Member");
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
        public ActionResult DeleteBook([DefaultValue(0)]int id)
        {
            Session["LoginUser"] = HttpContext.User.Identity.Name;
            if (HttpContext.User.Identity.Name.ToString().Substring(0, 2) != "A_")
                return RedirectToAction("Index", "Member");
            Book booktodelete = db.Books.Find(id);
            if (booktodelete == null)
                return HttpNotFound();
            return View(booktodelete);
        }


        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteBook(Book booktodelete, string answer)
        {
            if (answer == "Yes")
            {
                TempData["Notification"] = "Delete " + booktodelete.BookName + " successfully.";
                List<RequestEntry> removeEntry = db.RequestList.Where(target => target.RequestBook.BookID == booktodelete.BookID).ToList();
                List<BorrowEntry> removeBorrowEntry = db.BorrowList.Where(target => target.BorrowBook.BookID == booktodelete.BookID).ToList();
                db.RequestList.RemoveRange(removeEntry);
                db.BorrowList.RemoveRange(removeBorrowEntry);
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

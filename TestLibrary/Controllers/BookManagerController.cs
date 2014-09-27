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
        LibraryRepository libRepo = new LibraryRepository();

        [Authorize]
        public ActionResult Index()
        {
            Session["LoginUser"] = HttpContext.User.Identity.Name;
            if (HttpContext.User.Identity.Name.ToString().Substring(0, 2) != "A_")
                return RedirectToAction("Index", "Account");
            return View(libRepo.BookRepo.List());
        }


        //Will edit later
        [Authorize]
        public ActionResult ViewBook([DefaultValue(0)]int id)
        {
            Session["LoginUser"] = HttpContext.User.Identity.Name;
            if (HttpContext.User.Identity.Name.ToString().Substring(0, 2) != "A_")
                return RedirectToAction("Index", "Account");
            Book booktoview = libRepo.BookRepo.Find(id);
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
                return RedirectToAction("Index", "Account");
            return View();
        }

        [Authorize]
        [ValidateAntiForgeryToken]
        [HttpPost]
        public ActionResult AddBook(Book bookToAdd)
        {
            if (ModelState.IsValid)
            {
                libRepo.BookRepo.Add(bookToAdd);
                libRepo.Save();
                TempData["Notification"] = "Add new book successfully.";
                return RedirectToAction("Index");
            }
            return View(bookToAdd);
        }


        [Authorize]
        public ActionResult EditBook([DefaultValue(0)]int id)
        {
            Session["LoginUser"] = HttpContext.User.Identity.Name;
            if (HttpContext.User.Identity.Name.ToString().Substring(0, 2) != "A_")
                return RedirectToAction("Index", "Account");
            Book booktoedit = libRepo.BookRepo.Find(id);
            if (booktoedit == null)
                return HttpNotFound();
            return View(booktoedit);
        }


        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Editbook(Book bookToEdit)
        {
            if (ModelState.IsValid)
            {
                libRepo.BookRepo.Update(bookToEdit);
                TempData["Notification"] = "Edit book successfully.";
                libRepo.Save();
                return RedirectToAction("Index");
            }
            return View(bookToEdit);
        }


        [Authorize]
        public ActionResult DeleteBook([DefaultValue(0)]int id)
        {
            Session["LoginUser"] = HttpContext.User.Identity.Name;
            if (HttpContext.User.Identity.Name.ToString().Substring(0, 2) != "A_")
                return RedirectToAction("Index", "Account");
            Book booktodelete = libRepo.BookRepo.Find(id);
            if (booktodelete == null)
                return HttpNotFound();
            return View(booktodelete);
        }


        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteBook(Book bookToDelete, string answer)
        {
            if (answer == "Yes")
            {
                TempData["Notification"] = "Delete " + bookToDelete.BookName + " successfully.";
                List<BorrowEntry> removeBorrowEntry = libRepo.BorrowEntryRepo.ListWhere(target => target.BorrowBook.BookID == bookToDelete.BookID).ToList();
                RequestEntry entryToRemove = libRepo.RequestEntryRepo.Find(bookToDelete.BookID);
                if (entryToRemove != null)
                    libRepo.RequestEntryRepo.Remove(entryToRemove);
                libRepo.BorrowEntryRepo.Remove(removeBorrowEntry);
                libRepo.BookRepo.Remove(bookToDelete);
                libRepo.Save();
                return RedirectToAction("Index");
            }
            return RedirectToAction("Index");
        }

    }
}

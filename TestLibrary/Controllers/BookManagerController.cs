using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TestLibrary.Models;
using TestLibrary.DataAccess;
using TestLibrary.ViewModels;
using System.Data.Entity;
using System.ComponentModel;
namespace TestLibrary.Controllers
{
    public class BookManagerController : Controller
    {
        LibraryRepository libRepo = new LibraryRepository();

        [Authorize]
        public ActionResult Index(int page = 1,int pageSize = 10)
        {

            Session["LoginUser"] = HttpContext.User.Identity.Name;
            if (HttpContext.User.Identity.Name.ToString().Substring(0, 2) != "A_")
                return RedirectToAction("Index", "Account");
            TempData["pageSize"] = pageSize;
            TempData["page"] = page;
            List<Book> bookList = libRepo.BookRepo.List();
            PageList<Book> pglist;
            int index = (page - 1) * pageSize;
            if (index < bookList.Count && ((index + pageSize) <= bookList.Count))
            {
                pglist = new PageList<Book>(bookList.GetRange((page - 1) * pageSize, pageSize));
            }
            else if (index < bookList.Count)
            {
                pglist = new PageList<Book>(bookList.GetRange((page - 1) * pageSize,bookList.Count % pageSize));
            }
            else if (bookList.Count == 0)
            {
                TempData["Notification"] = "No book list to show please create one to start the magic.";
                return View();
            }
            else
            {
                TempData["Notification"] = "Invalid list view parameter please refresh this page to try again.";
                return View();
            }

            if(bookList.Count % pageSize == 0)
                pglist.SetPageSize(bookList.Count / pageSize);
            else
                pglist.SetPageSize((bookList.Count / pageSize + 1));
            pglist.SetCurrentPage(page);
            return View(pglist);
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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TestLibrary.Models;
using TestLibrary.DataAccess;
using TestLibrary.ViewModels;
namespace TestLibrary.Controllers
{
    public class BookSearchController : Controller
    {
        LibraryRepository libRepo = new LibraryRepository();
        public ActionResult Index()
        {
            if (HttpContext.User.Identity.IsAuthenticated)
                Session["LoginUser"] = HttpContext.User.Identity.Name;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Basic(string keyword, string searchType)
        {
            ViewBag.Keyword = keyword;
            ViewBag.SearchType = searchType;
            if (searchType == "")
            {
                TempData["Notification"] = "Please select search type.";
                return View("Index");
            }
            else if (searchType == "BookName")
            {
                List<Book> targetlist = libRepo.BookRepo.ListWhere(target => target.BookName.Contains(keyword)).OrderBy(booksort => booksort.BookName).ToList();
                if (targetlist.Count == 0)
                    TempData["Notification"] = "No book result found.";
                return View("Index", targetlist);
            }
            else if (searchType == "Author")
            {
                List<Book> targetlist = libRepo.BookRepo.ListWhere(target => target.Author.Contains(keyword)).OrderBy(booksort => booksort.BookName).ToList();
                if (targetlist.Count == 0)
                    TempData["Notification"] = "No book result found.";
                return View("Index", targetlist);
            }
            else if (searchType == "Publisher")
            {
                List<Book> targetlist = libRepo.BookRepo.ListWhere(target => target.Publisher.Contains(keyword)).OrderBy(booksort => booksort.BookName).ToList();
                if (targetlist.Count == 0)
                    TempData["Notification"] = "No book result found.";
                return View("Index", targetlist);
            }

            else if (searchType == "Year")
            {
                try
                {
                    int year = int.Parse(keyword);
                    List<Book> targetlist = libRepo.BookRepo.ListWhere(target => target.Year == year).OrderBy(booksort => booksort.BookName).ToList();
                    if (targetlist.Count == 0)
                        TempData["Notification"] = "No book result found.";
                    return View("Index", targetlist);
                }
                catch (FormatException)
                {
                    TempData["Notification"] = "Input string was not in a correct format.";
                    return View("Index");
                }
            }
            else
                return View("Index");
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Advance([Bind(Include = "BookName,Author,Publisher,Year")]Book bookToSearch)
        {
            ModelState.Remove("BookName");
            TempData["BookName"] = bookToSearch.BookName;
            TempData["Author"] = bookToSearch.Author;
            TempData["Publisher"] = bookToSearch.Publisher;
            TempData["Year"] = bookToSearch.Year;
            if (ModelState.IsValid)
            {

                List<Book> targetlist;
                bookToSearch.BookName = (bookToSearch.BookName == null) ? "" : bookToSearch.BookName;
                bookToSearch.Author = (bookToSearch.Author == null) ? "" : bookToSearch.Author;
                bookToSearch.Publisher = (bookToSearch.Publisher == null) ? "" : bookToSearch.Publisher;

                if (bookToSearch.Year != null)
                {
                    targetlist = libRepo.BookRepo.ListWhere(target => (target.BookName.Contains(bookToSearch.BookName)) &&
                    (target.Author.Contains(bookToSearch.Author)) && (target.Publisher.Contains(bookToSearch.Publisher)) &&
                     (target.Year == bookToSearch.Year)).ToList();
                }

                else
                {
                    targetlist = libRepo.BookRepo.ListWhere(target => (target.BookName.Contains(bookToSearch.BookName)) &&
                    (target.Author.Contains(bookToSearch.Author)) && (target.Publisher.Contains(bookToSearch.Publisher))).ToList();
                }
                TempData["AdvanceSearch"] = "Advance";
                if (targetlist.Count == 0)
                    TempData["Notification"] = "No result book found.";
                return View("Index",targetlist);
            }
            else
                TempData["Notification"] = "Input string was not in a correct format.";
            return View("Index");
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult QuickSearch(string bookName)
        {
            ViewBag.quicksearchkey = bookName;
            List<Book> booklist = libRepo.BookRepo.ListWhere(target => target.BookName.Contains(bookName));
            return View(booklist);
        }
    }
}

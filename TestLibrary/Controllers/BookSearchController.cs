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
        public ActionResult Basic(string keyword, string searchType,int page = 1,int pageSize = 10)
        {
            ViewBag.Keyword = keyword;
            ViewBag.SearchType = searchType;
            List<Book> bookList;
            if (searchType == "")
            {
                TempData["Notification"] = "Please select search type.";
                return View("Index");
            }
            else if (searchType == "BookName")
            {
                bookList = libRepo.BookRepo.ListWhere(target => target.BookName.Contains(keyword)).OrderBy(booksort => booksort.BookName).ToList();
            }
            else if (searchType == "Author")
            {
                bookList = libRepo.BookRepo.ListWhere(target => target.Author.Contains(keyword)).OrderBy(booksort => booksort.BookName).ToList();
                
            }
            else if (searchType == "Publisher")
            {
                bookList = libRepo.BookRepo.ListWhere(target => target.Publisher.Contains(keyword)).OrderBy(booksort => booksort.BookName).ToList();
            }

            else if (searchType == "Year")
            {
                try
                {
                    int year = int.Parse(keyword);
                    bookList = libRepo.BookRepo.ListWhere(target => target.Year == year).OrderBy(booksort => booksort.BookName).ToList();
                }
                catch (FormatException)
                {
                    TempData["Notification"] = "Input string was not in a correct format.";
                    return View("Index");
                }
            }
            else
            {
                TempData["Notification"] = "Something was error.";
                return View("Index");
            }

            TempData["pageSize"] = pageSize;
            TempData["page"] = page;
            PageList<Book> pglist = new PageList<Book>(bookList, page, pageSize);
            switch (pglist.Categorized())
            {
                case PageListResult.Ok: { return View("Index", pglist); }
                case PageListResult.Empty:
                    {                
                    TempData["Notification"] = "No book result found.";
                    return View("Index");
                    }
                default:
                    {
                        TempData["Notification"] = "Invalid list view parameter please refresh this page to try again.";
                        return View("Index");
                    }
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Advance([Bind(Include = "BookName,Author,Publisher,Year")]Book bookToSearch,int page = 1,int pageSize = 10)
        {
            ModelState.Remove("BookName");
            TempData["BookName"] = bookToSearch.BookName;
            TempData["Author"] = bookToSearch.Author;
            TempData["Publisher"] = bookToSearch.Publisher;
            TempData["Year"] = bookToSearch.Year;
            if (ModelState.IsValid)
            {

                List<Book> bookList;
                bookToSearch.BookName = (bookToSearch.BookName == null) ? "" : bookToSearch.BookName;
                bookToSearch.Author = (bookToSearch.Author == null) ? "" : bookToSearch.Author;
                bookToSearch.Publisher = (bookToSearch.Publisher == null) ? "" : bookToSearch.Publisher;

                if (bookToSearch.Year != null)
                {
                    bookList = libRepo.BookRepo.ListWhere(target => (target.BookName.Contains(bookToSearch.BookName)) &&
                    (target.Author.Contains(bookToSearch.Author)) && (target.Publisher.Contains(bookToSearch.Publisher)) &&
                     (target.Year == bookToSearch.Year)).ToList();
                }

                else
                {
                    bookList = libRepo.BookRepo.ListWhere(target => (target.BookName.Contains(bookToSearch.BookName)) &&
                    (target.Author.Contains(bookToSearch.Author)) && (target.Publisher.Contains(bookToSearch.Publisher))).ToList();
                }
                TempData["AdvanceSearch"] = "Advance";
                TempData["pageSize"] = pageSize;
                TempData["page"] = page;
                PageList<Book> pglist = new PageList<Book>(bookList, page, pageSize);
                switch (pglist.Categorized())
                {
                    case PageListResult.Ok: { return View("Index", pglist); }
                    case PageListResult.Empty:
                        {
                            TempData["Notification"] = "No book result found.";
                            return View("Index");
                        }
                    default:
                        {
                            TempData["Notification"] = "Invalid list view parameter please refresh this page to try again.";
                            return View("Index");
                        }
                }
            }
            else
                TempData["Notification"] = "Input string was not in a correct format.";
            return View("Index");
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult QuickSearch(string bookName,int page = 1,int pageSize = 10)
        {
            TempData["quicksearchkey"] = bookName;
            List<Book> bookList = libRepo.BookRepo.ListWhere(target => target.BookName.Contains(bookName));
            TempData["pageSize"] = pageSize;
            TempData["page"] = page;
            PageList<Book> pglist = new PageList<Book>(bookList, page, pageSize);
            switch (pglist.Categorized())
            {
                case PageListResult.Ok: { return View(pglist); }
                case PageListResult.Empty:
                    {
                        TempData["Notification"] = "No book result found.";
                        return View();
                    }
                default:
                    {
                        TempData["Notification"] = "Invalid list view parameter please refresh this page to try again.";
                        return View();
                    }
            }
        }
    }
}

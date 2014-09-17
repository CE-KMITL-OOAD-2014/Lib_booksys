using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TestLibrary.Models;
using TestLibrary.DataAccess;
namespace TestLibrary.Controllers
{
    public class BookSearchController : Controller
    {
        LibraryContext db = new LibraryContext();
        public ActionResult Index()
        {
            if (HttpContext.User.Identity.IsAuthenticated)
                Session["LoginUser"] = HttpContext.User.Identity.Name;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Basic(string Keyword, string SearchType)
        {
            ViewBag.Keyword = Keyword;
            ViewBag.SearchType = SearchType;
            if (SearchType == "")
            {
                TempData["Notification"] = "Please select search type.";
                return View("Index");
            }
            else if (SearchType == "BookName")
            {
                List<Book> targetlist = db.Books.Where(target => target.BookName.Contains(Keyword)).OrderBy(booksort => booksort.BookName).ToList();
                if (targetlist.Count == 0)
                    TempData["Notification"] = "No book result found.";
                return View("Index", targetlist);
            }
            else if (SearchType == "Author")
            {
                List<Book> targetlist = db.Books.Where(target => target.Author.Contains(Keyword)).OrderBy(booksort => booksort.BookName).ToList();
                if (targetlist.Count == 0)
                    TempData["Notification"] = "No book result found.";
                return View("Index", targetlist);
            }
            else if (SearchType == "Publisher")
            {
                List<Book> targetlist = db.Books.Where(target => target.Publisher.Contains(Keyword)).OrderBy(booksort => booksort.BookName).ToList();
                if (targetlist.Count == 0)
                    TempData["Notification"] = "No book result found.";
                return View("Index", targetlist);
            }

            else if (SearchType == "Year")
            {
                try
                {
                    int year = int.Parse(Keyword);
                    List<Book> targetlist = db.Books.Where(target => target.Year == year).OrderBy(booksort => booksort.BookName).ToList();
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
        public ActionResult Advance([Bind(Include = "BookName,Author,Publisher,Year")]Book booktosearch)
        {
            ModelState.Remove("BookName");
            TempData["BookName"] = booktosearch.BookName;
            TempData["Author"] = booktosearch.Author;
            TempData["Publisher"] = booktosearch.Publisher;
            TempData["Year"] = booktosearch.Year;
            if (ModelState.IsValid)
            {

                List<Book> targetlist;
                booktosearch.BookName = (booktosearch.BookName == null) ? "" : booktosearch.BookName;
                booktosearch.Author = (booktosearch.Author == null) ? "" : booktosearch.Author;
                booktosearch.Publisher = (booktosearch.Publisher == null) ? "" : booktosearch.Publisher;

                if (booktosearch.Year != null)
                {
                    targetlist = db.Books.Where(target => (target.BookName.Contains(booktosearch.BookName)) &&
                    (target.Author.Contains(booktosearch.Author)) && (target.Publisher.Contains(booktosearch.Publisher)) &&
                     (target.Year == booktosearch.Year)).ToList();
                }

                else
                {
                    targetlist = db.Books.Where(target => (target.BookName.Contains(booktosearch.BookName)) &&
                    (target.Author.Contains(booktosearch.Author)) && (target.Publisher.Contains(booktosearch.Publisher))).ToList();
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
        public ActionResult QuickSearch(string BookName)
        {
            ViewBag.quicksearchkey = BookName;
            IQueryable<Book> booklist = db.Books.Where(target => target.BookName.Contains(BookName));
            return View(booklist.ToList());
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}

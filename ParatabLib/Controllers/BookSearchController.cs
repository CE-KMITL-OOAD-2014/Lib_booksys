using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ParatabLib.Models;
using ParatabLib.DataAccess;
using ParatabLib.ViewModels;
using ParatabLib.Utilities;
namespace ParatabLib.Controllers
{
    public class BookSearchController : Controller
    {
        LibraryRepository libRepo;

        public BookSearchController()
        {
            libRepo = new LibraryRepository();
        }

        public BookSearchController(LibraryRepository libRepo)
        {
            this.libRepo = libRepo;
        }

        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Basic(string keyword, string searchType,int page = 1,int pageSize = 10)
        {
            TempData["Keyword"] = keyword;
            ViewBag.SearchType = searchType;
            List<Book> bookList;
            if (searchType == "")
            {
                TempData["ErrorNoti"] = "Please select search type.";
                return View("Index");
            }
            else if (searchType == "BookName")
            {
                bookList = libRepo.BookRepo.ListWhere(target => target.BookName.Contains(keyword)).OrderBy(booksort => booksort.BookName).ToList();
            }
            else if (searchType == "Author")
            {
                bookList = libRepo.BookRepo.ListWhere(target => StringUtil.IsContains(target.Author,keyword)).OrderBy(booksort => booksort.BookName).ToList();
                
            }
            else if (searchType == "Publisher")
            {
                bookList = libRepo.BookRepo.ListWhere(target => StringUtil.IsContains(target.Publisher, keyword)).OrderBy(booksort => booksort.BookName).ToList();
            }
            else if (searchType == "Callno")
            {
                bookList = libRepo.BookRepo.ListWhere(target => target.CallNumber.ToLower().Contains(keyword.ToLower())).OrderBy(booksort => booksort.CallNumber).ToList();
            }
            else if (searchType == "Year")
            {
                try
                {
                    if (keyword != "")
                    {
                        int year = int.Parse(keyword);
                        bookList = libRepo.BookRepo.ListWhere(target => target.Year == year).OrderBy(booksort => booksort.BookName).ToList();
                    }
                    else
                    {
                        bookList = libRepo.BookRepo.List();
                    }
                }
                catch (FormatException)
                {
                    TempData["ErrorNoti"] = "Input string was not in a correct format.";
                    return View("Index");
                }
            }
            else
            {
                TempData["ErrorNoti"] = "Something was error.";
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
                        TempData["ErrorNoti"] = "No book result found.";
                    return View("Index");
                    }
                default:
                    {
                        TempData["ErrorNoti"] = "Invalid list view parameter please refresh this page to try again.";
                        return View("Index");
                    }
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Advance([Bind(Include = "BookName,Author,Publisher,Year,CallNumber")]Book bookToSearch,int page = 1,int pageSize = 10)
        {
            ModelState.Remove("BookName");
            ModelState.Remove("CallNumber");
            TempData["BookName"] = bookToSearch.BookName;
            TempData["Author"] = bookToSearch.Author;
            TempData["Publisher"] = bookToSearch.Publisher;
            TempData["Year"] = bookToSearch.Year;
            TempData["Callno"] = bookToSearch.CallNumber;
            if (ModelState.IsValid)
            {
                List<Book> bookList;
                bookToSearch.BookName = (bookToSearch.BookName == null) ? "" : bookToSearch.BookName;
                bookToSearch.Author = (bookToSearch.Author == null) ? "" : bookToSearch.Author;
                bookToSearch.Publisher = (bookToSearch.Publisher == null) ? "" : bookToSearch.Publisher;
                bookToSearch.CallNumber = (bookToSearch.CallNumber == null) ? "" : bookToSearch.CallNumber.ToLower();
                if (bookToSearch.Year != null)
                {
                    bookList = libRepo.BookRepo.ListWhere(target => (target.BookName.Contains(bookToSearch.BookName)) &&
                        (target.CallNumber.ToLower().Contains(bookToSearch.CallNumber)) &&
                        (StringUtil.IsContains(target.Author, bookToSearch.Author)) &&
                       (StringUtil.IsContains(target.Publisher, bookToSearch.Publisher)) &&
                     (target.Year == bookToSearch.Year)).ToList();
                }

                else
                {
                    bookList = libRepo.BookRepo.ListWhere(target => (target.BookName.Contains(bookToSearch.BookName)) &&
                    (target.CallNumber.ToLower().Contains(bookToSearch.CallNumber)) &&
                    (StringUtil.IsContains(target.Author, bookToSearch.Author)) && 
                    (StringUtil.IsContains(target.Publisher, bookToSearch.Publisher))).ToList();
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
                            TempData["ErrorNoti"] = "No book result found.";
                            return View("Index");
                        }
                    default:
                        {
                            TempData["ErrorNoti"] = "Invalid list view parameter please refresh this page to try again.";
                            return View("Index");
                        }
                }
            }
            else
                TempData["ErrorNoti"] = "Input string was not in a correct format.";
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
                        TempData["ErrorNoti"] = "No book result found.";
                        return View();
                    }
                default:
                    {
                        TempData["ErrorNoti"] = "Invalid list view parameter please refresh this page to try again.";
                        return View();
                    }
            }
        }

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (Request.HttpMethod == "GET")
            {
                if (HttpContext.User.Identity.IsAuthenticated)
                {
                    if (AuthenticateController.IsUserValid(HttpContext.User.Identity.Name.Substring(2)))
                        Session["LoginUser"] = HttpContext.User.Identity.Name;
                    else
                    {
                        AuthenticateController.OnInvalidSession(ref filterContext);
                        return;
                    }
                }
                else
                    Session["LoginUser"] = null;
            }
        }


    }
}

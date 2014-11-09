using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ParatabLib.Models;
using ParatabLib.DataAccess;
using ParatabLib.ViewModels;
using ParatabLib.Utilities;
using System.Diagnostics;
namespace ParatabLib.Controllers
{
    //This class use to handle book search unit
    public class BookSearchController : Controller
    {
        LibraryRepository libRepo;

        /* Both of constructors use to set libRepo properties whether to use default(instantiate new)
         * or via parameter.
         */ 
        public BookSearchController()
        {
            libRepo = new LibraryRepository();
        }

        public BookSearchController(LibraryRepository libRepo)
        {
            this.libRepo = libRepo;
        }

        //This method use to call index search page and return it to user.
        public ActionResult Index()
        {
            return View();
        }

        /* This method use to submit search data in basic mode which passing keyword and searchType as string
         * parameterized of page and pageSize to paging list for search result.
         * Check type of searching by compare searchType with desired flow,
         * special case for year properties since year is integer but keyword is passing as string]
         * it must be parse(convert) from string to integer if exception from this occur notify user to input
         * correct numeric string.Finally show result as paged search result list to user(in normal case).
         * Moreover if result is not null show find time(from Timer object) in seconds.
         */ 
        [HttpPost]
        [OutputCache(Duration = 0, NoStore = true)]
        public ActionResult Basic(string keyword, string searchType,int page = 1,int pageSize = 10)
        {
            //Start timer
            Stopwatch Timer = new Stopwatch();
            Timer.Start();
            
            //Memorized search parameter to return to search page.
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
            //Stop timer
            Timer.Stop();
            TempData["pageSize"] = pageSize;
            TempData["page"] = page;
            PageList<Book> pglist = new PageList<Book>(bookList, page, pageSize);
            switch (pglist.Categorized())
            {
                /* if result is not empty or null set FindTime in TempData by use 
                 * ElapsedMilliseconds divide by 1000.0
                 * this will result in double value.
                 */
                case PageListResult.Ok: {
                    TempData["TotalResult"] = bookList.Count;
                    TempData["FindTime"] = Timer.ElapsedMilliseconds /1000.0;
                    return View("Index", pglist); 
                }
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

        /* This method use to submit search data in advance mode which passing all book properties in one Book object
         * parameterized of page and pageSize to paging list for search result.
         * Check that user is input year data or not if yes include year data in searching
         * otherwise exclude year data and find.
         * Special case for year properties since year is integer but data passing as string]
         * Modelstate.Isvalid will check that year data is in correct format or not.If not notify user to input
         * correct numeric string.Finally show result as paged search result list to user(in normal case).
         * Moreover if result is not null show find time(from Timer object) in seconds.
         */
        [HttpPost]
        [OutputCache(Duration = 0, NoStore = true)]
        public ActionResult Advance([Bind(Include = "BookName,Author,Publisher,Year,CallNumber")]Book bookToSearch,int page = 1,int pageSize = 10)
        {
            ModelState.Remove("BookName");
            ModelState.Remove("CallNumber");

            //Memorize search parameter to return to search page.
            TempData["BookName"] = bookToSearch.BookName;
            TempData["Author"] = bookToSearch.Author;
            TempData["Publisher"] = bookToSearch.Publisher;
            TempData["Year"] = bookToSearch.Year;
            TempData["Callno"] = bookToSearch.CallNumber;
            
            //Start timer
            Stopwatch Timer = new Stopwatch();
            Timer.Start();
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

                //Stop timer
                Timer.Stop();
                TempData["AdvanceSearch"] = "Advance";
                TempData["pageSize"] = pageSize;
                TempData["page"] = page;
                PageList<Book> pglist = new PageList<Book>(bookList, page, pageSize);
                switch (pglist.Categorized())
                {
                    case PageListResult.Ok: {
                        /* if result is not empty or null set FindTime in TempData by use 
                         * ElapsedMilliseconds divide by 1000.0
                         * this will result in double value.
                         */
                        TempData["TotalResult"] = bookList.Count;
                        TempData["FindTime"] = Timer.ElapsedMilliseconds / 1000.0;
                        return View("Index", pglist); 
                    }
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

        /* This method use to find book in QuickSearch mode by passing bookName as string
         * with parameterized of page and pageSize that use for paging search list result.
         * quicksearch is search related book base on bookName parameter,finally return result
         * to user.
         */
        [HttpPost]
        [OutputCache(Duration = 0, NoStore = true)]
        public ActionResult QuickSearch(string bookName,int page = 1,int pageSize = 10)
        {
            TempData["quicksearchkey"] = bookName;
            Stopwatch Timer = new Stopwatch();
            
            //Start timer
            Timer.Start();
            List<Book> bookList = libRepo.BookRepo.ListWhere(target => target.BookName.Contains(bookName));

            //Stop timer
            Timer.Stop();
            TempData["pageSize"] = pageSize;
            TempData["page"] = page;
            PageList<Book> pglist = new PageList<Book>(bookList, page, pageSize);
            switch (pglist.Categorized())
            {
                /* if result is not empty or null set FindTime in TempData by use 
                 * ElapsedMilliseconds divide by 1000.0
                 * this will result in double value.
                 */
                case PageListResult.Ok: {
                    TempData["TotalResult"] = bookList.Count;
                    TempData["FindTime"] = Timer.ElapsedMilliseconds / 1000.0; 
                    return View(pglist); 
                }
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


        /* [Override method]
         * This method use to check that whether current user for current session is exist in system or not.
         * If not,call and pass by reference of current HTTPrequest in AuthenticateController.OnInvalidSession
         * to set appropiate page result.
         */
        protected override void OnActionExecuting(ActionExecutingContext filterContext)
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

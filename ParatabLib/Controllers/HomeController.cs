using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ParatabLib.DataAccess;
using ParatabLib.Models;
using ParatabLib.ViewModels;
namespace ParatabLib.Controllers
{
    //This class use to handle general page include error page and API documentation
    public class HomeController : Controller
    {
        LibraryRepository libRepo = new LibraryRepository();
        
        /* This method use to call Homepage of this website,
         * include news data to display in page and sort data by date.
         */
        [OutputCache(Duration = 0, NoStore = true)]
        public ActionResult Index()
        {
            return View(NewsController.getLatestNews());
        }

        //This method use to call about page and return it to user.
        public ActionResult About()
        {
            return View();
        }

        //This method use to call contact page and return it to user.
        public ActionResult Contact()
        {
            return View();
        }

        //This method use to call ChangeLog page and return it to user.
        public ActionResult ChangeLog()
        {
            RedirectToRouteResult n = RedirectToAction("Login", "Authenticate");
            return View();
        }

        /* This method use to call TopTen page and return it to user,
         * by find the first ten member and book that has max number of
         * borrowentry and sort list then parameterized to viewModel via TopTenViewer object.
         */
        [OutputCache(Duration = 0, NoStore = true)]
        public ActionResult TopTen()
        {
            TopTenViewer viewer = new TopTenViewer();

            //Use of OrderByDescending will result in ordered list
            List<Member> topMember = libRepo.MemberRepo.List().OrderByDescending(member => member.GetRelatedBorrowEntry(ref libRepo).Count).ToList();
            List<Book> topBorrowBook = libRepo.BookRepo.List().OrderByDescending(book => book.GetRelatedBorrowEntry(ref libRepo).Count).ToList();
            
            //For newly site ranking may not more than 10 so check it.
            if (topMember.Count > 10)
                topMember = topMember.GetRange(0, 10);
            else
                topMember = topMember.GetRange(0, topMember.Count);

            if (topBorrowBook.Count > 10)
                topBorrowBook = topBorrowBook.GetRange(0, 10);
            else
                topBorrowBook = topBorrowBook.GetRange(0, topBorrowBook.Count);

            //Parameterized it to this TopTenViewer object.
            viewer.SetTopBook(topBorrowBook);
            viewer.SetTopMember(topMember);
            return View(viewer);
        }

        //This method use to call LibraryApi page(API documentation page) and return it to user
        public ActionResult LibraryApi()
        {
            return View();
        }

        //This method use to call HTTP error 404 page to handle HttpNotFound
        public ActionResult Error404()
        {
            return View();
        }

        //This method use to call HTTP error 400 page to handle HttpBadRequest
        public ActionResult Error400()
        {
            return View();
        }

        //This method use to call HTTP error 500 page to handle HttpInternalServerError
        public ActionResult Error500()
        {
            return View();
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

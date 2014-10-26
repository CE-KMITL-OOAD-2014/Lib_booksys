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
    public class HomeController : Controller
    {
        LibraryRepository libRepo = new LibraryRepository();
       // public HomeController() : this(new LibraryRepository()) { }
       /* public HomeController(IRepository libRepo)
        {
            this.libRepo = libRepo;
        }
        */

        public ActionResult Index()
        {
            return View(libRepo.NewsRepo.List().OrderByDescending(news => news.PostTime));
        }

        public ActionResult About()
        {
            return View();
        }

        public ActionResult Contact()
        {
            return View();
        }
        public ActionResult ChangeLog()
        {
            RedirectToRouteResult n = RedirectToAction("Login", "Authenticate");
            return View();
        }

        public ActionResult TopTen()
        {
            TopTenViewer viewer = new TopTenViewer();
            List<Member> topMember = libRepo.MemberRepo.List().OrderByDescending(member => member.GetRelatedBorrowEntry().Count).ToList();
            List<Book> topBorrowBook = libRepo.BookRepo.List().OrderByDescending(book => book.GetRelatedBorrowEntry().Count).ToList();
            if (topMember.Count > 10)
                topMember = topMember.GetRange(0, 10);
            else
                topMember = topMember.GetRange(0, topMember.Count);

            if (topBorrowBook.Count > 10)
                topBorrowBook = topBorrowBook.GetRange(0, 10);
            else
                topBorrowBook = topBorrowBook.GetRange(0, topBorrowBook.Count);

            viewer.SetTopBook(topBorrowBook);
            viewer.SetTopMember(topMember);
            return View(viewer);
        }

        public ActionResult LibraryApi()
        {
            return View();
        }

        public ActionResult Error404()
        {
            return View();
        }

        public ActionResult Error400()
        {
            return View();
        }

        public ActionResult Error500()
        {
            return View();
        }

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

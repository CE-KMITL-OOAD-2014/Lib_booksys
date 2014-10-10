using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TestLibrary.DataAccess;
using TestLibrary.Models;
using TestLibrary.ViewModels;
namespace TestLibrary.Controllers
{
    public class HomeController : Controller
    {
        LibraryRepository libRepo = new LibraryRepository();
        public ActionResult Index()
        {
            if (HttpContext.User.Identity.IsAuthenticated)
                Session["LoginUser"] = HttpContext.User.Identity.Name;
            return View(libRepo.NewsRepo.List().OrderByDescending(news => news.PostTime));
        }

        public ActionResult About()
        {
            if (HttpContext.User.Identity.IsAuthenticated)
                Session["LoginUser"] = HttpContext.User.Identity.Name;
            return View();
        }

        public ActionResult Contact()
        {
            if (HttpContext.User.Identity.IsAuthenticated)
                Session["LoginUser"] = HttpContext.User.Identity.Name;
            return View();
        }
        public ActionResult ChangeLog()
        {
            if (HttpContext.User.Identity.IsAuthenticated)
                Session["LoginUser"] = HttpContext.User.Identity.Name;
            return View();
        }

        public ActionResult TopTen()
        {
            if (HttpContext.User.Identity.IsAuthenticated)
                Session["LoginUser"] = HttpContext.User.Identity.Name;
            TopTenViewer viewer = new TopTenViewer();
            List<Member> topMember = libRepo.MemberRepo.List().OrderByDescending(member => member.BorrowEntries.Count).ToList();
            List<Book> topBorrowBook = libRepo.BookRepo.List().OrderByDescending(book => book.BorrowEntries.Count).ToList();

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
            if (HttpContext.User.Identity.IsAuthenticated)
                Session["LoginUser"] = HttpContext.User.Identity.Name;
            return View();
        }

        public ActionResult Error404()
        {
            if (HttpContext.User.Identity.IsAuthenticated)
                Session["LoginUser"] = HttpContext.User.Identity.Name;
            return View();
        }

        public ActionResult Error400()
        {
            if (HttpContext.User.Identity.IsAuthenticated)
                Session["LoginUser"] = HttpContext.User.Identity.Name;
            return View();
        }

        public ActionResult Error500()
        {
            if (HttpContext.User.Identity.IsAuthenticated)
                Session["LoginUser"] = HttpContext.User.Identity.Name;
            return View();
        }
    }
}

﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using TestLibrary.DataAccess;
using TestLibrary.Models;
using System.ComponentModel;
namespace TestLibrary.Controllers
{
    public class BookController : Controller
    {
        LibraryContext db = new LibraryContext();

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Search()
        {
            if (HttpContext.User.Identity.IsAuthenticated)
                Session["LoginUser"] = HttpContext.User.Identity.Name;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Search(string Keyword,string SearchType)
        {
            ViewBag.Keyword = Keyword;
            ViewBag.SearchType = SearchType;
            if (SearchType == "")
            {
                TempData["Notification"] = "Please select search type.";
                return Search();
            }
            else if (SearchType == "BookName")
            {
                List<Book> targetlist = db.Books.Where(target => target.BookName.Contains(Keyword)).OrderBy(booksort => booksort.BookName).ToList();
                if (targetlist.Count == 0)
                    TempData["Notification"] = "No book result found.";
                return View(targetlist);
            }
            else if (SearchType == "Author")
            {
                List<Book> targetlist = db.Books.Where(target => target.Author.Contains(Keyword)).OrderBy(booksort => booksort.BookName).ToList();
                if (targetlist.Count == 0)
                    TempData["Notification"] = "No book result found.";
                return View(targetlist);
            }
            else if (SearchType == "Publisher")
            {
                List<Book> targetlist =db.Books.Where(target => target.Publisher.Contains(Keyword)).OrderBy(booksort => booksort.BookName).ToList();
                if (targetlist.Count == 0)
                    TempData["Notification"] = "No book result found.";
                return View(targetlist);
            }

            else if (SearchType == "Year")
            {
                try
                {
                    int year = int.Parse(Keyword);
                    List<Book> targetlist =db.Books.Where(target => target.Year == year).OrderBy(booksort => booksort.BookName).ToList();
                    if (targetlist.Count == 0)
                    TempData["Notification"] = "No book result found.";
                    return View(targetlist);
                }
                catch (FormatException)
                {
                    TempData["Notification"] = "Input string was not in a correct format.";
                    return View();
                }
            }
            else
                return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SearchResult(string BookName)
        {
            ViewBag.quicksearchkey = BookName;
             IQueryable<Book> booklist = db.Books.Where(target => target.BookName.Contains(BookName));
            return View(booklist.ToList());
        }

    }
}

﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using TestLibrary.DataAccess;
using TestLibrary.Models;
using System.ComponentModel;
using System.Data.Entity;
namespace TestLibrary.Controllers
{
    public class BookController : Controller
    {
        LibraryContext db = new LibraryContext();


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
        public ActionResult AdvanceSearch([Bind(Include="BookName,Author,Publisher,Year")]Book booktosearch)
        {
            ModelState.Remove("BookName");
            TempData["BookName"] = booktosearch.BookName;
            TempData["Author"] = booktosearch.Author;
            TempData["Publisher"] = booktosearch.Publisher;
            TempData["Year"] = booktosearch.Year;
            if(ModelState.IsValid){

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
                return View("Search", targetlist);
            }
            else
                TempData["Notification"] = "Input string was not in a correct format.";
            return View("Search");
            
        }

        [Authorize]
        public ActionResult Renew(int id)
        {
            Session["LoginUser"] = HttpContext.User.Identity.Name;
            if(HttpContext.User.Identity.Name.ToString().Substring(0,2)!="M_")
                return RedirectToAction("Index","Manage");

            BorrowEntry renewentry = db.BorrowList.SingleOrDefault(target => target.ID == id &&
                                        target.ReturnDate == null);
            if (renewentry == null)
            {
                TempData["Notification"] = "Invalid renew book id.";
                return RedirectToAction("Index","Member");
            }

            if (renewentry.Borrower.UserName != Session["LoginUser"].ToString().Substring(2))
            {
                TempData["Notification"] = "Invalid renew operation.";
                return RedirectToAction("Index","Member");
            }

            if (renewentry.RenewCount == 3)
            {
                TempData["Notification"] = "Your renew of book ID."+renewentry.BorrowBook.BookID+" is exceed maximum!";
                return RedirectToAction("Index", "Member");
            }
            return View(renewentry);
        }

        [Authorize]
        [ValidateAntiForgeryToken]
        [HttpPost]
        public ActionResult Renew(BorrowEntry renewentry, string answer)
        {
            if (ModelState.IsValid && answer == "Yes")
            {
                if (db.RequestList.ToList().LastOrDefault(target => target.BookID == renewentry.BookID && target.ExpireDate == null) != null)
                {
                    TempData["Notification"] = "This book is ON HOLD.";
                }
                else
                {
                    renewentry.DueDate = DateTime.Now.Date.AddDays(7);
                    renewentry.RenewCount++;
                    db.Entry(renewentry).State = EntityState.Modified;
                    db.SaveChanges();
                    TempData["Notification"] = "Renew successful!";
                }
            }
            return RedirectToAction("Index", "Member");
        }





        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SearchResult(string BookName)
        {
            ViewBag.quicksearchkey = BookName;
             IQueryable<Book> booklist = db.Books.Where(target => target.BookName.Contains(BookName));
            return View(booklist.ToList());
        }


        [Authorize]
        public ActionResult Request()
        {
            Session["LoginUser"] = HttpContext.User.Identity.Name;
            if (HttpContext.User.Identity.Name.ToString().Substring(0, 2) != "M_")
                return RedirectToAction("Index", "Manage");
            return View();
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Request(RequestEntry newentry)
        {
            if (ModelState.IsValid)
            {
                Book booktorequest;
                if ((booktorequest = db.Books.Find(newentry.BookID)) == null)
                {
                    TempData["Notification"] = "No book with prefer ID exists.";
                    return View();
                }
                if (booktorequest.BookStatus != Status.Borrowed && booktorequest.BookStatus != Status.Reserved)
                {
                    TempData["Notification"] = "Can't request this book due to it is "
                        + booktorequest.BookStatus.ToString() + ".";
                    return View();
                }

                if (db.RequestList.ToList().LastOrDefault(target => target.BookID == booktorequest.BookID 
                                && target.ExpireDate == null) != null || booktorequest.BookStatus == Status.Reserved)
                {
                    TempData["Notification"] = "This book is already requested.";
                    return View();
                }

                Member request_member = db.Members.Where(target => target.UserName ==
                                                HttpContext.User.Identity.Name.ToString().Substring(2)).Single();

                if(db.BorrowList.ToList().LastOrDefault(target => target.BookID == newentry.BookID &&
                                                        target.Borrower == request_member && target.ReturnDate == null) != null)
                {
                    TempData["Notification"] = "Can't request your current borrowed book.";
                    return View();
                }

                newentry.UserID = request_member.UserID;
                newentry.RequestDate = DateTime.Now;
                db.RequestList.Add(newentry);
                db.SaveChanges();
                TempData["Notification"] = "Request book successfully.";
                return RedirectToAction("Index","Member");
            }
            else
                return View();
        }


        [Authorize]
        public ActionResult CancelRequest(int id)
        {
            Session["LoginUser"] = HttpContext.User.Identity.Name;
            if (HttpContext.User.Identity.Name.ToString().Substring(0, 2) != "M_")
                return RedirectToAction("Index", "Manage");
            RequestEntry wantedEntry = db.RequestList.Find(id);
            if (wantedEntry == null)
            {
                TempData["Notification"] = "No book with prefered id exists.";
                return RedirectToAction("Index", "Member");
            }
            Member preferMember = db.Members.SingleOrDefault(target => target.UserName == HttpContext.User.Identity.Name.ToString().Substring(2));
            if (wantedEntry.RequestUser != preferMember)
            {
                TempData["Notification"] = "Can't cancel other member's book request.";
                return RedirectToAction("Index", "Member");
            }
            return View(wantedEntry);
        }


        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CancelRequest(RequestEntry cancelEntry, string answer)
        {
            if (ModelState.IsValid)
            {
                if (answer == "Yes")
                {
                    Book bookToCheck = db.Books.Find(cancelEntry.BookID);
                    if (bookToCheck == null)
                    {
                        TempData["Notification"] = "Something went wrong,please try again.";
                        return RedirectToAction("Index", "Member");
                    }
                    else
                    {
                        if (bookToCheck.BookStatus == Status.Reserved)
                        {
                            bookToCheck.BookStatus = Status.Available;
                            db.Entry(bookToCheck).State = EntityState.Modified;
                        }
                    }
                    db.Entry(cancelEntry).State = EntityState.Deleted;
                    
                    db.SaveChanges();
                    TempData["Notification"] = "Cancel request successfully.";
                }
                return RedirectToAction("Index", "Member");
            }
            TempData["Notification"] = "Something went wrong,please try again.";
            return RedirectToAction("Index", "Member");
        }
        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}

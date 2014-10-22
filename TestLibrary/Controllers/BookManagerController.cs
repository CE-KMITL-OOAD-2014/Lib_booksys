﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TestLibrary.Models;
using TestLibrary.DataAccess;
using TestLibrary.ViewModels;
using System.Data.Entity;
using System.ComponentModel;
namespace TestLibrary.Controllers
{
    public class BookManagerController : Controller
    {
        LibraryRepository libRepo = new LibraryRepository();

        [Authorize]
        public ActionResult Index(int page = 1,int pageSize = 10)
        {

            Session["LoginUser"] = HttpContext.User.Identity.Name;
            if (HttpContext.User.Identity.Name.ToString().Substring(0, 2) != "A_")
                return RedirectToAction("Index", "Account");
            TempData["pageSize"] = pageSize;
            TempData["page"] = page;
            List<Book> bookList = libRepo.BookRepo.List();
            PageList<Book> pglist = new PageList<Book>(bookList, page, pageSize);
            switch (pglist.Categorized())
            {
                case PageListResult.Ok: { return View(pglist); }
                case PageListResult.Empty:
                    {
                        TempData["ErrorNoti"] = "No book list to show.";
                        return View();
                    }
                default:
                    {
                        TempData["ErrorNoti"] = "Invalid list view parameter please refresh this page to try again.";
                        return View();
                    }
            }
        }

        //Will edit later
        [Authorize]
        public ActionResult ViewBook([DefaultValue(0)]int id)
        {
            Session["LoginUser"] = HttpContext.User.Identity.Name;
            if (HttpContext.User.Identity.Name.ToString().Substring(0, 2) != "A_")
                return RedirectToAction("Index", "Account");
            Book booktoview = libRepo.BookRepo.Find(id);
            if (booktoview == null)
                return HttpNotFound();
            else
                return View(booktoview);
        }


        [Authorize]
        public ActionResult AddBook()
        {
            Session["LoginUser"] = HttpContext.User.Identity.Name;
            if (HttpContext.User.Identity.Name.ToString().Substring(0, 2) != "A_")
                return RedirectToAction("Index", "Account");
            return View();
        }

        [Authorize]
        [ValidateAntiForgeryToken]
        [HttpPost]
        public ActionResult AddBook(Book bookToAdd)
        {
            if (ModelState.IsValid)
            {
                libRepo.BookRepo.Add(bookToAdd);
                libRepo.Save();
                TempData["SuccessNoti"] = "Add new book successfully.";
                return RedirectToAction("Index");
            }
            return View(bookToAdd);
        }


        [Authorize]
        public ActionResult EditBook([DefaultValue(0)]int id)
        {
            Session["LoginUser"] = HttpContext.User.Identity.Name;
            if (HttpContext.User.Identity.Name.ToString().Substring(0, 2) != "A_")
                return RedirectToAction("Index", "Account");
            Book booktoedit = libRepo.BookRepo.Find(id);
            if (booktoedit == null)
                return HttpNotFound();
            return View(booktoedit);
        }


        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Editbook(Book bookToEdit)
        {
            if (ModelState.IsValid)
            {
                //If book is lost check related borrowentry and force return&delete req.entry//
                if (bookToEdit.BookStatus == Status.Lost)
                {
                    BorrowEntry entry = libRepo.BorrowEntryRepo.ListWhere(target => target.BookID == bookToEdit.BookID && target.ReturnDate == null).SingleOrDefault();
                    if (entry != null)
                    {
                        entry.ReturnDate = DateTime.Now.Date;
                        RequestEntry removeEntry = entry.GetBorrowBook().GetRelatedRequestEntry();
                        if (removeEntry != null)
                            libRepo.RequestEntryRepo.Remove(removeEntry);
                    }
                }
                //If book is available make sure that book is not reserve or borrowed//
                else if (bookToEdit.BookStatus == Status.Available)
                {
                    Book bookToFind = libRepo.BookRepo.Find(bookToEdit.BookID);
                    if (bookToFind.BookStatus == Status.Borrowed || bookToFind.BookStatus == Status.Reserved)
                    {
                        TempData["ErrorNoti"] = "Can't edit book status due to this book is " + bookToFind.BookStatus.ToString()+".";
                        return RedirectToAction("Index");
                    }
                    bookToFind.BookName = bookToEdit.BookName;
                    bookToFind.Author = bookToEdit.Author;
                    bookToFind.Publisher = bookToEdit.Publisher;
                    bookToFind.Year = bookToEdit.Year;
                    bookToFind.Detail = bookToEdit.Detail;
                    bookToFind.BookStatus = bookToEdit.BookStatus;
                    libRepo.BookRepo.Update(bookToFind);
                    TempData["SuccessNoti"] = "Edit book successfully.";
                    libRepo.Save();
                    return RedirectToAction("Index");
                }

                //Another check???? Avail -> Borrowed? Avail -> Req.
                libRepo.BookRepo.Update(bookToEdit);
                TempData["SuccessNoti"] = "Edit book successfully.";
                libRepo.Save();
                return RedirectToAction("Index");
            }
            return View(bookToEdit);
        }


        [Authorize]
        public ActionResult DeleteBook([DefaultValue(0)]int id)
        {
            Session["LoginUser"] = HttpContext.User.Identity.Name;
            if (HttpContext.User.Identity.Name.ToString().Substring(0, 2) != "A_")
                return RedirectToAction("Index", "Account");
            Book booktodelete = libRepo.BookRepo.Find(id);
            if (booktodelete == null)
                return HttpNotFound();
            return View(booktodelete);
        }


        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteBook(Book bookToDelete, string answer)
        {
            if (answer == "Yes")
            {
                TempData["SuccessNoti"] = "Delete " + bookToDelete.BookName + " successfully.";
                List<BorrowEntry> removeBorrowEntry = libRepo.BorrowEntryRepo.ListWhere(target => target.BookID == bookToDelete.BookID).ToList();
                Book bookToDel = libRepo.BookRepo.Find(bookToDelete.BookID);
                RequestEntry entryToCheck = bookToDel.GetRelatedRequestEntry();
                if (entryToCheck != null)
                    libRepo.RequestEntryRepo.Remove(entryToCheck);
                libRepo.BorrowEntryRepo.Remove(removeBorrowEntry);
                libRepo.BookRepo.Remove(bookToDel);
                libRepo.Save();
                return RedirectToAction("Index");
            }
            return RedirectToAction("Index");
        }

    }
}

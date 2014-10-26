using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ParatabLib.Models;
using ParatabLib.DataAccess;
using ParatabLib.ViewModels;
using System.Data.Entity;
namespace ParatabLib.Controllers
{
    public class LibrarianTransactionController : Controller
    {

        LibraryRepository libRepo = new LibraryRepository();
        private MemberTransactionViewer Check(BorrowEntry entry)
        {
            if (libRepo.MemberRepo.Find(entry.UserID) == null)
            {
                TempData["ErrorNoti"] = "No member that id's exists.";
                return null;
            }

            MemberTransactionViewer viewer = new MemberTransactionViewer();
            viewer.SetBorrowEntryViews(libRepo.BorrowEntryRepo.ListWhere(targetEntry => targetEntry.UserID == entry.UserID
                && targetEntry.ReturnDate == null));

            viewer.SetRequestEntryViews(libRepo.RequestEntryRepo.ListWhere(targetentry => targetentry.UserID == entry.UserID));
            return viewer;
        }

        private MemberTransactionViewer Borrow(BorrowEntry entry)
        {
            if (libRepo.MemberRepo.Find(entry.UserID) == null)
                    {
                        TempData["ErrorNoti"] = "No member that id's exists.";
                        return null;
                    }

                    if (libRepo.BorrowEntryRepo.ListWhere(target => target.UserID == entry.UserID
                        && target.ReturnDate == null).ToList().Count == 3)
                    {
                        TempData["ErrorNoti"] = "This member borrow exceed maximum allowed.";
                        return Check(entry);
                    }

                    Book booktoborrow = libRepo.BookRepo.Find(entry.BookID);

                    if (booktoborrow == null)
                    {
                        TempData["ErrorNoti"] = "No book was found in database.";
                        return Check(entry);
                    }
                    else if (booktoborrow.BookStatus == Status.Available)
                    {
                        entry.BorrowDate = DateTime.Now.Date;
                        entry.DueDate = DateTime.Now.Date.AddDays(7);
                        booktoborrow.BookStatus = Status.Borrowed;
                        libRepo.BookRepo.Update(booktoborrow);
                        libRepo.BorrowEntryRepo.Add(entry);
                        libRepo.Save();
                        TempData["SuccessNoti"] = "OK";
                        return Check(entry);
                    }
                    else if (booktoborrow.BookStatus == Status.Lost)
                    {
                        TempData["ErrorNoti"] = "This book is lost.";
                        return Check(entry);
                    }

                    else if (booktoborrow.BookStatus == Status.Reserved)
                    {
                        RequestEntry reqentry = booktoborrow.GetRelatedRequestEntry();
                        if (reqentry.ExpireDate.Value.Date < DateTime.Now.Date)
                        {
                            libRepo.RequestEntryRepo.Remove(reqentry);
                            booktoborrow.BookStatus = Status.Borrowed;
                            libRepo.BookRepo.Update(booktoborrow);
                            entry.BorrowDate = DateTime.Now.Date;
                            entry.DueDate = DateTime.Now.Date.AddDays(7);
                            libRepo.BorrowEntryRepo.Add(entry);
                            libRepo.Save();
                            TempData["SuccessNoti"] = "Delete expire req.//OK.";
                            return Check(entry);
                        }
                        else if (reqentry.UserID != entry.UserID)
                        {
                            TempData["ErrorNoti"] = "This user has no permission to borrow the requested book by others.";
                            return Check(entry);
                        }
                        else
                        {
                            libRepo.RequestEntryRepo.Remove(reqentry);
                            booktoborrow.BookStatus = Status.Borrowed;
                            entry.BorrowDate = DateTime.Now.Date;
                            entry.DueDate = DateTime.Now.Date.AddDays(7);
                            libRepo.BookRepo.Update(booktoborrow);
                            libRepo.BorrowEntryRepo.Add(entry);
                            libRepo.Save();
                            TempData["SuccessNoti"] = "User accept reserved book//OK";
                            return Check(entry);
                        }
                    }

                    else
                    {
                        TempData["ErrorNoti"] = "This book is already borrowed.";
                        return Check(entry);
                    }
               
        }

        private MemberTransactionViewer Return(BorrowEntry entry)
        {
            BorrowEntry returnentry = libRepo.BorrowEntryRepo.Find(entry.ID);
            if (returnentry == null)
            {
                TempData["ErrorNoti"] = "No borrow record found to do return.";
                return null;
            }
            else if (returnentry.ReturnDate != null)
            {
                TempData["ErrorNoti"] = "This book is already returned.";
                return Check(returnentry);
            }
            else
            {
                RequestEntry reqToCheck = returnentry.GetBorrowBook().GetRelatedRequestEntry();
                if (reqToCheck != null)
                {
                    Book bookToUpdate = returnentry.GetBorrowBook();
                    bookToUpdate.BookStatus = Status.Reserved;
                    reqToCheck.ExpireDate = DateTime.Now.Date.AddDays(3);
                    libRepo.BookRepo.Update(bookToUpdate);
                    libRepo.RequestEntryRepo.Update(reqToCheck);
                }
                else
                {
                    Book bookToUpdate = returnentry.GetBorrowBook();
                    bookToUpdate.BookStatus = Status.Available;
                    libRepo.BookRepo.Update(bookToUpdate);
                }
                if(returnentry.DueDate.Date < DateTime.Now.Date){
                    int dif = DateTime.Now.Subtract(returnentry.DueDate.Date).Days;
                    //Warn noti
                    TempData["WarnNoti"] = "Return successfully.Fine " + (dif * 5) + " baht.";
                    }
                else
                    TempData["SuccessNoti"] = "Return successfully.";
                returnentry.ReturnDate = DateTime.Now.Date;
                libRepo.BorrowEntryRepo.Update(returnentry);
                libRepo.Save();

                return Check(returnentry);
            }

        }

        [Authorize]
        public ActionResult Index(int page = 1,int pageSize = 10)
        {
            TempData["pageSize"] = pageSize;
            TempData["page"] = page;
            List<BorrowEntry> borrowList = libRepo.BorrowEntryRepo.List();
            PageList<BorrowEntry> pglist = new PageList<BorrowEntry>(borrowList, page, pageSize);
            switch (pglist.Categorized())
            {
                case PageListResult.Ok: { return View(pglist); }
                case PageListResult.Empty:
                    {
                        TempData["ErrorNoti"] = "No borrowed list to show.";
                        return View();
                    }
                default:
                    {
                        TempData["ErrorNoti"] = "Invalid list view parameter please refresh this page to try again.";
                        return View();
                    }
            }
        }

        //Show viewtable if user click 'Check'
        //To decide whether prefer request is OK?
        [Authorize]
        public ActionResult Transaction()
        {
            return View();
        }


        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Transaction(BorrowEntry entry,string operation)
        {
            ModelState.Remove("BookID");
            TempData["UserID"] = (entry.UserID == 0)?"":entry.UserID.ToString();
            TempData["BookID"] = (entry.BookID == 0)?null:entry.BookID.ToString();
            if (ModelState.IsValid)
            {
                if (operation == "Check")
                {
                    return View(Check(entry));
                }

                else if (operation == "Submit")
                {
                    return View(Borrow(entry));
                }
                else if (operation == "Return")
                {
                    return View(Return(entry));
                }
                else
                {
                    TempData["ErrorNoti"] = "Invalid operation.";
                    return View();
                }
            }
            else
            {
                TempData["ErrorNoti"] = "Please enter Member ID";
                return View();
            }
        }

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (Request.HttpMethod == "GET")
            {
                if (AuthenticateController.IsUserValid(HttpContext.User.Identity.Name.Substring(2)))
                {
                    Session["LoginUser"] = HttpContext.User.Identity.Name;
                    if (HttpContext.User.Identity.Name.ToString().Substring(0, 2) != "A_")
                    {
                        filterContext.Result = RedirectToAction("Index", "Account");
                        return;
                    }
                }
                else
                {
                    AuthenticateController.OnInvalidSession(ref filterContext);
                    return;
                }
            }
        }
    }
}

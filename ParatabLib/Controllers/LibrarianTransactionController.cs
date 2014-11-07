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
    //This class use to handle librarian transaction unit.
    public class LibrarianTransactionController : Controller
    {

        LibraryRepository libRepo = new LibraryRepository();

        /* [Private] MemberTransactionViewer Check(BorrowEntry entry)
         * This method will get related borrow entry and request entry of desired user
         * then parameterized it to MemberTransactionViewer to return as result.
         * If user input incorrect UserID notify user that "No member that id's exists."
         */
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

        /* [Private] MemberTransactionViewer Borrow(BorrowEntry entry)
         * This method will do borrow book operation,
         * first of all check that current user is exists then check current borrow book for desired
         * user whether is equal 3 or not,next is check that desired borrow book is exists,continue by
         * check book status... ,if it is Available do borrow normally(set borrow entry data and change 
         * book's status to borrowed and return success result.)
         * if it is Lost or Borrowed notify user that book status is XXXX where XXXX is Lost or Borrowed,
         * if it is reserved check that whether request is expired or not if yes delete expired req. and do borrow like normal case
         * if no;next check is current user that want to borrow book is same as in requestentry...
         * if yes remove request entry and do borrow like normal case if not notify user that no permission to borrow
         * other request book.
         * then parameterized it to MemberTransactionViewer to return as result.
         * If user input incorrect UserID notify user that "No member that id's exists."
         * Finally call "check" method or set return value to null to return and update borrow/return list for desired user.
         */
        private MemberTransactionViewer Borrow(BorrowEntry entry)
        {
            //Check that current user is exists.
            if (libRepo.MemberRepo.Find(entry.UserID) == null)
                    {
                        TempData["ErrorNoti"] = "No member that id's exists.";
                        return null;
                    }

                    //Check current borrow book for desired user whether is equal 3 or not.
                    if (libRepo.BorrowEntryRepo.ListWhere(target => target.UserID == entry.UserID
                        && target.ReturnDate == null).ToList().Count == 3)
                    {
                        TempData["ErrorNoti"] = "This member borrow exceed maximum allowed.";
                        return Check(entry);
                    }

                    //Check that desired borrow book is exists.
                    Book booktoborrow = libRepo.BookRepo.Find(entry.BookID);

                    if (booktoborrow == null)
                    {
                        TempData["ErrorNoti"] = "No book was found in database.";
                        return Check(entry);
                    }

                    /*check book status... ,if it is Available do borrow normally(set borrow entry data and change 
                     * book's status to borrowed and return success result.)
                     */ 
                    else if (booktoborrow.BookStatus == Status.Available)
                    {
                        entry.BorrowDate = DateTime.Now.Date;
                        entry.DueDate = DateTime.Now.Date.AddDays(7);
                        booktoborrow.BookStatus = Status.Borrowed;
                        libRepo.BookRepo.Update(booktoborrow);
                        libRepo.BorrowEntryRepo.Add(entry);
                        libRepo.Save();
                        TempData["SuccessNoti"] = "Borrow for member#"+entry.UserID+" success.";
                        return Check(entry);
                    }

                        //If book status is lost,notify user that this book is lost.
                    else if (booktoborrow.BookStatus == Status.Lost)
                    {
                        TempData["ErrorNoti"] = "This book is lost.";
                        return Check(entry);
                    }

                    else if (booktoborrow.BookStatus == Status.Reserved)
                    {
                       /* if book status is reserved check that whether request is expired or not 
                        * if yes delete expired req. and do borrow like normal case.
                        */ 
                        RequestEntry reqentry = booktoborrow.GetRelatedRequestEntry(ref libRepo);
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
                        /* check is current user that want to borrow book is same as in requestentry...
                         * if yes remove request entry and do borrow like normal case if not notify user 
                         * that no permission to borrow other request book.
                         */ 
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
                    //If book status is borrowed,notify user that this book is already borrowed.
                    else
                    {
                        TempData["ErrorNoti"] = "This book is already borrowed.";
                        return Check(entry);
                    }
               
        }

        /* [Private] MemberTransactionViewer Return(BorrowEntry entry)
         * first of all check that desired entry is exist or not base on entry's id,
         * next is check that whether return date is null or not(to detect whether this entry has passed entry),
         * continued by check that there is exist request that related to return book
         * if it is exists change book status to Reserved and update expire date then save it to database
         * if it is not exists change book status to Available and set return date then save it to database.
         * Moreover check that due date is more that presennt date or not,
         * if not it mean that user is return an overdue borrow book,just calculate fine with number of day that passed
         * in linear rate base on static fine configuration class.Finally return result to user.
         * Finally call "check" method or set return value to null to return and update borrow/return list for desired user.
         */
        private MemberTransactionViewer Return(BorrowEntry entry)
        {
            //check that desired entry is exist or not base on entry's id.
            BorrowEntry returnentry = libRepo.BorrowEntryRepo.Find(entry.ID);
            if (returnentry == null)
            {
                TempData["ErrorNoti"] = "No borrow record found to do return.";
                return null;
            }

            //check that whether return date is null or not(to detect whether this entry has passed entry)
            else if (returnentry.ReturnDate != null)
            {
                TempData["ErrorNoti"] = "This book is already returned.";
                return Check(returnentry);
            }

            /* check that there is exist request that related to return book
             * if it is exists change book status to Reserved and update expire date then save it to database
             * if it is not exists change book status to Available and set return date then save it to database
             */ 
            else
            {
                RequestEntry reqToCheck = returnentry.GetBorrowBook(ref libRepo).GetRelatedRequestEntry(ref libRepo);
                if (reqToCheck != null)
                {
                    Book bookToUpdate = returnentry.GetBorrowBook(ref libRepo);
                    bookToUpdate.BookStatus = Status.Reserved;
                    reqToCheck.ExpireDate = DateTime.Now.Date.AddDays(3);
                    libRepo.BookRepo.Update(bookToUpdate);
                    libRepo.RequestEntryRepo.Update(reqToCheck);
                }
                else
                {
                    Book bookToUpdate = returnentry.GetBorrowBook(ref libRepo);
                    bookToUpdate.BookStatus = Status.Available;
                    libRepo.BookRepo.Update(bookToUpdate);
                }

                /* check that due date is more that presennt date or not,
                 * if not it mean that user is return an overdue borrow book,
                 * just calculate fine with number of day that passed
                 * in linear rate base on static fine configuration class.
                 */ 
                if(returnentry.DueDate.Date < DateTime.Now.Date){
                    int dif = DateTime.Now.Subtract(returnentry.DueDate.Date).Days;
                    TempData["WarnNoti"] = "Return successfully.Fine " + (dif * ConfigurationController.getFine()) + " baht.";
                    }
                else
                    TempData["SuccessNoti"] = "Return successfully.";
                returnentry.ReturnDate = DateTime.Now.Date;
                libRepo.BorrowEntryRepo.Update(returnentry);
                libRepo.Save();

                return Check(returnentry);
            }

        }

        /* This method use to call index page for librarian transaction with
         * parameterized of page and pageSize use for paging borrowentry list then
         * return result to user.
         */ 
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

        //This method use to call HTTPGET of transaction page and return it to user.
        [Authorize]
        public ActionResult Transaction()
        {
            return View();
        }

        /* This method use to submit borrow/return data on HTTPPOST by passing borrow/return data
         * (include UserID,BookID and ID of borrowentry) and operation as string
         * check opeation that what operation it is.
         * If operation == "Check" -> Call check method and return result from it as page.
         * If operation == "Submit" -> Call borrow method and return result from it as page.
         * If operation == "Return" -> Call return method and return result from it as page.
         * If user is not enter UserID,notify user that please input UserID.
         */ 
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Transaction(BorrowEntry entry,string operation)
        {
            ModelState.Remove("BookID");
            //Memorized UserID and BookID data for return to page.
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

        /* [Override method]
         * This method use to check that whether current user for current session is exist in system or not.
         * If not,call and pass by reference of current HTTPrequest in AuthenticateController.OnInvalidSession
         * to set appropiate page result.Moreover check that current user is librarian if not redirect user to
         * account index page.
         */
        protected override void OnActionExecuting(ActionExecutingContext filterContext)
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

        /* [Override method]
         * This method use to handle exception that may occur in system
         * for some specific exception Redirect user to another page and pretend that no error occur
         * for another exception throw it and use HTTP error 500 page to handle instead.
         */
        protected override void OnException(ExceptionContext filterContext)
        {
            filterContext.ExceptionHandled = true;
            if (filterContext.Exception.GetType().Name == typeof(HttpAntiForgeryException).Name)
            {
                filterContext.Result = RedirectToAction("Index", "Account");
            }
            else
            {
                throw filterContext.Exception;
            }

        }
    }
}

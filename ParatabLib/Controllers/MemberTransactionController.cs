using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity;
using ParatabLib.Models;
using ParatabLib.DataAccess;
using ParatabLib.ViewModels;
namespace ParatabLib.Controllers
{
    //This class use to handle member transaction(Renew/Request book/Cancel request)
    public class MemberTransactionController : Controller
    {

        LibraryRepository libRepo;


        /* Both of constructors use to set libRepo properties whether to use default(instantiate new)
         * or via parameter.
         */ 
        public MemberTransactionController(LibraryRepository libRepo)
        {
            this.libRepo = libRepo;
        }

        public MemberTransactionController()
        {
            libRepo = new LibraryRepository();
        }

        /* This method use to call index page of member transaction and return it to user
         * with borrow/request data in MemberTransactionViewer object.
         */ 
        [Authorize]
        public ActionResult Index()
        {
                MemberTransactionViewer viewer = new MemberTransactionViewer();
                viewer.SetBorrowEntryViews(libRepo.BorrowEntryRepo.ListWhere(target => target.GetBorrower(ref libRepo).UserName ==
                                           HttpContext.User.Identity.Name.ToString().Substring(2) &&
                                           target.ReturnDate == null));
                viewer.SetRequestEntryViews((libRepo.RequestEntryRepo.ListWhere(target => target.GetRequestUser(ref libRepo).UserName ==
                                           HttpContext.User.Identity.Name.ToString().Substring(2))));
                return View(viewer);
            
        }

        /* This method use to call renew confirmation page for desired borrowentry(which contain borrow book data)
         * Find desired borrowentry base on id parameter if it exists return renew confirmation page,
         * If it not exists notify user that "Invalid renew book id."
         * Special case for this method1:If current wanted-to renew book is not match in borrow data
         * notify user that "Invalid renew operation."
         * Special case for this method2:If current wanted-to renew book's renew count is equal 3
         * notify user that cannot renew due to renew count is exceed maximum.
         */ 
        [Authorize]
        public ActionResult Renew(int id)
        {
            BorrowEntry renewentry = libRepo.BorrowEntryRepo.ListWhere(target => target.ID == id &&
                                        target.ReturnDate == null).SingleOrDefault();
            if (renewentry == null)
            {
                TempData["ErrorNoti"] = "Invalid renew book id.";
                return RedirectToAction("Index");
            }

            if (renewentry.GetBorrower(ref libRepo).UserName != Session["LoginUser"].ToString().Substring(2))
            {
                TempData["ErrorNoti"] = "Invalid renew operation.";
                return RedirectToAction("Index");
            }

            if (renewentry.RenewCount == 3)
            {
                TempData["ErrorNoti"] = "Your renew of book ID." + renewentry.GetBorrowBook(ref libRepo).BookID + " is exceed maximum!";
                return RedirectToAction("Index");
            }
            return View(renewentry);
        }


        /* This method use to submit renew confirmation,simply check that whether that book is already requested,
         * if yes notify user that "This book is ON HOLD." otherwise update duedate by 7 days,count renew time by 1
         * and save it to database,finally notify success result.
         */ 
        [Authorize]
        [ValidateAntiForgeryToken]
        [HttpPost]
        public ActionResult Renew(BorrowEntry entry)
        {
            if (ModelState.IsValid)
            {
                if (libRepo.RequestEntryRepo.List().LastOrDefault(target => target.BookID == entry.BookID && target.ExpireDate == null) != null)
                {
                    TempData["ErrorNoti"] = "This book is ON HOLD.";
                }
                else
                {
                    entry.DueDate = DateTime.Now.Date.AddDays(7);
                    entry.RenewCount++;
                    libRepo.BorrowEntryRepo.Update(entry);
                    libRepo.Save();
                    TempData["SuccessNoti"] = "Renew successful!";
                }
            }
            return RedirectToAction("Index");
        }

        //This method simply call request page and return it to user.
        [Authorize]
        public ActionResult Request()
        {
            return View();
        }

        /* This method use to submit book that want to requested by passing entry as RequestEntry.
         * First of all,check that whether desired book is exists in database,
         * next check status of book that is that book is borrowed then 
         * check that desired book is already requested or not the last check is
         * check that current request user is same as current borrower's desired book
         * if last check is not add request entry to mark that desired is requested.
         * Finally notify user for success result.
         */ 
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Request(RequestEntry entry)
        {
            if (ModelState.IsValid)
            {
                //Check that whether desired book is exists in database.
                Book booktorequest;
                if ((booktorequest = libRepo.BookRepo.Find(entry.BookID)) == null)
                {
                    TempData["ErrorNoti"] = "No book with prefer ID exists.";
                    return View();
                }
                //Check status of book that is that book is borrowed.
                if (booktorequest.BookStatus != Status.Borrowed && booktorequest.BookStatus != Status.Reserved)
                {
                    TempData["ErrorNoti"] = "Can't request this book due to it is "
                        + booktorequest.BookStatus.ToString() + ".";
                    return View();
                }

                //Check that desired book is already requested or not.
                if (libRepo.RequestEntryRepo.List().LastOrDefault(target => target.BookID == booktorequest.BookID
                                && target.ExpireDate == null) != null || booktorequest.BookStatus == Status.Reserved)
                {
                    TempData["ErrorNoti"] = "This book is already requested.";
                    return View();
                }

                //Improve this code
                /* Check that current request user is same as current borrower's desired book
                 * if this check is not add request entry to mark that desired is requested.
                 */
                Member request_member = libRepo.MemberRepo.ListWhere(target => target.UserName ==
                                                HttpContext.User.Identity.Name.ToString().Substring(2)).Single();

                if (libRepo.BorrowEntryRepo.List().LastOrDefault(target => target.BookID == entry.BookID &&
                                                        target.GetBorrower(ref libRepo) == request_member && target.ReturnDate == null) != null)
                {
                    TempData["ErrorNoti"] = "Can't request your current borrowed book.";
                    return View();
                }

                entry.UserID = request_member.UserID;
                entry.RequestDate = DateTime.Now;
                libRepo.RequestEntryRepo.Add(entry);
                libRepo.Save();
                TempData["SuccessNoti"] = "Request book successfully.";
                return RedirectToAction("Index");
            }
            else
            {
                TempData["ErrorNoti"] = "Input was not in correct format.";
                return View();
            }
        }

        /* This method is use to call cancelRequest confirmation page on desired requestentry
         * check that desired requestentry is exist or not base on id.
         * If no notify that "No cancel request with prefered id exists."
         * If yes check that current user is same as request user that contain in database to prevent hack,
         * if last check is passed return cancelRequest confirmation page with desired requestentry
         * if last check is not passed,notify that "Can't cancel other member's book request."
         */ 
        [Authorize]
        public ActionResult CancelRequest(int id)
        {
            RequestEntry wantedEntry = libRepo.RequestEntryRepo.Find(id);
            if (wantedEntry == null)
            {
                TempData["ErrorNoti"] = "No cancel request with prefered id exists.";
                return RedirectToAction("Index");
            }
            Member preferMember = libRepo.MemberRepo.ListWhere(target => target.UserName == HttpContext.User.Identity.Name.ToString().Substring(2)).SingleOrDefault();
            if (wantedEntry.GetRequestUser(ref libRepo).UserID != preferMember.UserID)
            {
                TempData["ErrorNoti"] = "Can't cancel other member's book request.";
                return RedirectToAction("Index");
            }
            return View(wantedEntry);
        }

        /* This method use to submit cancelRequest confirmation of desired entry on HTTPPOST
         * First of all,check that the data that passed in view before user confirmation occured is changed,
         * if yes,notify user that "Something went wrong,please try again."
         * if no check status of request book that is it Requested if yes change it to Available
         * then remove desire entry from database with notify of success result.
         */ 
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CancelRequest(RequestEntry entry)
        {
            if (ModelState.IsValid)
            {
                    Book bookToCheck = libRepo.BookRepo.Find(entry.BookID);
                    if (bookToCheck == null)
                    {
                        TempData["ErrorNoti"] = "Something went wrong,please try again.";
                        return RedirectToAction("Index");
                    }
                    if (bookToCheck.BookStatus == Status.Reserved)
                    {
                            bookToCheck.BookStatus = Status.Available;
                            libRepo.BookRepo.Update(bookToCheck);
                    }
                    libRepo.RequestEntryRepo.Remove(entry);
                    libRepo.Save();
                    TempData["SuccessNoti"] = "Cancel request successfully.";
                    return RedirectToAction("Index");
            }
            TempData["ErrorNoti"] = "Something went wrong,please try again.";
            return RedirectToAction("Index");
        }

        /* This method use to view borrow history page base on current user session with
         * parameterized of page and pageSize to paging borrow history list then return
         * result to user.
         */ 
        [Authorize]
        public ActionResult BorrowHistory(int page = 1,int pageSize = 10)
        {
            string username;
            username = HttpContext.User.Identity.Name.ToString().Substring(2);
            Member currentMember = libRepo.MemberRepo.ListWhere(target => target.UserName == username).SingleOrDefault();
            List<BorrowEntry> EntryList = currentMember.GetRelatedBorrowEntry(ref libRepo);
            PageList<BorrowEntry> pglist = new PageList<BorrowEntry>(EntryList,page,pageSize);
            TempData["BorrowCount"] = EntryList.Count();
            switch (pglist.Categorized())
            {
                case PageListResult.Ok: { return View(pglist); }
                case PageListResult.Empty:
                    {
                        TempData["ErrorNoti"] = "No borrow history to show.Please do transaction to see your history.";
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
         * to set appropiate page result.Moreover check that current user is member if not redirect user to
         * account index page.
         */
        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
                if (AuthenticateController.IsUserValid(HttpContext.User.Identity.Name.Substring(2)))
                {
                    Session["LoginUser"] = HttpContext.User.Identity.Name;
                    if (HttpContext.User.Identity.Name.ToString().Substring(0, 2) != "M_")
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

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
    public class MemberTransactionController : Controller
    {

        LibraryRepository libRepo;

        public MemberTransactionController(LibraryRepository libRepo)
        {
            this.libRepo = libRepo;
        }

        public MemberTransactionController()
        {
            libRepo = new LibraryRepository();
        }

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


        [Authorize]
        public ActionResult Request()
        {
            return View();
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Request(RequestEntry entry)
        {
            if (ModelState.IsValid)
            {
                Book booktorequest;
                if ((booktorequest = libRepo.BookRepo.Find(entry.BookID)) == null)
                {
                    TempData["ErrorNoti"] = "No book with prefer ID exists.";
                    return View();
                }
                if (booktorequest.BookStatus != Status.Borrowed && booktorequest.BookStatus != Status.Reserved)
                {
                    TempData["ErrorNoti"] = "Can't request this book due to it is "
                        + booktorequest.BookStatus.ToString() + ".";
                    return View();
                }

                if (libRepo.RequestEntryRepo.List().LastOrDefault(target => target.BookID == booktorequest.BookID
                                && target.ExpireDate == null) != null || booktorequest.BookStatus == Status.Reserved)
                {
                    TempData["ErrorNoti"] = "This book is already requested.";
                    return View();
                }

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
    }
}

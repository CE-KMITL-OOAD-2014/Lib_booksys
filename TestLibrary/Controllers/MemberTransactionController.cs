using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity;
using TestLibrary.Models;
using TestLibrary.DataAccess;
using TestLibrary.ViewModels;
namespace TestLibrary.Controllers
{
    public class MemberTransactionController : Controller
    {

        LibraryRepository libRepo = new LibraryRepository();
        [Authorize]
        public ActionResult Index()
        {
            Session["LoginUser"] = HttpContext.User.Identity.Name;
            if (HttpContext.User.Identity.Name.ToString().Substring(0, 2) == "M_")
            {
                MemberTransactionViewer viewer = new MemberTransactionViewer();
                viewer.SetBorrowEntryViews(libRepo.BorrowEntryRepo.ListWhere(target => target.GetBorrower().UserName ==
                                           HttpContext.User.Identity.Name.ToString().Substring(2) &&
                                           target.ReturnDate == null));
                viewer.SetRequestEntryViews((libRepo.RequestEntryRepo.ListWhere(target => target.GetRequestUser().UserName ==
                                           HttpContext.User.Identity.Name.ToString().Substring(2))));
                return View(viewer);
            }
            else
                return RedirectToAction("Index", "Account");
        }

        [Authorize]
        public ActionResult Renew(int id)
        {
            Session["LoginUser"] = HttpContext.User.Identity.Name;
            if (HttpContext.User.Identity.Name.ToString().Substring(0, 2) != "M_")
                return RedirectToAction("Index", "Account");

            BorrowEntry renewentry = libRepo.BorrowEntryRepo.ListWhere(target => target.ID == id &&
                                        target.ReturnDate == null).SingleOrDefault();
            if (renewentry == null)
            {
                TempData["Notification"] = "Invalid renew book id.";
                return RedirectToAction("Index");
            }

            if (renewentry.GetBorrower().UserName != Session["LoginUser"].ToString().Substring(2))
            {
                TempData["Notification"] = "Invalid renew operation.";
                return RedirectToAction("Index");
            }

            if (renewentry.RenewCount == 3)
            {
                TempData["Notification"] = "Your renew of book ID." + renewentry.GetBorrowBook().BookID + " is exceed maximum!";
                return RedirectToAction("Index");
            }
            return View(renewentry);
        }


        [Authorize]
        [ValidateAntiForgeryToken]
        [HttpPost]
        public ActionResult Renew(BorrowEntry entry, string answer)
        {
            if (ModelState.IsValid && answer == "Yes")
            {
                if (libRepo.RequestEntryRepo.List().LastOrDefault(target => target.BookID == entry.BookID && target.ExpireDate == null) != null)
                {
                    TempData["Notification"] = "This book is ON HOLD.";
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
            Session["LoginUser"] = HttpContext.User.Identity.Name;
            if (HttpContext.User.Identity.Name.ToString().Substring(0, 2) != "M_")
                return RedirectToAction("Index", "Account");
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
                    TempData["Notification"] = "No book with prefer ID exists.";
                    return View();
                }
                if (booktorequest.BookStatus != Status.Borrowed && booktorequest.BookStatus != Status.Reserved)
                {
                    TempData["Notification"] = "Can't request this book due to it is "
                        + booktorequest.BookStatus.ToString() + ".";
                    return View();
                }

                if (libRepo.RequestEntryRepo.List().LastOrDefault(target => target.BookID == booktorequest.BookID
                                && target.ExpireDate == null) != null || booktorequest.BookStatus == Status.Reserved)
                {
                    TempData["Notification"] = "This book is already requested.";
                    return View();
                }

                Member request_member = libRepo.MemberRepo.ListWhere(target => target.UserName ==
                                                HttpContext.User.Identity.Name.ToString().Substring(2)).Single();

                if (libRepo.BorrowEntryRepo.List().LastOrDefault(target => target.BookID == entry.BookID &&
                                                        target.GetBorrower() == request_member && target.ReturnDate == null) != null)
                {
                    TempData["Notification"] = "Can't request your current borrowed book.";
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
                return View();
        }

        [Authorize]
        public ActionResult CancelRequest(int id)
        {
            Session["LoginUser"] = HttpContext.User.Identity.Name;
            if (HttpContext.User.Identity.Name.ToString().Substring(0, 2) != "M_")
                return RedirectToAction("Index", "Account");
            RequestEntry wantedEntry = libRepo.RequestEntryRepo.Find(id);
            if (wantedEntry == null)
            {
                TempData["Notification"] = "No cancel request with prefered id exists.";
                return RedirectToAction("Index");
            }
            Member preferMember = libRepo.MemberRepo.ListWhere(target => target.UserName == HttpContext.User.Identity.Name.ToString().Substring(2)).SingleOrDefault();
            if (wantedEntry.GetRequestUser().UserID != preferMember.UserID)
            {
                TempData["Notification"] = "Can't cancel other member's book request.";
                return RedirectToAction("Index");
            }
            return View(wantedEntry);
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CancelRequest(RequestEntry entry, string answer)
        {
            if (ModelState.IsValid)
            {
                if (answer == "Yes")
                {
                    Book bookToCheck = libRepo.BookRepo.Find(entry.BookID);
                    if (bookToCheck == null)
                    {
                        TempData["Notification"] = "Something went wrong,please try again.";
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
                }
                return RedirectToAction("Index");
            }
            TempData["Notification"] = "Something went wrong,please try again.";
            return RedirectToAction("Index");
        }

        [Authorize]
        public ActionResult BorrowHistory(int page = 1,int pageSize = 10)
        {
            Session["LoginUser"] = HttpContext.User.Identity.Name;
            if (HttpContext.User.Identity.Name.ToString().Substring(0, 2) != "M_")
                return RedirectToAction("Index", "Account");
            string username;
            username = HttpContext.User.Identity.Name.ToString().Substring(2);
            Member currentMember = libRepo.MemberRepo.ListWhere(target => target.UserName == username).SingleOrDefault();
            List<BorrowEntry> EntryList = currentMember.GetRelatedBorrowEntry();
            PageList<BorrowEntry> pglist = new PageList<BorrowEntry>(EntryList,page,pageSize);
            TempData["BorrowCount"] = EntryList.Count();
            switch (pglist.Categorized())
            {
                case PageListResult.Ok: { return View(pglist); }
                case PageListResult.Empty:
                    {
                        TempData["Notification"] = "No borrow history to show.Please do transaction to see your history.";
                        return View();
                    }
                default:
                    {
                        TempData["Notification"] = "Invalid list view parameter please refresh this page to try again.";
                        return View();
                    }
            }
        }
    }
}

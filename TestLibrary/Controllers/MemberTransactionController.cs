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
        LibraryContext db = new LibraryContext();
        [Authorize]
        public ActionResult Index()
        {
            Session["LoginUser"] = HttpContext.User.Identity.Name;
            if (HttpContext.User.Identity.Name.ToString().Substring(0, 2) == "M_")
            {
                MemberTransactionViewer viewer = new MemberTransactionViewer();
                viewer.SetBorrowEntryViews(db.BorrowList.Where(target => target.Borrower.UserName ==
                                           HttpContext.User.Identity.Name.ToString().Substring(2) &&
                                           target.ReturnDate == null).ToList());
                viewer.SetRequestEntryViews((db.RequestList.Where(target => target.RequestUser.UserName ==
                                           HttpContext.User.Identity.Name.ToString().Substring(2)).ToList()));

                List<RequestEntry> temp = new List<RequestEntry>();
                temp.AddRange(viewer.GetRequestEntryViews());
                foreach (RequestEntry item in temp)
                {
                    if (item.ExpireDate != null)
                    {
                        if (DateTime.Now.Date > item.ExpireDate.Value.Date)
                        {
                            viewer.GetRequestEntryViews().Remove(item);
                        }
                    }
                }
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

            BorrowEntry renewentry = db.BorrowList.SingleOrDefault(target => target.ID == id &&
                                        target.ReturnDate == null);
            if (renewentry == null)
            {
                TempData["Notification"] = "Invalid renew book id.";
                return RedirectToAction("Index");
            }

            if (renewentry.Borrower.UserName != Session["LoginUser"].ToString().Substring(2))
            {
                TempData["Notification"] = "Invalid renew operation.";
                return RedirectToAction("Index");
            }

            if (renewentry.RenewCount == 3)
            {
                TempData["Notification"] = "Your renew of book ID." + renewentry.BorrowBook.BookID + " is exceed maximum!";
                return RedirectToAction("Index");
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

                if (db.BorrowList.ToList().LastOrDefault(target => target.BookID == newentry.BookID &&
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
            RequestEntry wantedEntry = db.RequestList.Find(id);
            if (wantedEntry == null)
            {
                TempData["Notification"] = "No cancel request with prefered id exists.";
                return RedirectToAction("Index");
            }
            Member preferMember = db.Members.SingleOrDefault(target => target.UserName == HttpContext.User.Identity.Name.ToString().Substring(2));
            if (wantedEntry.RequestUser != preferMember)
            {
                TempData["Notification"] = "Can't cancel other member's book request.";
                return RedirectToAction("Index");
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
                        return RedirectToAction("Index");
                    }
                    if (bookToCheck.BookStatus == Status.Reserved)
                    {
                            bookToCheck.BookStatus = Status.Available;
                            db.Entry(bookToCheck).State = EntityState.Modified;
                    }
                    db.Entry(cancelEntry).State = EntityState.Deleted;

                    db.SaveChanges();
                    TempData["Notification"] = "Cancel request successfully.";
                }
                return RedirectToAction("Index");
            }
            TempData["Notification"] = "Something went wrong,please try again.";
            return RedirectToAction("Index");
        }
        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }

    }
}

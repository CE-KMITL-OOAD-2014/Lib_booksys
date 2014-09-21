using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TestLibrary.Models;
using TestLibrary.DataAccess;
using System.Data.Entity;
namespace TestLibrary.Controllers
{
    public class AdminTransactionController : Controller
    {

        LibraryContext db = new LibraryContext();

        [Authorize]
        public ActionResult Index()
        {
            Session["LoginUser"] = HttpContext.User.Identity.Name;
            if (HttpContext.User.Identity.Name.ToString().Substring(0, 2) != "A_")
                return RedirectToAction("Index", "Account");
            return View(db.BorrowList.ToList());
        }

        //Show viewtable if user click 'Check'
        //To decide whether prefer request is OK?
        [Authorize]
        public ActionResult Borrow()
        {
            if (HttpContext.User.Identity.Name.ToString().Substring(0, 2) != "A_")
                return RedirectToAction("Index", "Account");
            return View();
        }


        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Borrow(BorrowEntry newentry)
        {
            if (ModelState.IsValid)
            {
                if (db.Members.Find(newentry.UserID) == null)
                {
                    TempData["Notification"] = "No member that id's exists.";
                    return View();
                }

                if (db.BorrowList.Where(target => target.UserID == newentry.UserID
                    && target.ReturnDate == null).ToList().Count == 3)
                {
                    TempData["Notification"] = "This member borrow exceed maximum allowed.";
                    return View();
                }

                Book booktoborrow = db.Books.Find(newentry.BookID);

                if (booktoborrow == null)
                {
                    TempData["Notification"] = "No book was found in database.";
                    return View();
                }
                else if (booktoborrow.BookStatus == Status.Available)
                {
                    newentry.BorrowDate = DateTime.Now.Date;
                    newentry.DueDate = DateTime.Now.Date.AddDays(7);
                    booktoborrow.BookStatus = Status.Borrowed;
                    db.Entry(booktoborrow).State = EntityState.Modified;
                    db.BorrowList.Add(newentry);
                    db.SaveChanges();
                    TempData["Notification"] = "OK";
                    return View();
                }
                else if (booktoborrow.BookStatus == Status.Lost)
                {
                    TempData["Notification"] = "This book is lost.";
                    return View();
                }

                else if (booktoborrow.BookStatus == Status.Reserved)
                {
                    RequestEntry reqentry = db.RequestList.ToList().LastOrDefault(target => target.RequestBook == booktoborrow);
                    if (reqentry.ExpireDate.Value.Date < DateTime.Now.Date)
                    {
                        db.Entry(reqentry).State = EntityState.Deleted;
                        booktoborrow.BookStatus = Status.Borrowed;
                        db.Entry(booktoborrow).State = EntityState.Modified;
                        newentry.BorrowDate = DateTime.Now.Date;
                        newentry.DueDate = DateTime.Now.Date.AddDays(7);
                        db.BorrowList.Add(newentry);
                        db.SaveChanges();
                        TempData["Notification"] = "Delete expire req.//OK.";
                        return View();
                    }
                    else if (reqentry.UserID != newentry.UserID)
                    {
                        TempData["Notification"] = "This user has no permission to borrow the requested book by others.";
                        return View();
                    }
                    else
                    {
                        db.Entry(reqentry).State = EntityState.Deleted;
                        booktoborrow.BookStatus = Status.Borrowed;
                        newentry.BorrowDate = DateTime.Now.Date;
                        newentry.DueDate = DateTime.Now.Date.AddDays(7);
                        db.Entry(booktoborrow).State = EntityState.Modified;
                        db.BorrowList.Add(newentry);
                        db.SaveChanges();
                        TempData["Notification"] = "User accept reserved book//OK";
                        return View();
                    }
                }

                else
                {
                    TempData["Notification"] = "This book is already borrowed.";
                    return View();
                }
            }
            else return View();
        }

        //Input UserID and wait for return value op.from viewtable
        [Authorize]
        public ActionResult Return()
        {
            Session["LoginUser"] = HttpContext.User.Identity.Name;
            if (HttpContext.User.Identity.Name.ToString().Substring(0, 2) != "A_")
                return RedirectToAction("Index", "Account");
            return View();
        }

        [Authorize]
        [ValidateAntiForgeryToken]
        [HttpPost]
        public ActionResult Return(int? BookID)
        {


            BorrowEntry targetentry = db.BorrowList.ToList().LastOrDefault(target => target.BookID == BookID
                && target.ReturnDate == null);
            if (targetentry == null)
            {
                TempData["Notification"] = "No book to return in database.";
                return View();
            }

            RequestEntry reqentry = db.RequestList.ToList().LastOrDefault(target => target.BookID == BookID && target.ExpireDate == null);
            if (reqentry != null)
            {
                targetentry.BorrowBook.BookStatus = Status.Reserved;
                reqentry.ExpireDate = DateTime.Now.Date.AddDays(3);
            }
            else
                targetentry.BorrowBook.BookStatus = Status.Available;

            targetentry.ReturnDate = DateTime.Now;
            db.Entry(targetentry).State = EntityState.Modified;
            db.SaveChanges();
            TempData["Notification"] = "Return book success!";
            return View();
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}

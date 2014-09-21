using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TestLibrary.Models;
using TestLibrary.DataAccess;
using TestLibrary.ViewModels;
using System.Data.Entity;
namespace TestLibrary.Controllers
{
    public class AdminTransactionController : Controller
    {

        LibraryContext db = new LibraryContext();
        private MemberTransactionViewer Check(BorrowEntry newentry)
        {
            if (db.Members.Find(newentry.UserID) == null)
            {
                TempData["Notification"] = "No member that id's exists.";
                return null;
            }

            MemberTransactionViewer viewer = new MemberTransactionViewer();
            viewer.SetBorrowEntryViews(db.BorrowList.Where(entry => entry.UserID == newentry.UserID
                && entry.ReturnDate == null).ToList());

            List<RequestEntry> ReqList = db.RequestList.ToList();
            List<RequestEntry> expireList = ReqList.Where(entry => entry.ExpireDate != null).ToList();

            //Call list of requestlist of preferred member that expiredate field is null.
            List<RequestEntry> WantedList = ReqList.Where(entry => entry.UserID == newentry.UserID
                                            && entry.ExpireDate == null).ToList();
            //Append wantedList with expiredate in range
            WantedList.AddRange(expireList.Where(entry => entry.UserID == newentry.UserID
                                                && entry.ExpireDate >= DateTime.Now.Date).ToList());

            //Fetch requestList up-to-date by delete expire list.
            expireList = expireList.Where(entry => entry.ExpireDate < DateTime.Now.Date).ToList();
            if (expireList != null)
            {
                foreach (var item in expireList)
                    item.RequestBook.BookStatus = Status.Available;

                db.RequestList.RemoveRange(expireList);
            }
            db.SaveChanges();
            viewer.SetRequestEntryViews(WantedList);
            return viewer;
        }


        private MemberTransactionViewer Borrow(BorrowEntry newentry)
        {
            if (db.Members.Find(newentry.UserID) == null)
                    {
                        TempData["Notification"] = "No member that id's exists.";
                        return null;
                    }

                    if (db.BorrowList.Where(target => target.UserID == newentry.UserID
                        && target.ReturnDate == null).ToList().Count == 3)
                    {
                        TempData["Notification"] = "This member borrow exceed maximum allowed.";
                        return Check(newentry);
                    }

                    Book booktoborrow = db.Books.Find(newentry.BookID);

                    if (booktoborrow == null)
                    {
                        TempData["Notification"] = "No book was found in database.";
                        return Check(newentry);
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
                        return Check(newentry);
                    }
                    else if (booktoborrow.BookStatus == Status.Lost)
                    {
                        TempData["Notification"] = "This book is lost.";
                        return Check(newentry);
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
                            return Check(newentry);
                        }
                        else if (reqentry.UserID != newentry.UserID)
                        {
                            TempData["Notification"] = "This user has no permission to borrow the requested book by others.";
                            return Check(newentry);
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
                            return Check(newentry);
                        }
                    }

                    else
                    {
                        TempData["Notification"] = "This book is already borrowed.";
                        return Check(newentry);
                    }
               
        }

        private MemberTransactionViewer Return(BorrowEntry returnentry)
        {
            BorrowEntry entry = db.BorrowList.Find(returnentry.ID);
            if (entry == null)
            {
                TempData["Notification"] = "No borrow record found to do return.";
                return null;
            }
            else if (entry.ReturnDate != null)
            {
                TempData["Notification"] = "This book is already returned.";
                return Check(entry);
            }
            else
            {
                if (entry.BorrowBook.RequestRecord != null)
                {
                    entry.BorrowBook.BookStatus = Status.Reserved;
                    entry.BorrowBook.RequestRecord.ExpireDate = DateTime.Now.Date.AddDays(3);
                }
                else
                {
                    entry.BorrowBook.BookStatus = Status.Available;
                }
                entry.ReturnDate = DateTime.Now.Date;
                db.Entry(entry).State = EntityState.Modified;
                db.SaveChanges();
                TempData["Notification"] = "Return successfully.";
                return Check(entry);
            }

        }


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
        public ActionResult Transaction()
        {
            if (HttpContext.User.Identity.Name.ToString().Substring(0, 2) != "A_")
                return RedirectToAction("Index", "Account");
            return View();
        }


        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Transaction(BorrowEntry newentry,string operation)
        {
            ModelState.Remove("BookID");
            TempData["UserID"] = newentry.UserID;
            TempData["BookID"] = (newentry.BookID == 0)?null:newentry.BookID.ToString();
            if (ModelState.IsValid)
            {
                if (operation == "Check")
                {
                    return View(Check(newentry));
                }

                else if (operation == "Submit")
                {
                    return View(Borrow(newentry));
                }
                else if (operation == "Return")
                {
                    return View(Return(newentry));
                }
                else
                {
                    TempData["Notification"] = "Invalid operation.";
                    return View();
                }
            }
            else return View();
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}

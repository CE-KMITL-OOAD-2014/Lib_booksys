using System;
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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TestLibrary.Models;
using System.Data.Entity;
using TestLibrary.DataAccess;
using System.Web.Security;
using System.Data.Entity.Validation;
using System.ComponentModel;
namespace TestLibrary.Controllers
{
    public class ManageController : Controller
    {
       
        LibraryContext db = new LibraryContext();

        [Authorize]
        public ActionResult Index()
        {
            Session["LoginUser"] = HttpContext.User.Identity.Name;
            if (HttpContext.User.Identity.Name.ToString().Substring(0, 2) != "A_")
                return RedirectToAction("Index", "Member");
            return View();
        }

        [Authorize]
        public ActionResult Detail()
        {
            Session["LoginUser"] = HttpContext.User.Identity.Name;
            if (HttpContext.User.Identity.Name.ToString().Substring(0, 2) != "A_")
                return RedirectToAction("Index", "Member");
            Admin a = db.Admins.Where(s => s.UserName == HttpContext.User.Identity.Name.ToString().Substring(2)).SingleOrDefault();
            return View(a);
        }

        [Authorize]
        public ActionResult Edit()
        {
            Session["LoginUser"] = HttpContext.User.Identity.Name;
            if (HttpContext.User.Identity.Name.ToString().Substring(0, 2) != "A_")
                return RedirectToAction("Index", "Member");
            Admin a = db.Admins.Where(s => s.UserName == HttpContext.User.Identity.Name.ToString().Substring(2)).SingleOrDefault();
            return View(a);
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Admin admin)
        {
            if (ModelState.IsValid)
            {
                db.Entry(admin).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Detail");
            }
            return View(admin);
        }
        [Authorize]
        public ActionResult ChangePassword()
        {
            Session["LoginUser"] = HttpContext.User.Identity.Name;
            if (HttpContext.User.Identity.Name.ToString().Substring(0, 2) != "A_")
                return RedirectToAction("Index", "Member");
            Admin a = db.Admins.Where(s => s.UserName == HttpContext.User.Identity.Name.ToString().Substring(2)).SingleOrDefault();
            return View(a);
        }

        [Authorize]
        public ActionResult AddAdmin()
        {
            Session["LoginUser"] = HttpContext.User.Identity.Name;
            if (HttpContext.User.Identity.Name.ToString().Substring(0, 2) != "A_")
                return RedirectToAction("Index", "Member");
            return View();
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddAdmin(Admin admin)
        {
            
            if (ModelState.IsValid)
            {
                if ((db.Admins.Where(s => s.UserName == admin.UserName).SingleOrDefault() == null)&&
                    (db.Members.Where(m => m.UserName == admin.UserName).SingleOrDefault() == null))
                {
                    db.Entry(admin).State = EntityState.Added;
                    db.SaveChanges();
                    TempData["Notification"] = "Add admin " + admin.UserName + " successfully.";
                    return RedirectToAction("AdminList");
                }
                ModelState.AddModelError("UserName","This username is already exists.");
            }
            return View(admin);
        }

        [Authorize]
        public ActionResult Delete([DefaultValue(0)] int id)
        {
            Session["LoginUser"] = HttpContext.User.Identity.Name;
            if (HttpContext.User.Identity.Name.ToString().Substring(0, 2) != "A_")
                return RedirectToAction("Index", "Member");
            Admin target = db.Admins.SingleOrDefault(targetadmin => targetadmin.UserID == id);
            if (target == null)
                return HttpNotFound();
            return View(target);
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id,string answer)
        {
            if (answer == "Yes")
            {
                Admin admintodelete = db.Admins.Find(id);
                TempData["Notification"] = "Delete " + admintodelete.UserName + " successfully.";
                db.Admins.Remove(admintodelete);
                db.SaveChanges();
            }
                return RedirectToAction("AdminList");
                
        }

        [Authorize]
        public ActionResult AdminList()
        {
            Session["loginUser"] = HttpContext.User.Identity.Name;
            if (HttpContext.User.Identity.Name.ToString().Substring(0, 2) != "A_")
                return RedirectToAction("Index", "Member");
            return View(db.Admins.ToList());
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ChangePassword(Admin admin,string oldpass,string confirm)
        {
            if (ModelState.IsValid)
            {
                Admin a = db.Admins.Where(s => s.UserID == admin.UserID).SingleOrDefault();
               if (a.Password == oldpass && admin.Password == confirm)
                {
                    a.Password = admin.Password;
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
                else
                    return View();
            }
            return View();
        }
        

        [Authorize]
        public ActionResult AddBook()
        {
            Session["LoginUser"] = HttpContext.User.Identity.Name;
            if (HttpContext.User.Identity.Name.ToString().Substring(0, 2) != "A_")
                return RedirectToAction("Index", "Member");
            return View();
        }

        [Authorize]
        [ValidateAntiForgeryToken]
        [HttpPost]
        public ActionResult AddBook(Book bookToAdd)
        {
            if (ModelState.IsValid)
            {
                db.Entry(bookToAdd).State = EntityState.Added;
                db.SaveChanges();
                TempData["Notification"] = "Add new book successfully.";
                return RedirectToAction("BookList");
            }
            return View(bookToAdd);
        }

        [Authorize]
        public ActionResult BookList()
        {
            Session["LoginUser"] = HttpContext.User.Identity.Name;
            if (HttpContext.User.Identity.Name.ToString().Substring(0, 2) != "A_")
                return RedirectToAction("Index", "Member");
            return View(db.Books.ToList());
        }

        [Authorize]
        public ActionResult ViewBook([DefaultValue(0)]int id)
        {
            Session["LoginUser"] = HttpContext.User.Identity.Name;
            if (HttpContext.User.Identity.Name.ToString().Substring(0, 2) != "A_")
                return RedirectToAction("Index", "Member");
            Book booktoview = db.Books.Find(id);
            if (booktoview == null)
                return HttpNotFound();
            else
                return View(booktoview);
        }

        [Authorize]
        public ActionResult EditBook([DefaultValue(0)]int id)
        {
            Session["LoginUser"] = HttpContext.User.Identity.Name;
            if (HttpContext.User.Identity.Name.ToString().Substring(0, 2) != "A_")
                return RedirectToAction("Index", "Member");
            Book booktoedit = db.Books.Find(id);
            if (booktoedit == null)
                return HttpNotFound();
            return View(booktoedit);
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Editbook(Book booktoedit)
        {
            if (ModelState.IsValid)
            {
                db.Entry(booktoedit).State = EntityState.Modified;
                TempData["Notification"] = "Edit book successfully.";
                db.SaveChanges();
                return RedirectToAction("BookList");
            }
            return View(booktoedit);
        }



        [Authorize]
        public ActionResult DeleteBook([DefaultValue(0)]int id){
            Session["LoginUser"] = HttpContext.User.Identity.Name;
            if (HttpContext.User.Identity.Name.ToString().Substring(0, 2) != "A_")
                return RedirectToAction("Index", "Member");
            Book booktodelete = db.Books.Find(id);
            if (booktodelete == null)
                return HttpNotFound();
            return View(booktodelete);
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteBook(Book booktodelete,string answer)
        {
            if (answer == "Yes")
            {
                TempData["Notification"] = "Delete " + booktodelete.BookName + " successfully.";
                db.Entry(booktodelete).State = EntityState.Deleted;
                db.SaveChanges();
                return RedirectToAction("BookList");
            }
            return RedirectToAction("BookList");
        }

        [Authorize]
        public ActionResult Borrow()
        {
            if (HttpContext.User.Identity.Name.ToString().Substring(0, 2) != "A_")
                return RedirectToAction("Index", "Member");
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
                    newentry.BorrowDate = DateTime.Now;
                    newentry.DueDate = DateTime.Now.AddDays(7);
                    booktoborrow.BookStatus = Status.Borrowed;
                    db.Entry(booktoborrow).State = EntityState.Modified;
                    db.BorrowList.Add(newentry);
                    db.SaveChanges();
                    TempData["Notification"] = "OK";
                    return View();
                }
                else
                {
                    TempData["Notification"] = "This book is already borrowed.";
                    return View();
                }
            }
            else return View();
        }

        [Authorize]
        public ActionResult Return()
        {
            Session["LoginUser"] = HttpContext.User.Identity.Name;
            if (HttpContext.User.Identity.Name.ToString().Substring(0, 2) != "A_")
                return RedirectToAction("Index", "Member");
            return View();
        }

        [Authorize]
        [ValidateAntiForgeryToken]
        [HttpPost]
        public ActionResult Return(int? BookID)
        {
            ModelState.Remove("UserID");

            
                BorrowEntry targetentry = db.BorrowList.ToList().LastOrDefault(target => target.BookID == BookID 
                    && target.ReturnDate == null);
                if (targetentry == null)
                {
                    TempData["Notification"] = "No book to return in database.";
                    return View();
                }
                targetentry.BorrowBook.BookStatus = Status.Available;
                targetentry.ReturnDate = DateTime.Now;
                db.Entry(targetentry).State = EntityState.Modified;
                db.SaveChanges();
                TempData["Notification"] = "Return book success!";
            return View();
        }



        [Authorize]
        public ActionResult BorrowTracking()
        {
            Session["LoginUser"] = HttpContext.User.Identity.Name;
            if (HttpContext.User.Identity.Name.ToString().Substring(0, 2) != "A_")
                return RedirectToAction("Index", "Member");
            return View(db.BorrowList.Include(list => list.BorrowBook).Include(list => list.Borrower).ToList());
        }




        [Authorize]
        public ActionResult AddNews()
        {
            Session["LoginUser"] = HttpContext.User.Identity.Name;
            if (HttpContext.User.Identity.Name.ToString().Substring(0, 2) != "A_")
                return RedirectToAction("Index", "Member");
            return View();
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddNews(News newstoadd)
        {
            if (ModelState.IsValid)
            {
                newstoadd.PostTime = DateTime.Now;
                db.Entry(newstoadd).State = EntityState.Added;
                db.SaveChanges();
                TempData["Notification"] = "Add news successfully.";
                return RedirectToAction("NewsList");
            }
            return View(newstoadd);
        }

        [Authorize]
        public ActionResult NewsList()
        {
            Session["LoginUser"] = HttpContext.User.Identity.Name;
            if (HttpContext.User.Identity.Name.ToString().Substring(0, 2) != "A_")
                return RedirectToAction("Index", "Member");
            return View(db.NewsList.ToList());
        }

        [Authorize]
        public ActionResult EditNews(int id)
        {
            Session["LoginUser"] = HttpContext.User.Identity.Name;
            if (HttpContext.User.Identity.Name.ToString().Substring(0, 2) != "A_")
                return RedirectToAction("Index", "Member");
            News newstoedit = db.NewsList.Find(id);
            if (newstoedit != null)
                return View(newstoedit);
            return HttpNotFound();

        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditNews(News newstoedit)
        {
            if (ModelState.IsValid)
            {
                db.Entry(newstoedit).State = EntityState.Modified;
                db.SaveChanges();
                TempData["Notification"] = "Edit news successfully.";
                return RedirectToAction("NewsList");
            }
            return View(newstoedit);
        }

        [Authorize]
        public ActionResult DeleteNews(int id)
        {
            Session["LoginUser"] = HttpContext.User.Identity.Name;
            if (HttpContext.User.Identity.Name.ToString().Substring(0, 2) != "A_")
                return RedirectToAction("Index", "Member");
            News newstodelete = db.NewsList.Find(id);
            if (newstodelete != null)
                return View(newstodelete);
            return HttpNotFound();
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteNews(News newstodelete,string answer)
        {
            if (answer == "Yes")
            {
                db.Entry(newstodelete).State = EntityState.Deleted;
                TempData["Notification"] = "Delete news successfully.";
                db.SaveChanges();
                return RedirectToAction("NewsList");
            }
            return RedirectToAction("NewsList");
        }
        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}

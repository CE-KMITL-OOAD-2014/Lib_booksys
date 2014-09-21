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
    public class AccountController : Controller
    {
        LibraryContext db = new LibraryContext();

        [Authorize]
        public ActionResult Index()
        {
            string userName;
            Session["LoginUser"] = userName = HttpContext.User.Identity.Name;
            userName = userName.Substring(2);
            Person CurrentLoginUser = db.Admins.SingleOrDefault(target => target.UserName == userName);
            if (CurrentLoginUser == null)
                CurrentLoginUser = db.Members.SingleOrDefault(target => target.UserName == userName);
            return View(CurrentLoginUser);
        }
        [Authorize]
        public ActionResult ChangePassword()
        {
            Session["LoginUser"] = HttpContext.User.Identity.Name;
            return View();
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ChangePassword(PasswordChanger pwdToChange){
            if (ModelState.IsValid)
            {
                string userName = HttpContext.User.Identity.Name.ToString().Substring(2);
                Person target = db.Admins.Where(admin => admin.UserName == userName).SingleOrDefault();
                if (target == null)
                    target = db.Members.Where(member => member.UserName == userName).SingleOrDefault();
                if (target.Password == pwdToChange.oldPassword)
                {
                    if (pwdToChange.isEqualPassword())
                    {
                        target.Password = pwdToChange.newPassword;
                        db.Entry(target).State = EntityState.Modified;
                        db.SaveChanges();
                        TempData["Notification"] = "Change password successfully.";
                        return RedirectToAction("Index");
                    }
                    else
                    {
                        TempData["Notification"] = "New password and confirm password is not match.";
                        return View();
                    }
                }
                else
                {
                    TempData["Notification"] = "Your current password information is incorrect.";
                    return View();
                }
            }
            return View();
        }

        [Authorize]
        public ActionResult EditAccount()
        {
            Session["LoginUser"] = HttpContext.User.Identity.Name;
            Person target = db.Admins.Where(admin => admin.UserName == HttpContext.User.Identity.Name.ToString().Substring(2)).SingleOrDefault();
            if (target == null)
                target = db.Members.Where(member => member.UserName == HttpContext.User.Identity.Name.ToString().Substring(2)).SingleOrDefault();
            AccountEditor editor = new AccountEditor();
            editor.Name = target.Name;
            editor.Email = target.Email;
            return View(editor);
        }

        [Authorize]
        [ValidateAntiForgeryToken]
        [HttpPost]
        public ActionResult EditAccount(AccountEditor editor)
        {
            
            if (ModelState.IsValid)
            {
                string userName = HttpContext.User.Identity.Name.ToString().Substring(2);
                Person target = db.Admins.Where(admin => admin.UserName == userName).SingleOrDefault();
                if (target == null)
                {
                    target = db.Members.Where(member => member.UserName == userName).SingleOrDefault();
                }
                target.Name = editor.Name;
                target.Email = editor.Email;
                db.Entry(target).State = EntityState.Modified;
                db.SaveChanges();
                TempData["Notification"] = "Edit account successfully.";
                return RedirectToAction("Index");
            }
            return View(editor);
        }

        [Authorize]
        public ActionResult AdminPortal()
        {
            Session["LoginUser"] = HttpContext.User.Identity.Name;
            if (HttpContext.User.Identity.Name.ToString().Substring(0, 2) != "A_")
                return RedirectToAction("Index");
            return View();
        }


        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }

        
    }

}

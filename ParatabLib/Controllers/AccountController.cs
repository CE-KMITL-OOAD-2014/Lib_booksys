using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Helpers;
using System.Data.Entity;
using ParatabLib.Models;
using ParatabLib.DataAccess;
using ParatabLib.ViewModels;
namespace ParatabLib.Controllers
{
    //This class use to handle about personnal account management
    public class AccountController : Controller
    {
        LibraryRepository libRepo = new LibraryRepository();

        /*This method use to call Index page for this class and return it to user.
         * for member user check that current user have overdue borrow book or not
         * if yes,set notify data and return it to user.
        */
        [Authorize]
        public ActionResult Index()
        {
            string userName;
            userName = HttpContext.User.Identity.Name;
            userName = userName.Substring(2);
            Person CurrentLoginUser = libRepo.LibrarianRepo.ListWhere(target => target.UserName == userName).SingleOrDefault();
            if (CurrentLoginUser == null)
                CurrentLoginUser = libRepo.MemberRepo.ListWhere(target => target.UserName == userName).SingleOrDefault();
            if (CurrentLoginUser.Identify().StartsWith("Member"))
            {
                List<BorrowEntry> checkList = libRepo.BorrowEntryRepo.ListWhere(entry => entry.UserID == CurrentLoginUser.UserID 
                                                        && entry.ReturnDate == null && entry.DueDate.Date < DateTime.Now.Date);
                if (checkList.Count > 0)
                    TempData["WarnNoti"] = "You have overdue borrow please check your borrowlist";
            }
            return View(CurrentLoginUser);
        }


        //This method use to call ChangePassword page for this class and return it to user.
        [Authorize]
        public ActionResult ChangePassword()
        {
            return View();
        }

        /* This method use to submit changepassword value on HTTPPOST by passing pwdToChange as PasswordChanger 
         * then update password in database via libraryRepository
         * and finally return result to user by setting appropiate TempData value.
         */
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ChangePassword(PasswordChanger pwdToChange){
            if (ModelState.IsValid)
            {
                string userName = HttpContext.User.Identity.Name.ToString().Substring(2);
                Person target = libRepo.LibrarianRepo.ListWhere(librarian => librarian.UserName == userName).SingleOrDefault();
                if (target == null)
                    target = libRepo.MemberRepo.ListWhere(member => member.UserName == userName).SingleOrDefault();
                if (Crypto.VerifyHashedPassword(target.Password, pwdToChange.oldPassword))
                {
                    if (pwdToChange.isEqualPassword())
                    {
                        target.Password = Crypto.HashPassword(pwdToChange.newPassword);
                        if (target.Identify().StartsWith("Librarian"))
                            libRepo.LibrarianRepo.Update((Librarian)target);
                        else
                            libRepo.MemberRepo.Update((Member)target);
                        libRepo.Save();
                        TempData["SuccessNoti"] = "Change password successfully.";
                        return RedirectToAction("Index");
                    }
                    else
                    {
                        TempData["ErrorNoti"] = "New password and confirm password is not match.";
                        return View();
                    }
                }
                else
                {
                    TempData["ErrorNoti"] = "Your current password information is incorrect.";
                    return View();
                }
            }
            return View();
        }

        //This method use to call edit account page for current user session and return it to user.
        [Authorize]
        [OutputCache(Duration=0,NoStore=true)]
        public ActionResult EditAccount()
        {
            Person target = libRepo.LibrarianRepo.ListWhere(librarian => librarian.UserName == HttpContext.User.Identity.Name.ToString().Substring(2)).SingleOrDefault();
            if (target == null)
                target = libRepo.MemberRepo.ListWhere(member => member.UserName == HttpContext.User.Identity.Name.ToString().Substring(2)).SingleOrDefault();
            AccountEditor editor = new AccountEditor();
            editor.Name = target.Name;
            editor.Email = target.Email;
            return View(editor);
        }

        /* This method use to submit edit account data on HTTPPOST by passing editor as AccountEditor
         * then update name&email in database via libraryRepository
         * then return result to user by setting appropiate TempData value.
         */
        [Authorize]
        [ValidateAntiForgeryToken]
        [HttpPost]
        public ActionResult EditAccount(AccountEditor editor)
        {
            
            if (ModelState.IsValid)
            {
                string userName = HttpContext.User.Identity.Name.ToString().Substring(2);
                if ((libRepo.MemberRepo.ListWhere(targetusr => targetusr.Email.ToLower() == editor.Email.ToLower() && targetusr.UserName.ToLower() != userName.ToLower()).SingleOrDefault() == null) &&
                   (libRepo.LibrarianRepo.ListWhere(targetusr => targetusr.Email.ToLower() == editor.Email.ToLower() && targetusr.UserName.ToLower() != userName.ToLower()).SingleOrDefault() == null))
                {
                    Person target = libRepo.LibrarianRepo.ListWhere(librarian => librarian.UserName == userName).SingleOrDefault();
                    if (target == null)
                    {
                        target = libRepo.MemberRepo.ListWhere(member => member.UserName == userName).SingleOrDefault();
                    }
                    target.Name = editor.Name;
                    target.Email = editor.Email;
                    if (target.Identify().StartsWith("Librarian"))
                        libRepo.LibrarianRepo.Update((Librarian)target);
                    else
                        libRepo.MemberRepo.Update((Member)target);
                    libRepo.Save();
                    TempData["SuccessNoti"] = "Edit account successfully.";
                    return RedirectToAction("Index");
                }
                else
                {
                    TempData["ErrorNoti"] = "This e-mail is already exists.";
                    return View(editor);
                }
            }
            return View(editor);
        }

        //This method use to call LibrarianPortal for librarian user
        [Authorize]
        public ActionResult LibrarianPortal()
        {
            return View();
        }

        /* [Override method]
         * This method use to check that whether current user is exist in system or not.
         * If not,call and pass by reference of current HTTPrequest in AuthenticateController.OnInvalidSession
         * to set appropiate page result.
         */
        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
                if (AuthenticateController.IsUserValid(HttpContext.User.Identity.Name.Substring(2))){
                    Session["LoginUser"] = HttpContext.User.Identity.Name;

                    /*Since method LibrarianPortal can access by librarian user only
                    check that current user is librarian or not*/
                    if (filterContext.ActionDescriptor.ActionName == "LibrarianPortal")
                    {
                        if (HttpContext.User.Identity.Name.ToString().Substring(0, 2) != "A_")
                        {
                            filterContext.Result = RedirectToAction("Index");
                            return;
                        }
                    }
                }
                else{
                    AuthenticateController.OnInvalidSession(ref filterContext);
                    return;
                }
        }

        /*[Override method]
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

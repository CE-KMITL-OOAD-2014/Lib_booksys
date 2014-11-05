using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Helpers;
using System.ComponentModel;
using ParatabLib.DataAccess;
using ParatabLib.Models;
using ParatabLib.ViewModels;
using ParatabLib.Utilities;
namespace ParatabLib.Controllers
{
    //This class use to handle librarian list management
    public class LibrarianListManagerController : Controller
    {
        LibraryRepository libRepo = new LibraryRepository();
        
        /* This method use to call index librarian list management page with
         * parameterized page and pageSize for paging librarian list,finally return it to user.
         */ 
        [Authorize]
        public ActionResult Index(int page = 1,int pageSize = 10)
        {
            TempData["pageSize"] = pageSize;
            TempData["page"] = page;
            List<Librarian> librarianList = libRepo.LibrarianRepo.List();
            PageList<Librarian> pglist = new PageList<Librarian>(librarianList, page, pageSize);
            switch (pglist.Categorized())
            {
                case PageListResult.Ok: { return View(pglist); }
                case PageListResult.Empty:
                    {
                        TempData["ErrorNoti"] = "No librarian list to show.";
                        return View();
                    }
                default:
                    {
                        TempData["ErrorNoti"] = "Invalid list view parameter please refresh this page to try again.";
                        return View();
                    }
            }
        }

        //This method use to call add librarian page and return it to user.
        [Authorize]
        public ActionResult AddLibrarian()
        {
            return View();
        }

        /* This method use to submit new librarian data and update it by passing newLibrarian as Librarian
         * check username character that contain only ascii or not(Exclude special character)
         * check the exist of username and e-mail,encrypt password and add user to database,
         * finally notify user for success or fail result.
         */
        [Authorize]
        [ValidateAntiForgeryToken]
        [HttpPost]
        public ActionResult AddLibrarian(Librarian newLibrarian)
        {
            if (ModelState.IsValid)
            {
                if (newLibrarian.UserName.Contains(" "))
                {
                    TempData["ErrorNoti"] = "Username can't have space character.";
                    return View(newLibrarian);
                }
                else if (!StringUtil.IsAsciiCharacter(newLibrarian.UserName))
                {
                    TempData["ErrorNoti"] = "Username can't have non-ascii character.";
                    return View(newLibrarian);
                }
                else if ((libRepo.MemberRepo.ListWhere(target => target.UserName.ToLower() == newLibrarian.UserName.ToLower() || target.Email.ToLower() == newLibrarian.Email.ToLower()).Count == 0) &&
                (libRepo.LibrarianRepo.ListWhere(target => target.UserName.ToLower() == newLibrarian.UserName.ToLower() || target.Email.ToLower() == newLibrarian.Email.ToLower()).Count == 0))
                {
                    newLibrarian.Password = Crypto.HashPassword(newLibrarian.Password);
                    libRepo.LibrarianRepo.Add(newLibrarian);
                    libRepo.Save();
                    AuthenticateController.AddUser(newLibrarian.UserName);
                    TempData["SuccessNoti"] = "Add librarian " + newLibrarian.UserName + " successfully.";
                    return RedirectToAction("Index");
                }
                else
                {
                    TempData["ErrorNoti"] = "This username or e-mail is already exists.";
                }
            }
            return View(newLibrarian);
        }

        /* This method use to call and view detail of desired librarian by passing id
         * find target librarian in database base on id if it exists return desired page with librarian detail
         * otherwise notify user to input correct librarian id.
         */ 
        [Authorize]
        public ActionResult View([DefaultValue(0)]int id)
        {
                Librarian target = libRepo.LibrarianRepo.Find(id);
                if (target != null)
                    return View(target);
                TempData["ErrorNoti"] = "Please specified correct Librarian ID.";
                return RedirectToAction("Index");
        }

        /* This method use to call and show delete confirmation page of desired librarian by passing id
         * find target librarian in database base on id if it exists return desired page with librarian data
         * otherwise notify user to input correct librarian id.
         */ 
        [Authorize]
        public ActionResult Delete([DefaultValue(0)]int id)
        {
            Librarian target = libRepo.LibrarianRepo.Find(id);
            if (target != null)
                return View(target);
            TempData["ErrorNoti"] = "Please specified correct Librarian ID.";
            return RedirectToAction("Index");
        }

        /* this method use to delete desired librarian from comfirmation page.
         * validation librarian data to ensure that the wanted librarian is exist
         * then remove that librarian data from database,finally notify result to user.
         * Special case for only one remain librarian in library,prevent it from delete operation
         * by notify that can't delete only one librarian remain out from system.
         * Another special case is delete itself in which target librarian equal to current user session
         * this case will result not notify success result but use OnActionExecuting in background work to
         * handle instead(which suddenly redirect user to log in page).
         */
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(Librarian target)
        {
            if (ModelState.IsValid)
            {
                if (libRepo.LibrarianRepo.List().Count == 1)
                {
                    TempData["ErrorNoti"] = "Can't delete the only one librarian remain out of system.";
                    return RedirectToAction("Index");
                }
                    libRepo.LibrarianRepo.Remove(target);
                    libRepo.Save();
                    AuthenticateController.RemoveUser(target.UserName);
                    if (target.UserName != HttpContext.User.Identity.Name.Substring(2))
                    {
                        TempData["SuccessNoti"] = "Delete librarian " + target.UserName + " successfully.";
                    }
                    return RedirectToAction("Index");
            }
            return RedirectToAction("Index");
        }

        /* [Override method]
         * This method use to check that whether current user for current session is exist in system or not.
         * If not,call and pass by reference of current HTTPrequest in AuthenticateController.OnInvalidSession
         * to set appropiate page result.Moreover check that current user is librarian if not redirect user to
         * account index page.
         */
        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
                if (AuthenticateController.IsUserValid(HttpContext.User.Identity.Name.Substring(2)))
                {
                    Session["LoginUser"] = HttpContext.User.Identity.Name;
                    if (HttpContext.User.Identity.Name.ToString().Substring(0, 2) != "A_")
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

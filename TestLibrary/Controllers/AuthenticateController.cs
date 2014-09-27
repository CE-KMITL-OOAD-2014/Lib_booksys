using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using System.Web.Helpers;
using TestLibrary.Models;
using TestLibrary.DataAccess;
using TestLibrary.ViewModels;
namespace TestLibrary.Controllers
{
    public class AuthenticateController : Controller
    {
        LibraryRepository libRepo = new LibraryRepository();
        public ActionResult Login()
        {
            if (HttpContext.User.Identity.IsAuthenticated)
            {
                Session["LoginUser"] = HttpContext.User.Identity.Name;
                return RedirectToAction("Index", "Home");
            }
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(LoginForm submitData)
        {
            if (ModelState.IsValid)
            {
                Person loginUser;
                loginUser = libRepo.MemberRepo.ListWhere(target => target.UserName == submitData.UserName && Crypto.VerifyHashedPassword(target.Password,submitData.Password)).SingleOrDefault();
                if (loginUser != null)
                {
                    FormsAuthentication.SetAuthCookie("M_" + submitData.UserName, submitData.Remember);
                    Session["LoginUser"] = "M_" + submitData.UserName;
                    return RedirectToAction("Index", "Account");
                }
                else
                {
                    loginUser = libRepo.LibrarianRepo.ListWhere(target => target.UserName == submitData.UserName && Crypto.VerifyHashedPassword(target.Password, submitData.Password)).SingleOrDefault();
                    if (loginUser != null)
                    {
                        FormsAuthentication.SetAuthCookie("A_" + submitData.UserName, submitData.Remember);
                        Session["LoginUser"] = "A_" + submitData.UserName;
                        return RedirectToAction("Index", "Account");
                    }
                    TempData["Notification"] = "Login info is incorrect.";
                    return View(submitData);
                }
            }
                return View(submitData);
        }

        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            Session["LoginUser"] = null;
            return RedirectToAction("Index","Home");
        }

        public ActionResult Register()
        {
                if (HttpContext.User.Identity.IsAuthenticated)
                {
                    Session["LoginUser"] = HttpContext.User.Identity.Name;
                    return RedirectToAction("Index","Home");
                }
                return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Register(Member regist, string ConfirmPassword)
        {
            if (ModelState.IsValid)
            {
                if ((libRepo.MemberRepo.ListWhere(target => target.UserName == (regist.UserName)).SingleOrDefault() == null) &&
                    (libRepo.LibrarianRepo.ListWhere(target => target.UserName == (regist.UserName)).SingleOrDefault() == null))
                {
                    if (regist.Password == ConfirmPassword)
                    {
                        regist.Password = Crypto.HashPassword(regist.Password);
                        libRepo.MemberRepo.Add(regist);
                        libRepo.Save();
                        TempData["Notification"] = "Register successful,please login for first use.";
                        return RedirectToAction("Login");
                    }
                    else
                    {
                        TempData["Notification"] = "Password did not match.";
                        return View(regist);
                    }
                }
                else
                {
                    TempData["Notification"] = "This user name is already exists.";
                    return View(regist);
                }
            }
            else
                return View(regist);
        }

        public ActionResult ForgotPassword()
        {
            if (HttpContext.User.Identity.IsAuthenticated)
            {
                Session["LoginUser"] = HttpContext.User.Identity.Name;
                return RedirectToAction("Index", "Home");
            }
            else
                return View();
        }

    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using System.Web.Helpers;
using System.Net.Mail;
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
                loginUser = libRepo.MemberRepo.ListWhere(target => target.UserName == submitData.UserName && Crypto.VerifyHashedPassword(target.Password, submitData.Password)).SingleOrDefault();
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
            return RedirectToAction("Index", "Home");
        }

        public ActionResult Register()
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
        public ActionResult Register(Member regist, string confirmPwd)
        {
            if (ModelState.IsValid)
            {
                if ((libRepo.MemberRepo.ListWhere(target => target.UserName == (regist.UserName)).SingleOrDefault() == null) &&
                    (libRepo.LibrarianRepo.ListWhere(target => target.UserName == (regist.UserName)).SingleOrDefault() == null))
                {
                    if (regist.Password == confirmPwd)
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

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ForgotPassword(string email)
        {

            if (email != null)
            {
                Member memberToRecover = libRepo.MemberRepo.ListWhere(target => target.Email == email).SingleOrDefault();
                if (memberToRecover != null)
                {
                    //Send email

                    MailMessage mail = new MailMessage();
                    mail.From = new MailAddress("paratabplus@gmail.com", "Paratab+");
                    mail.To.Add(new MailAddress(memberToRecover.Email));
                    mail.Subject = "Reset password for " + memberToRecover.UserName;
                    mail.IsBodyHtml = true;
                    mail.Body = "Hi! " + memberToRecover.UserName + " <br> Please click <a href = \""+Request.Url.GetLeftPart(UriPartial.Authority) +"/Authenticate/ResetPassword/?token="+HttpUtility.UrlEncode(memberToRecover.Password) + "\">here</a> to reset password.<br>Thank you for use our service.";
                    SmtpClient smtp = new SmtpClient("smtp.gmail.com", 587);
                    smtp.EnableSsl = true;
                    smtp.Credentials = new System.Net.NetworkCredential("paratabplus@gmail.com", "Dream1357");
                    smtp.Send(mail);
                    TempData["isSuccess"] = "true";
                    TempData["email"] = email;
                    TempData["Notification"] = "Send email successfully.";
                    return View();
                }
                TempData["Notification"] = "Error! No member was found to recover.";
                return View();
            }
            else
            {
                TempData["Notification"] = "Please enter your e-mail address.";
                return View();
            }
        }

        [HttpGet]
        public ActionResult ResetPassword(string token)
        {
            if(!HttpContext.User.Identity.IsAuthenticated)
            {
                Member memberToRecover = libRepo.MemberRepo.ListWhere(target => target.Password == token).SingleOrDefault();
                if (memberToRecover != null)
                {
                    return View(memberToRecover);
                }
                else
                {
                    TempData["Notification"] = "Oops! Something went wrong.";
                    return RedirectToAction("Login", "Authenticate");
                }
            }
            else
            {
                Session["LoginUser"] = HttpContext.User.Identity.Name;
                return RedirectToAction("Index", "Account");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ResetPassword(Member memberToRecover, PasswordChanger pwdToChange)
        {
            ModelState.Remove("oldPassword");
            if (ModelState.IsValid)
            {
                if (pwdToChange.isEqualPassword())
                {
                    try
                    {
                        memberToRecover.Password = Crypto.HashPassword(pwdToChange.newPassword);
                        libRepo.MemberRepo.Update(memberToRecover);
                        libRepo.Save();
                        TempData["Notification"] = "Reset password successfully.";
                        return RedirectToAction("Login");
                    }
                    catch (Exception)
                    {
                        TempData["Notification"] = "Oops! Something went wrong.";
                        return RedirectToAction("Login");
                    }
                }
                else
                {
                    TempData["Notification"] = "Password did not match.";
                    return View(memberToRecover);
                }

            }
            else
            {
                TempData["Notification"] = "Please fill in the blank of password and comfirm password.";
                return View(memberToRecover);
            }
        }
    }
}

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
                if ((libRepo.MemberRepo.ListWhere(target => target.UserName == (regist.UserName) || target.Email == regist.Email).SingleOrDefault() == null) &&
                    (libRepo.LibrarianRepo.ListWhere(target => target.UserName == (regist.UserName) || target.Email == regist.Email).SingleOrDefault() == null))
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
                    TempData["Notification"] = "This user name or e-mail is already exists.";
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
                Person userToRecover = libRepo.MemberRepo.ListWhere(target => target.Email == email).SingleOrDefault();
                if(userToRecover == null)
                    userToRecover = libRepo.LibrarianRepo.ListWhere(target => target.Email == email).SingleOrDefault();
                if (userToRecover != null)
                {
                    //Send email

                    MailMessage mail = new MailMessage();
                    mail.From = new MailAddress("paratabplus@gmail.com", "Paratab+");
                    mail.To.Add(new MailAddress(userToRecover.Email));
                    mail.Subject = "Reset password for " + userToRecover.UserName;
                    mail.IsBodyHtml = true;
                    mail.Body = "Hi! " + userToRecover.UserName + " <br> Please click <a href = \""+Request.Url.GetLeftPart(UriPartial.Authority) +"/Authenticate/ResetPassword/?token="+HttpUtility.UrlEncode(userToRecover.Password) + "\">here</a> to reset password.<br>Thank you for use our service.";
                    SmtpClient smtp = new SmtpClient("smtp.gmail.com", 587);
                    smtp.EnableSsl = true;
                    smtp.Credentials = new System.Net.NetworkCredential("paratabplus@gmail.com", "Dream1357");
                    smtp.Send(mail);
                    TempData["isSuccess"] = "true";
                    TempData["email"] = email;
                    TempData["Notification"] = "Send email successfully.";
                    return View();
                }
                TempData["Notification"] = "Error! No user was found to recover.";
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
                Person userToRecover = libRepo.MemberRepo.ListWhere(target => target.Password == token).SingleOrDefault();
                if (userToRecover == null)
                userToRecover = libRepo.LibrarianRepo.ListWhere(target => target.Password == token).SingleOrDefault();
                if (userToRecover != null)
                {
                    TempData["UserName"] = userToRecover.UserName;
                    TempData["Token"] = token;
                    return View(userToRecover);
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
        public ActionResult ResetPassword(string userName,PasswordChanger pwdToChange)
        {
            ModelState.Remove("oldPassword");
            TempData["UserName"] = userName;
            TempData["Token"] = pwdToChange.oldPassword;
            if (ModelState.IsValid)
            {
                Person userToRecover = libRepo.MemberRepo.ListWhere(target => target.Password == pwdToChange.oldPassword 
                                                                && target.UserName == userName).SingleOrDefault();
                if(userToRecover == null)
                    userToRecover = libRepo.LibrarianRepo.ListWhere(target => target.Password == pwdToChange.oldPassword
                                                             && target.UserName == userName).SingleOrDefault();
                if (userToRecover == null)
                {
                    TempData["Notification"] = "Oops! Something went wrong.";
                    return RedirectToAction("Login");
                }

                if (pwdToChange.isEqualPassword())
                {
                    try
                    {
                        userToRecover.Password = Crypto.HashPassword(pwdToChange.newPassword);
                        if (userToRecover.Identify().StartsWith("Member"))
                        {
                            libRepo.MemberRepo.Update((Member)userToRecover);
                        }
                        else
                            libRepo.LibrarianRepo.Update((Librarian)userToRecover);
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
                    return View();
                }
            }
            else
            {
                TempData["Notification"] = "Please fill in the blank of password and comfirm password.";
                return View();
            }
        }

        public ActionResult test(string term)
        {
            string s;
            Member m = libRepo.MemberRepo.ListWhere(target => target.UserName == term).SingleOrDefault();
            if (m != null)
                s = m.UserName;
            else
                s = "Not found";
            return this.Json(s, JsonRequestBehavior.AllowGet);

        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using System.Web.Helpers;
using System.Net.Mail;
using ParatabLib.Models;
using ParatabLib.DataAccess;
using ParatabLib.ViewModels;
using ParatabLib.Utilities;
namespace ParatabLib.Controllers
{
    public class AuthenticateController : Controller
    {
        LibraryRepository libRepo = new LibraryRepository();
        static List<string> AuthorizedList = new List<string>();
        static RedirectToRouteResult LoginRoute = new RedirectToRouteResult("",
            new System.Web.Routing.RouteValueDictionary(new Dictionary<string, object>() { 
            { "action", "Login" }, { "controller", "Authenticate" } }), false);
        public static void AddUser(string userName){
            AuthorizedList.Add(userName);
        }

        public static void RemoveUser(string userName)
        {
            AuthorizedList.Remove(userName);
        }

        public static bool IsUserValid(string userName)
        {
            return AuthorizedList.Where(wantedUser => wantedUser == userName).SingleOrDefault() != null;
        }


        public ActionResult Login()
        {
            return View();
        }
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(LoginForm submitData)
        {
                if (ModelState.IsValid)
                {
                    Person loginUser;
                    loginUser = libRepo.MemberRepo.ListWhere(target => target.UserName.ToLower() == submitData.UserName.ToLower() && Crypto.VerifyHashedPassword(target.Password, submitData.Password)).SingleOrDefault();
                    if (loginUser != null)
                    {
                        FormsAuthentication.SetAuthCookie("M_" + loginUser.UserName, submitData.Remember);
                        Session["LoginUser"] = "M_" + loginUser.UserName;
                        return RedirectToAction("Index", "Account");
                    }
                    else
                    {
                        loginUser = libRepo.LibrarianRepo.ListWhere(target => target.UserName.ToLower() == submitData.UserName.ToLower() && Crypto.VerifyHashedPassword(target.Password, submitData.Password)).SingleOrDefault();
                        if (loginUser != null)
                        {
                            FormsAuthentication.SetAuthCookie("A_" + loginUser.UserName, submitData.Remember);
                            Session["LoginUser"] = "A_" + loginUser.UserName;
                            return RedirectToAction("Index", "Account");
                        }
                        TempData["ErrorNoti"] = "Login info is incorrect.";
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
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Register(RegisterEditor submitData)
        {
            if (ModelState.IsValid)
            {
                if (submitData.UserName.Contains(" "))
                {
                    TempData["ErrorNoti"] = "Username can't have space character.";
                    return View(submitData);
                }
                else if (!StringUtil.IsAsciiCharacter(submitData.UserName))
                {
                    TempData["ErrorNoti"] = "Username can't have non-ascii character.";
                    return View(submitData);
                }
                else if ((libRepo.MemberRepo.ListWhere(target => target.UserName.ToLower() == (submitData.UserName.ToLower()) || target.Email.ToLower() == submitData.Email.ToLower()).Count == 0) &&
                    (libRepo.LibrarianRepo.ListWhere(target => target.UserName.ToLower() == (submitData.UserName.ToLower()) || target.Email.ToLower() == submitData.Email.ToLower()).Count == 0))
                {
                    if (submitData.Password == submitData.ConfirmPassword)
                    {
                        submitData.Password = Crypto.HashPassword(submitData.Password);
                        libRepo.MemberRepo.Add(
                            new Member
                            {
                                UserName = submitData.UserName,
                                Name = submitData.Name,
                                Email = submitData.Email,
                                Password = submitData.Password
                            });
                        libRepo.Save();
                        AddUser(submitData.UserName);
                        TempData["SuccessNoti"] = "Register successful,please login for first use.";
                        return RedirectToAction("Login");
                    }
                    else
                    {
                        TempData["ErrorNoti"] = "Password did not match.";
                        return View(submitData);
                    }
                }
                else
                {
                    TempData["ErrorNoti"] = "This user name or e-mail is already exists.";
                    return View(submitData);
                }
            }
            else
                return View(submitData);
        }

        public ActionResult ForgotPassword()
        {
                return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ForgotPassword(string email)
        {

            if (email != "")
            {
                Person userToRecover = libRepo.MemberRepo.ListWhere(target => target.Email.ToLower() == email.ToLower()).SingleOrDefault();
                if(userToRecover == null)
                    userToRecover = libRepo.LibrarianRepo.ListWhere(target => target.Email.ToLower() == email.ToLower()).SingleOrDefault();
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
                    TempData["WarnNoti"] = "Send email successfully.";
                    return View();
                }
                TempData["ErrorNoti"] = "Error! No user was found to recover.";
                return View();
            }
            else
            {
                TempData["ErrorNoti"] = "Please enter your e-mail address.";
                return View();
            }
        }

        [HttpGet]
        public ActionResult ResetPassword(string token)
        {
                Person userToRecover = libRepo.MemberRepo.ListWhere(target => target.Password == token).SingleOrDefault();
                if (userToRecover == null)
                userToRecover = libRepo.LibrarianRepo.ListWhere(target => target.Password == token).SingleOrDefault();
                if (userToRecover != null)
                {
                    TempData["UserName"] = userToRecover.UserName;
                    TempData["Token"] = token;
                    return View();
                }
                else
                {
                    TempData["ErrorNoti"] = "Oops! Something went wrong.";
                    return RedirectToAction("Login", "Authenticate");
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
                    TempData["ErrorNoti"] = "Oops! Something went wrong.";
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
                        TempData["SuccessNoti"] = "Reset password successfully.";
                        return RedirectToAction("Login");
                    }
                    catch (Exception)
                    {
                        TempData["ErrorNoti"] = "Oops! Something went wrong.";
                        return RedirectToAction("Login");
                    }
                }
                else
                {
                    TempData["ErrorNoti"] = "Password did not match.";
                    return View();
                }
            }
            else
            {
                TempData["ErrorNoti"] = "Please fill in the blank of password and comfirm password.";
                return View();
            }
        }

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (Request.HttpMethod == "GET")
            {
                if (filterContext.ActionDescriptor.ActionName == "Login" || filterContext.ActionDescriptor.ActionName == "Register"
                    || filterContext.ActionDescriptor.ActionName == "ForgotPassword")
                {
                    if (HttpContext.User.Identity.IsAuthenticated)
                    {
                        if (AuthenticateController.IsUserValid(HttpContext.User.Identity.Name.Substring(2)))
                        {
                            Session["LoginUser"] = HttpContext.User.Identity.Name;
                            filterContext.Result = RedirectToAction("Index", "Home");
                            return;
                        }
                        else
                        {
                            FormsAuthentication.SignOut();
                            Session["LoginUser"] = null;
                            TempData["ErrorNoti"] = "Your session is invalid or your account is deleted while you logged in.";
                            filterContext.Result = RedirectToAction("Login", "Authenticate");
                            return;
                        }
                    }
                    Session["LoginUser"] = null;
                }
                else if (filterContext.ActionDescriptor.ActionName == "ResetPassword")
                {
                    if (HttpContext.User.Identity.IsAuthenticated)
                    {
                        if (AuthenticateController.IsUserValid(HttpContext.User.Identity.Name.Substring(2)))
                        {
                            TempData["ErrorNoti"] = "Invalid operation.";
                            Session["LoginUser"] = HttpContext.User.Identity.Name;
                            filterContext.Result = RedirectToAction("Index", "Account");
                            return;
                        }
                        else
                        {
                            OnInvalidSession(ref filterContext);
                            return;
                        }
                    }
                    else
                    {
                        Session["LoginUser"] = null;
                    }
                }
            }

        }
        
        public static void OnInvalidSession(ref ActionExecutingContext action)
        {
            FormsAuthentication.SignOut(); 
            action.HttpContext.Session["LoginUser"] = null;
            action.Controller.TempData["ErrorNoti"] = "Your session is invalid or your account is deleted while you logged in.";
            action.Result = LoginRoute;
        }

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

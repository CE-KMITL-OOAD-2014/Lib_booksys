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
    //This class use to handle about authorization/registration and account recovery
    public class AuthenticateController : Controller
    {
        LibraryRepository libRepo = new LibraryRepository();

        /* This static AuthorizedList properties use to keep current user that in exist in database
         * purpose of this static properties is to use for session management and check that
         * current user is exist or not.If current user is deleted while login if implementation
         * must check at database it could not be useful so use this static properties instead
         * to increase performance
         */

        static List<string> AuthorizedList = new List<string>();

        /*
         * Static LoginRoute proerties use for set redirect to define page when session of
         * current user is invalid.
         */
        static RedirectToRouteResult LoginRoute = new RedirectToRouteResult("",
            new System.Web.Routing.RouteValueDictionary(new Dictionary<string, object>() { 
            { "action", "Login" }, { "controller", "Authenticate" } }), false);
        
        /*
         * 3 Method below related-to AuthorizedList properties
         * [1.]AddUser use to add new user to list.
         * [2.]RemoveUser use to remove user to list.
         * [3.]IsUserValid use to check that current user is exist in list.
         */
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

        //This method use to call Login page and return it to user.
        [OutputCache(Duration = 0, NoStore = true)]
        public ActionResult Login()
        {
            return View();
        }

        /* This method use submit login data on HTTPPOST by passing submitData as LoginForm
         * check username data and password that match in database
         * create session for user and redirect user to account index page.
         */
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
        

        /* This method use to log out from system
         * by use FormsAuthentication.SignOut() to clear cookie with clear session
         * and redirect user to Homepage.
         */
        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            Session["LoginUser"] = null;
            return RedirectToAction("Index", "Home");
        }

        //This method use to call register page and return it to user.
        [OutputCache(Duration = 0, NoStore = true)]
        public ActionResult Register()
        {
            return View();
        }


         /* This method use submit register data on HTTPPOST by passing submitData as RegisterEditor
          * check username character that contain only ascii or not(Exclude special character)
          * check the exist of username and e-mail,encrypt password and add user to database
          * finally notify user to login for first time use.
          */
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

        //This method use to call ForgotPassword page and return it to user.
        [OutputCache(Duration = 0, NoStore = true)]
        public ActionResult ForgotPassword()
        {
                return View();
        }

        /* This method use to submit email data on HTTPPOST by passing email as string
         * then check existence of email in database and send resetpassword email to
         * target email by use MailMessage to set mail data and SmtpClient as object to command
         * smtp server to send mail.
         */
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

        /* This method use to call resetPassword page by passing token via querystring
         * check token that is valid or not if yes return desired page,otherwise redirect
         * user to another page.
         */
        [HttpGet]
        [OutputCache(Duration=0,NoStore=true)]
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

        /* This method use to submit resetPassword data on HTTPPOST by passing username and 
         * pwdToChange as PasswordChanger,check existence of user to reset to prevent hack
         * check match of password and update database with new encrpypt password
         * finally notify user for success result.
         */
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

        /* [Override method]
         * This method use to check that whether current user is already logged in or not
         * If yes check the existence of user whether current user is exist
         * If not,call and pass by reference of current HTTPrequest in AuthenticateController.OnInvalidSession
         * to set appropiate page result.If that user is not login return normal desired page.
         */
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

        /* 
         * This static method sue to handle invalid session for delete user of fake user
         * by clear session and related cookie then redirect user to page that set in
         * static properties LoginRoute.The ActionExecutingContext parameter is pass by reference
         * so any change in this method will result in caller.
         */
        public static void OnInvalidSession(ref ActionExecutingContext action)
        {
            FormsAuthentication.SignOut(); 
            action.HttpContext.Session["LoginUser"] = null;
            action.Controller.TempData["ErrorNoti"] = "Your session is invalid or your account is deleted while you logged in.";
            action.Result = LoginRoute;
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

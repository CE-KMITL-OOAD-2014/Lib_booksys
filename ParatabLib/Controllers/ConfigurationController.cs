using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ParatabLib.Controllers
{
    //This class use to handle library's configuration
    public class ConfigurationController : Controller
    {
        //This static properties keep fine value that will be use in static method
        private static int fine;
        
        //Both below static methods use to set or get fine value. 
        public static void setFine(int value)
        {
            fine = value;
        }

        public static int getFine()
        {
            return fine;
        }

        /*This method use to call index configuration page and return it to current user
         * include current fine value setting.
         */ 
        [Authorize]
        public ActionResult Index()
        {
            TempData["Fine"] = fine;
            return View();
        }

        /*This method use to call EditFine page and return it to current user
         * include current fine value setting and set TempData["Operation"] 
         * as Edit to tell View to show edit page too.
         */ 
        [Authorize]
        public ActionResult EditFine()
        {
            TempData["Fine"] = fine;
            TempData["Operation"] = "Edit";
            return View("Index");
        }

        /* This method use to submit edit data on HTTPPOST by passing newfine as string,
         * convert this string to integer and update static properties then save configuration to file.
         * finally return result whether it success or not(in case of string parameter is not numeric).
         */ 
        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public ActionResult UpdateFine(string newfine)
        {
            try
            {
                fine = int.Parse(newfine);
                System.IO.File.WriteAllText(HttpRuntime.AppDomainAppPath + "/lib-config", newfine);
                TempData["SuccessNoti"] = "Update configuration successfully.";
                return RedirectToAction("Index");
            }
            catch (FormatException)
            {
                TempData["ErrorNoti"] = "Input is not in correct format.";
                return RedirectToAction("Index");
            }
        }

        /* [Override method]
         * This method use to check that whether current user for current session is exist in system or not.
         * If not,call and pass by reference of current HTTPrequest in AuthenticateController.OnInvalidSession
         * to set appropiate page result.Moreover check that current user is librarian if not redirect user to
         * account index page.
         */
        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
                if(AuthenticateController.IsUserValid(HttpContext.User.Identity.Name.Substring(2)))
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
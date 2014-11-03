using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ParatabLib.Controllers
{
    public class ConfigurationController : Controller
    {
        private static int fine;
        public static void setFine(int value)
        {
            fine = value;
        }

        public static int getFine()
        {
            return fine;
        }

        [Authorize]
        public ActionResult Index()
        {
            TempData["Fine"] = fine;
            return View();
        }

        [Authorize]
        public ActionResult EditFine()
        {
            TempData["Fine"] = fine;
            TempData["Operation"] = "Edit";
            return View("Index");
        }

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
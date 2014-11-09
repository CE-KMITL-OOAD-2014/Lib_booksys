using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ParatabLib.DataAccess;
using ParatabLib.Models;
using System.ComponentModel;
using System.Data.Entity;
namespace ParatabLib.Controllers
{
    /* Basicly,this class use to view book detail by call View method
     * (pass id for book that want to see as integer).
     */
    public class BookController : Controller
    {
        LibraryRepository libRepo = new LibraryRepository();

        [OutputCache(Duration = 0, NoStore = true)]
        public ActionResult View([DefaultValue(0)]int id)
        {
            return View(libRepo.BookRepo.Find(id));
        }

        /* [Override method]
         * This method use to check that whether current user is exist in system or not.
         * If not,call and pass by reference of current HTTPrequest in AuthenticateController.OnInvalidSession
         * to set appropiate page result.
         */
        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (HttpContext.User.Identity.IsAuthenticated)
            {
                if (AuthenticateController.IsUserValid(HttpContext.User.Identity.Name.Substring(2)))
                    Session["LoginUser"] = HttpContext.User.Identity.Name;
                else
                {
                    AuthenticateController.OnInvalidSession(ref filterContext);
                    return;
                }
            }
            else
                Session["LoginUser"] = null;
        }
    }
}

﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using TestLibrary.DataAccess;
using TestLibrary.Models;
using System.ComponentModel;
using System.Data.Entity;
namespace TestLibrary.Controllers
{
    public class BookController : Controller
    {
        LibraryRepository libRepo = new LibraryRepository();
        public ActionResult View([DefaultValue(0)]int id)
        {
            return View(libRepo.BookRepo.Find(id));
        }

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (HttpContext.User.Identity.IsAuthenticated)
            {
                if (AuthenticateController.IsUserValid(HttpContext.User.Identity.Name.Substring(2)))
                    Session["LoginUser"] = HttpContext.User.Identity.Name;
                else
                {
                    FormsAuthentication.SignOut();
                    Session["LoginUser"] = null;
                    TempData["ErrorNoti"] = "Your session is invalid or your account is deleted while you logged in.";
                    filterContext.Result = RedirectToAction("Login", "Authenticate");
                    return;
                }
            }
            else
                Session["LoginUser"] = null;
        }
    }
}

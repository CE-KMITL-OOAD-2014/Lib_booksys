using System;
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
            if (HttpContext.User.Identity.IsAuthenticated)
            {
                Session["LoginUser"] = HttpContext.User.Identity.Name;
            }
            return View(libRepo.BookRepo.Find(id));
        }
    }
}

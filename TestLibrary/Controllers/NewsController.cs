using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TestLibrary.DataAccess;
using TestLibrary.Models;
namespace TestLibrary.Controllers
{
    public class NewsController : Controller
    {
        LibraryRepository libRepo = new LibraryRepository();
        public ActionResult View(int id)
        {
            if (HttpContext.User.Identity.IsAuthenticated)
            Session["LoginUser"] = HttpContext.User.Identity.Name;
            News newstoview = libRepo.NewsRepo.Find(id);
            if (newstoview != null)
                return View(newstoview);
            else
                return HttpNotFound();
        }

        public ActionResult ViewAll()
        {
            if (HttpContext.User.Identity.IsAuthenticated)
            {
                Session["LoginUser"] = HttpContext.User.Identity.Name;
            }
            return View(libRepo.NewsRepo.List().OrderByDescending(target => target.PostTime));
        }
    }
}

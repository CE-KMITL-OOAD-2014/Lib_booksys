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
        LibraryContext db = new LibraryContext();
        public ActionResult Detail(int id)
        {
            if (HttpContext.User.Identity.IsAuthenticated)
            Session["LoginUser"] = HttpContext.User.Identity.Name;
            News newstoview = db.NewsList.Find(id);
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
            return View(db.NewsList.ToList());
        }

    }
}

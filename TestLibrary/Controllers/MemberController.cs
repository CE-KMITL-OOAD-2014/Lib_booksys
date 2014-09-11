using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TestLibrary.DataAccess;
using TestLibrary.Models;
namespace TestLibrary.Controllers
{
    public class MemberController : Controller
    {
        LibraryContext db = new LibraryContext();

        [Authorize]
        public ActionResult Index()
        {
            Session["LoginUser"] = HttpContext.User.Identity.Name;
            if (HttpContext.User.Identity.Name.ToString().Substring(0, 2) == "M_")
               return View(db.BorrowList.ToList().Where((target => target.Borrower.UserName == 
                    Session["LoginUser"].ToString().Substring(2) &&
                    target.ReturnDate == null)).ToList());
            else
                return RedirectToAction("Index", "Manage");
        }

        [Authorize]
        public ActionResult Detail()
        {
            Session["LoginUser"] = HttpContext.User.Identity.Name;
            if (HttpContext.User.Identity.Name.ToString().Substring(0, 2) == "M_")
            {
                Member loginmember = db.Members.Where(member => member.UserName ==
                    HttpContext.User.Identity.Name.ToString().Substring(2)).Single();
                return View(loginmember);
            }
            else
                return RedirectToAction("Index", "Manage");
        }



    }
}

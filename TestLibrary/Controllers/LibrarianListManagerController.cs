using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Helpers;
using System.ComponentModel;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using TestLibrary.DataAccess;
using TestLibrary.Models;
using TestLibrary.ViewModels;
namespace TestLibrary.Controllers
{
    public class LibrarianListManagerController : Controller
    {
        LibraryRepository libRepo = new LibraryRepository();
        
        [Authorize]
        public ActionResult Index(int page = 1,int pageSize = 10)
        {
            Session["LoginUser"] = HttpContext.User.Identity.Name;
            if (HttpContext.User.Identity.Name.ToString().Substring(0, 2) != "A_")
                return RedirectToAction("Index", "Account");
            TempData["pageSize"] = pageSize;
            TempData["page"] = page;
            List<Librarian> librarianList = libRepo.LibrarianRepo.List();
            PageList<Librarian> pglist = new PageList<Librarian>(librarianList, page, pageSize);
            switch (pglist.Categorized())
            {
                case PageListResult.Ok: { return View(pglist); }
                case PageListResult.Empty:
                    {
                        TempData["Notification"] = "No librarian list to show.";
                        return View();
                    }
                default:
                    {
                        TempData["Notification"] = "Invalid list view parameter please refresh this page to try again.";
                        return View();
                    }
            }
        }

        [Authorize]
        public ActionResult AddLibrarian()
        {
            Session["LoginUser"] = HttpContext.User.Identity.Name;
            if (HttpContext.User.Identity.Name.ToString().Substring(0, 2) != "A_")
                return RedirectToAction("Index", "Account");
            return View();
        }

        [Authorize]
        [ValidateAntiForgeryToken]
        [HttpPost]
        public ActionResult AddLibrarian(Librarian newLibrarian)
        {
            if (ModelState.IsValid)
            {
                if (newLibrarian.UserName.Contains(" "))
                {
                    TempData["Notification"] = "Username can't have space character.";
                    return View(newLibrarian);
                }
                else if ((libRepo.MemberRepo.ListWhere(target => target.UserName.ToLower() == newLibrarian.UserName.ToLower() || target.Email.ToLower() == newLibrarian.Email.ToLower()).Count == 0) &&
                (libRepo.LibrarianRepo.ListWhere(target => target.UserName.ToLower() == newLibrarian.UserName.ToLower() || target.Email.ToLower() == newLibrarian.Email.ToLower()).Count == 0))
                {
                    newLibrarian.Password = Crypto.HashPassword(newLibrarian.Password);
                    libRepo.LibrarianRepo.Add(newLibrarian);
                    libRepo.Save();
                    TempData["Notification"] = "Add librarian " + newLibrarian.UserName + " successfully.";
                    return RedirectToAction("Index");
                }
                else
                {
                    TempData["Notification"] = "This username or e-mail is already exists.";
                }
            }
            return View(newLibrarian);
        }
        [Authorize]
        public ActionResult View([DefaultValue(0)]int id)
        {
                Session["LoginUser"] = HttpContext.User.Identity.Name;
                if (HttpContext.User.Identity.Name.ToString().Substring(0, 2) != "A_")
                    return RedirectToAction("Index", "Account");
                Librarian target = libRepo.LibrarianRepo.Find(id);
                if (target != null)
                    return View(target);
                TempData["Notification"] = "Please specified correct Librarian ID.";
                return RedirectToAction("Index");
        }

        [Authorize]
        public ActionResult Delete([DefaultValue(0)]int id)
        {
            Session["LoginUser"] = HttpContext.User.Identity.Name;
            if (HttpContext.User.Identity.Name.ToString().Substring(0, 2) != "A_")
                return RedirectToAction("Index", "Account");
            Librarian target = libRepo.LibrarianRepo.Find(id);
            if (target != null)
                return View(target);
            TempData["Notification"] = "Please specified correct Librarian ID.";
            return RedirectToAction("Index");
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(Librarian target,string answer)
        {
            if (answer == "Yes" && ModelState.IsValid)
            {
                if (libRepo.LibrarianRepo.List().Count == 1)
                {
                    TempData["Notification"] = "Can't delete the only one librarian remain out of system.";
                    return RedirectToAction("Index");
                }
                    libRepo.LibrarianRepo.Remove(target);
                    libRepo.Save();
                    TempData["Notification"] = "Delete librarian " + target.UserName + " successfully.";
                    return RedirectToAction("Index");
            }
            return RedirectToAction("Index");
        }
    }
}

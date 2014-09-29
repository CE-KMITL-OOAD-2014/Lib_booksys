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
namespace TestLibrary.Controllers
{
    public class LibrarianListManagerController : Controller
    {
        LibraryRepository libRepo = new LibraryRepository();
        
        [Authorize]
        public ActionResult Index()
        {
            Session["LoginUser"] = HttpContext.User.Identity.Name;
            if (HttpContext.User.Identity.Name.ToString().Substring(0, 2) != "A_")
                return RedirectToAction("Index", "Account");
            return View(libRepo.LibrarianRepo.List());
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
                if ((libRepo.MemberRepo.ListWhere(target => target.UserName == newLibrarian.UserName || target.Email == newLibrarian.Email).SingleOrDefault() == null) &&
                (libRepo.LibrarianRepo.ListWhere(target => target.UserName == newLibrarian.UserName || target.Email == newLibrarian.Email).SingleOrDefault() == null))
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
                try
                {
                    libRepo.LibrarianRepo.Remove(target);
                    libRepo.Save();
                    TempData["Notification"] = "Delete librarian " + target.UserName + " successfully.";
                    return RedirectToAction("Index");
                }
                catch (DbUpdateException)
                {
                    TempData["Notification"] = "DbUpdateException!!!";
                    return RedirectToAction("Index");
                }
            }
            return RedirectToAction("Index");
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity;
using TestLibrary.Models;
using TestLibrary.DataAccess;

namespace TestLibrary.Controllers
{
    public class NewsManagerController : Controller
    {
        LibraryRepository libRepo = new LibraryRepository();

        [Authorize]
        public ActionResult Index()
        {
            Session["LoginUser"] = HttpContext.User.Identity.Name;
            if (HttpContext.User.Identity.Name.ToString().Substring(0, 2) != "A_")
                return RedirectToAction("Index", "Account");
            return View(libRepo.NewsRepo.List().OrderByDescending(news => news.PostTime));
        }
        
        [Authorize]
        public ActionResult AddNews()
        {
            Session["LoginUser"] = HttpContext.User.Identity.Name;
            if (HttpContext.User.Identity.Name.ToString().Substring(0, 2) != "A_")
                return RedirectToAction("Index", "Account");
            return View();
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddNews(News newsToAdd)
        {
            if (ModelState.IsValid)
            {
                newsToAdd.PostTime = DateTime.Now;
                libRepo.NewsRepo.Add(newsToAdd);
                libRepo.Save();
                TempData["Notification"] = "Add news successfully.";
                return RedirectToAction("Index");
            }
            return View(newsToAdd);
        }


        [Authorize]
        public ActionResult EditNews([DefaultValue(0)]int id)
        {
            Session["LoginUser"] = HttpContext.User.Identity.Name;
            if (HttpContext.User.Identity.Name.ToString().Substring(0, 2) != "A_")
                return RedirectToAction("Index", "Account");
            News newsToEdit = libRepo.NewsRepo.Find(id);
            if (newsToEdit != null)
                return View(newsToEdit);
            return HttpNotFound();

        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditNews(News newsToEdit)
        {
            if (ModelState.IsValid)
            {
                libRepo.NewsRepo.Update(newsToEdit);
                libRepo.Save();
                TempData["Notification"] = "Edit news successfully.";
                return RedirectToAction("Index");
            }
            return View(newsToEdit);
        }

        [Authorize]
        public ActionResult DeleteNews(int id)
        {
            Session["LoginUser"] = HttpContext.User.Identity.Name;
            if (HttpContext.User.Identity.Name.ToString().Substring(0, 2) != "A_")
                return RedirectToAction("Index", "Account");
            News newsToDelete = libRepo.NewsRepo.Find(id);
            if (newsToDelete != null)
                return View(newsToDelete);
            return HttpNotFound();
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteNews(News newsToDelete, string answer)
        {
            if (answer == "Yes")
            {
                libRepo.NewsRepo.Remove(newsToDelete);
                TempData["Notification"] = "Delete news successfully.";
                libRepo.Save();
            }
            return RedirectToAction("Index");
        }
    }
}

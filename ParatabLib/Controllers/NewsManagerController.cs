using System;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using ParatabLib.Models;
using ParatabLib.DataAccess;
using ParatabLib.ViewModels;
namespace ParatabLib.Controllers
{
    public class NewsManagerController : Controller
    {
        LibraryRepository libRepo = new LibraryRepository();

        [Authorize]
        public ActionResult Index(int page = 1,int pageSize = 10)
        {
            TempData["pageSize"] = pageSize;
            TempData["page"] = page;
            List<News> newsList = libRepo.NewsRepo.List().OrderByDescending(news => news.PostTime).ToList();
            PageList<News> pglist = new PageList<News>(newsList,page,pageSize);
            switch (pglist.Categorized())
            {
                case PageListResult.Ok: { return View(pglist); }
                case PageListResult.Empty:
                    {
                        TempData["ErrorNoti"] = "No news list to show now.";
                        return View();
                    }
                default:
                    {
                        TempData["ErrorNoti"] = "Invalid list view parameter please refresh this page to try again.";
                        return View();
                    }
            }
        }
        
        [Authorize]
        public ActionResult AddNews()
        {
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
                TempData["SuccessNoti"] = "Add news successfully.";
                return RedirectToAction("Index");
            }
            return View(newsToAdd);
        }


        [Authorize]
        public ActionResult EditNews([DefaultValue(0)]int id)
        {
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
                TempData["SuccessNoti"] = "Edit news successfully.";
                return RedirectToAction("Index");
            }
            return View(newsToEdit);
        }

        [Authorize]
        public ActionResult DeleteNews(int id)
        {
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
                TempData["SuccessNoti"] = "Delete news successfully.";
                libRepo.Save();
            }
            return RedirectToAction("Index");
        }

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (Request.HttpMethod == "GET")
            {
                if (AuthenticateController.IsUserValid(HttpContext.User.Identity.Name.Substring(2)))
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
        }

    }
}

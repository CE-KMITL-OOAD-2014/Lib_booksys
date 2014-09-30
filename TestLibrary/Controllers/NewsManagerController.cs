﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity;
using TestLibrary.Models;
using TestLibrary.DataAccess;
using TestLibrary.ViewModels;
namespace TestLibrary.Controllers
{
    public class NewsManagerController : Controller
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
            List<News> newsList = libRepo.NewsRepo.List().OrderByDescending(news => news.PostTime).ToList();
            PageList<News> pglist;
            int index = (page - 1) * pageSize;
            if (index < newsList.Count && ((index + pageSize) <= newsList.Count))
            {
                pglist = new PageList<News>(newsList.GetRange((page - 1) * pageSize, pageSize));
            }
            else if (index < newsList.Count)
            {
                pglist = new PageList<News>(newsList.GetRange((page - 1) * pageSize, newsList.Count % pageSize));
            }
            else if (newsList.Count == 0)
            {
                TempData["Notification"] = "No news list to show please create one to start the magic.";
                return View();
            }
            else
            {
                TempData["Notification"] = "Invalid list view parameter please refresh this page to try again.";
                return View();
            }

            if (newsList.Count % pageSize == 0)
                pglist.SetPageSize(newsList.Count / pageSize);
            else
                pglist.SetPageSize((newsList.Count / pageSize + 1));
            pglist.SetCurrentPage(page);
            return View(pglist);
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

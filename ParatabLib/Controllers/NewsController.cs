﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ParatabLib.DataAccess;
using ParatabLib.Models;
using ParatabLib.ViewModels;
namespace ParatabLib.Controllers
{
    public class NewsController : Controller
    {
        LibraryRepository libRepo = new LibraryRepository();
        public ActionResult View(int id)
        {
            News newstoview = libRepo.NewsRepo.Find(id);
            if (newstoview != null)
                return View(newstoview);
            else
                return HttpNotFound();
        }

        public ActionResult ViewAll(int page = 1,int pageSize = 10)
        {
            TempData["pageSize"] = pageSize;
            TempData["page"] = page;
            List<News> newsList = libRepo.NewsRepo.List().OrderByDescending(news => news.PostTime).ToList();
            PageList<News> pglist = new PageList<News>(newsList, page, pageSize);
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

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (HttpContext.User.Identity.IsAuthenticated)
            {
                if (AuthenticateController.IsUserValid(HttpContext.User.Identity.Name.Substring(2)))
                {
                    Session["LoginUser"] = HttpContext.User.Identity.Name;
                }
                else
                {
                    AuthenticateController.OnInvalidSession(ref filterContext);
                    return;
                }
            }
            else
                Session["LoginUser"] = null;
        }
    }
}
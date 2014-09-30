using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TestLibrary.DataAccess;
using TestLibrary.Models;
using TestLibrary.ViewModels;
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

        public ActionResult ViewAll(int page = 1,int pageSize = 10)
        {
            if (HttpContext.User.Identity.IsAuthenticated)
            {
                Session["LoginUser"] = HttpContext.User.Identity.Name;
            }

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
                TempData["Notification"] = "No news to show now.";
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
    }
}

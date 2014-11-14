using System;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel;
using System.Web;
using System.Web.Mvc;
using ParatabLib.Models;
using ParatabLib.DataAccess;
using ParatabLib.ViewModels;
namespace ParatabLib.Controllers
{
    //This class use to handle news management unit
    public class NewsManagerController : Controller
    {
        LibraryRepository libRepo = new LibraryRepository();

        /* This method use to call index page of news management with
         * parameterized of page and pageSize for paging news list then return result to user.
         */ 
        [Authorize]
        [OutputCache(Duration = 0, NoStore = true)]
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
        
        //This method use to call Add news page then return result to user.
        [Authorize]
        [OutputCache(Duration = 0, NoStore = true)]
        public ActionResult AddNews()
        {
            return View();
        }

        /* This class use to submit detail of news that want to add by passing newsToAdd as news
         * simply add it to database then notify success result to user.
         */ 
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
                UpdateLatestNews();
                return RedirectToAction("Index");
            }
            return View(newsToAdd);
        }

        /* This method use to call edit news page for desired news base on id parameter.
         * Find desired news in database if it exists return edit news page with desired news data,
         * otherwise notify user to input correct NewsID.
         */ 
        [Authorize]
        [OutputCache(Duration = 0, NoStore = true)]
        public ActionResult EditNews([DefaultValue(0)]int id)
        {
            News newsToEdit = libRepo.NewsRepo.Find(id);
            if (newsToEdit != null)
                return View(newsToEdit);
            TempData["ErrorNoti"] = "Please specify correct newsID.";
            return RedirectToAction("Index");

        }

        /* This method use to submit edited news data on HTTPPOST by passing newsToEdit as News
         * Then update it to related news record via libRepo object,finally notify success result.
         */ 
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
                UpdateLatestNews();
                return RedirectToAction("Index");
            }
            return View(newsToEdit);
        }

        /* This method use to call delete news confirmation page for desired news base on id.
         * Find news base on id and check that desired news is exists or not,
         * if yes return delete news confirmation page with desired news data,otherwise
         * notify user to input correct NewsID.
         */ 
        [Authorize]
        [OutputCache(Duration = 0, NoStore = true)]
        public ActionResult DeleteNews(int id)
        {
            News newsToDelete = libRepo.NewsRepo.Find(id);
            if (newsToDelete != null)
                return View(newsToDelete);
            TempData["ErrorNoti"] = "Please specify correct newsID.";
            return RedirectToAction("Index");
        }

        /* This method use to submit delete news confirmation data by passing newsToDelete as News.
         * Simply find desired news and remove it via libRepo object then notify user for success result.
         */ 
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteNews(News newsToDelete)
        {
                
                libRepo.NewsRepo.Remove(newsToDelete);
                libRepo.Save();
                TempData["SuccessNoti"] = "Delete news successfully.";
                UpdateLatestNews();
                return RedirectToAction("Index");
        }

        //This method use to update latest news from which happen from add/edit/delete news
        private void UpdateLatestNews()
        {
            List<News> newsList = libRepo.NewsRepo.List().OrderByDescending(news => news.PostTime).ToList();
                if (newsList.Count >= 7)
                    newsList = newsList.GetRange(0, 7);
                else
                    newsList = newsList.GetRange(0, newsList.Count);
                lock (typeof(List<News>))
                {
                    Controllers.NewsController.setLatestNews(newsList);
                }
        }

        /* [Override method]
         * This method use to check that whether current user for current session is exist in system or not.
         * If not,call and pass by reference of current HTTPrequest in AuthenticateController.OnInvalidSession
         * to set appropiate page result.Moreover check that current user is librarian if not redirect user to
         * account index page.
         */
        protected override void OnActionExecuting(ActionExecutingContext filterContext)
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

        /* [Override method]
         * This method use to handle exception that may occur in system
         * for some specific exception Redirect user to another page and pretend that no error occur
         * for another exception throw it and use HTTP error 500 page to handle instead.
         */
        protected override void OnException(ExceptionContext filterContext)
        {
            filterContext.ExceptionHandled = true;
            if (filterContext.Exception.GetType().Name == typeof(HttpAntiForgeryException).Name)
            {
                filterContext.Result = RedirectToAction("Index", "Account");
            }
            else if ((filterContext.Exception.GetType().Name == 
                typeof(System.Data.Entity.Infrastructure.DbUpdateConcurrencyException).Name)
                && filterContext.RouteData.Values["action"].ToString() == "DeleteNews")
            {
                TempData["ErrorNoti"] = "The news that you want to delete is already deleted.";
                filterContext.Result = RedirectToAction("Index");
            }
            else
            {
                throw filterContext.Exception;
            }

        }
    }
}

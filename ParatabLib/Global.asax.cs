using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.Optimization;
using System.IO;
using ParatabLib.Models;

namespace ParatabLib
{
    public class MvcApplication : System.Web.HttpApplication
    {
        //This method is work when server start website
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            WebApiConfig.Register(GlobalConfiguration.Configuration);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            //Set current exist users from database to AuthenticateController static properties via AddUser method
            DataAccess.LibraryRepository libRepo = new DataAccess.LibraryRepository();
            foreach (Member member in libRepo.MemberRepo.List())
                Controllers.AuthenticateController.AddUser(member.UserName);
            foreach (Librarian librarian in libRepo.LibrarianRepo.List())
                Controllers.AuthenticateController.AddUser(librarian.UserName);

            //Set latestNews that will be load in HomePage
            List<News> newsList = libRepo.NewsRepo.List().OrderByDescending(news => news.PostTime).ToList();
            if (newsList.Count != 0)
            {
                if (newsList.Count >= 7)
                    newsList = newsList.GetRange(0, 7);
                else
                    newsList = newsList.GetRange(0, newsList.Count);
                Controllers.NewsController.setLatestNews(newsList);
            }

            /* Load configuration of fine penalty from file if it is not exist create new file and set it
             * to default value(5) otherwise load value from file and set it to ConfigurationController's static properties
             * via setFine method.
             */ 

            if (!File.Exists(HttpRuntime.AppDomainAppPath+"/lib-config"))
            {

                File.AppendAllText(HttpRuntime.AppDomainAppPath + "/lib-config", "5");
            }
            else
            {
                Controllers.ConfigurationController.setFine(int.Parse
                    (File.ReadAllText(HttpRuntime.AppDomainAppPath + "/lib-config")));
            }
        }
    }
}
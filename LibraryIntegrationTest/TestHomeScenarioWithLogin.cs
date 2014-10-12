using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SpecsFor.Mvc;
using TestLibrary.Controllers;
using MvcContrib.TestHelper;
using TestLibrary.ViewModels;
using OpenQA.Selenium;

namespace LibraryIntegrationTest
{
    [TestClass]
    public class TestHomeScenarioWithLogin
    {

        private static MvcWebApp app;
        [ClassInitialize]
        public static void InitialApp(TestContext testContext){
            app = new MvcWebApp();
        }
        [TestMethod]
        public void IndexNavigate()
        {
            app.NavigateTo<HomeController>(c => c.Index());
            app.Route.ShouldMapTo<HomeController>(c => c.Index());
        }

        [TestMethod]
        public void Top10Navigate()
        {
            app.NavigateTo<HomeController>(c => c.TopTen());
            app.Route.ShouldMapTo<HomeController>(c => c.TopTen());
        }

        [TestMethod]
        public void AboutNavigate()
        {
            app.NavigateTo<HomeController>(c => c.About());
            app.Route.ShouldMapTo<HomeController>(c => c.About());
        }

        [TestMethod]
        public void ContactNavigate()
        {
            app.NavigateTo<HomeController>(c => c.Contact());
            app.Route.ShouldMapTo<HomeController>(c => c.Contact());
        }

        [TestMethod]
        public void ChangeLogNavigate()
        {
            app.NavigateTo<HomeController>(c => c.ChangeLog());
            app.Route.ShouldMapTo<HomeController>(c => c.ChangeLog());
        }


        [TestMethod]
        public void TestNonPrivilegedBrowsing()
        {
            app.NavigateTo<LibrarianTransactionController>(c => c.Index(1,10));
            app.Route.ShouldMapTo<AuthenticateController>(c => c.Login());
        }

        [TestMethod]
        public void LoginScenario()
        {
            app.NavigateTo<AuthenticateController>(c => c.Login());
            app.Browser.FindElement(By.Name("UserName")).SendKeys("paratabadmin");
            app.Browser.FindElement(By.Name("Password")).SendKeys("surawit");
            app.Browser.FindElement(By.Name("Remember")).SendKeys("0");
            app.Browser.FindElement(By.CssSelector("form input[value=\"Login\"]")).Submit();
            app.Route.ShouldMapTo<AccountController>(c => c.Index());
        }
    }
}

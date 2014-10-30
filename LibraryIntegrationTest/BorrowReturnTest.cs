using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SpecsFor.Mvc;
using MvcContrib.TestHelper;
using ParatabLib.Controllers;
using ParatabLib.ViewModels;
using OpenQA.Selenium;
namespace LibraryIntegrationTest
{
    [TestClass]
    public class BorrowReturnTest
    {
        private static MvcWebApp app;
        [ClassInitialize]
        public static void InitialTest(TestContext testContext)
        {
            app = new MvcWebApp();
        }
        [TestMethod]
        public void NavigateToHome()
        {
            app.NavigateTo<HomeController>(c => c.Index());
            app.Route.ShouldMapTo<HomeController>(c => c.Index());
        }

        [TestMethod]
        public void DotransactionTest()
        {
            app.NavigateTo<AuthenticateController>(c => c.Login());
            app.Route.ShouldMapTo<AuthenticateController>(c => c.Login());
            app.FindFormFor<LoginForm>().Field(f => f.UserName).SetValueTo("paratabadmin")
                                        .Field(f => f.Password).SetValueTo("surawits").Submit();
            app.Route.ShouldMapTo<AccountController>(c => c.Index());
            app.Browser.FindElement(By.CssSelector("body a[href=\"/Account/LibrarianPortal\"]")).Click();
            app.Route.ShouldMapTo<AccountController>(c => c.LibrarianPortal());
            app.Browser.FindElement(By.CssSelector("body a[href=\"/LibrarianTransaction/\"]")).Click();
            app.Route.ShouldMapTo<LibrarianTransactionController>();
            app.Browser.FindElement(By.CssSelector("body a[href=\"/LibrarianTransaction/Transaction\"]")).Click();
            app.Route.ShouldMapTo<LibrarianTransactionController>(c => c.Transaction());
            app.Browser.FindElement(By.Name("UserID")).SendKeys("23");
            app.Browser.FindElement(By.CssSelector("input[type=\"submit\"][value=\"Check\"]")).Submit();
            app.Browser.FindElement(By.Name("BookID")).SendKeys("37");
            app.Browser.FindElement(By.CssSelector("input[type=\"submit\"][value=\"Submit\"]")).Submit();
            Assert.AreEqual(true, app.Browser.FindElement(By.ClassName("noti-green")).Text.Contains("Borrow for member"));
            app.Browser.FindElement(By.Name("UserID")).Clear();
            app.Browser.FindElement(By.Name("UserID")).SendKeys("14");
            app.Browser.FindElement(By.CssSelector("input[type=\"submit\"][value=\"Check\"]")).Submit();
            IReadOnlyCollection<IWebElement> returnbtt = app.Browser.FindElements(By.CssSelector
                ("input[type=\"submit\"][value=\"Return\"]"));
            IEnumerator<IWebElement> it = returnbtt.GetEnumerator();
            it.MoveNext();
            it.Current.Click();
            Assert.AreEqual(true, app.Browser.FindElement(By.ClassName("noti-green")).Text.Contains("Return successfully."));
            app.NavigateTo<AuthenticateController>(c => c.Logout());
            app.Route.ShouldMapTo<HomeController>(c => c.Index());
        }
    }
}

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
    //This class is integration test for Borrow-return test scenario
    [TestClass]
    public class BorrowReturnTest
    {
        private static MvcWebApp app;

        /* MvcWebApp use to instantiate web browse object that will open in PC and user can use
         * any method/properties to control browse(Ex.Find element of DOM).
         */
        [ClassInitialize]
        public static void InitialTest(TestContext testContext)
        {
            app = new MvcWebApp();
        }

        //The integration test start by browing to homepage and check that whether it browse to correct page... 
        /* [TestMethod]
         public void NavigateToHome()
         {
             app.NavigateTo<HomeController>(c => c.Index());
             app.Route.ShouldMapTo<HomeController>(c => c.Index());
         }*/

        //The second step is login as librarian and do the transaction in one try....
        [TestMethod]
        public void DotransactionTest()
        {
            // -------Login -------
            app.NavigateTo<AuthenticateController>(c => c.Login());
            app.Route.ShouldMapTo<AuthenticateController>(c => c.Login());
            app.FindFormFor<LoginForm>().Field(f => f.UserName).SetValueTo("paratabadmin")
                                        .Field(f => f.Password).SetValueTo("surawits").Submit();
            app.Route.ShouldMapTo<AccountController>(c => c.Index());
            // -------------------- 
            /* If librarian is login success next step is go to librarian portal.
             * Browse to librarianPortal page and check that is it browse to correct page.
             */
            app.Browser.FindElement(By.CssSelector("body a[href=\"/Account/LibrarianPortal\"]")).Click();
            app.Route.ShouldMapTo<AccountController>(c => c.LibrarianPortal());

            /* If last test is success,next step is go to LibrarianTransaction index page.
             * Browse to that page and check that is it browse to correct page.
             */
            app.Browser.FindElement(By.CssSelector("body a[href=\"/LibrarianTransaction/\"]")).Click();
            app.Route.ShouldMapTo<LibrarianTransactionController>();

            /* If last test is success,next step is go to Transaction page.
             * Browse to that page and check that is it browse to correct page.
             */
            app.Browser.FindElement(By.CssSelector("body a[href=\"/LibrarianTransaction/Transaction\"]")).Click();
            app.Route.ShouldMapTo<LibrarianTransactionController>(c => c.Transaction());

            /* Wow we reach to the one of main test of this integration test
             * test borrow book for member#23/borrow book 35 and check notify result
             */
            app.Browser.FindElement(By.Name("UserID")).SendKeys("23");
            app.Browser.FindElement(By.CssSelector("input[type=\"submit\"][value=\"Check\"]")).Submit();
            app.Browser.FindElement(By.Name("BookID")).SendKeys("35");
            app.Browser.FindElement(By.CssSelector("input[type=\"submit\"][value=\"Submit\"]")).Submit();
            Assert.AreEqual(true, app.Browser.FindElement(By.ClassName("noti-green")).Text.Contains("Borrow for member"));

            /* The last one of main test of this integration test
             * test return book for member#14 and first borrow book that appear on borrow list in 
             * returnbtt.GetEnumerator() use click() method on IWebElement object and check notify result.
             */
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

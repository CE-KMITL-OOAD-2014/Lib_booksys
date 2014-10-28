using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;
using ParatabLib.Controllers;
using ParatabLib.ViewModels;
using MvcContrib.TestHelper;
using SpecsFor.Mvc;

namespace LibraryIntegrationTest
{
    [TestClass]
    public class BrowseEveryPageTest
    {
        private static MvcWebApp app;
        [ClassInitialize]
        public static void InitApp(TestContext testContext)
        {
            app = new MvcWebApp();
        }

        [TestMethod]
        public void HomeIndex()
        {
            app.NavigateTo<HomeController>(c => c.Index());
            app.Route.ShouldMapTo<HomeController>(c => c.Index());
        }

        [TestMethod]
        public void HomeTopTen()
        {
            app.NavigateTo<HomeController>(c => c.TopTen());
            app.Route.ShouldMapTo<HomeController>(c => c.TopTen());
        }

        [TestMethod]
        public void HomeAboutUs()
        {
            app.NavigateTo<HomeController>(c => c.About());
            app.Route.ShouldMapTo<HomeController>(c => c.About());
        }

        [TestMethod]
        public void HomeContact()
        {
            app.NavigateTo<HomeController>(c => c.Contact());
            app.Route.ShouldMapTo<HomeController>(c => c.Contact());
        }

        [TestMethod]
        public void HomeChangeLog()
        {
            app.NavigateTo<HomeController>(c => c.ChangeLog());
            app.Route.ShouldMapTo<HomeController>(c => c.ChangeLog());
        }

        [TestMethod]
        public void HomeLibraryApi()
        {
            app.NavigateTo<HomeController>(c => c.LibraryApi());
            app.Route.ShouldMapTo<HomeController>(c => c.LibraryApi());
        }

        [TestMethod]
        public void AuthenticateRegister()
        {
            app.NavigateTo<AuthenticateController>(c => c.Register());
            app.Route.ShouldMapTo<AuthenticateController>(c => c.Register());
        }

        [TestMethod]
        public void AuthenticateForgotPwd()
        {
            app.NavigateTo<AuthenticateController>(c => c.ForgotPassword());
            app.Route.ShouldMapTo<AuthenticateController>(c => c.ForgotPassword());
        }

        [TestMethod]
        public void AuthenticateResetPwd()
        {
            app.NavigateTo<AuthenticateController>(c => c.ResetPassword(""));
            app.Route.ShouldMapTo<AuthenticateController>(c => c.Login());
        }

        [TestMethod]
        public void AuthenticateLogin()
        {
            app.NavigateTo<AuthenticateController>(c => c.Login());
            app.Route.ShouldMapTo<AuthenticateController>(c => c.Login());
        }

        [TestMethod]
        public void AccountIndex()
        {
            app.NavigateTo<AccountController>(c => c.Index());
            app.Route.ShouldMapTo<AuthenticateController>(c => c.Login());
        }

        [TestMethod]
        public void AccountChangePassword()
        {
            app.NavigateTo<AccountController>(c => c.ChangePassword());
            app.Route.ShouldMapTo<AuthenticateController>(c => c.Login());
        }

        [TestMethod]
        public void AccountEditAccount()
        {
            app.NavigateTo<AccountController>(c => c.EditAccount());
            app.Route.ShouldMapTo<AuthenticateController>(c => c.Login());
        }

        [TestMethod]
        public void BookMntIndex()
        {
            app.NavigateTo<BookManagerController>(c => c.Index(1, 10));
            app.Route.ShouldMapTo<AuthenticateController>(c => c.Login());
        }

        [TestMethod]
        public void BookMntAddBook()
        {
            app.NavigateTo<BookManagerController>(c => c.AddBook());
            app.Route.ShouldMapTo<AuthenticateController>(c => c.Login());
        }

        [TestMethod]
        public void BookMntEditBook()
        {
            app.NavigateTo<BookManagerController>(c => c.EditBook(1));
            app.Route.ShouldMapTo<AuthenticateController>(c => c.Login());
        }

        [TestMethod]
        public void BookMntDeleteBook()
        {
            app.NavigateTo<BookManagerController>(c => c.DeleteBook(1));
            app.Route.ShouldMapTo<AuthenticateController>(c => c.Login());
        }

        [TestMethod]
        public void LibrarianListIndex()
        {
            app.NavigateTo<LibrarianListManagerController>(c => c.Index(1, 10));
            app.Route.ShouldMapTo<AuthenticateController>(c => c.Login());
        }

        [TestMethod]
        public void LibrarianListAddLibrarian()
        {
            app.NavigateTo<LibrarianListManagerController>(c => c.AddLibrarian());
            app.Route.ShouldMapTo<AuthenticateController>(c => c.Login());
        }

        [TestMethod]
        public void LibrarianListView()
        {
            app.NavigateTo<LibrarianListManagerController>(c => c.View(8));
            app.Route.ShouldMapTo<AuthenticateController>(c => c.Login());
        }

        [TestMethod]
        public void LibrarianListDelete()
        {
            app.NavigateTo<LibrarianListManagerController>(c => c.Delete(8));
            app.Route.ShouldMapTo<AuthenticateController>(c => c.Login());
        }

        [TestMethod]
        public void LibTransactionIndex()
        {
            app.NavigateTo<LibrarianTransactionController>(c => c.Index(1, 10));
            app.Route.ShouldMapTo<AuthenticateController>(c => c.Login());
        }

        [TestMethod]
        public void LibTransactionTransaction()
        {
            app.NavigateTo<LibrarianTransactionController>(c => c.Transaction());
            app.Route.ShouldMapTo<AuthenticateController>(c => c.Login());
        }

        [TestMethod]
        public void MemberListIndex()
        {
            app.NavigateTo<MemberListManagerController>(c => c.Index(1, 10));
            app.Route.ShouldMapTo<AuthenticateController>(c => c.Login());
        }

        [TestMethod]
        public void MemberListView()
        {
            app.NavigateTo<MemberListManagerController>(c => c.View(23));
            app.Route.ShouldMapTo<AuthenticateController>(c => c.Login());
        }

        [TestMethod]
        public void MemberListDelete()
        {
            app.NavigateTo<MemberListManagerController>(c => c.Delete(23));
            app.Route.ShouldMapTo<AuthenticateController>(c => c.Login());
        }

        [TestMethod]
        public void MemberTransactionIndex()
        {
            app.NavigateTo<MemberTransactionController>(c => c.Index());
            app.Route.ShouldMapTo<AuthenticateController>(c => c.Login());
        }

        [TestMethod]
        public void MemberTransactionRenew()
        {
            app.NavigateTo<MemberTransactionController>(c => c.Renew(30));
            app.Route.ShouldMapTo<AuthenticateController>(c => c.Login());
        }

        [TestMethod]
        public void MemberTransactionRequest()
        {
            app.NavigateTo<MemberTransactionController>(c => c.Request());
            app.Route.ShouldMapTo<AuthenticateController>(c => c.Login());
        }

        [TestMethod]
        public void MemberTransactionCancelReq()
        {
            app.NavigateTo<MemberTransactionController>(c => c.CancelRequest(100));
            app.Route.ShouldMapTo<AuthenticateController>(c => c.Login());
        }

        [TestMethod]
        public void MemberTransactionBrHistory()
        {
            app.NavigateTo<MemberTransactionController>(c => c.BorrowHistory(1, 10));
            app.Route.ShouldMapTo<AuthenticateController>(c => c.Login());
        }

        [TestMethod]
        public void NewsMntIndex()
        {
            app.NavigateTo<NewsManagerController>(c => c.Index(1, 10));
            app.Route.ShouldMapTo<AuthenticateController>(c => c.Login());
        }

        [TestMethod]
        public void NewsMntAddNews()
        {
            app.NavigateTo<NewsManagerController>(c => c.AddNews());
            app.Route.ShouldMapTo<AuthenticateController>(c => c.Login());
        }

        [TestMethod]
        public void NewsMntEditNews()
        {
            app.NavigateTo<NewsManagerController>(c => c.EditNews(10));
            app.Route.ShouldMapTo<AuthenticateController>(c => c.Login());
        }

        [TestMethod]
        public void NewsMntDeleteNews()
        {
            app.NavigateTo<NewsManagerController>(c => c.DeleteNews(10));
            app.Route.ShouldMapTo<AuthenticateController>(c => c.Login());
        }

        [TestMethod]
        public void BackToHome()
        {
            app.NavigateTo<HomeController>(c => c.Index());
            app.Route.ShouldMapTo<HomeController>(c => c.Index());
        }
        
        [TestMethod]
        public void QuickSearch()
        {
            app.NavigateTo<HomeController>(c => c.Index());
            app.Browser.FindElement(By.CssSelector("form.search-bar input[value=\"Search\"]")).Submit();
            app.Route.ShouldMapTo<BookSearchController>();
            app.Browser.Title.AssertSameStringAs("Search result");
        }

        [TestMethod]
        public void BookSearchAndView()
        {
            app.NavigateTo<BookSearchController>(c => c.Index());
            app.Browser.FindElement(By.CssSelector("form select[name=\"SearchType\"]")).Click();
            app.Browser.FindElement(By.CssSelector("form option[value=\"BookName\"]")).Click();
            app.Browser.FindElement(By.CssSelector("form select[name=\"SearchType\"]")).Click();
            app.Browser.FindElement(By.CssSelector("form[action=\"/BookSearch/Basic\"]")).Submit();
            app.Route.ShouldMapTo<BookSearchController>();
            IReadOnlyCollection<IWebElement> test = app.Browser.FindElements(By.ClassName("view-btt"));
            IEnumerator<IWebElement> it = test.GetEnumerator();
            it.MoveNext();
            it.Current.Click();
            app.Route.ShouldMapTo<BookController>();
        }

        [TestMethod]
        public void MemberLogin()
        {
            app.NavigateTo<AuthenticateController>(c => c.Login());
            app.FindFormFor<LoginForm>().Field<string>(c => c.UserName).SetValueTo("benchbb05")
                                        .Field<string>(c => c.Password).SetValueTo("12345678")
                                        .Submit();
            app.Route.ShouldMapTo<AccountController>(c => c.Index());
        }

        //Test view news
        //Loopback to test all browsing
    }
}

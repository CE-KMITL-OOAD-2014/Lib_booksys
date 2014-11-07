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
    //This class is integration test of BrowseEverypage test for each type of user.
    [TestClass]
    public class BrowseEveryPageTest
    {
        private static MvcWebApp app;

        /* MvcWebApp use to instantiate web browse object that will open in PC and user can use
         * any method/properties to control browse(Ex.Find element of DOM).
         */ 

        [ClassInitialize]
        public static void InitApp(TestContext testContext)
        {
            app = new MvcWebApp();
        }

        /* This method is test scenario for general user
         * no authentication browse every page directly! Finally check result for each browsing
         */ 

        [TestMethod]
        public void GeneralUserBrowsing()
        {
            BrowseEveryPage();
        }

        /* The second one is test scenario for member user
         * by browse to login page and login as member user
         * then try to work and test like general user and check result.
         * If user travel all page,sign out suddenly.
         */ 
        [TestMethod]
        public void MemberBrowsing()
        {
            app.NavigateTo<AuthenticateController>(c => c.Login());
            app.FindFormFor<LoginForm>().Field(f => f.UserName).SetValueTo("benchbb05").
                                         Field(f => f.Password).SetValueTo("12345678").
                                         Submit();
            app.Route.ShouldMapTo<AccountController>(c => c.Index());
            BrowseEveryPage();
            UserSignout();
        }

        /* The second one is test scenario for librarian user
         * by browse to login page and login as librarian user
         * then try to work and test like general user and check result.
         * If user travel all page,sign out suddenly.
         */ 
        [TestMethod]
        public void GeneralLibrarianBrowsing()
        {
            app.NavigateTo<AuthenticateController>(c => c.Login());
            app.FindFormFor<LoginForm>().Field(f => f.UserName).SetValueTo("paratabadmin")
                                        .Field(f => f.Password).SetValueTo("surawits").Submit();
            app.Route.ShouldMapTo<AccountController>(c => c.Index());
            BrowseEveryPage();
            UserSignout();
        }

        /* This method include all page that user probably to browsing
         * by call each method user and browseing to desired page(or rediect to another page)
         */ 
        private void BrowseEveryPage()
        {
            HomeIndex();
            HomeTopTen();
            HomeAboutUs();
            HomeContact();
            HomeChangeLog();
            HomeLibraryApi();
            AuthenticateRegister();
            AuthenticateForgotPwd();
            AuthenticateResetPwd();
            AuthenticateLogin();
            AccountIndex();
            AccountChangePassword();
            AccountEditAccount();
            BookMntIndex();
            BookMntAddBook();
            BookMntEditBook();
            BookMntDeleteBook();
            LibrarianListIndex();
            LibrarianListAddLibrarian();
            LibrarianListView();
            LibrarianListDelete();
            LibTransactionIndex();
            LibTransactionTransaction();
            MemberListIndex();
            MemberListView();
            MemberListDelete();
            MemberTransactionIndex();
            MemberTransactionRenew();
            MemberTransactionRequest();
            MemberTransactionCancelReq();
            MemberTransactionBrHistory();
            NewsMntIndex();
            NewsMntAddNews();
            NewsMntEditNews();
            NewsMntDeleteNews();
            BackToHome();
            QuickSearch();
            BookSearchAndView();
            BrowseNews();
            BrowseAllNews();
        }

        /* This method use to check that for member user is application work properly
         * when member browsing to "only-member-access" page and check that if
         * librarian or general user browse to those page,the appliation redirect those user
         * to index or login.
         */ 
        private void IsRedirectToRightPathForMember<Controller>(System.Linq.Expressions.
            Expression<Func<Controller,System.Web.Mvc.ActionResult>>action) 
            where Controller : System.Web.Mvc.Controller
        {
            IWebElement userfield = app.Browser.FindElement(By.ClassName("reg-login-linkzone"));
            if (userfield.Text.Contains("Librarian portal"))
                app.Route.ShouldMapTo<AccountController>(c => c.Index());
            else if (userfield.Text.Contains("My account"))
                app.Route.ShouldMapTo<Controller>(action);
            else
                app.Route.ShouldMapTo<AuthenticateController>(c => c.Login());
        }

        /* This method use to check that for librarian user is application work properly
         * when librarian browsing to "only-librarian-access" page and check that if
         * member or general user browse to those page,the appliation redirect those user
         * to index or login.
         */
        private void IsRedirectToRightPathForLibrarian<Controller>(System.Linq.Expressions.Expression<Func<Controller, System.Web.Mvc.ActionResult>> action)
                        where Controller : System.Web.Mvc.Controller
        {
            IWebElement userfield = app.Browser.FindElement(By.ClassName("reg-login-linkzone"));
            if (userfield.Text.Contains("Librarian portal"))
                app.Route.ShouldMapTo<Controller>(action);
            else if (userfield.Text.Contains("My account"))
                app.Route.ShouldMapTo<AccountController>(c => c.Index());
            else
                app.Route.ShouldMapTo<AuthenticateController>(c => c.Login());
        }

        /* This method use to check that for authenticated user is application work properly
         * when user browsing to "only-authorized user" page and check that if
         * general user browse to those page,the appliation redirect those user
         * to login.
         */
        private void IsRedirectToRightPathForBoth<Controller>(System.Linq.Expressions.Expression<Func<Controller, System.Web.Mvc.ActionResult>> action)
        where Controller : System.Web.Mvc.Controller
        {
            IWebElement userfield = app.Browser.FindElement(By.ClassName("reg-login-linkzone"));
            if (userfield.Text.Contains("Librarian portal") || userfield.Text.Contains("My account"))
                app.Route.ShouldMapTo<Controller>(action);
            else
                app.Route.ShouldMapTo<AuthenticateController>(c => c.Login());
        }

        /* This method use to check that for general user is application work properly
         * when general user browsing to "only-anonymous user" page and check that if
         * authorized user browse to those page,the appliation redirect those user
         * to another page.
         */
        private void IsRedirectToRightPathForGenUser<Controller>(System.Linq.Expressions.Expression<Func<Controller, System.Web.Mvc.ActionResult>> action)
                where Controller : System.Web.Mvc.Controller
        {
            IWebElement userfield = app.Browser.FindElement(By.ClassName("reg-login-linkzone"));
            if (userfield.Text.Contains("Librarian portal") || userfield.Text.Contains("My account"))
                app.Route.ShouldMapTo<HomeController>(c => c.Index());
            else
                app.Route.ShouldMapTo<Controller>(action);
        }

        //The rest of integration test code is implementation of browsing and checks for each page.
        private void HomeIndex()
        {
           app.NavigateTo<HomeController>(c => c.Index());
           app.Route.ShouldMapTo<HomeController>(c => c.Index());
        }

        private void HomeTopTen()
        {
            app.NavigateTo<HomeController>(c => c.TopTen());
            app.Route.ShouldMapTo<HomeController>(c => c.TopTen());
        }

        private void HomeAboutUs()
        {
            app.NavigateTo<HomeController>(c => c.About());
            app.Route.ShouldMapTo<HomeController>(c => c.About());
        }

        private void HomeContact()
        {
            app.NavigateTo<HomeController>(c => c.Contact());
            app.Route.ShouldMapTo<HomeController>(c => c.Contact());
        }

        private void HomeChangeLog()
        {
            app.NavigateTo<HomeController>(c => c.ChangeLog());
            app.Route.ShouldMapTo<HomeController>(c => c.ChangeLog());
        }
        
        private void HomeLibraryApi()
        {
            app.NavigateTo<HomeController>(c => c.LibraryApi());
            app.Route.ShouldMapTo<HomeController>(c => c.LibraryApi());
        }

        private void AuthenticateRegister()
        {
            app.NavigateTo<AuthenticateController>(c => c.Register());
            IsRedirectToRightPathForGenUser<AuthenticateController>(c => c.Register());
        }

        private void AuthenticateForgotPwd()
        {
            app.NavigateTo<AuthenticateController>(c => c.ForgotPassword());
            IsRedirectToRightPathForGenUser<AuthenticateController>(c => c.ForgotPassword());
        }

        private void AuthenticateResetPwd()
        {
            app.NavigateTo<AuthenticateController>(c => c.ResetPassword(""));
            IWebElement userfield = app.Browser.FindElement(By.ClassName("reg-login-linkzone"));
            if (userfield.Text.Contains("Librarian portal") || userfield.Text.Contains("My account"))
                app.Route.ShouldMapTo<AccountController>(c => c.Index());
            else
                app.Route.ShouldMapTo<AuthenticateController>(c => c.Login());
        }

        private void AuthenticateLogin()
        {
            app.NavigateTo<AuthenticateController>(c => c.Login());
            IsRedirectToRightPathForGenUser<AuthenticateController>(c => c.Login());
        }

        private void AccountIndex()
        {
            app.NavigateTo<AccountController>(c => c.Index());
            IsRedirectToRightPathForBoth<AccountController>(c => c.Index());
        }

        private void AccountChangePassword()
        {
            app.NavigateTo<AccountController>(c => c.ChangePassword());
            IsRedirectToRightPathForBoth<AccountController>(c => c.ChangePassword());
        }

        private void AccountEditAccount()
        {
            app.NavigateTo<AccountController>(c => c.EditAccount());
            IsRedirectToRightPathForBoth<AccountController>(c => c.EditAccount());
        }

        private void BookMntIndex()
        {
            app.NavigateTo<BookManagerController>(c => c.Index(1, 10));
            IsRedirectToRightPathForLibrarian<BookManagerController>(c => c.Index(1, 10));
        }

        private void BookMntAddBook()
        {
            app.NavigateTo<BookManagerController>(c => c.AddBook());
            IsRedirectToRightPathForLibrarian<BookManagerController>(c => c.AddBook());
        }

        private void BookMntEditBook()
        {
            app.NavigateTo<BookManagerController>(c => c.EditBook(33));
            IsRedirectToRightPathForLibrarian<BookManagerController>(c => c.EditBook(33));
        }

        private void BookMntDeleteBook()
        {
            app.NavigateTo<BookManagerController>(c => c.DeleteBook(33));
            IsRedirectToRightPathForLibrarian<BookManagerController>(c => c.DeleteBook(33));
        }

        private void LibrarianListIndex()
        {
            app.NavigateTo<LibrarianListManagerController>(c => c.Index(1, 10));
            IsRedirectToRightPathForLibrarian<LibrarianListManagerController>(c => c.Index(1, 10));
        }

        private void LibrarianListAddLibrarian()
        {
            app.NavigateTo<LibrarianListManagerController>(c => c.AddLibrarian());
            IsRedirectToRightPathForLibrarian<LibrarianListManagerController>(c => c.AddLibrarian());
        }

        private void LibrarianListView()
        {
            app.NavigateTo<LibrarianListManagerController>(c => c.View(8));
            IsRedirectToRightPathForLibrarian<LibrarianListManagerController>(c => c.View(8));
        }

        private void LibrarianListDelete()
        {
            app.NavigateTo<LibrarianListManagerController>(c => c.Delete(8));
            IsRedirectToRightPathForLibrarian<LibrarianListManagerController>(c => c.Delete(8));
        }

        private void LibTransactionIndex()
        {
            app.NavigateTo<LibrarianTransactionController>(c => c.Index(1, 10));
            IsRedirectToRightPathForLibrarian<LibrarianTransactionController>(c => c.Index(1, 10));
        }

        private void LibTransactionTransaction()
        {
            app.NavigateTo<LibrarianTransactionController>(c => c.Transaction());
            IsRedirectToRightPathForLibrarian<LibrarianTransactionController>(c => c.Transaction());
        }

        private void MemberListIndex()
        {
            app.NavigateTo<MemberListManagerController>(c => c.Index(1, 10));
            IsRedirectToRightPathForLibrarian<MemberListManagerController>(c => c.Index(1, 10));
        }

        private void MemberListView()
        {
            app.NavigateTo<MemberListManagerController>(c => c.View(23));
            IsRedirectToRightPathForLibrarian<MemberListManagerController>(c => c.View(23));
        }

        private void MemberListDelete()
        {
            app.NavigateTo<MemberListManagerController>(c => c.Delete(23));
            IsRedirectToRightPathForLibrarian<MemberListManagerController>(c => c.Delete(23));
        }

        private void MemberTransactionIndex()
        {
            app.NavigateTo<MemberTransactionController>(c => c.Index());
            IsRedirectToRightPathForMember<MemberTransactionController>(c => c.Index());
        }

        private void MemberTransactionRenew()
        {
            app.NavigateTo<MemberTransactionController>(c => c.Renew(30));
            IsRedirectToRightPathForMember<MemberTransactionController>(c => c.Index());
        }

        private void MemberTransactionRequest()
        {
            app.NavigateTo<MemberTransactionController>(c => c.Request());
            IsRedirectToRightPathForMember<MemberTransactionController>(c => c.Request());
        }

        private void MemberTransactionCancelReq()
        {
            app.NavigateTo<MemberTransactionController>(c => c.CancelRequest(100));
            IsRedirectToRightPathForMember<MemberTransactionController>(c => c.Index());
        }

        private void MemberTransactionBrHistory()
        {
            app.NavigateTo<MemberTransactionController>(c => c.BorrowHistory(1, 10));
            IsRedirectToRightPathForMember<MemberTransactionController>(c => c.BorrowHistory(1, 10));
        }

        private void NewsMntIndex()
        {
            app.NavigateTo<NewsManagerController>(c => c.Index(1, 10));
            IsRedirectToRightPathForLibrarian<NewsManagerController>(c => c.Index(1, 10));
        }

        private void NewsMntAddNews()
        {
            app.NavigateTo<NewsManagerController>(c => c.AddNews());
            IsRedirectToRightPathForLibrarian<NewsManagerController>(c => c.AddNews());
        }

        private void NewsMntEditNews()
        {
            app.NavigateTo<NewsManagerController>(c => c.EditNews(11));
            IsRedirectToRightPathForLibrarian<NewsManagerController>(c => c.EditNews(11));
        }

        private void NewsMntDeleteNews()
        {
            app.NavigateTo<NewsManagerController>(c => c.DeleteNews(11));
            IsRedirectToRightPathForLibrarian<NewsManagerController>(c => c.DeleteNews(11));
        }

        private void BackToHome()
        {
            app.NavigateTo<HomeController>(c => c.Index());
            app.Route.ShouldMapTo<HomeController>(c => c.Index());
        }

        //The below method use to test search but not it main purpose...
        private void QuickSearch()
        {
            app.NavigateTo<HomeController>(c => c.Index());
            app.Browser.FindElement(By.CssSelector("form.search-bar input[value=\"Search\"]")).Submit();
            app.Route.ShouldMapTo<BookSearchController>();
            app.Browser.Title.AssertSameStringAs("Search result");
        }

        private void BookSearchAndView()
        {
            app.NavigateTo<BookSearchController>(c => c.Index());
            app.Browser.FindElement(By.CssSelector("form select[name=\"SearchType\"]")).Click();
            app.Browser.FindElement(By.CssSelector("form option[value=\"BookName\"]")).Click();
            app.Browser.FindElement(By.CssSelector("form select[name=\"SearchType\"]")).Click();
            app.Browser.FindElement(By.CssSelector("form[action=\"/BookSearch/Basic\"]")).Submit();
            app.Route.ShouldMapTo<BookSearchController>();
            IReadOnlyCollection<IWebElement> viewbttlist = app.Browser.FindElements(By.ClassName("view-btt"));
            IEnumerator<IWebElement> it = viewbttlist.GetEnumerator();
            it.MoveNext();
            it.Current.Click();
            app.Route.ShouldMapTo<BookController>();
        }

        private void UserSignout()
        {
            app.NavigateTo<AuthenticateController>(c => c.Logout());
            app.Route.ShouldMapTo<HomeController>(c => c.Index());
        }

        private void BrowseNews()
        {
            app.NavigateTo<HomeController>(c => c.Index());
            IReadOnlyCollection<IWebElement> newslink = app.Browser.FindElements(By.ClassName("news-link"));
            IEnumerator<IWebElement> it = newslink.GetEnumerator(); 
            it.MoveNext();
            it.Current.Click();
            app.Route.ShouldMapTo<NewsController>();
        }

        private void BrowseAllNews()
        {
            app.NavigateTo<HomeController>(c => c.Index());
            app.Browser.FindElement(By.CssSelector("a[href=\"/News/ViewAll\"]")).Click();
            app.Route.ShouldMapTo<NewsController>();
        }
    }
}
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
        public void GeneralUserBrowsing()
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
        }

        [TestMethod]
        public void MemberBrowsing()
        {
            app.NavigateTo<AuthenticateController>(c => c.Login());
            app.FindFormFor<LoginForm>().Field(f => f.UserName).SetValueTo("benchbb05").
                                         Field(f => f.Password).SetValueTo("12345678").
                                         Submit();
            app.Route.ShouldMapTo<AccountController>(c => c.Index());
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
            UserSignout();
        }

        [TestMethod]
        public void GeneralLibrarianBrowsing()
        {
            app.NavigateTo<AuthenticateController>(c => c.Login());
            app.FindFormFor<LoginForm>().Field(f => f.UserName).SetValueTo("paratabadmin")
                                        .Field(f => f.Password).SetValueTo("surawits").Submit();
            app.Route.ShouldMapTo<AccountController>(c => c.Index());
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
            UserSignout();
        }
        private void IsRedirectToRightPathForMember<Controller>(System.Linq.Expressions.Expression<Func<Controller,System.Web.Mvc.ActionResult>>action) 
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


        private void IsRedirectToRightPathForBoth<Controller>(System.Linq.Expressions.Expression<Func<Controller, System.Web.Mvc.ActionResult>> action)
        where Controller : System.Web.Mvc.Controller
        {
            IWebElement userfield = app.Browser.FindElement(By.ClassName("reg-login-linkzone"));
            if (userfield.Text.Contains("Librarian portal") || userfield.Text.Contains("My account"))
                app.Route.ShouldMapTo<Controller>(action);
            else
                app.Route.ShouldMapTo<AuthenticateController>(c => c.Login());
        }


        private void IsRedirectToRightPathForGenUser<Controller>(System.Linq.Expressions.Expression<Func<Controller, System.Web.Mvc.ActionResult>> action)
                where Controller : System.Web.Mvc.Controller
        {
            IWebElement userfield = app.Browser.FindElement(By.ClassName("reg-login-linkzone"));
            if (userfield.Text.Contains("Librarian portal") || userfield.Text.Contains("My account"))
                app.Route.ShouldMapTo<HomeController>(c => c.Index());
            else
                app.Route.ShouldMapTo<Controller>(action);
        }

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
            app.NavigateTo<BookManagerController>(c => c.EditBook(1));
            IsRedirectToRightPathForLibrarian<BookManagerController>(c => c.EditBook(1));
        }

        private void BookMntDeleteBook()
        {
            app.NavigateTo<BookManagerController>(c => c.DeleteBook(1));
            IsRedirectToRightPathForLibrarian<BookManagerController>(c => c.DeleteBook(1));
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
            app.NavigateTo<NewsManagerController>(c => c.EditNews(10));
            IsRedirectToRightPathForLibrarian<NewsManagerController>(c => c.EditNews(10));
        }

        private void NewsMntDeleteNews()
        {
            app.NavigateTo<NewsManagerController>(c => c.DeleteNews(10));
            IsRedirectToRightPathForLibrarian<NewsManagerController>(c => c.DeleteNews(10));
        }

        private void BackToHome()
        {
            app.NavigateTo<HomeController>(c => c.Index());
            app.Route.ShouldMapTo<HomeController>(c => c.Index());
        }

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
      /*  [TestMethod]
        public void MemberLogin()
        {
            app.NavigateTo<AuthenticateController>(c => c.Login());
            app.FindFormFor<LoginForm>().Field<string>(c => c.UserName).SetValueTo("benchbb05")
                                        .Field<string>(c => c.Password).SetValueTo("12345678")
                                        .Submit();
            app.Route.ShouldMapTo<AccountController>(c => c.Index());
            AuthenticateRegister();
        }

        


        private void TravelEveryPage()
        {

        }*/
        //Test view news
        //Loopback to test all browsing
    }
}

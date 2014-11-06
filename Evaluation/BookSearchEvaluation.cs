using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;
using OpenQA;
using SpecsFor.Mvc;
using System.Threading;
using MvcContrib.TestHelper;
using OpenQA.Selenium.IE;
using OpenQA.Selenium.Support.UI;
using OpenQA.Selenium.Chrome;
namespace LibraryIntegrationTest
{
    /* This class is evaluation for book search performance measure.
     * by use integration test to measure it.
     */
    [TestClass]
    public class RobustnessTest
    {
        /* This object is not same as MvcWebApp it is object of Selenium so this experiment
         * is not use virtual server simulation but use only automated-browser.
         */ 
        private static InternetExplorerDriver driver;
        [TestMethod]
        public void TestBookSearch()
        {
            //First step is browse to website and go to search page.
            driver = new InternetExplorerDriver();
            driver.Manage().Window.Maximize();
            driver.Navigate().GoToUrl("paratabplus.cloudapp.net");
            driver.FindElementByClassName("menulist").FindElement(By.CssSelector("li a[href=\"/BookSearch/\"]")).Click();
            Assert.AreEqual("Search", driver.Title);
            //-----------------------------------
            /* Next step is find book from define keywords then submit
             * measure time from result that display in grey character on find result(if search result is found.)
             * record it and use these measure data to summarize.
             */ 
            driver.FindElementByName("Keyword").SendKeys("The");
            driver.FindElementByTagName("select").Click();
            driver.Keyboard.SendKeys(Keys.Down);
            driver.Keyboard.SendKeys(Keys.Down);
            driver.Keyboard.SendKeys(Keys.Enter);
            driver.Keyboard.SendKeys(Keys.Tab);
            driver.Manage().Timeouts().SetPageLoadTimeout(TimeSpan.FromMilliseconds(1000));
            IWebElement test = driver.FindElementByCssSelector("form[action=\"/BookSearch/Basic\"]");
            DateTime d = DateTime.Now;
            test.Submit();
            Assert.AreEqual(DateTime.Now.Date, (d - DateTime.Now).Negate());

            //-----------------------------------
            driver.FindElementByCssSelector("input[type=\"text\"][name=\"CallNumber\"]").SendKeys("ETC");
            driver.FindElementByCssSelector("form input[placeholder=\"Book name\"]").SendKeys("The first");
            driver.FindElementByCssSelector("form input[placeholder=\"Author\"]").SendKeys("Suratchanan");
            driver.FindElementByCssSelector("form input[placeholder=\"Publisher\"]").SendKeys("SK");
            driver.FindElementByCssSelector("form input[placeholder=\"Year\"]").SendKeys("2014");
            driver.FindElementByCssSelector("form[action=\"/BookSearch/Advance\"] input[type=\"submit\"]").Click();
        }

        //Close browser to reallocate memory.
        [ClassCleanup]
        public static void clean()
        {
            driver.Close();
        }
    }
}

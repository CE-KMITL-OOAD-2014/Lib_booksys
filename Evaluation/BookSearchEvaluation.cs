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
    public class BookSearchEval
    {
        /* This object is not same as MvcWebApp it is object of Selenium so this experiment
         * is not use virtual server simulation but use only automated-browser.
         */
        private static InternetExplorerDriver driver;
        public void AssertSearchResult(string caption)
        {
            IWebElement e;
            try
            {
                e = driver.FindElementByCssSelector("span#find-time");
                System.IO.File.AppendAllText("result.txt", caption + " => " + e.Text + " second(s).\r\n");
            }
            catch (NoSuchElementException)
            {
                System.IO.File.AppendAllText("result.txt",caption + " => Not found\r\n");
            }
        }
        [TestMethod]
        public void TestBookSearch()
        {
            //First step is browse to website and go to search page.
            System.IO.File.WriteAllText("result.txt","");
            driver = new InternetExplorerDriver();
            driver.Manage().Window.Maximize();
            driver.Navigate().GoToUrl("paratabplus.cloudapp.net");
            driver.FindElementByClassName("menulist").FindElement(By.CssSelector("li a[href=\"/BookSearch/\"]")).Click();
            Assert.AreEqual("Search", driver.Title);
            IWebElement form;
            //-----------------------------------
            /* Next step is find book from define keywords then submit
             * measure time from result that display in grey character on find result(if search result is found.)
             * record it and use these measure data to summarize.
             */
            driver.FindElementByName("Keyword").Clear();
            driver.FindElementByName("Keyword").SendKeys("COM-FL2-2812");
            driver.FindElementByTagName("select").Click();
            driver.Keyboard.SendKeys(Keys.Down);
            driver.Keyboard.SendKeys(Keys.Enter);
            form = driver.FindElementByCssSelector("form[action=\"/BookSearch/Basic\"]");
            form.Submit();
            AssertSearchResult("Call no:COM-FL2-2812");

            driver.FindElementByName("Keyword").Clear();
            driver.FindElementByName("Keyword").SendKeys("Computer");
            driver.FindElementByTagName("select").Click();
            driver.Keyboard.SendKeys(Keys.Down);
            driver.Keyboard.SendKeys(Keys.Enter);
            form = driver.FindElementByCssSelector("form[action=\"/BookSearch/Basic\"]");
            form.Submit();
            AssertSearchResult("Bookname:Computer");

            driver.FindElementByName("Keyword").Clear();
            driver.FindElementByName("Keyword").SendKeys("James");
            driver.FindElementByTagName("select").Click();
            driver.Keyboard.SendKeys(Keys.Down);
            driver.Keyboard.SendKeys(Keys.Enter);
            form = driver.FindElementByCssSelector("form[action=\"/BookSearch/Basic\"]");
            form.Submit();
            AssertSearchResult("Author:James");

            driver.FindElementByName("Keyword").Clear();
            driver.FindElementByName("Keyword").SendKeys("shogakukan");
            driver.FindElementByTagName("select").Click();
            driver.Keyboard.SendKeys(Keys.Down);
            driver.Keyboard.SendKeys(Keys.Enter);
            form = driver.FindElementByCssSelector("form[action=\"/BookSearch/Basic\"]");
            form.Submit();
            AssertSearchResult("Publisher:shogakukan");

            driver.FindElementByName("Keyword").Clear();
            driver.FindElementByName("Keyword").SendKeys("1995");
            driver.FindElementByTagName("select").Click();
            driver.Keyboard.SendKeys(Keys.Down);
            driver.Keyboard.SendKeys(Keys.Enter);
            form = driver.FindElementByCssSelector("form[action=\"/BookSearch/Basic\"]");
            form.Submit();
            AssertSearchResult("Year:1995");


            //-----------------------------------
            driver.FindElementByCssSelector("input[type=\"text\"][name=\"CallNumber\"]").SendKeys("NOV");
            driver.FindElementByCssSelector("form input[placeholder=\"Book name\"]").SendKeys("Sword Art Online Aincrad");
            driver.FindElementByCssSelector("form input[placeholder=\"Author\"]").SendKeys("Reki Kawahara");
            driver.FindElementByCssSelector("form input[placeholder=\"Publisher\"]").SendKeys("Zenshu");
            driver.FindElementByCssSelector("form[action=\"/BookSearch/Advance\"] input[type=\"submit\"]").Click();
            AssertSearchResult("\r\nCall no:NOV\r\nBookname:Sword Art Online Aincrad\r\nAuthor:Reki Kawahara\r\n" +
                "Publisher:Zenshu\r\nYear:null");

            driver.FindElementByCssSelector("input[type=\"text\"][name=\"CallNumber\"]").Clear();
            driver.FindElementByCssSelector("input[type=\"text\"][name=\"CallNumber\"]").SendKeys("PE-FL1-0005");
            driver.FindElementByCssSelector("form input[placeholder=\"Book name\"]").Clear();
            driver.FindElementByCssSelector("form input[placeholder=\"Book name\"]").SendKeys("Football training");
            driver.FindElementByCssSelector("form input[placeholder=\"Author\"]").Clear();
            driver.FindElementByCssSelector("form input[placeholder=\"Author\"]").SendKeys("Graham Taylor");
            driver.FindElementByCssSelector("form input[placeholder=\"Publisher\"]").Clear();
            driver.FindElementByCssSelector("form input[placeholder=\"Publisher\"]").SendKeys("Leopard Books");
            driver.FindElementByCssSelector("form input[placeholder=\"Year\"]").Clear();
            driver.FindElementByCssSelector("form input[placeholder=\"Year\"]").SendKeys("1995");
            driver.FindElementByCssSelector("form[action=\"/BookSearch/Advance\"] input[type=\"submit\"]").Click();
            AssertSearchResult("\r\nCall no:PE-FL1-0005\r\nBookname:Football training\r\nAuthor:Graham Taylor\r\n" +
                "Publisher:Leopard Books\r\nYear:1995");

            driver.FindElementByCssSelector("input[type=\"text\"][name=\"CallNumber\"]").Clear();
            driver.FindElementByCssSelector("input[type=\"text\"][name=\"CallNumber\"]").SendKeys("MAT");
            driver.FindElementByCssSelector("form input[placeholder=\"Book name\"]").Clear();
            driver.FindElementByCssSelector("form input[placeholder=\"Book name\"]").SendKeys("Theory of computation");
            driver.FindElementByCssSelector("form input[placeholder=\"Author\"]").Clear();
            driver.FindElementByCssSelector("form input[placeholder=\"Author\"]").SendKeys("รศ.ดร.เกียรติกูล เจียรนัยธนะกิจ");
            driver.FindElementByCssSelector("form input[placeholder=\"Publisher\"]").Clear();
            driver.FindElementByCssSelector("form input[placeholder=\"Year\"]").Clear();
            driver.FindElementByCssSelector("form input[placeholder=\"Year\"]").SendKeys("2009");
            driver.FindElementByCssSelector("form[action=\"/BookSearch/Advance\"] input[type=\"submit\"]").Click();
            AssertSearchResult("\r\nCall no:MAT\r\nBookname:Theory of computation\r\nAuthor:รศ.ดร.เกียรติกูล เจียรนัยธนะกิจ\r\n" +
                "Publisher:null\r\nYear:2009");

            driver.FindElementByCssSelector("input[type=\"text\"][name=\"CallNumber\"]").Clear();
            driver.FindElementByCssSelector("input[type=\"text\"][name=\"CallNumber\"]").SendKeys("NOV");
            driver.FindElementByCssSelector("form input[placeholder=\"Book name\"]").Clear();
            driver.FindElementByCssSelector("form input[placeholder=\"Book name\"]").SendKeys("นิทาน");
            driver.FindElementByCssSelector("form input[placeholder=\"Author\"]").Clear();
            driver.FindElementByCssSelector("form input[placeholder=\"Publisher\"]").Clear();
            driver.FindElementByCssSelector("form input[placeholder=\"Year\"]").Clear();
            driver.FindElementByCssSelector("form[action=\"/BookSearch/Advance\"] input[type=\"submit\"]").Click();
            AssertSearchResult("\r\nCall no:NOV\r\nBookname:นิทาน\r\nAuthor:null\r\n" +
                "Publisher:null\r\nYear:null");

        }

        //Close browser to reallocate memory.
        [ClassCleanup]
        public static void clean()
        {
            driver.Close();
        }
    }
}

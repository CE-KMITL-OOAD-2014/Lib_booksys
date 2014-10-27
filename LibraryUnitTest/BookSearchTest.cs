using System;
using System.Linq;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ParatabLib;
using ParatabLib.Controllers;
using System.Web.Mvc;
using System.Web;
using System.Web.Routing;
using System.IO;
using ParatabLib.DataAccess;
using System.Data.Entity;
using ParatabLib.Models;
using Moq;
using LibraryTester.MockClass;
using ParatabLib.ViewModels;

namespace LibraryTester
{
    [TestClass]
    public class BookSearchTest
    {
        private Mock<LibraryRepository> libRepo;
        private BookSearchController controller;

        public void InitialController(string username, LibraryRepository libRepo)
        {
            controller = new BookSearchController(libRepo);
            controller.ControllerContext = new ControllerContext()
            {
                Controller = controller,
                RequestContext = new RequestContext(new MockHttpContext(username), new RouteData())
            };
            controller.Session["LoginUser"] = username;
        }

        [TestInitialize]
        public void InitialRepository()
        {
            Mock<LibraryContext> context = new Mock<LibraryContext>();

            Mock<DbSet<Book>> booklist = new Mock<DbSet<Book>>();

            IQueryable<Book> blist = new List<Book> { 
            new Book{BookID = 1,
                CallNumber = "COM-FL1-9870",
                BookName = "The magic of java",
                Author = "Rob winson",
                Publisher = "US company",
                Year = 2013,
                BookStatus = Status.Borrowed,
            },
            new Book{BookID = 2,
                CallNumber = "SOC-FL3-0007",
                BookName = "Historical of bhuddha",
                Author = "Thawul Lertpatthanakul",
                Publisher = "TH company",
                Year = 2007,
                BookStatus = Status.Reserved,
            },
            new Book{BookID = 3,
                CallNumber = "ENG-FL1-4772",
                BookName = "Oxford dictionary master",
                Author = "Dr.Oxford",
                Publisher = "UK company",
                Year = 2001,
                BookStatus = Status.Borrowed,
            },
            new Book{BookID = 4,
                CallNumber = "THA-FL4-2823",
                BookName = "Thai usage for communication",
                Author = "Chanchira Chancharoensuk",
                Publisher = "KMITL publisher",
                Year = 2012,
                BookStatus = Status.Available,
            },
            new Book{BookID = 5,
                CallNumber = "SOC-FL1-0017",
                BookName = "Fundamental law for daily life",
                Author = "Prasitsak Sirijarupat",
                Publisher = "KMITL publisher",
                Year = 2013,
                BookStatus = Status.Lost,
            },
            new Book{BookID = 6,
                CallNumber = "ETC-FL1-0174",
                BookName = "Harry Potter and the Deathly Hallows",
                Author = "J.K. Rowling",
                Publisher = " 	Bloomsbury Publishing",
                Year = 2006,
                BookStatus = Status.Borrowed
            },
            new Book{BookID = 7,
                CallNumber = "ETC-FL1-0175",
                BookName = " Harry Potter and the Philosopher's Stone",
                Author = "J.K. Rowling",
                Publisher = "Bloomsbury Publishing",
                Year = 1997,
                BookStatus = Status.Available
            },
            new Book{BookID = 8,
                CallNumber = "ETC-FL1-0176",
                BookName = "Harry Potter and the Chamber of Secrets",
                Author = "J.K. Rowling",
                Publisher = "Bloomsbury Publishing",
                Year = 1998,
                BookStatus = Status.Available
            },
            new Book{BookID = 9,
                CallNumber = "ETC-FL1-0177",
                BookName = "Harry Potter and the Prisoner of Azkaban",
                Author = "J.K. Rowling",
                Publisher = "Bloomsbury Publishing",
                Year = 1999,
                BookStatus = Status.Available
            },
            new Book{BookID = 10,
                CallNumber = "ETC-FL1-0178",
                BookName = "Harry Potter and the Goblet of Fire",
                Author = "J.K. Rowling",
                Publisher = "Bloomsbury Publishing",
                Year = 2000,
                BookStatus = Status.Available
            },
            new Book{BookID = 11,
                CallNumber = "ETC-FL1-0179",
                BookName = "Harry Potter and the Order of the Phoenix",
                Author = "J.K. Rowling",
                Publisher = "Bloomsbury Publishing",
                Year = 2002,
                BookStatus = Status.Available
            },
            new Book{BookID = 12,
                CallNumber = "ETC-FL1-0180",
                BookName = "Harry Potter and the Half-Blood Prince",
                Author = "J.K. Rowling",
                Publisher = "Bloomsbury Publishing",
                Year = 2005,
                BookStatus = Status.Available
            },
            new Book{BookID = 13,
                CallNumber = "MOV-FL1-0200",
                BookName = "Doraemon: Nobita's Dinosaur",
                Author = "Fujiko F. Fujio",
                Publisher = "Shogakukan",
                Year = 1980,
                BookStatus = Status.Available
            },
            new Book{BookID = 14,
                CallNumber = "MOV-FL1-0201",
                BookName = "Doraemon: The Records of Nobita, Spaceblazer",
                Author = "Fujiko F. Fujio",
                Publisher = "Shogakukan",
                Year = 1981,
                BookStatus = Status.Available
            },
            new Book{BookID = 15,
                CallNumber = "MOV-FL1-0202",
                BookName = "Doraemon: Nobita and the Haunts of Evil",
                Author = "Fujiko F. Fujio",
                Publisher = "Shogakukan",
                Year = 1982,
                BookStatus = Status.Available
            },
            new Book{BookID = 16,
                CallNumber = "MOV-FL1-0203",
                BookName = "Doraemon: Nobita and the Castle of the Undersea Devil",
                Author = "Fujiko F. Fujio",
                Publisher = "Shogakukan",
                Year = 1983,
                BookStatus = Status.Available,
            },
            new Book{BookID = 17,
                CallNumber = "MOV-FL1-0204",
                BookName = "Doraemon: Nobita's Great Adventure into the Underworld",
                Author = "Fujiko F. Fujio",
                Publisher = "Shogakukan",
                Year = 1984,
                BookStatus = Status.Available,
            },
            new Book{BookID = 18,
                CallNumber = "MOV-FL1-0205",
                BookName = "Doraemon: Nobita's Little Star Wars",
                Author = "Fujiko F. Fujio",
                Publisher = "Shogakukan",
                Year = 1985,
                BookStatus = Status.Available,
            },
            new Book{BookID = 19,
                CallNumber = "MOV-FL1-0206",
                BookName = "Doraemon: Nobita and the Steel Troops",
                Author = "Fujiko F. Fujio",
                Publisher = "Shogakukan",
                Year = 1986,
                BookStatus = Status.Available,
            },
            new Book{BookID = 20,
                CallNumber = "MOV-FL1-0207",
                BookName = "Doraemon: Nobita and the Knights on Dinosaurs",
                Author = "Fujiko F. Fujio",
                Publisher = "Shogakukan",
                Year = 1987,
                BookStatus = Status.Available,
            },
            new Book{BookID = 21,
                CallNumber = "MOV-FL1-0208",
                BookName = "Doraemon: The Record of Nobita's Parallel Visit to the West",
                Author = "Fujiko F. Fujio",
                Publisher = "Shogakukan",
                Year = 1988,
                BookStatus = Status.Available,
            },
            new Book{BookID = 22,
                CallNumber = "MAT-FL1-1707",
                BookName = "Theory of Computation",
                Author = "Kietikul Jearanaitanakij",
                Publisher = "KMITL",
                Year = 2009,
                BookStatus = Status.Available,
            },
            new Book{BookID = 23,
                CallNumber = "HWA-FL1-1000",
                BookName = "Computer Architecture",
                Author = "Surin Kittitornkun",
                Publisher = "KMITL",
                Year = 2010,
                BookStatus = Status.Available,
            },
            new Book{BookID = 24,
                CallNumber = "MAT-FL1-1023",
                BookName = "Mathematics in daily life",
                Author = "Ngamcherd danpattanamongkhol",
                Publisher = "KMITL",
                Year = 2014,
                BookStatus = Status.Available,
            },
            new Book{BookID = 25,
                CallNumber = "COM-FL1-0071",
                BookName = "Computer and programming",
                Author = "Khanut Tungtisanon",
                Publisher = "KMITL",
                Year = 2014,
                BookStatus = Status.Available,
            },
            new Book{BookID = 26,
                CallNumber = "HWA-FL2-2014",
                BookName = "Digital circuit with VHDL",
                Author = "Charoen Vongchumyen",
                Publisher = "SE-ED",
                Year = 2009,
                BookStatus = Status.Available,
            },
            new Book{BookID = 27,
                CallNumber = "HWA-FL2-1003",
                BookName = "Electronic devices and circuit theory",
                Author = "Robert L. Boylestad",
                Publisher = "Pearson",
                Year = 2008,
                BookStatus = Status.Available,
            },
            new Book{BookID = 28,
                CallNumber = "MAT-FL3-1830",
                BookName = "Image Processing",
                Author = "Orachat Chitsobhuk",
                Publisher = "KMITL",
                Year = 2011,
                BookStatus = Status.Available,
            },
            new Book{BookID = 29,
                CallNumber = "PHY-FL1-8830",
                BookName = "Fundamental statistics in psychology and education",
                Author = "J P Guilford",
                Publisher = "McGraw-Hill",
                Year = 1965,
                BookStatus = Status.Available,
            },
            new Book{BookID = 30,
                CallNumber = "PHY-FL1-8831",
                BookName = "The four fundamental concepts of psycho-analysis",
                Author = "Jacques Lacan",
                Publisher = "Norton",
                Year = 1978,
                BookStatus = Status.Available,
            }
            }.AsQueryable();

            booklist.As<IQueryable<Book>>().Setup(c => c.Expression).Returns(blist.Expression);
            booklist.As<IQueryable<Book>>().Setup(c => c.Provider).Returns(blist.Provider);
            booklist.As<IQueryable<Book>>().Setup(c => c.ElementType).Returns(blist.ElementType);
            booklist.As<IQueryable<Book>>().Setup(c => c.GetEnumerator()).Returns(blist.GetEnumerator());
            booklist.Setup(c => c.Find(It.IsAny<object[]>())).Returns<object[]>(
                id => blist.Where(target => target.BookID == int.Parse(id[0].ToString())).SingleOrDefault());

            context.Setup(c => c.Books).Returns(booklist.Object);

            Mock<IGenericRepository<Book>> localbooklist = new Mock<IGenericRepository<Book>>();

            localbooklist.Setup(l => l.List()).Returns(context.Object.Books.ToList());
            localbooklist.Setup(l => l.ListWhere(It.IsAny<Func<Book, bool>>())).Returns<Func<Book, bool>>
                (condition => localbooklist.Object.List().Where(condition).ToList());
            localbooklist.Setup(l => l.Find(It.IsAny<int>())).Returns<int>(id => context.Object.Books.Find(id));

            libRepo = new Mock<LibraryRepository>(context.Object);
            libRepo.Setup(l => l.BookRepo).Returns(localbooklist.Object);
        }

        [TestMethod]
        public void TestBrowseIndexAction()
        {
            InitialController("M_ce51benz", libRepo.Object);
            ViewResult result = controller.Index() as ViewResult;
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void TestQuickSearchAction1()
        {
            InitialController("M_ce51benz", libRepo.Object);
            ViewResult result = controller.QuickSearch("Doraemon",1,10) as ViewResult;
            Assert.IsNotNull(result.Model as PageList<Book>);
            Assert.AreEqual(1, (result.Model as PageList<Book>).GetPageSize());
            Assert.AreEqual(9, (result.Model as PageList<Book>).GetList().Count);
            Assert.AreEqual(1980, (result.Model as PageList<Book>).GetList().
                Where(target => target.BookName == "Doraemon: Nobita's Dinosaur").SingleOrDefault().Year);
            Assert.AreEqual("Doraemon: Nobita's Great Adventure into the Underworld", 
                (result.Model as PageList<Book>).GetList().Where(target => target.CallNumber == "MOV-FL1-0204").SingleOrDefault().BookName);
        }

        [TestMethod]
        public void TestQuickSearchAction2()
        {
            InitialController("M_ce51benz", libRepo.Object);
            ViewResult result = controller.QuickSearch("Fundamental", 1, 10) as ViewResult;
            Assert.IsNotNull(result.Model as PageList<Book>);
            Assert.AreEqual(1, (result.Model as PageList<Book>).GetPageSize());
            Assert.AreEqual(2, (result.Model as PageList<Book>).GetList().Count);
            Assert.AreEqual("PHY-FL1-8830", (result.Model as PageList<Book>).GetList().
                Where(target => target.BookName == "Fundamental statistics in psychology and education").SingleOrDefault().CallNumber);
        }

        [TestMethod]
        public void TestQuickSearchAction3()
        {
            InitialController("M_ce51benz", libRepo.Object);
            ViewResult result = controller.QuickSearch("Faksdsaopdkasdposapdoksapod", 1, 10) as ViewResult;
            Assert.IsNull(result.Model as PageList<Book>);
            Assert.AreEqual("No book result found.", result.TempData["ErrorNoti"]);
        }

        [TestMethod]
        public void TestQuickSearchAction4()
        {
            InitialController("M_ce51benz", libRepo.Object);
            ViewResult result = controller.QuickSearch("Doraemon", 100, 100) as ViewResult;
            Assert.IsNull(result.Model as PageList<Book>);
            Assert.AreEqual("Invalid list view parameter please refresh this page to try again.", result.TempData["ErrorNoti"]);
        }

        [TestMethod]
        public void TestBasicSearchAction1()
        {
            InitialController("M_ce51benz", libRepo.Object);
            ViewResult result = controller.Basic("The magic of java","BookName", 1, 10) as ViewResult;
            Assert.IsNotNull(result.Model as PageList<Book>);
            Assert.AreEqual(1,(result.Model as PageList<Book>).GetList().Count);
            Assert.AreEqual("Rob winson", (result.Model as PageList<Book>).GetList().First().Author);
            Assert.IsNull(result.TempData["ErrorNoti"]);
            Assert.AreEqual("The magic of java",result.TempData["Keyword"]);
        }


        [TestMethod]
        public void TestBasicSearchAction2()
        {
            InitialController("M_ce51benz", libRepo.Object);
            ViewResult result = controller.Basic("The magic of javaaaa","BookName", 1, 10) as ViewResult;
            Assert.IsNull(result.Model as PageList<Book>);
            Assert.AreEqual("No book result found.", result.TempData["ErrorNoti"]);
            Assert.AreEqual("The magic of javaaaa", result.TempData["Keyword"]);
        }


        [TestMethod]
        public void TestBasicSearchAction3()
        {
            InitialController("M_ce51benz", libRepo.Object);
            ViewResult result = controller.Basic("The magic of java","BookName", 100, 100) as ViewResult;
            Assert.IsNull(result.Model as PageList<Book>);
            Assert.AreEqual("Invalid list view parameter please refresh this page to try again.", result.TempData["ErrorNoti"]);
            Assert.AreEqual("The magic of java", result.TempData["Keyword"]);
        }

        [TestMethod]
        public void TestBasicSearchAction4()
        {
            InitialController("M_ce51benz", libRepo.Object);
            ViewResult result = controller.Basic("The magic of java","wrongsearchtype", 1, 10) as ViewResult;
            Assert.IsNull(result.Model as PageList<Book>);
            Assert.AreEqual("Something was error.", result.TempData["ErrorNoti"]);
            Assert.AreEqual("The magic of java", result.TempData["Keyword"]);
        }

        [TestMethod]
        public void TestBasicSearchAction5()
        {
            InitialController("M_ce51benz", libRepo.Object);
            ViewResult result = controller.Basic("The magic of java", "", 1, 10) as ViewResult;
            Assert.IsNull(result.Model as PageList<Book>);
            Assert.AreEqual("Please select search type.", result.TempData["ErrorNoti"]);
            Assert.AreEqual("The magic of java", result.TempData["Keyword"]);
        }


        //Test with other type of search
        [TestMethod]
        public void TestBasicSearchAction6()
        {
            InitialController("M_ce51benz", libRepo.Object);
            ViewResult result = controller.Basic("The magic of java", "", 1, 10) as ViewResult;
            Assert.IsNull(result.Model as PageList<Book>);
            Assert.AreEqual("Please select search type.", result.TempData["ErrorNoti"]);
            Assert.AreEqual("The magic of java", result.TempData["Keyword"]);
        }

        //Test with advance in many case of search.
    }
}

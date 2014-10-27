﻿using System;
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
    public class MemberTransactionTest
    {
        private Mock<LibraryRepository> libRepo;
        private MemberTransactionController controller;
        public void InitialController(string username,LibraryRepository libRepo)
        {
            controller = new MemberTransactionController(libRepo);
            controller.ControllerContext = new ControllerContext()
            {
                Controller = controller,
                RequestContext = new RequestContext(new MockHttpContext(username),new RouteData())
            };
            controller.Session["LoginUser"] = username;
        }

        [TestInitialize]
        public void InitialRepository()
        {
            Mock<LibraryContext> context = new Mock<LibraryContext>();

            Mock<DbSet<Member>> memberlist = new Mock<DbSet<Member>>();
            Mock<DbSet<Book>> booklist = new Mock<DbSet<Book>>();
            Mock<DbSet<BorrowEntry>> borrowlist = new Mock<DbSet<BorrowEntry>>();
            Mock<DbSet<RequestEntry>> reqlist = new Mock<DbSet<RequestEntry>>();

            IQueryable<Member> mlist = new List<Member>{
            new Member{UserID = 1,
                UserName = "paratab",
                Name = "Surawit Sangjan"
            },
            new Member{UserID = 2,
                UserName = "ce51benz",
                Name = "Suratchanan Kraidech"
            },
            new Member{UserID = 3,
                UserName = "baybaybay",
                Name = "Worawit Saetan"
            },
            new Member{UserID = 4,
                UserName = "fonniz",
                Name = "Panita Phinyophab"
            },
            new Member{UserID = 5,
                UserName = "tomza2014",
                Name = "Purmpol Kurung"}
            }
                .AsQueryable();

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
                BookStatus = Status.Borrowed,
            }
            }.AsQueryable();

            IQueryable<BorrowEntry> brlist = new List<BorrowEntry> { 
            new BorrowEntry{
            ID = 1,
            BookID = 1,
            UserID = 2,
            BorrowDate = new DateTime(2014,10,20,0,0,0),
            DueDate = new DateTime(2014,10,27,0,0,0),
            RenewCount = 0,
            ReturnDate = null
            },
            new BorrowEntry{
            ID = 2,
            BookID = 5,
            UserID = 5,
            BorrowDate = new DateTime(2014,10,10,0,0,0),
            DueDate = new DateTime(2014,10,17,0,0,0),
            RenewCount = 2,
            ReturnDate = new DateTime(2014,10,16,0,0,0)
            },
            new BorrowEntry{
            ID = 3,
            BookID = 4,
            UserID = 1,
            BorrowDate = new DateTime(2014,9,9,0,0,0),
            DueDate = new DateTime(2014,9,16,0,0,0),
            RenewCount = 3,
            ReturnDate = new DateTime(2014,9,17,0,0,0)
            },
            new BorrowEntry{
            ID = 4,
            BookID = 3,
            UserID = 2,
            BorrowDate = new DateTime(2014,10,20,0,0,0),
            DueDate = new DateTime(2014,10,27,0,0,0),
            RenewCount = 0,
            ReturnDate = null
            },
            new BorrowEntry{
            ID = 5,
            BookID = 2,
            UserID = 3,
            BorrowDate = new DateTime(2014,11,20,0,0,0),
            DueDate = new DateTime(2014,11,27,0,0,0),
            RenewCount = 0,
            ReturnDate = new DateTime(2014,8,22,0,0,0)
            },
            new BorrowEntry{
            ID = 6,
            BookID = 6,
            UserID = 5,
            BorrowDate = new DateTime(2014,12,20,0,0,0),
            DueDate = new DateTime(2014,12,27,0,0,0),
            RenewCount = 3,
            ReturnDate = null
            }
            }.AsQueryable();


            IQueryable<RequestEntry> rqlist = new List<RequestEntry> { 
            new RequestEntry{
                BookID = 2,
                UserID = 5,
                ExpireDate = new DateTime(2014,11,30,0,0,0),
                RequestDate = new DateTime(2014,11,1,0,0,0)
            },
            new RequestEntry{
                BookID = 1,
                UserID = 4,
                ExpireDate = null,
                RequestDate = new DateTime(2014,10,24,0,0,0)
            }
            }.AsQueryable();

            memberlist.As<IQueryable<Member>>().Setup(db => db.Expression).Returns(mlist.Expression);
            memberlist.As<IQueryable<Member>>().Setup(db => db.Provider).Returns(mlist.Provider);
            memberlist.As<IQueryable<Member>>().Setup(db => db.ElementType).Returns(mlist.ElementType);
            memberlist.As<IQueryable<Member>>().Setup(db => db.GetEnumerator()).Returns(mlist.GetEnumerator());
            memberlist.Setup(db => db.Find(It.IsAny<object[]>())).Returns<object[]>(id => mlist.Where(target => target.UserID == (int.Parse(id[0].ToString()))).SingleOrDefault());


            booklist.As<IQueryable<Book>>().Setup(db => db.Expression).Returns(blist.Expression);
            booklist.As<IQueryable<Book>>().Setup(db => db.Provider).Returns(blist.Provider);
            booklist.As<IQueryable<Book>>().Setup(db => db.ElementType).Returns(blist.ElementType);
            booklist.As<IQueryable<Book>>().Setup(db => db.GetEnumerator()).Returns(blist.GetEnumerator());
            booklist.Setup(db => db.Find(It.IsAny<object[]>())).Returns<object[]>(id => blist.Where(target => target.BookID == (int.Parse(id[0].ToString()))).SingleOrDefault());


            borrowlist.As<IQueryable<BorrowEntry>>().Setup(db => db.Expression).Returns(brlist.Expression);
            borrowlist.As<IQueryable<BorrowEntry>>().Setup(db => db.Provider).Returns(brlist.Provider);
            borrowlist.As<IQueryable<BorrowEntry>>().Setup(db => db.ElementType).Returns(brlist.ElementType);
            borrowlist.As<IQueryable<BorrowEntry>>().Setup(db => db.GetEnumerator()).Returns(brlist.GetEnumerator());
            borrowlist.Setup(db => db.Find(It.IsAny<object[]>())).Returns<object[]>(id => brlist.Where(target => target.ID == (int.Parse(id[0].ToString()))).SingleOrDefault());


            reqlist.As<IQueryable<RequestEntry>>().Setup(db => db.Expression).Returns(rqlist.Expression);
            reqlist.As<IQueryable<RequestEntry>>().Setup(db => db.Provider).Returns(rqlist.Provider);
            reqlist.As<IQueryable<RequestEntry>>().Setup(db => db.ElementType).Returns(rqlist.ElementType);
            reqlist.As<IQueryable<RequestEntry>>().Setup(db => db.GetEnumerator()).Returns(rqlist.GetEnumerator());
            reqlist.Setup(db => db.Find(It.IsAny<object[]>())).Returns<object[]>(id => rqlist.Where(target => target.BookID == (int.Parse(id[0].ToString()))).SingleOrDefault());

            context.Setup(c => c.Books).Returns(booklist.Object);
            context.Setup(c => c.Members).Returns(memberlist.Object);
            context.Setup(c => c.RequestList).Returns(reqlist.Object);
            context.Setup(c => c.BorrowList).Returns(borrowlist.Object);

            Mock<IGenericRepository<Book>> localbooklist = new Mock<IGenericRepository<Book>>();
            Mock<IGenericRepository<Member>> localmemberlist = new Mock<IGenericRepository<Member>>();
            Mock<IGenericRepository<BorrowEntry>> localbrlist = new Mock<IGenericRepository<BorrowEntry>>();
            Mock<IGenericRepository<RequestEntry>> localreqlist = new Mock<IGenericRepository<RequestEntry>>();
            

            localbooklist.Setup(l => l.List()).Returns(context.Object.Books.ToList());
            localbooklist.Setup(l => l.ListWhere(It.IsAny<Func<Book, bool>>())).
            Returns<Func<Book, bool>>(condition => localbooklist.Object.List().Where(condition).ToList());
            localbooklist.Setup(l => l.Find(It.IsAny<int>())).Returns<int>(id => context.Object.Books.Find(id));
            
            localmemberlist.Setup(l => l.List()).Returns(context.Object.Members.ToList());
            localmemberlist.Setup(l => l.ListWhere(It.IsAny<Func<Member, bool>>())).
            Returns<Func<Member, bool>>(condition => localmemberlist.Object.List().Where(condition).ToList());
            localmemberlist.Setup(l => l.Find(It.IsAny<int>())).Returns<int>(id => context.Object.Members.Find(id));

            localbrlist.Setup(l => l.List()).Returns(context.Object.BorrowList.ToList());
            localbrlist.Setup(l => l.ListWhere(It.IsAny<Func<BorrowEntry, bool>>())).
            Returns<Func<BorrowEntry, bool>>(condition => localbrlist.Object.List().Where(condition).ToList());
            localbrlist.Setup(l => l.Find(It.IsAny<int>())).Returns<int>(id => context.Object.BorrowList.Find(id));

            localreqlist.Setup(l => l.List()).Returns(context.Object.RequestList.ToList());
            localreqlist.Setup(l => l.ListWhere(It.IsAny<Func<RequestEntry, bool>>())).
            Returns<Func<RequestEntry, bool>>(condition => localreqlist.Object.List().Where(condition).ToList());
            localreqlist.Setup(l => l.Find(It.IsAny<int>())).Returns<int>(id => context.Object.RequestList.Find(id));

            //Assert.AreEqual(0, localbooklist.Object.ListWhere(target => target.Author.Contains("a")).Count);
            //Assert.AreEqual(0, localbooklist.Object.ListWhere(target => target.BookID ==5).Count);

            libRepo = new Mock<LibraryRepository>(context.Object);
            libRepo.Setup(l => l.BookRepo).Returns(localbooklist.Object);
            libRepo.Setup(l => l.MemberRepo).Returns(localmemberlist.Object);
            libRepo.Setup(l => l.BorrowEntryRepo).Returns(localbrlist.Object);
            libRepo.Setup(l => l.RequestEntryRepo).Returns(localreqlist.Object);

        }

        [TestMethod]
        public void TestBrowseIndexAction1()
        {
            InitialController("M_ce51benz", libRepo.Object);

            ViewResult result = controller.Index() as ViewResult;
            Assert.AreEqual(2, (result.Model as MemberTransactionViewer).GetBorrowEntryViews().Count);
            Assert.AreEqual(0, (result.Model as MemberTransactionViewer).GetRequestEntryViews().Count);
        }

        [TestMethod]
        public void TestBrowseIndexAction2()
        {
            InitialController("M_tomza2014", libRepo.Object);

            ViewResult result = controller.Index() as ViewResult;
            Assert.AreEqual(1, (result.Model as MemberTransactionViewer).GetBorrowEntryViews().Count);
            Assert.AreEqual(1, (result.Model as MemberTransactionViewer).GetRequestEntryViews().Count);
        }

        [TestMethod]
        public void TestBrowseIndexAction3()
        {
            InitialController("M_paratab", libRepo.Object);

            ViewResult result = controller.Index() as ViewResult;
            Assert.AreEqual(0, (result.Model as MemberTransactionViewer).GetBorrowEntryViews().Count);
            Assert.AreEqual(0, (result.Model as MemberTransactionViewer).GetRequestEntryViews().Count);
        }


        [TestMethod]
        public void TestRenewAction1()
        {
            InitialController("M_ce51benz", libRepo.Object);
            ViewResult result = controller.Renew(1) as ViewResult;
            Assert.IsNotNull(result.Model as BorrowEntry);
            Assert.AreEqual(1,(result.Model as BorrowEntry).ID);
            Assert.AreEqual(1, (result.Model as BorrowEntry).BookID);
            Assert.AreEqual(2, (result.Model as BorrowEntry).UserID);
            Assert.IsNull(result.TempData["ErrorNoti"]);
        }

        [TestMethod]
        public void TestRenewAction2()
        {
            InitialController("M_ce51benz", libRepo.Object);
            RedirectToRouteResult result = controller.Renew(10) as RedirectToRouteResult;
            Assert.AreEqual("Index", result.RouteValues["action"]);
            Assert.AreEqual("Invalid renew book id.", controller.TempData["ErrorNoti"]);
        }

        [TestMethod]
        public void TestRenewAction3()
        {
            InitialController("M_paratab", libRepo.Object);
            RedirectToRouteResult result = controller.Renew(4) as RedirectToRouteResult;
            Assert.AreEqual("Index", result.RouteValues["action"]);
            Assert.AreEqual("Invalid renew operation.", controller.TempData["ErrorNoti"]);
        }

        [TestMethod]
        public void TestRenewAction4()
        {
            InitialController("M_tomza2014", libRepo.Object);
            RedirectToRouteResult result = controller.Renew(6) as RedirectToRouteResult;
            Assert.AreEqual("Index", result.RouteValues["action"]);
            Assert.AreEqual("Your renew of book ID.6 is exceed maximum!", controller.TempData["ErrorNoti"]);
        }

        [TestMethod]
        public void TestRenewPostAction1()
        {
            InitialController("M_ce51benz", libRepo.Object);
            RedirectToRouteResult result = controller.Renew(libRepo.Object.BorrowEntryRepo.ListWhere
                (target => target.ID == 1).Single()) as RedirectToRouteResult;
            Assert.AreEqual("Index", result.RouteValues["action"]);
            Assert.IsNull(controller.ControllerContext.Controller.TempData["SuccessNoti"]);
            Assert.AreEqual("This book is ON HOLD.",controller.ControllerContext.Controller.TempData["ErrorNoti"]);
        }

        [TestMethod]
        public void TestRenewPostAction2()
        {
            InitialController("M_ce51benz", libRepo.Object);
            RedirectToRouteResult result = controller.Renew(libRepo.Object.BorrowEntryRepo.ListWhere
                (target => target.ID == 4).Single()) as RedirectToRouteResult;
            Assert.AreEqual("Index", result.RouteValues["action"]);
            Assert.IsNull(controller.ControllerContext.Controller.TempData["ErrorNoti"]);
            Assert.AreEqual("Renew successful!", controller.ControllerContext.Controller.TempData["SuccessNoti"]);
        }

        [TestMethod]
        public void TestRequestAction()
        {
            InitialController("M_baybaybay", libRepo.Object);
            ViewResult result = controller.Request() as ViewResult;
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void TestRequestPostAction1()
        {
            InitialController("M_baybaybay", libRepo.Object);
            RequestEntry postReq = new RequestEntry { BookID = 3};
            RedirectToRouteResult result = controller.Request(postReq) as RedirectToRouteResult;
            Assert.AreEqual("Index", result.RouteValues["action"]);
            Assert.AreEqual("Request book successfully.", controller.ControllerContext.Controller.TempData["SuccessNoti"]);
            Assert.IsNull(controller.ControllerContext.Controller.TempData["ErrorNoti"]);
        }

        [TestMethod]
        public void TestRequestPostAction2()
        {
            InitialController("M_baybaybay", libRepo.Object);
            RequestEntry postReq = new RequestEntry { BookID = 100 };
            ViewResult result = controller.Request(postReq) as ViewResult;
            Assert.AreEqual("No book with prefer ID exists.", result.TempData["ErrorNoti"]);
            Assert.IsNull(controller.ControllerContext.Controller.TempData["SuccessNoti"]);
        }

        [TestMethod]
        public void TestRequestPostAction3()
        {
            InitialController("M_ce51benz", libRepo.Object);
            RequestEntry postReq = new RequestEntry { BookID = 3 };
            ViewResult result = controller.Request(postReq) as ViewResult;
            Assert.AreEqual("Can't request your current borrowed book.", result.TempData["ErrorNoti"]);
            Assert.IsNull(controller.ControllerContext.Controller.TempData["SuccessNoti"]);
        }

        [TestMethod]
        public void TestRequestPostAction4()
        {
            InitialController("M_fonniz", libRepo.Object);
            RequestEntry postReq = new RequestEntry { BookID = 2 };
            ViewResult result = controller.Request(postReq) as ViewResult;
            Assert.AreEqual("This book is already requested.", result.TempData["ErrorNoti"]);
            Assert.IsNull(controller.ControllerContext.Controller.TempData["SuccessNoti"]);
        }

        [TestMethod]
        public void TestRequestPostAction5()
        {
            InitialController("M_fonniz", libRepo.Object);
            RequestEntry postReq = new RequestEntry { BookID = 5 };
            ViewResult result = controller.Request(postReq) as ViewResult;
            Assert.AreEqual("Can't request this book due to it is Lost.", result.TempData["ErrorNoti"]);
            Assert.IsNull(controller.ControllerContext.Controller.TempData["SuccessNoti"]);
        }

        [TestMethod]
        public void TestRequestPostAction6()
        {
            InitialController("M_paratab", libRepo.Object);
            RequestEntry postReq = new RequestEntry { BookID = 4 };
            ViewResult result = controller.Request(postReq) as ViewResult;
            Assert.AreEqual("Can't request this book due to it is Available.", result.TempData["ErrorNoti"]);
            Assert.IsNull(controller.ControllerContext.Controller.TempData["SuccessNoti"]);
        }

        [TestMethod]
        public void TestCancelRequestAction1()
        {
            InitialController("M_paratab", libRepo.Object);
            RedirectToRouteResult result = controller.CancelRequest(100) as RedirectToRouteResult;
            Assert.AreEqual("No cancel request with prefered id exists.", 
                controller.ControllerContext.Controller.TempData["ErrorNoti"]);
            Assert.IsNull(controller.ControllerContext.Controller.TempData["SuccessNoti"]);
        }

        [TestMethod]
        public void TestCancelRequestAction2()
        {
            InitialController("M_paratab", libRepo.Object);
            RedirectToRouteResult result = controller.CancelRequest(2) as RedirectToRouteResult;
            Assert.AreEqual("Can't cancel other member's book request.",
                controller.ControllerContext.Controller.TempData["ErrorNoti"]);
            Assert.IsNull(controller.ControllerContext.Controller.TempData["SuccessNoti"]);
        }

        [TestMethod]
        public void TestCancelRequestAction3()
        {
            InitialController("M_tomza2014", libRepo.Object);
            ViewResult result = controller.CancelRequest(2) as ViewResult;
            Assert.IsNull(controller.ControllerContext.Controller.TempData["ErrorNoti"]);
            Assert.IsNull(controller.ControllerContext.Controller.TempData["SuccessNoti"]);
            Assert.AreEqual(2, (result.Model as RequestEntry).BookID);
        }

        [TestMethod]
        public void TestCancelRequestPostAction1()
        {
            InitialController("M_tomza2014", libRepo.Object);
            RedirectToRouteResult result = controller.CancelRequest(new RequestEntry { BookID = 2 }) as RedirectToRouteResult;
            Assert.AreEqual("Index",result.RouteValues["action"]);
            Assert.IsNull(controller.ControllerContext.Controller.TempData["ErrorNoti"]);
            Assert.AreEqual("Cancel request successfully.", controller.ControllerContext.Controller.TempData["SuccessNoti"]);
        }

        [TestMethod]
        public void TestCancelRequestPostAction2()
        {
            InitialController("M_tomza2014", libRepo.Object);
            RedirectToRouteResult result = controller.CancelRequest(new RequestEntry { BookID = 222 }) as RedirectToRouteResult;
            Assert.AreEqual("Index", result.RouteValues["action"]);
            Assert.IsNull(controller.ControllerContext.Controller.TempData["SuccessNoti"]);
            Assert.AreEqual("Something went wrong,please try again.", controller.ControllerContext.Controller.TempData["ErrorNoti"]);
        }


        [TestMethod]
        public void TestBrowseBorrowHistoryAction1()
        {
            InitialController("M_tomza2014", libRepo.Object);
            ViewResult result = controller.BorrowHistory(1, 10) as ViewResult;
            Assert.IsNotNull(result.Model as PageList<BorrowEntry>);
            Assert.IsNull(controller.ControllerContext.Controller.TempData["ErrorNoti"]);
        }

        [TestMethod]
        public void TestBrowseBorrowHistoryAction2()
        {
            InitialController("M_fonniz", libRepo.Object);
            ViewResult result = controller.BorrowHistory(1, 10) as ViewResult;
            Assert.IsNull(result.Model as PageList<BorrowEntry>);
            Assert.AreEqual("No borrow history to show.Please do transaction to see your history.",
                controller.ControllerContext.Controller.TempData["ErrorNoti"]);
        }
    }
}
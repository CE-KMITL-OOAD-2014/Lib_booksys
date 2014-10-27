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

            booklist.As<IQueryable<Book>>().Setup(db => db.Expression).Returns(blist.Expression);
            booklist.As<IQueryable<Book>>().Setup(db => db.Provider).Returns(blist.Provider);
            booklist.As<IQueryable<Book>>().Setup(db => db.ElementType).Returns(blist.ElementType);
            booklist.As<IQueryable<Book>>().Setup(db => db.GetEnumerator()).Returns(blist.GetEnumerator());


            borrowlist.As<IQueryable<BorrowEntry>>().Setup(db => db.Expression).Returns(brlist.Expression);
            borrowlist.As<IQueryable<BorrowEntry>>().Setup(db => db.Provider).Returns(brlist.Provider);
            borrowlist.As<IQueryable<BorrowEntry>>().Setup(db => db.ElementType).Returns(brlist.ElementType);
            borrowlist.As<IQueryable<BorrowEntry>>().Setup(db => db.GetEnumerator()).Returns(brlist.GetEnumerator());

            reqlist.As<IQueryable<RequestEntry>>().Setup(db => db.Expression).Returns(rqlist.Expression);
            reqlist.As<IQueryable<RequestEntry>>().Setup(db => db.Provider).Returns(rqlist.Provider);
            reqlist.As<IQueryable<RequestEntry>>().Setup(db => db.ElementType).Returns(rqlist.ElementType);
            reqlist.As<IQueryable<RequestEntry>>().Setup(db => db.GetEnumerator()).Returns(rqlist.GetEnumerator());


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

            localmemberlist.Setup(l => l.List()).Returns(context.Object.Members.ToList());
            localmemberlist.Setup(l => l.ListWhere(It.IsAny<Func<Member, bool>>())).
            Returns<Func<Member, bool>>(condition => localmemberlist.Object.List().Where(condition).ToList());

            localbrlist.Setup(l => l.List()).Returns(context.Object.BorrowList.ToList());
            localbrlist.Setup(l => l.ListWhere(It.IsAny<Func<BorrowEntry, bool>>())).
            Returns<Func<BorrowEntry, bool>>(condition => localbrlist.Object.List().Where(condition).ToList());

            localreqlist.Setup(l => l.List()).Returns(context.Object.RequestList.ToList());
            localreqlist.Setup(l => l.ListWhere(It.IsAny<Func<RequestEntry, bool>>())).
            Returns<Func<RequestEntry, bool>>(condition => localreqlist.Object.List().Where(condition).ToList());

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

            ViewResult result1 = controller.Index() as ViewResult;
            Assert.AreEqual(0, (result1.Model as MemberTransactionViewer).GetBorrowEntryViews().Count);
            Assert.AreEqual(1, (result1.Model as MemberTransactionViewer).GetRequestEntryViews().Count);
        }


        [TestMethod]
        public void Borrow()
        {
        }


        [TestMethod]
        public void Return()
        {
        }
    }
}

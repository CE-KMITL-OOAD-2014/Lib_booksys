using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TestLibrary;
using TestLibrary.Controllers;
using System.Web.Mvc;
using System.Web.Security;
using System.Security.Principal;
using System.Web.SessionState;
using System.Web;
using System.Web.Routing;
using System.IO;
namespace LibraryTester
{
    [TestClass]
    public class HomeControllerTest
    {
        public HomeController Initialized()
        {
            HomeController controller = new HomeController();
            controller.ControllerContext = new ControllerContext()
            {
                Controller = controller,
                RequestContext = new System.Web.Routing.RequestContext(new MockHttpContext(), new RouteData())
            };
            return controller;
        }
        [TestMethod]
        public void ChangeLog()
        {
            HomeController controller = Initialized();

            ViewResult result = controller.ChangeLog() as ViewResult;
            Assert.IsNotNull(result);
        }
    }


    //MockHttpContext use for unit test
    //By implement Identity of user and MockHttpSession as Session
    //Implement to prevent NotImplementException
    public class MockHttpContext : HttpContextBase
    {

        private readonly IPrincipal _user = new GenericPrincipal(new GenericIdentity("surawit"), null);
        private readonly HttpSessionStateBase _session = new MockHttpSession();

        public override HttpSessionStateBase Session
        {
            get
            {
               return _session;
            }
        }

        public override IPrincipal User
        {
            get
            {
                return _user;
            }
            set
            {
                base.User = value;
            }
        }
    }

    public class MockHttpSession : HttpSessionStateBase
    {
        Dictionary<string, object> Storage = new Dictionary<string, object>();
        public override object this[string name]
        {
            get
            {
                return Storage[name];
            }
            set
            {
                Storage[name] = value;
            }
        }
    }
}

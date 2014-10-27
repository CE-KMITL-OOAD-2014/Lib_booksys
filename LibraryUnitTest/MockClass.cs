using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Security.Principal;
namespace LibraryTester.MockClass
{
    //MockHttpContext use for unit test
    //By implement Identity of user and MockHttpSession as Session
    //Implement to prevent NotImplementException
    public class MockHttpContext : HttpContextBase
    {

        private readonly IPrincipal _user;
        private readonly HttpSessionStateBase _session = new MockHttpSession();

        public MockHttpContext(string Username)
        {
            _user = new GenericPrincipal(new GenericIdentity(Username), null);
        }
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

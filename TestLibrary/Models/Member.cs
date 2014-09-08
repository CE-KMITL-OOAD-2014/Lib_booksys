using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TestLibrary.Models
{
    public class Member:Person
    {
        public override string Identify()
        {
            return ("Member");
        }
    }
}
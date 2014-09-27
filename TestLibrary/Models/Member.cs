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
            return ("Member "+this.UserName);
        }

        private ICollection<BorrowEntry> _BorrowEntries;
        private ICollection<RequestEntry> _RequestEntries;
        public virtual ICollection<BorrowEntry> BorrowEntries { get{return _BorrowEntries;} set{ _BorrowEntries = value; } }
        public virtual ICollection<RequestEntry> RequestEntries { get { return _RequestEntries; } set { _RequestEntries = value; } }
    }
}
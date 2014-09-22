using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace TestLibrary.Models
{
    public class RequestEntry
    {

        private int _BookID;
        [Key]
        [ForeignKey("RequestBook")]
        public int BookID { get { return _BookID; } set { _BookID = value; } }

        private int _UserID;
        [ForeignKey("RequestUser")]
        public int UserID { get { return _UserID; } set { _UserID = value; } }

        private DateTime _RequestDate;
        [Column(TypeName="date")]
        public DateTime RequestDate { get { return _RequestDate; } set { _RequestDate = value; } }

        private DateTime? _ExpireDate;
        [Column(TypeName="date")]
        public DateTime? ExpireDate { get { return _ExpireDate; } set { _ExpireDate = value; } }

        private Book _RequestBook;
        public virtual Book RequestBook { get { return _RequestBook; } set { _RequestBook = value; } }

        private Member _RequestUser;
        public virtual Member RequestUser { get { return _RequestUser; } set { _RequestUser = value; } }
    }
}
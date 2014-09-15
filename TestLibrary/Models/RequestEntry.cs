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
        [Key]
        public int ID { get; set; }

        [ForeignKey("RequestUser")]
        public int UserID { get; set; }
        [ForeignKey("RequestBook")]
        public int BookID { get; set; }

        public DateTime RequestDate { get; set; }
        public DateTime? ExpireDate { get; set; }

        public virtual Book RequestBook { get; set; }
        public virtual Member RequestUser {get; set; }
    }
}
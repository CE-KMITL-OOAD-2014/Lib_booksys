using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace TestLibrary.Models
{
    public class Admin:Person
    {

        public override string Identify()
        {
            return "Administrator";
        }
    }
}
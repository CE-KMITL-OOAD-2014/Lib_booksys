using System;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace ParatabLib.Models
{
    //This class is representation of Librarian which inherit from Person class.
    public class Librarian:Person
    {

        public override string Identify()
        {
            return "Librarian "+this.UserName;
        }
    }
}
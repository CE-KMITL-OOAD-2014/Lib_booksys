using System;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace ParatabLib.Models
{
    //This class is abstract class of Person
    public abstract class Person
    {
        private string _UserName;
        [MinLength(4,ErrorMessage="Username length must more than 4 characters.")]
        [Required(ErrorMessage = "Username is required.")]
        public string UserName { get { return _UserName; } set { _UserName = value; } }

        private string _Name;
        [Required(ErrorMessage = "Name is required.")]
        public string Name { get { return _Name; } set { _Name = value; } }


        private string _Password;
        
        [MinLength(8,ErrorMessage="For security,password length must more than 8 characters.")]
        [Required(ErrorMessage="Password is required.")]
        [DataType(DataType.Password)]
        public string Password { get { return _Password; } set { _Password = value; } }

        private string _Email;
        
        [EmailAddress(ErrorMessage="The e-mail format is not correct.")]
        [Required(ErrorMessage = "E-mail is required.")]
        public string Email { get { return _Email; } set { _Email = value; } }

        private int _UserID;
        [Key]
        public int UserID { get { return _UserID; } set { _UserID = value; } }
        
        //In general Identify method must return type of that object.
        public abstract string Identify();
    }
}
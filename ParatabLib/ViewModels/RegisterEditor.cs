using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ParatabLib.Models;
using System.ComponentModel.DataAnnotations;
namespace ParatabLib.ViewModels
{
    public class RegisterEditor
    {
        private string _UserName;
        [MinLength(4, ErrorMessage = "Username length must more than 4 characters.")]
        [Required(ErrorMessage = "Username is required.")]
        public string UserName { get { return _UserName; } set { _UserName = value; } }

        private string _Name;
        [Required(ErrorMessage = "Name is required.")]
        public string Name { get { return _Name; } set { _Name = value; } }


        private string _Password;

        [MinLength(8, ErrorMessage = "For security,password length must more than 8 characters.")]
        [Required(ErrorMessage = "Password is required.")]
        [DataType(DataType.Password)]
        [Compare("ConfirmPassword", ErrorMessage = "Not match")]
        public string Password { get { return _Password; } set { _Password = value; } }

        private string _Email;

        [EmailAddress(ErrorMessage = "The e-mail format is not correct.")]
        [Required(ErrorMessage = "E-mail is required.")]
        public string Email { get { return _Email; } set { _Email = value; } }

        private string _ConfirmPassword;
        [Required(ErrorMessage = "Confirm pwd is required.")]
        public string ConfirmPassword { get { return _ConfirmPassword; } set { _ConfirmPassword = value; } }

    }
}
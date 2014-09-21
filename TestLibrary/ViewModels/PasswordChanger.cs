using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
namespace TestLibrary.ViewModels
{
    public class PasswordChanger
    {
        [Required]
        public string oldPassword { get; set; }
        [Required]
        public string newPassword { get; set; }
        [Required]
        public string confirmPassword { get; set; }
        public bool isEqualPassword()
        {
            return newPassword == confirmPassword;
        }
    }
}
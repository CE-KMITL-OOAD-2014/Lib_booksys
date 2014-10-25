using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
namespace ParatabLib.ViewModels
{
    public class PasswordChanger
    {
        [Required(ErrorMessage="old password is required.")]
        [MinLength(8, ErrorMessage = "For security,password length must more than 8 characters.")]
        public string oldPassword { get; set; }

        [Compare("confirmPassword",ErrorMessage="Not match")]
        [Required(ErrorMessage = "New password is required.")]
        [MinLength(8, ErrorMessage = "For security,password length must more than 8 characters.")]
        public string newPassword { get; set; }

        [Required(ErrorMessage = "confirm pwd is required.")]
        [MinLength(8, ErrorMessage = "For security,password length must more than 8 characters.")]
        public string confirmPassword { get; set; }

        public bool isEqualPassword()
        {
            return newPassword == confirmPassword;
        }
    }
}
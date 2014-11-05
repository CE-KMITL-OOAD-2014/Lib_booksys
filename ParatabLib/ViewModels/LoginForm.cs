using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
namespace ParatabLib.ViewModels
{
    //This class use to set and receive login data in authenticate module
    public class LoginForm
    {
        [Required(ErrorMessage="Username field is required")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "Password field is required")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        public bool Remember { get; set; }
    }
}
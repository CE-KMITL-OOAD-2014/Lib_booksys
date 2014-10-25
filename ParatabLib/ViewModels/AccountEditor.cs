using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
namespace ParatabLib.ViewModels
{
    public class AccountEditor
    {
            
        [Required(ErrorMessage="Name field is required.")]
        public string Name { get; set; }

        [EmailAddress(ErrorMessage = "The e-mail format is not correct.")]
        [Required]
        public string Email { get; set; }
    }
}
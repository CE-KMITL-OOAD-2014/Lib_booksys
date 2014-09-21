using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
namespace TestLibrary.ViewModels
{
    public class AccountEditor
    {
            
        [Required]
        public string Name { get; set; }
        [Required]
        public string Email { get; set; }
    }
}
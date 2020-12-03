using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace TachyHealth.ViewModels
{
    public class RegisterViewModel
    {
        [Required]
        [Display(Name ="Full Name")]
        public string FullName { get; set; }
        [Required]
        [EmailAddress]
        [Remote(action: "IsEmailInUse", controller: "Account")]
        public string Email { get; set; }
        [Required]
        [Display(Name ="Phone Number")]
        [Phone]
        public string PhoneNumber { get; set; }
        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }
        [DataType(DataType.Password)]
        [Display(Name ="Confirm Passowrd")]
        [Compare("Password",ErrorMessage ="Passowrd and confirmation password do not match")]
        public string ConfirmPassword { get; set; }
    }
}

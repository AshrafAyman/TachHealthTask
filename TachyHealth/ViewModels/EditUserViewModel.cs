using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace TachyHealth.ViewModels
{
    public class EditUserViewModel
    {

        public string Id { get; set; }

        [Required]
        [Display(Name ="Full Name")]
        public string FullName { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [Display(Name ="Phone Number")]
        public string PhoneNumber { get; set; }
        
    }
}

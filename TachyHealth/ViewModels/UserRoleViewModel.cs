using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace TachyHealth.ViewModels
{
    public class UserRoleViewModel
    {
        public string RoleId { get; set; }
        public string UserId { get; set; }
        public string RoleName { get; set; }
        [Required(ErrorMessage ="You masut select at least one role")]
        public bool IsSelected { get; set; }
    }
}

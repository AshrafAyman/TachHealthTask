using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TachyHealth.Data;

namespace TachyHealth.ViewModels
{
    public class UsersViewModel
    {
        public string UserId { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public IEnumerable<string> Roles { get; set; }
    }
}

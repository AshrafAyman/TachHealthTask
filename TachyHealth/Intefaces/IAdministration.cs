using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TachyHealth.Data;
using TachyHealth.ViewModels;

namespace TachyHealth.Intefaces
{
    public interface IAdministration
    {
        Task<UserManagerResponse> CreateRole(CreateRoleViewModel model);
        CreateRoleViewModel ListRoles();
        Task<List< UsersViewModel>> ListUsers();
        Task<List<string>> GetUserRoles(ApplicationUser user);
        Task<List<UserRoleViewModel>> ManageRole(string id);
        Task<UserManagerResponse> ManageRolePost(List<UserRoleViewModel>model,string id);
        Task<EditUserViewModel> EditUser(string id);
        Task<UserManagerResponse> EditUserPost(EditUserViewModel model);
        Task<UserManagerResponse> DeleteUser(string id);
        Task<UserManagerResponse> CheckPassword(string email,string password);
    }
}

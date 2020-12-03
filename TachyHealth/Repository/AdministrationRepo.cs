using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TachyHealth.Data;
using TachyHealth.Intefaces;
using TachyHealth.ViewModels;

namespace TachyHealth.Repository
{
    public class AdministrationRepo : IAdministration
    {
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        public AdministrationRepo(RoleManager<IdentityRole> roleManager,UserManager<ApplicationUser> userManager,SignInManager<ApplicationUser> signInManager )
        {
            _roleManager = roleManager;
            _userManager=userManager;
            _signInManager=signInManager;
        }

        public async Task<UserManagerResponse> CreateRole(CreateRoleViewModel model)
        {
            IdentityRole identityRole = new IdentityRole
            {
                Name = model.RoleName
            };
            
            var result = await _roleManager.CreateAsync(identityRole);
            if (result.Succeeded)
            {
                return new UserManagerResponse
                {
                    IsSucess = true
                }; 
            }
            return new UserManagerResponse
            {
                IsSucess = false,
                Errors = result.Errors.Select(e=>e.Description)
            }; 
        }

        public CreateRoleViewModel ListRoles()
        {
            var roles = _roleManager.Roles;
            return new CreateRoleViewModel
            {
                Roles = roles
            };
        }

        public async Task<List< UsersViewModel>> ListUsers()
        {
            List<UsersViewModel> usersList = new List<UsersViewModel>();

            var users = await _userManager.Users.ToListAsync();
            foreach (var user in users)
            {
                var thisViewModel = new UsersViewModel();
                thisViewModel.UserId = user.Id;
                thisViewModel.Email = user.Email;
                thisViewModel.FullName = user.FullName;
                thisViewModel.PhoneNumber = user.PhoneNumber;
                thisViewModel.Roles = await GetUserRoles(user);
                usersList.Add(thisViewModel);
            }
            return  usersList;
        }

        public async Task<List<string>> GetUserRoles(ApplicationUser user)
        {
            return new List<string> (await _userManager.GetRolesAsync(user));
        }

        public async Task<List<UserRoleViewModel>> ManageRole(string id)
        {
            var user = _userManager.Users.FirstOrDefault(e => e.Id == id);
            var model = new List<UserRoleViewModel>();
            foreach (var role in _roleManager.Roles)
            {
                var userRole = new UserRoleViewModel
                {
                    RoleId = role.Id,
                    RoleName = role.Name
                };
                if (await _userManager.IsInRoleAsync(user, role.Name))
                {
                    userRole.IsSelected = true;
                }
                else
                {
                    userRole.IsSelected = false;
                }

                model.Add(userRole);
            }
            return model;
        }

        public async Task<UserManagerResponse> ManageRolePost(List<UserRoleViewModel> model, string id)
        {
             var user = await _userManager.FindByIdAsync(id);

            if (user == null)
            {
                return new UserManagerResponse
                {
                    Message= $"User with Id = {id} cannot be found",
                    IsSucess=false
                };
            }

            var roles = await _userManager.GetRolesAsync(user);
            var result = await _userManager.RemoveFromRolesAsync(user, roles);

            if (!result.Succeeded)
            {
                return new UserManagerResponse
                {
                        IsSucess = false,
                        Errors = result.Errors.Select(e=>e.Description)
                };
            }

            result = await _userManager.AddToRolesAsync(user, model.Where(x => x.IsSelected).Select(y => y.RoleName));

            if (!result.Succeeded)
            {
                return new UserManagerResponse
                {
                        IsSucess = false,
                        Errors = result.Errors.Select(e=>e.Description)
                };
            }

            return new UserManagerResponse
                {
                        IsSucess = true
                };
        }

        public async Task<EditUserViewModel> EditUser(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            var model = new EditUserViewModel
            {
                Id = user.Id,
                Email = user.Email,
                FullName = user.FullName,
                PhoneNumber = user.PhoneNumber
            };
            return model;
        }

        public async Task<UserManagerResponse> EditUserPost(EditUserViewModel model)
        {

            var user = await _userManager.FindByIdAsync(model.Id);

            if (user != null)
            {
                user.Email = model.Email;
                user.FullName = model.FullName;
                user.PhoneNumber = model.PhoneNumber;
            }
            var result = await _userManager.UpdateAsync(user);

            if (result.Succeeded)
            {
                return new UserManagerResponse
                {
                    IsSucess=true
                };
            }
            return new UserManagerResponse
            {
                IsSucess=false,
                Errors=result.Errors.Select(e=>e.Description)
            };
        }

        public async Task<UserManagerResponse> DeleteUser(string id)
        {
            var user = await _userManager.FindByIdAsync(id);

            if (user == null)
            {
                return new UserManagerResponse
                {
                    IsSucess=false
                };
            }
            var result = await _userManager.DeleteAsync(user);
            if (result.Succeeded)
            {
                return new UserManagerResponse
                {
                    IsSucess=true
                };
            }
            return new UserManagerResponse
            {
                IsSucess=false,
                Errors=result.Errors.Select(e=>e.Description)
            };
        }

        public async Task<UserManagerResponse> CheckPassword(string email,string password)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (await _userManager.CheckPasswordAsync(user, password))
            {
                return new UserManagerResponse
                {
                    IsSucess = true
                };
            }
            else
            {
                return new UserManagerResponse
                {
                    IsSucess = false
                };
            }
        }
    }
    
}

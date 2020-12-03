using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TachyHealth.Data;
using TachyHealth.ViewModels;

namespace TachyHealth.Intefaces
{
    public interface IAccount
    {
      Task<UserManagerResponse> RegisterUserAsync(RegisterViewModel model);
      Task<UserManagerResponse> LoginUserAsync(LoginViewModel model);
      Task<UserManagerResponse> CheckEmail(string email);
      Task LogoutAsync();
      Task<UserManagerResponse> CheckEmailConfirmed(LoginViewModel model);
      Task<IdentityResult> ConfirmEmailAsync(string uid,string token);
      Task GenerateEmailConfirmationTokenAsync(ApplicationUser user);
      Task<ApplicationUser> GetUserByEmailAsync(string email);
      Task GenerateForgetPasswordTokenAsync(ApplicationUser user);
      Task<IdentityResult> ResetPasswordAsync(ResetPasswordViewModel model);
    }
}

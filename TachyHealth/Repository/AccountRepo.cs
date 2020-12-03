using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TachyHealth.Data;
using TachyHealth.Intefaces;
using TachyHealth.Models;
using TachyHealth.Services;
using TachyHealth.ViewModels;

namespace TachyHealth.Repository
{
    public class AccountRepo : IAccount
    {
        private UserManager<ApplicationUser> _userManager;
        private SignInManager<ApplicationUser> _signInManager;
        private readonly IEmailService _emailService;
        private readonly IConfiguration _configuration;

        public AccountRepo(UserManager<ApplicationUser> userManager , SignInManager<ApplicationUser> signInManager,IEmailService emailService,IConfiguration configuration)
        {
            _userManager=userManager;
            _signInManager=signInManager;
            _emailService = emailService;
            _configuration = configuration;
        }

        public async Task<UserManagerResponse> CheckEmail(string email)
        {
            var user = await GetUserByEmailAsync(email);
            if (user == null)
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

        public async Task<UserManagerResponse> CheckEmailConfirmed(LoginViewModel model)
        {
            var user = await GetUserByEmailAsync(model.Email);

            if (user != null && !user.EmailConfirmed &&
                        (await _userManager.CheckPasswordAsync(user, model.Password)))
            {
                return new UserManagerResponse
                {
                    IsSucess=false
                };
            }
            return new UserManagerResponse
                {
                    IsSucess=true
                };
        }

        public async Task<UserManagerResponse> LoginUserAsync(LoginViewModel model)
        {
            var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, false);
            if (result.Succeeded)
            {
                return new UserManagerResponse
                {
                    IsSucess = true
                }; 
            }
            return new UserManagerResponse
                {
                    IsSucess = false
                };
        }

        public async Task LogoutAsync()
        {
           await _signInManager.SignOutAsync();
        }

        public async Task<UserManagerResponse> RegisterUserAsync(RegisterViewModel model)
        {
            var user = new ApplicationUser{
                UserName=model.Email,
                Email=model.Email,
                FullName=model.FullName,
                PhoneNumber=model.PhoneNumber
                };
             var result = await _userManager.CreateAsync(user,model.Password);
            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(user, Enums.Roles.NormalUser.ToString());
                await GenerateEmailConfirmationTokenAsync(user);
                //await _signInManager.SignInAsync(user, isPersistent: false);
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
        
        private async Task SendConfirmationEmail(ApplicationUser user,string token)
        {
            string appDomain = _configuration.GetSection("Application:AppDomain").Value;
            string confirmationLink = _configuration.GetSection("Application:EmailConfirmation").Value;

            UserEmailOptions options = new UserEmailOptions
            {
                ToEmails = new List<string>() { user.Email },
                PlaceHolders = new List<KeyValuePair<string, string>>()
                {
                    new KeyValuePair<string, string>("{{UserName}}", user.FullName),
                    new KeyValuePair<string, string>("{{Link}}",
                        string.Format(appDomain + confirmationLink, user.Id, token))
                }
            };

            await _emailService.SendEmailForEmailConfirmation(options);
        }
        private async Task SendForgetPasswordEmail(ApplicationUser user,string token)
        {
            string appDomain = _configuration.GetSection("Application:AppDomain").Value;
            string confirmationLink = _configuration.GetSection("Application:ForgetPassword").Value;

            UserEmailOptions options = new UserEmailOptions
            {
                ToEmails = new List<string>() { user.Email },
                PlaceHolders = new List<KeyValuePair<string, string>>()
                {
                    new KeyValuePair<string, string>("{{UserName}}", user.FullName),
                    new KeyValuePair<string, string>("{{Link}}",
                        string.Format(appDomain + confirmationLink, user.Id, token))
                }
            };

            await _emailService.SendEmailForForgotPassword(options);
        }

        public async Task GenerateEmailConfirmationTokenAsync(ApplicationUser user)
        {
            var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            if (!string.IsNullOrEmpty(token))
            {
                await SendConfirmationEmail(user, token);
            }
        }

        public async Task GenerateForgetPasswordTokenAsync(ApplicationUser user)
        {
            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            if (!string.IsNullOrEmpty(token))
            {
                await SendForgetPasswordEmail(user, token);
            }
        }
        public async Task<IdentityResult> ConfirmEmailAsync(string uid,string token)
        {
            var user = await _userManager.FindByIdAsync(uid);
            var confirmEmail =await _userManager.ConfirmEmailAsync(user,token);
            return confirmEmail;
        }
        public async Task<IdentityResult> ResetPasswordAsync(ResetPasswordViewModel model)
        {
            var user = await _userManager.FindByIdAsync(model.UserId);
            var resetPassword =await _userManager.ResetPasswordAsync(user,model.Token,model.NewPassword);
            return resetPassword;
        }
        public async Task<ApplicationUser> GetUserByEmailAsync(string email)
        {
            return await _userManager.FindByEmailAsync(email);
        }
    }
}

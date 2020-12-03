using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using TachyHealth.Data;
using TachyHealth.Intefaces;
using TachyHealth.Services;
using TachyHealth.ViewModels;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace TachyHealth.Controllers
{
    public class AccountController : Controller
    {
        private IAccount _account;
        private readonly IUserService _userService;

        public AccountController(IAccount account,IUserService userService)
        {
            _account=account;
            _userService = userService;
        }
        [HttpGet]
        public IActionResult ToConfirmEmail()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await _account.LogoutAsync();
            return RedirectToAction("index", "home");
        }
     
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }
        
        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                var result = await _account.RegisterUserAsync(model);
                if (result.IsSucess == true)
                {
                    return RedirectToAction("ConfirmEmail",new { email=model.Email});
                }
                else
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError("", error);
                    }
                }
            }
            return View(model);
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [AcceptVerbs("Get", "Post")]
        public async Task<IActionResult> IsEmailInUse(string email)
        {
            var user = await _account.CheckEmail(email);

            if (user.IsSucess)
            {
                return Json(true);
            }
            else
            {
                return Json($" This email is already in use.");
            }
        }
        
        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var checkEmail= await _account.CheckEmailConfirmed(model);
                if (checkEmail.IsSucess)
                {
                    var result=await _account.LoginUserAsync(model);
                    if (result.IsSucess)
                    {
                        return RedirectToAction("index", "home");
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, "Invalid Login Attempt");
                    }
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Email not confirmed yet");
                }
                
            }
            return View(model);
        }

        [HttpGet("confirm-email")]
        public async Task<IActionResult> ConfirmEmail(string uid,string token,string email)
        {
            EmailConfirmedViewModel model = new EmailConfirmedViewModel
            {
                Email=email
            };

            if (!string.IsNullOrEmpty(uid) && !string.IsNullOrEmpty(token))
            {
               token= token.Replace(' ','+');
               var result = await _account.ConfirmEmailAsync(uid,token);
                if (result.Succeeded)
                {
                    model.EmailVerified=true;
                }
            }
            return View(model);
        }

        [HttpPost("confirm-email")]
        public async Task<IActionResult> ConfirmEmail(EmailConfirmedViewModel model)
        {
            var user = await _account.GetUserByEmailAsync(model.Email);
            if (user != null)
            {
                if (user.EmailConfirmed)
                {
                    model.IsConfirmed = true;
                    return View(model);
                }
                await _account.GenerateEmailConfirmationTokenAsync(user);
                model.EmailSent=true;
                ModelState.Clear();
            }
            else
            {
                ModelState.AddModelError("","Somting went wrong");
            }
            return View(model);
        }

        [AllowAnonymous,HttpGet("forget-password")]
        public IActionResult ForgetPassword()
        {
            return View();
        }

        [AllowAnonymous,HttpPost("forget-password")]
        public async Task< IActionResult> ForgetPassword(ForgetPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user =await _account.GetUserByEmailAsync(model.Email);
                if (user != null)
                {
                    await _account.GenerateForgetPasswordTokenAsync(user);
                }
                ModelState.Clear();
                model.EmailSent=true;
            }
            return View(model);
        }

        [AllowAnonymous,HttpGet("reset-password")]
        public IActionResult ResetPassword(string uid,string token)
        {
            ResetPasswordViewModel model = new ResetPasswordViewModel
            {
                Token=token,
                UserId=uid
            };
            return View(model);
        }

        [AllowAnonymous,HttpPost("reset-password")]
        public async Task< IActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                model.Token= model.Token.Replace(' ','+');
                var result = await _account.ResetPasswordAsync(model);
                if (result.Succeeded)
                {
                    ModelState.Clear();
                    model.IsSuccess=true;
                    return View(model);
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("",error.Description);
                }
            }
            return View(model);
        }
    }
}

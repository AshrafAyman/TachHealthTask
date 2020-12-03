using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TachyHealth.Intefaces;
using TachyHealth.Services;
using TachyHealth.ViewModels;

namespace TachyHealth.Controllers
{
    public class AdministrationController : Controller
    {
        private readonly IAdministration _administration;
        private readonly IUserService _userService;

        public AdministrationController(IAdministration administration,IUserService userService)
        {
            _administration = administration;
            _userService = userService;
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> ListUsers()
        {
            var usersList =await _administration.ListUsers();
            return View(usersList);
        }

        [HttpGet]
        [Authorize(Roles = "NormalUser,Admin")]
        public async Task<IActionResult> EditUser(string id)
        {
            if (id == null)
            {
                return Content("Invalid Id");
                //return RedirectToAction(nameof(ListUsers));
            }
            var editedUser= await _administration.EditUser(id);
        
            return View(editedUser);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> EditUser(EditUserViewModel model)
        {
            if (ModelState.IsValid)
            {
                var editedUser = await _administration.EditUserPost(model);
                if (editedUser.IsSucess)
                {
                    return RedirectToAction(nameof(ListUsers));
                }
                foreach (var error in editedUser.Errors)
                {
                    ModelState.AddModelError("", error);
                }
            }
        
            return View(model);
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> ManageRole(string id)
        {
            if (id == null)
            {
                return Content("Invalid Id");
            }
            ViewBag.userId = id;
            var userRole= await _administration.ManageRole(id);
            return View(userRole);
        }
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> ManageRole(List<UserRoleViewModel>model, string id)
        {
            if (id == null)
            {
                return Content("Invalid Id");
            }
            if (ModelState.IsValid)
            {
                var userRole= await _administration.ManageRolePost(model,id);
                if (userRole.IsSucess)
                {
                    return RedirectToAction(nameof(ListUsers));
                }
                foreach (var error in userRole.Errors)
                {
                    ModelState.AddModelError("", error);
                }
            }
            
            return View(model);
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public IActionResult CreateRole()
        {
            var rolesList = _administration.ListRoles();
            return View(rolesList);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateRole(CreateRoleViewModel model)
        {
            if (ModelState.IsValid)
            {
                var result = await _administration.CreateRole(model);

                if (result.IsSucess)
                {
                    return RedirectToAction(nameof(CreateRole));
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error);
                }
            }
            return View(model);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteUser(string id)
        {
            if (id == null)
            {
                return Content("Invalid Id");
            }
            var deleteUser=await _administration.DeleteUser(id);
            if (deleteUser.IsSucess)
            {
                return RedirectToAction(nameof(ListUsers));
            }
            foreach (var error in deleteUser.Errors)
            {
                ModelState.AddModelError("", error);
            }
            return Content("Can't delete this user");
        }

        [HttpGet]
        [Authorize(Roles = "NormalUser")]
        public IActionResult UserProfile()
        {
            var uid= _userService.GetUserId();
            return RedirectToAction("EditUser",new { id=uid});
        }
        [HttpPost]
        [Authorize(Roles = "NormalUser")]
        public async Task<IActionResult> UserPassowrdCheckPost([FromBody]CheckPasswordViewModel model)
        {
            var result =await _administration.CheckPassword(model.Email,model.Password);
            if (result.IsSucess)
            {
                EditUserViewModel editUser = new EditUserViewModel
                {
                    Id = model.Id,
                    Email = model.Email,
                    FullName = model.FullName,
                    PhoneNumber=model.PhoneNumber
                } ;
                var editedUser= await _administration.EditUserPost(editUser);
                return Json(true);
            }
            else
            {
                return Json(false);
            }
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult AccessDenied()
        {
            return View();
        }

    }
}
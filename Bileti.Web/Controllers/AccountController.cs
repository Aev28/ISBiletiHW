using Bileti.Domain.DTO;
using Bileti.Domain.Identity;
using Bileti.Domain.Models;
using Bileti.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Bileti.Web.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<CustomUser> userManager;
        private readonly SignInManager<CustomUser> signInManager;
        private readonly RoleManager<IdentityRole> roleManager;

        public AccountController(UserManager<CustomUser> userManager, SignInManager<CustomUser> signInManager, RoleManager<IdentityRole> roleManager)
        {

            this.userManager = userManager;
            this.signInManager = signInManager;
            this.roleManager = roleManager;
        }

        public IActionResult Register()
        {
            UserRegistrationDto model = new UserRegistrationDto();
            return View(model);
        }

        [HttpPost, AllowAnonymous]
        public async Task<IActionResult> Register(UserRegistrationDto request)
        {
            if (ModelState.IsValid)
            {
                var userCheck = await userManager.FindByEmailAsync(request.Email);
                if (userCheck == null)
                {
                    var user = new CustomUser
                    {
                        Name = request.Name,
                        Surname = request.Surname,
                        UserName = request.Email,
                        NormalizedUserName = request.Email,
                        Email = request.Email,
                        EmailConfirmed = true,
                        UserCart = new ShoppingCart()
                    };
                    var result = await userManager.CreateAsync(user, request.Password);
                    if (result.Succeeded)
                    {
                        return RedirectToAction("Login");
                    }
                    else
                    {
                        if (result.Errors.Count() > 0)
                        {
                            foreach (var error in result.Errors)
                            {
                                ModelState.AddModelError("message", error.Description);
                            }
                        }
                        return View(request);
                    }
                }
                else
                {
                    ModelState.AddModelError("message", "Email already exists.");
                    return View(request);
                }
            }
            return View(request);

        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Login()
        {
            UserLoginDto model = new UserLoginDto();
            return View(model);
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Login(UserLoginDto model)
        {
            if (ModelState.IsValid)
            {
                var user = await userManager.FindByEmailAsync(model.Email);
                if (user != null && !user.EmailConfirmed)
                {
                    ModelState.AddModelError("message", "Email not confirmed yet");
                    return View(model);

                }
                if (await userManager.CheckPasswordAsync(user, model.Password) == false)
                {
                    ModelState.AddModelError("message", "Invalid credentials");
                    return View(model);

                }

                var result = await signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, true);

                if (result.Succeeded)
                {
                    await userManager.AddClaimAsync(user, new Claim("UserRole", "Admin"));
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    ModelState.AddModelError("message", "Invalid login attempt");
                    return View(model);
                }
            }
            return View(model);
        }


        public async Task<IActionResult> Logout()
        {
            await signInManager.SignOutAsync();
            return RedirectToAction("Login", "Account");
        }

        [HttpGet]
        public async Task<ActionResult> AddUserToRole()
        {
            var user = new UserDTO();
            var serviceProvider = HttpContext.RequestServices;
            var roleManager = serviceProvider.GetService<RoleManager<IdentityRole>>();

            user.Roles = await roleManager.Roles.Select(r => r.Name).ToListAsync();

            var currentUser = await userManager.GetUserAsync(User);
            if (currentUser != null)
            {
                var userRoles = await userManager.GetRolesAsync(currentUser);
                user.SelectedRole = userRoles.FirstOrDefault();
            }

            if (string.IsNullOrEmpty(user.SelectedRole))
            {
                user.SelectedRole = "STANDARD";
            }

            return View(user);
        }

        [HttpPost]
        public async Task<ActionResult> AddUserToRole(UserDTO user)
        {
            if (ModelState.IsValid)
            {
                var email = user.Email;
                var userByEmail = await userManager.FindByEmailAsync(email);
                if (userByEmail == null)
                {
                    return NotFound();
                }
                var standardRoleName = "STANDARD";
                var administratorRoleName = "ADMINISTRATOR";
                var standardRole = await roleManager.FindByNameAsync(standardRoleName);
                if (standardRole == null)
                {
                    standardRole = new IdentityRole(standardRoleName);
                    await roleManager.CreateAsync(standardRole);
                }
                var administratorRole = await roleManager.FindByNameAsync(administratorRoleName);
                if (administratorRole == null)
                {
                    administratorRole = new IdentityRole(administratorRoleName);
                    await roleManager.CreateAsync(administratorRole);
                }
                var rolesToRemove = await userManager.GetRolesAsync(userByEmail);
                await userManager.RemoveFromRolesAsync(userByEmail, rolesToRemove);

                if (user.SelectedRole == administratorRoleName)
                {
                    await userManager.AddToRoleAsync(userByEmail, administratorRoleName);
                }
                else
                {
                    await userManager.AddToRoleAsync(userByEmail, standardRoleName);
                }

                user.Roles = await roleManager.Roles.Select(r => r.Name).ToListAsync();
                var userRoles = await userManager.GetRolesAsync(userByEmail);
                user.SelectedRole = userRoles.FirstOrDefault();

                return RedirectToAction("Index", "Tickets");
            }

            user.Roles = await roleManager.Roles.Select(r => r.Name).ToListAsync();

            return View(user);
        }
    }
}

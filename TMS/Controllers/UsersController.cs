using TMS.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Storage;

namespace TMS.Controllers
{
    public class UsersController : Controller
    {
        private readonly UserManager<User> usrMgr;
        private readonly SignInManager<User> snMgr;
        private readonly RoleManager<IdentityRole> rMan;
        private readonly IHttpContextAccessor http;
        private readonly TMSContext context;

        public UsersController(UserManager<User> usrMgr, SignInManager<User> snMgr,
            RoleManager<IdentityRole> rMan, IHttpContextAccessor http, TMSContext context)
        {
            this.usrMgr = usrMgr;
            this.snMgr = snMgr;
            this.rMan = rMan;
            this.http = http;
            this.context = context;
        }

        [Authorize]
        public JsonResult Roles()
        {
            JsonResult json = new JsonResult("");
            json.StatusCode = 404;
            try
            {
                var data = context.Roles
                    .Select(a => new
                    {
                        a.Name
                    }).ToList();
                json.Value = data;
                json.StatusCode = 200;
                return json;
            }
            catch (Exception ex)
            {
                json.Value = "Error! " + ex.Message;
                return json;
            }
        }

        public async Task<IActionResult> Login()
        {
            await CreateRoles();
            return View();
        }
        [HttpPost]
        public async Task<JsonResult> Login(IFormCollection vm)
        {
            JsonResult json = new JsonResult("");
            if (string.IsNullOrEmpty(vm["UserName"].ToString()))
            {
                json.Value = "Error! Please enter username";
                json.StatusCode = 404;
                return json;
            }
            if (string.IsNullOrEmpty(vm["Password"].ToString()))
            {
                json.Value = "Error! Please enter password";
                json.StatusCode = 404;
                return json;
            }
            string userName = vm["UserName"].ToString();
            string password = vm["Password"].ToString();
            string returnUrl = vm["ReturnUrl"].ToString();
            try
            {
                var signInResult = await snMgr.PasswordSignInAsync(userName, password, true, false);

                if (signInResult.Succeeded)
                {
                    if (!string.IsNullOrEmpty(returnUrl) && returnUrl != "/" && returnUrl != "undefined")
                    {
                        json.Value = returnUrl;
                    }
                    else
                    {
                        json.Value = "/Home/Index";
                    }
                    json.StatusCode = 200;
                    return json;
                }

                json.Value = "Error! UserName or password incorrect";
                json.StatusCode = 404;
                return json;
            }
            catch (Exception ex)
            {
                json.Value = "Error! " + ex.Message;
                json.StatusCode = 404;
                return json;
            }
        }

        [Authorize]
        public IActionResult Register()
        {
            return View();
        }
        [HttpPost]
        public async Task<JsonResult> Register(IFormCollection vm)
        {
            JsonResult json = new JsonResult("");
            if (string.IsNullOrEmpty(vm["UserName"].ToString()))
            {
                json.Value = "Error! Please enter username";
                json.StatusCode = 404;
                return json;
            }
            if (string.IsNullOrEmpty(vm["Password"].ToString()))
            {
                json.Value = "Error! Please enter password";
                json.StatusCode = 404;
                return json;
            }
            if (string.IsNullOrEmpty(vm["ConfirmPassword"].ToString()))
            {
                json.Value = "Error! Please enter password again";
                json.StatusCode = 404;
                return json;
            }
            if (string.IsNullOrEmpty(vm["Role"].ToString()))
            {
                json.Value = "Error! Please select a role from list";
                json.StatusCode = 404;
                return json;
            }
            string userName = vm["UserName"].ToString();
            string password = vm["Password"].ToString();
            string confirmPassword = vm["ConfirmPassword"].ToString();
            string role = vm["Role"].ToString();
            var checkUser = await usrMgr.FindByNameAsync(userName);
            if (checkUser != null)
            {
                json.Value = "Error! User already registered";
                json.StatusCode = 404;
                return json;
            }
            if (password != confirmPassword)
            {
                json.Value = "Error! Passwords do not match";
                json.StatusCode = 404;
                return json;
            }
            string returnUrl = vm["ReturnUrl"].ToString();
            try
            {
                var newUser = new User();
                newUser.UserName = userName;

                using(IDbContextTransaction trans = context.Database.BeginTransaction())
                {
                    var addUserResult = await usrMgr.CreateAsync(newUser);
                    if (addUserResult.Succeeded)
                    {
                        var addPasswordResult = await usrMgr.AddPasswordAsync(newUser, password);
                        if (addPasswordResult.Succeeded)
                        {
                            await snMgr.SignInAsync(newUser, true);
                            if (addPasswordResult.Succeeded)
                            {
                                var roleResult = await usrMgr.AddToRoleAsync(newUser, role);
                                if (roleResult.Succeeded)
                                {
                                    trans.Commit();
                                    json.Value = "/Home/Index";
                                    json.StatusCode = 200;
                                    return json;
                                }
                                json.Value = "Error! " + roleResult.Errors.FirstOrDefault().Description;
                                json.StatusCode = 404;
                                return json;
                            }
                            json.Value = "Error! " + addPasswordResult.Errors.FirstOrDefault().Description;
                            json.StatusCode = 404;
                            return json;
                        }
                        json.Value = "Error! " + addPasswordResult.Errors.FirstOrDefault().Description;
                        json.StatusCode = 404;
                        return json;
                    }
                    json.Value = "Error! " + addUserResult.Errors.FirstOrDefault().Description;
                    json.StatusCode = 404;
                    return json;
                }
            }
            catch (Exception ex)
            {
                json.Value = "Error! " + ex.Message;
                json.StatusCode = 404;
                return json;
            }
        }

        [Authorize]
        public IActionResult ChangePassword()
        {
            return View();
        }
        [HttpPost, Authorize]
        public async Task<JsonResult> ChangePassword(IFormCollection vm)
        {
            JsonResult json = new JsonResult("");
            if (string.IsNullOrEmpty(vm["CurrentPassword"].ToString()))
            {
                json.Value = "Error! Please enter current password";
                json.StatusCode = 404;
                return json;
            }
            if (string.IsNullOrEmpty(vm["Password"].ToString()))
            {
                json.Value = "Error! Please enter password";
                json.StatusCode = 404;
                return json;
            }
            if (string.IsNullOrEmpty(vm["ConfirmPassword"].ToString()))
            {
                json.Value = "Error! Please enter password again";
                json.StatusCode = 404;
                return json;
            }
            string currentPassword = vm["CurrentPassword"].ToString();
            string password = vm["Password"].ToString();
            string confirmPassword = vm["ConfirmPassword"].ToString();
            if (password != confirmPassword)
            {
                json.Value = "Error! Passwords do not match";
                json.StatusCode = 404;
                return json;
            }
            try
            {
                User u = await usrMgr.GetUserAsync(User);
                var cpResult = await usrMgr.ChangePasswordAsync(u, currentPassword, password);
                if (cpResult.Succeeded)
                {
                    json.Value = "/Home/Index";
                    json.StatusCode = 200;
                    return json;
                }
                json.Value = "Error! " + cpResult.Errors.FirstOrDefault().Description;
                json.StatusCode = 404;
                return json;
            }
            catch (Exception ex)
            {
                json.Value = "Error! " + ex.Message;
                json.StatusCode = 404;
                return json;
            }
        }

        public async Task<IActionResult> Logout()
        {
            await snMgr.SignOutAsync();
            return RedirectToAction("Login");
        }

        public async Task<string> CreateRoles()
        {
            string[] roles = { "Admin", "Manager", "User" };
            foreach (var r in roles)
            {
                var checkRole = await rMan.RoleExistsAsync(r);
                if (!checkRole)
                {
                    IdentityRole role = new IdentityRole(r);
                    var roleResult = await rMan.CreateAsync(role);
                    if (!roleResult.Succeeded)
                    {
                        return roleResult.Errors.FirstOrDefault().Description;
                    }
                }
            }
            return "Success";
        }
    }
}

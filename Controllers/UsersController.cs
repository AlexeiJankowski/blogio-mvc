using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Blogio.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Blogio.Models;

namespace Blogio.Controllers
{
    
    public class UsersController : Controller
    {
        private UserManager<User> _userManager;
        public UsersController(UserManager<User> userManager)
        {
            _userManager = userManager;
        }

        [Authorize(Roles = "Admin")]
        public IActionResult Index()
        {
            return View(_userManager.Users.ToList());
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        } 

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> Create(CreateUserViewModel model)
        {
            if(ModelState.IsValid)
            {
                User user = new User { Email = model.Email, UserName = model.Email, NickName = model.NickName };
                IdentityResult result = await _userManager.CreateAsync(user, model.Password);
                if(result.Succeeded)
                {
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    foreach(var error in result.Errors)
                    {
                        ModelState.AddModelError("", error.Description);
                    }
                }
            }
            return View(model);
        }

        [Authorize(Roles = "Admin, Writer, User")]
        public async Task<IActionResult> Edit(string? id)
        {
            User user;
            if(id == null)
            {
                user = await _userManager.GetUserAsync(HttpContext.User);
            }
            else
            {
                user  = await _userManager.FindByIdAsync(id);
            }            
            if(user == null)
            {
                return NotFound();
            }
            EditUserViewModel model = new EditUserViewModel { Id = user.Id, Email = user.Email, NickName = user.NickName };
            return View(model);
        }

        [Authorize(Roles = "Admin, Writer, User")]
        [HttpPost]
        public async Task<IActionResult> Edit(EditUserViewModel model)
        {
            if(ModelState.IsValid)
            {
                User user = await _userManager.FindByIdAsync(model.Id);
                if(user != null)
                {
                    user.Email = model.Email;
                    user.UserName = model.Email;
                    user.NickName = model.NickName;

                    IdentityResult result = await _userManager.UpdateAsync(user);
                    if(result.Succeeded)
                    {
                        if(User.IsInRole("Admin, Writer"))
                        {
                            return RedirectToAction(nameof(Index));
                        }
                        else
                        {
                            return RedirectToAction(nameof(Index), "Home");
                        }
                    }
                    else
                    {
                        foreach(var error in result.Errors)
                        {
                            ModelState.AddModelError("", error.Description);
                        }
                    }
                }
            }
            return View(model);
        }

        [Authorize(Roles = "Admin, Writer, User")]
        [HttpGet]
        public async Task<IActionResult> ChangePassword(string id)
        {
            User user = await _userManager.FindByIdAsync(id);
            if(user == null)
            {
                return NotFound();
            }
            ChangePasswordViewModel model = new ChangePasswordViewModel { Id = user.Id, Email = user.Email };
            return View(model);
        }

        [Authorize(Roles = "Admin, Writer, User")]
        [HttpPost]
        public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            if(ModelState.IsValid)
            {
                User user = await _userManager.FindByIdAsync(model.Id);
                if(user != null)
                {
                    IdentityResult result = await _userManager.ChangePasswordAsync(user, model.OldPassword, model.NewPassword);
                    if(result.Succeeded)
                    {                        
                        return RedirectToAction(nameof(Edit));
                    }
                    else
                    {
                        foreach(var error in result.Errors)
                        {
                            ModelState.AddModelError("", error.Description);
                        }
                    }                    
                }
                else
                {
                    ModelState.AddModelError("", "User doesn't exist");
                }
            }
            return View(model);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> Delete(string id)
        {
            User user = await _userManager.FindByIdAsync(id);
            if(user != null)
            {
                IdentityResult result = await _userManager.DeleteAsync(user);
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
using System.Threading.Tasks;
using UserRegistrationApp.Models;
using UserRegistrationApp.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace UserRegistrationApp.Controllers
{
   [AllowAnonymous]
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> userManager;
        private readonly SignInManager<ApplicationUser> signInManager;
        public AccountController(UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
        }
      
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            var owner = await userManager.FindByNameAsync(model.Email);
            if (owner != null)
            {
                ModelState.AddModelError("Email", "Email Already Exists");
                return View();
            }
            if (ModelState.IsValid)
            {
                var user = new ApplicationUser
                {
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    Address = model.Address,
                    Dob = model.Dob,
                    UserName = model.Email,
                    Email = model.Email,
                };

                var result = await userManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    await signInManager.SignInAsync(user, isPersistent: false);
                    return RedirectToAction("Index", "Home");
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
            }
            return View();
        }


        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {            
            if (ModelState.IsValid)
            {                
                var result = await signInManager.PasswordSignInAsync(model.UserName, model.Password, model.RememberMe,lockoutOnFailure:false);
                if (result.Succeeded)
                {
                    return RedirectToAction("Index", "Home");
                }  
                else if (result.IsNotAllowed)
                {
                    ModelState.AddModelError("Password", "Invalid Credentials");
                }               
            }
            return View();
        }
    }
}

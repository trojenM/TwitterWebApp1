using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using TwitterWebApp1.Data;
using TwitterWebApp1.Models.Account;

namespace TwitterWebApp1.Controllers
{
    public class AccountController : BaseController
    {
        public AccountController(ILogger<BaseController> logger, AppDBContext context, UserManager<AppUser> userManager, SignInManager<AppUser> signInManager) : base(logger, context, userManager, signInManager)
        {
        }

        [HttpGet]
        public IActionResult Index()
        {
            if (User.Identity!.IsAuthenticated)
                return RedirectToAction("Index", "Home");

            return RedirectToAction("Login");
        }

        // GET: /Account/Login
        [HttpGet]
        public IActionResult Login()
        {
            if (User.Identity!.IsAuthenticated)
                return RedirectToAction("Index", "Home");

            return View();
        }

        // POST: /Account/Login
        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel vm)
        {
            if (ModelState.IsValid)
            {
                var user = await userManager.FindByEmailAsync(vm.Email);

                if (user == null)
                {
                    ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                    return View(vm);
                }

                var signInResult = signInManager.PasswordSignInAsync(user.UserName!, vm.Password, vm.RememberMe, false).Result;

                if (signInResult.Succeeded)
                {
                    return RedirectToAction("Index", "Home");
                }

                ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                return View(vm);
            }

            return View(vm);
        }

        // GET: /Account/Register
        [HttpGet]
        public IActionResult Register()
        {
            if (User.Identity!.IsAuthenticated)
                return RedirectToAction("Index", "Home");

            return View();
        }

        // POST: /Account/Register
        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel vm)
        {
            // TODO: Implement registration logic
            if (ModelState.IsValid)
            {
                var user = new AppUser
                {
                    Email = vm.Email,
                    UserName = vm.UserName,
                    Name = vm.Name,
                    CreatedAt = DateTime.Now
                };

                var result = await userManager.CreateAsync(user, vm.Password);

                if (result.Succeeded)
                {
                    await signInManager.SignInAsync(user, false);
                    return RedirectToAction("Index", "Home");
                }

                foreach (var error in result.Errors)
                    ModelState.AddModelError(string.Empty, error.Description);

                return View(vm);
            }

            return View(vm);
        }

        // GET: /Account/Logout
        [HttpGet]
        public async Task<IActionResult> Logout()
        {
            await signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }

        // Checks for username availability when registering.
        public JsonResult IsUserNameInUse(string username)
        {
            var user = context.Users.FirstOrDefault(u => u.UserName == username);
            return Json(user == null);
        }

        // Checks for email availability when registering.
        public JsonResult IsEmailInUse(string email)
        {
            var user = context.Users.FirstOrDefault(u => u.Email == email);
            return Json(user == null);
        }
    }
}
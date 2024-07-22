using Application.Dtos;
using Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace Application.Areas.Identity.Controllers
{
    [AllowAnonymous]
    [Area("Identity")]
    [Route("account")]
    public class SignInController(IUsersService usersService) : Controller
    {
        private readonly IUsersService _usersService = usersService;

        [Route("register")]
        public IActionResult Register()
        {
            UserRegistrationDto model = new();
            return View(model);
        }

        [Route("register")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(UserRegistrationDto request)
        {
            if (ModelState.IsValid)
            {
                var userResult = await _usersService.RegisterUserAsync(request);
                if (userResult.IsSuccess)
                {
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    foreach (var error in userResult.ValidationResult.Errors)
                    {
                        ModelState.AddModelError(error.Key, error.Value);
                    }
                }
            }

            return View(request);
        }

        [Route("login")]
        public IActionResult Login([FromQuery] string? returnUrl = null)
        {
            TempData["ReturnUrl"] = returnUrl;
            return View();
        }

        [Route("login")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(UserLoginDto request)
        {
            string? returnUrl = TempData["ReturnUrl"] as string;

            if (ModelState.IsValid)
            {
                var userResult = await _usersService.LoginUserAsync(request);
                if (userResult.IsSuccess)
                {
                    if (!returnUrl.IsNullOrEmpty() && Url.IsLocalUrl(returnUrl))
                    {
                        return Redirect(returnUrl);
                    }
                    else
                    {
                        return RedirectToAction("Index", "Home");
                    }
                }
                else
                {
                    foreach (var error in userResult.ValidationResult.Errors)
                    {
                        ModelState.AddModelError(error.Key, error.Value);
                    }
                }
            }

            return View(request);
        }

        [Route("logout")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _usersService.LogoutAsync();
            return RedirectToAction("Index", "Home");
        }
    }
}

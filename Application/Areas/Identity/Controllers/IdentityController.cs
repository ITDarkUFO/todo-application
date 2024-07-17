using Application.Areas.Identity.Models;
using Application.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.Text.RegularExpressions;

namespace Application.Areas.Identity.Controllers
{
    [AllowAnonymous]
    [Area("Identity")]
    [Route("account")]
    public partial class IdentityController(UserManager<User> userManager, SignInManager<User> signInManager) : Controller
    {
        [GeneratedRegex("^[a-zA-Z0-9]+(?:\\.[a-zA-Z0-9]+)*@[a-zA-Z0-9]+(?:\\.[a-zA-Z0-9]+)*$")]
        private static partial Regex emailRegex();

        private readonly UserManager<User> _userManager = userManager;
        private readonly SignInManager<User> _signInManager = signInManager;
        private readonly PasswordHasher<User> _passwordHasher = new();

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
            if (!emailRegex().IsMatch(request.Email))
                ModelState.AddModelError("Email", "Некорректный адрес электронной почты");

            if (await _userManager.FindByEmailAsync(request.Email) is not null)
                ModelState.AddModelError("Email", "Почта уже используется");

            if (await _userManager.FindByNameAsync(request.UserName) is not null)
                ModelState.AddModelError("UserName", "Данное имя уже занято");

            if (request.Password != request.ConfirmPassword)
                ModelState.AddModelError("ConfirmPassword", "Пароли не совпадают");

            if (ModelState.IsValid)
            {
                User newUser = new()
                {
                    Email = request.Email,
                    NormalizedEmail = request.Email.Normalize().ToUpperInvariant(),
                    UserName = request.UserName,
                    NormalizedUserName = request.UserName.Normalize().ToUpperInvariant()
                };

                newUser.PasswordHash = _passwordHasher.HashPassword(newUser, request.Password);

                await _userManager.CreateAsync(newUser);
                await _signInManager.SignInAsync(newUser, false);

                return RedirectToAction("Index", "Home");
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

            User? user = await _userManager.FindByNameAsync(request.UserName);

            if (user is null)
                ModelState.AddModelError("UserName", "Данный пользователь не найден");

            if (ModelState.IsValid)
            {
                await _signInManager.PasswordSignInAsync(user!, request.Password, request.RememberMe, false);

                if (_signInManager.IsSignedIn(User))
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
                    ModelState.AddModelError("Password", "Неправильный пароль");
                }
            }

            return View(request);
        }

        [Route("logout")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }
    }
}

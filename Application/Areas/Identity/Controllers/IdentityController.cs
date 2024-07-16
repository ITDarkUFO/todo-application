using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Application.Data;
using Application.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;

namespace Application.Areas.Identity.Controllers
{
    [Area("Identity")]
    [Route("account")]
    public class IdentityController(UserManager<User> userManager, SignInManager<User> signInManager) : Controller
    {
        private readonly UserManager<User> _userManager = userManager;
        private readonly SignInManager<User> _signInManager = signInManager;
        private readonly PasswordHasher<User> _passwordHasher = new();

        [Route("register")]
        public IActionResult Register()
        {
            return View();
        }

        [Route("register")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register([FromForm] string password, [Bind("UserName,Email,PhoneNumber")] User user)
        {
            if (string.IsNullOrEmpty(user.Email))
            {
                ModelState.AddModelError("Email", "Введите почтовый адрес");
            }
            if (string.IsNullOrEmpty(user.UserName))
            {
                ModelState.AddModelError("UserName", "Введите имя пользователя");
            }

            if (await _userManager.FindByEmailAsync(user.Email!.Normalize().ToUpperInvariant()) is not null)
            {
                ModelState.AddModelError("Email", "Почта уже зарегистрирована");
            }
            if (await _userManager.FindByNameAsync(user.UserName!.Normalize().ToUpperInvariant()) is not null)
            {
                ModelState.AddModelError("UserName", "Это имя уже занято");
            }

            if (string.IsNullOrEmpty(password))
            {
                ModelState.AddModelError("", "Введите пароль");
            }

            if (ModelState.IsValid)
            {
                user.NormalizedEmail = user.Email.Normalize().ToUpperInvariant();
                user.NormalizedUserName = user.UserName!.Normalize().ToUpperInvariant();
                user.PasswordHash = _passwordHasher.HashPassword(user, password);

                await _userManager.CreateAsync(user);
                await _signInManager.SignInAsync(user, false);

                return RedirectToAction("Index", "Home");
            }

            return View(user);
        }

        [Route("login")]
        public IActionResult Login(string? returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        [Route("login")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login([FromForm] string? returnUrl, [FromForm] string password, [Bind("Email")] User user)
        {
            if (string.IsNullOrEmpty(user.Email))
            {
                ModelState.AddModelError("Email", "Введите почтовый адрес");
            }

            if (ModelState.IsValid)
            {
                var userCredentials = await _userManager.FindByEmailAsync(user.Email!.Normalize().ToUpperInvariant());

                if (userCredentials is not null)
                {
                    var result = await _signInManager.PasswordSignInAsync(userCredentials, password, false, false);

                    if (result.Succeeded)
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
                        ModelState.AddModelError("", "Неверный пароль");
                    }
                }
                else
                {
                    ModelState.AddModelError("", "Указанный пользователь не найден");
                }
            }

            return View(user);
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

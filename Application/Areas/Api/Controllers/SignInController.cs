using Application.Dtos;
using Application.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Text.RegularExpressions;

namespace Application.Areas.Api.Controllers
{
    [AllowAnonymous]
    [ApiController]
    [Area("Api")]
    [Route("api/account")]
    public partial class SignInController(UserManager<User> userManager, SignInManager<User> signInManager) : ControllerBase
    {
        [GeneratedRegex("^[a-zA-Z0-9]+(?:\\.[a-zA-Z0-9]+)*@[a-zA-Z0-9]+(?:\\.[a-zA-Z0-9]+)*$")]
        private static partial Regex emailRegex();

        private readonly UserManager<User> _userManager = userManager;
        private readonly SignInManager<User> _signInManager = signInManager;
        private readonly PasswordHasher<User> _passwordHasher = new();

        [HttpPost]
        [Route("login")]
        public async Task<IActionResult> Login([FromBody] UserLoginDto request)
        {
            User? user = await _userManager.FindByNameAsync(request.UserName);

            if (user is null)
                ModelState.AddModelError("UserName", "Данный пользователь не найден");

            if (ModelState.IsValid)
            {
                await _signInManager.PasswordSignInAsync(user!, request.Password, request.RememberMe, false);

                if (_signInManager.IsSignedIn(User))
                {
                    return Ok("Вход выполнен");
                }
                else
                {
                    ModelState.AddModelError("Password", "Неправильный пароль");
                }
            }

            return BadRequest(ModelState);
        }

        [HttpPost]
        [Route("register")]
        public async Task<IActionResult> Register([FromBody] UserRegistrationDto request)
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

                return Ok("Регистрация прошла успешно");
            }

            return BadRequest(request);
        }

        [Route("logout")]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return Ok("Выход из системы выполнен");
        }
    }
}

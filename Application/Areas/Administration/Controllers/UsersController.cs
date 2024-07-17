using Application.Areas.Identity.Models;
using Application.Data;
using Application.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;

namespace Application.Areas.Administration.Controllers
{
    [Area("Administration")]
    [Route("admin/users")]
    public partial class UsersController(ApplicationDbContext context, UserManager<User> userManager, SignInManager<User> signInManager) : Controller
    {
        [GeneratedRegex("^[a-zA-Z0-9]+(?:\\.[a-zA-Z0-9]+)*@[a-zA-Z0-9]+(?:\\.[a-zA-Z0-9]+)*$")]
        private static partial Regex emailRegex();

        private readonly ApplicationDbContext _context = context;
        private readonly UserManager<User> _userManager = userManager;
        private readonly SignInManager<User> _signInManager = signInManager;
        private readonly PasswordHasher<User> _passwordHasher = new();

        [Route("")]
        public async Task<IActionResult> Index()
        {
            var users = await _context.Users.ToListAsync();

            return View(users);
        }

        [Route("create")]
        public IActionResult Create()
        {
            UserRegistrationDto model = new();
            return View(model);
        }

        [Route("create")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(UserRegistrationDto request)
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

                return RedirectToAction(nameof(Index));
            }

            return View(request);
        }

        [Route("delete")]
        public async Task<IActionResult> Delete([FromQuery] string? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }

        [Route("delete")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed([FromForm] string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user != null)
            {
                await _userManager.DeleteAsync(user);
            }

            return RedirectToAction(nameof(Index));
        }
    }
}

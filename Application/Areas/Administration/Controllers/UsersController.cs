using Application.Data;
using Application.Dtos;
using Application.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;

namespace Application.Areas.Administration.Controllers
{
    [Authorize(Roles = "admin")]
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

            foreach (var user in users)
            {
                user.ToDoItems = await _context.ToDoItems.Where(i => i.User == user.Id).ToListAsync();
            }

            return View(users);
        }

        [Route("detais")]
        public async Task<IActionResult> Details([FromQuery] string? id)
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

            user.ToDoItems = await _context.ToDoItems.Where(i => i.User == user.Id).ToListAsync();

            foreach (var item in user.ToDoItems)
            {
                item.PriorityNavigation = await _context.Priorities.FirstOrDefaultAsync(p => p.Id == item.Priority);
            }

            return View(user);
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
                //await _userManager.AddToRoleAsync(newUser, "admin");

                await _userManager.CreateAsync(newUser);

                return RedirectToAction(nameof(Index));
            }

            return View(request);
        }

        [Route("edit")]
        public async Task<IActionResult> Edit([FromQuery] string? id)
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

            UserEditDto userEdit = new()
            {
                Id = user.Id,
                UserName = user.UserName!,
                Email = user.Email!
            };

            ViewData["PreviousPage"] = Request.Headers.Referer.ToString();

            return View(userEdit);
        }

        [Route("edit")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit([FromForm] UserEditDto request)
        {
            User? user = await _userManager.FindByIdAsync(request.Id);

            if (user == null)
            {
                return NotFound();
            }

            if (!emailRegex().IsMatch(request.Email))
                ModelState.AddModelError("Email", "Некорректный адрес электронной почты");

            if (await _userManager.FindByEmailAsync(request.Email) is not null
                && !user.NormalizedEmail!.Equals(request.Email.Normalize(), StringComparison.InvariantCultureIgnoreCase))
                ModelState.AddModelError("Email", "Почта уже используется");

            if (await _userManager.FindByNameAsync(request.UserName) is not null
                && !user.NormalizedUserName!.Equals(request.UserName.Normalize(), StringComparison.InvariantCultureIgnoreCase))
                ModelState.AddModelError("UserName", "Данное имя уже занято");

            if (request.Password != request.ConfirmPassword)
                ModelState.AddModelError("ConfirmPassword", "Пароли не совпадают");

            //if (request.Password != null && request.CurrentPassword == null)
            //    ModelState.AddModelError("CurrentPassword", "Укажите текущий пароль");

            //if (request.CurrentPassword != null && request.Password != null)
            //    if (!await _userManager.CheckPasswordAsync(user, request.CurrentPassword))
            //        ModelState.AddModelError("CurrentPassword", "Неверный пароль");

            if (ModelState.IsValid)
            {
                if (!user.NormalizedEmail!.Equals(request.Email.Normalize(), StringComparison.InvariantCultureIgnoreCase))
                {
                    // INFO: Токен должен отправляться на новую почту, но так как это тестовое задание, он используется напрямую
                    var changeEmailToken = await _userManager.GenerateChangeEmailTokenAsync(user, request.Email);
                    await _userManager.ChangeEmailAsync(user, request.Email, changeEmailToken);
                }

                if (!user.NormalizedUserName!.Equals(request.UserName.Normalize(), StringComparison.InvariantCultureIgnoreCase))
                    await _userManager.SetUserNameAsync(user, request.UserName);

                if (request.Password != null)
                {
                    // INFO: Токен должен отправиться на почту, но так как это тестовое задание, он используется напрямую
                    var changePasswordToken = await _userManager.GeneratePasswordResetTokenAsync(user);
                    await _userManager.ResetPasswordAsync(user, changePasswordToken, request.Password);
                }

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

using Application.Data;
using Application.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Application.Areas.Administration.Controllers
{
    [Area("Administration")]
    [Route("admin/users")]
    public class UsersController(ApplicationDbContext context, UserManager<User> userManager, SignInManager<User> signInManager) : Controller
    {
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
            return View();
        }

        [Route("create")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([FromForm] string password, [Bind("UserName,Email,PhoneNumber")] User user)
        {
            if (string.IsNullOrEmpty(user.Email))
            {
                ModelState.AddModelError("Email", "Введите почтовый адрес");
            }
            if (string.IsNullOrEmpty(user.UserName))
            {
                ModelState.AddModelError("UserName", "Введите имя пользователя");
            }

            if (await _userManager.FindByEmailAsync(user.Email!.Normalize().ToUpperInvariant()) is null)
            {
                ModelState.AddModelError("Email", "Почта уже зарегистрирована");
            }
            if (await _userManager.FindByNameAsync(user.UserName!.Normalize().ToUpperInvariant()) is null)
            {
                ModelState.AddModelError("UserName", "Это имя уже занято");
            }

            if (string.IsNullOrEmpty(password))
            {
                ModelState.AddModelError("", "Введите пароль");
            }

            if (ModelState.IsValid)
            {
                user.NormalizedEmail = user.Email!.Normalize().ToUpperInvariant();
                user.NormalizedUserName = user.UserName!.Normalize().ToUpperInvariant();
                user.PasswordHash = _passwordHasher.HashPassword(user, password);

                await _userManager.CreateAsync(user);

                return RedirectToAction(nameof(Index));
            }

            return View(user);
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

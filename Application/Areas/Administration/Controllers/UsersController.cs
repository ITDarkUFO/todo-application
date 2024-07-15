using Application.Data;
using Application.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Application.Areas.Administration.Controllers
{
    [Area("Administration")]
    [Route("admin/users")]
    public class UsersController(ApplicationDbContext context) : Controller
    {
        private readonly ApplicationDbContext _context = context;
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

            if (ModelState.IsValid)
            {
                user.NormalizedUserName = user.UserName!.Normalize().ToUpperInvariant();
                user.NormalizedEmail = user.Email!.Normalize().ToUpperInvariant();
                user.PasswordHash = _passwordHasher.HashPassword(user, password);

                _context.Add(user);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            return View(user);
        }
    }
}

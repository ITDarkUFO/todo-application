using Application.Dtos;
using Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Application.Areas.Administration.Controllers
{
    [Authorize(Roles = "admin")]
    [Area("Administration")]
    [Route("admin/users")]
    public class UsersController(IUsersService usersService) : Controller
    {
        private readonly IUsersService _usersService = usersService;

        [Route("")]
        public async Task<IActionResult> Index()
        {
            var users = await _usersService.GetUsersListAsync();
            return View(users);
        }

        [Route("detais")]
        public async Task<IActionResult> Details([FromQuery] string? id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return NotFound();
            }

            var userResult = await _usersService.GetUserByIdAsync(User, id);
            if (!userResult.IsSuccess)
            {
                return NotFound();
            }

            return View(userResult.User);
        }

        [Route("create")]
        public IActionResult Create()
        {
            AdminUserRegistrationDto model = new();
            return View(model);
        }

        [Route("create")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(AdminUserRegistrationDto request)
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

        [Route("edit")]
        public async Task<IActionResult> Edit([FromQuery] string? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var userResult = await _usersService.GetUserByIdAsync(User, id);
            if (!userResult.IsSuccess)
            {
                return NotFound();
            }

            AdminUserEditDto userEdit = new()
            {
                Id = userResult.User!.Id,
                UserName = userResult.User.UserName!,
                Email = userResult.User.Email!,
                IsSuperUser = await _usersService.IsUserAdmin(userResult.User)
            };

            ViewData["PreviousPage"] = Request.Headers.Referer.ToString();

            return View(userEdit);
        }

        [Route("edit")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit([FromForm] AdminUserEditDto request)
        {
            if (ModelState.IsValid)
            {
                var userResult = await _usersService.EditUserAsync(request);
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

        [Route("delete")]
        public async Task<IActionResult> Delete([FromQuery] string? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var userResult = await _usersService.GetUserByIdAsync(User, id);
            if (!userResult.IsSuccess)
            {
                return NotFound();
            }

            return View(userResult.User);
        }

        [Route("delete")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed([FromForm] string id)
        {
            var userResult = await _usersService.DeleteUserAsync(id);
            if (userResult.IsSuccess)
            {
                return RedirectToAction(nameof(Index));
            }
            else
            {
                return NotFound();
            }
        }
    }
}

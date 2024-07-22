using Application.Dtos;
using Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Application.Areas.Api.Controllers
{
    [Authorize(Roles = "admin")]
    [ApiController]
    [Area("Api")]
    [Route("api/users")]
    public class UsersController(IUsersService usersService) : ControllerBase
    {
        private readonly IUsersService _usersService = usersService;

        [Route("")]
        public async Task<IActionResult> Index()
        {
            var users = await _usersService.GetUsersListAsync();
            return Ok(users);
        }

        [Route("details")]
        public async Task<IActionResult> Details([FromQuery] string? id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return NotFound();
            }

            var userResult = await _usersService.GetUserByIdAsync(User, id);
            if (userResult.IsSuccess)
            {
                return Ok(userResult.User);
            }

            return NotFound();
        }

        [Route("create")]
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] AdminUserRegistrationDto request)
        {
            if (ModelState.IsValid)
            {
                var userResult = await _usersService.RegisterUserAsync(request);
                if (userResult.IsSuccess)
                {
                    return CreatedAtAction(nameof(Create), userResult.User);
                }
                else
                {
                    foreach (var error in userResult.ValidationResult.Errors)
                    {
                        ModelState.AddModelError(error.Key, error.Value);
                    }
                }
            }

            return BadRequest(request);
        }

        [Route("edit")]
        [HttpPost]
        public async Task<IActionResult> Edit([FromBody] AdminUserEditDto request)
        {
            if (ModelState.IsValid)
            {
                var userResult = await _usersService.EditUserAsync(request);
                if (userResult.IsSuccess)
                {
                    return Ok(userResult.User);
                }
                else
                {
                    foreach (var error in userResult.ValidationResult.Errors)
                    {
                        ModelState.AddModelError(error.Key, error.Value);
                    }
                }
            }

            return BadRequest(request);
        }

        [Route("delete")]
        [HttpPost]
        public async Task<IActionResult> Delete([FromBody] string id)
        {
            var userResult = await _usersService.DeleteUserAsync(id);
            if (userResult.IsSuccess)
            {
                return Ok(userResult.User);
            }
            else
            {
                return NotFound();
            }
        }
    }
}

using Application.Dtos;
using Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.Blazor;

namespace Application.Areas.Api.Controllers
{
    [AllowAnonymous]
    [ApiController]
    [Area("Api")]
    [Route("api/account")]
    public class SignInController(IUsersService usersService) : ControllerBase
    {
        private readonly IUsersService _usersService = usersService;

        [HttpPost]
        [Route("login")]
        public async Task<IActionResult> Login([FromBody] UserLoginDto request)
        {
            if (ModelState.IsValid)
            {
                var userResult = await _usersService.LoginUserAsync(request);
                if (userResult.IsSuccess)
                {
                    return Ok("Выполнен вход в систему");
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

        [HttpPost]
        [Route("register")]
        public async Task<IActionResult> Register([FromBody] UserRegistrationDto request)
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

        [Route("logout")]
        public async Task<IActionResult> Logout()
        {
            await _usersService.LogoutAsync();
            return Ok("Выполнен выход из системы");
        }
    }
}

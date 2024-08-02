using Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Application.Areas.Api.Controllers
{
    [ApiController]
    [Area("Api")]
    [Route("api/account")]
    public class AccountsController(IUsersService usersService) : ControllerBase
    {
        private readonly IUsersService _usersService = usersService;

        [HttpGet("")]
        public async Task<IActionResult> Index()
        {
            var userResult = await _usersService.GetUserAsync(User);
            if (userResult.IsSuccess)
            {
                return Ok(userResult.User);
            }

            return NotFound();
        }
    }
}

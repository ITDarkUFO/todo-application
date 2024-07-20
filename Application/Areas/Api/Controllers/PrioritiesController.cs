using Application.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Application.Areas.Api.Controllers
{
    [ApiController]
    [Area("Api")]
    [Route("api/priorities")]
    public class PrioritiesController(ApplicationDbContext context) : ControllerBase
    {
        private readonly ApplicationDbContext _context = context;

        [Route("")]
        public async Task<IActionResult> Index()
        {
            var priorities = await _context.Priorities.OrderBy(p => p.Level).ToListAsync();

            foreach (var priority in priorities)
            {
                priority.ToDoItems = await _context.ToDoItems.Where(i => i.Priority == priority.Id).ToListAsync();
            }

            return Ok(priorities);
        }

        [Route("details")]
        public async Task<IActionResult> Details([FromQuery] int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var priority = await _context.Priorities.FirstOrDefaultAsync(p => p.Id == id);
            if (priority == null)
            {
                return NotFound();
            }

            priority.ToDoItems = await _context.ToDoItems.Where(i => i.Priority == priority.Id).ToListAsync();

            return Ok(priority);
        }
    }
}

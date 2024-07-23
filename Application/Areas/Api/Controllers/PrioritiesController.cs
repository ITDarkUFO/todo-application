using Application.Interfaces;
using Application.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Application.Areas.Api.Controllers
{
    [Authorize(Roles = "admin")]
    [ApiController]
    [Area("Api")]
    [Route("api/priorities")]
    public class PrioritiesController(IPrioritiesService priorityService) : ControllerBase
    {
        private readonly IPrioritiesService _priorityService = priorityService;

        [Route("")]
        public async Task<IActionResult> Index()
        {
            var priorities = await _priorityService.GetPriorityListAsync();

            if (priorities is null)
            {
                return NotFound();
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

            var priorityResult = await _priorityService.GetPriorityByIdAsync(id.Value);
            if (!priorityResult.IsSuccess)
            {
                return NotFound();
            }

            return Ok(priorityResult.Priority);
        }

        [HttpPost]
        [Route("create")]
        public async Task<IActionResult> Create([FromBody] Priority priority)
        {
            if (ModelState.IsValid)
            {
                var priorityResult = await _priorityService.CreatePriorityAsync(priority);

                if (priorityResult.ValidationResult.IsValid)
                {
                    return CreatedAtAction(nameof(Create), priorityResult.Priority);
                }
                else
                {
                    foreach (var error in priorityResult.ValidationResult.Errors)
                    {
                        ModelState.AddModelError(error.Key, error.Value);
                    }
                }
            }

            return BadRequest(ModelState);
        }

        [HttpPost]
        [Route("edit")]
        public async Task<IActionResult> Edit([FromBody] Priority priority)
        {
            if (ModelState.IsValid)
            {
                var priorityResult = await _priorityService.EditPriorityAsync(priority);

                if (priorityResult.ValidationResult.IsValid)
                {
                    return Ok(priorityResult.Priority);
                }
                else
                {
                    foreach (var error in priorityResult.ValidationResult.Errors)
                    {
                        ModelState.AddModelError(error.Key, error.Value);
                    }
                }
            }

            return BadRequest(ModelState);
        }

        [HttpPost]
        [Route("delete")]
        public async Task<IActionResult> Delete([FromBody] int id)
        {
            var priorityResult = await _priorityService.DeletePriorityAsync(id);

            if (!priorityResult.ValidationResult.IsValid)
            {
                NotFound();
            }

            return Ok(priorityResult.Priority);
        }
    }
}

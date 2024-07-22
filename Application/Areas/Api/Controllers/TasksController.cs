using Application.Data;
using Application.Enums;
using Application.Interfaces;
using Application.Models;
using Application.Services;
using Application.Utilities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace Application.Areas.Api.Controllers
{
    [ApiController]
    [Area("Api")]
    [Route("api/tasks")]
    public class TasksController(ITasksService tasksService) : ControllerBase
    {
        private readonly ITasksService _tasksService = tasksService;

        [Route("")]
        public async Task<IActionResult> Index([FromQuery] TasksSorterEnum? tasksSorter, [FromQuery] TaskStatusFilterEnum? taskStatusFilter,
            [FromQuery] int? taskPriorityFilter, [FromQuery] string? userLoginFilter)
        {
            var tasks = await _tasksService.GetTaskListAsync(User, tasksSorter, taskStatusFilter, taskPriorityFilter, userLoginFilter);
            if (tasks is null)
            {
                return NotFound();
            }

            return Ok(tasks);
        }

        [Route("details")]
        public async Task<IActionResult> Details([FromQuery] int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var taskResult = await _tasksService.GetTaskByIdAsync(User, id.Value);
            if (!taskResult.IsSuccess)
            {
                return NotFound();
            }

            return Ok(taskResult.Task);
        }

        [HttpPost]
        [Route("create")]
        public async Task<IActionResult> Create([FromBody] ToDoItem task, [FromQuery] TimeOnly? dueTime)
        {
            if (ModelState.IsValid)
            {
                var taskResult = await _tasksService.CreateTaskAsync(User, task, dueTime);

                if (taskResult.ValidationResult.IsValid)
                {
                    return CreatedAtAction(nameof(Create), taskResult.Task);
                }
                else
                {
                    foreach (var error in taskResult.ValidationResult.Errors)
                    {
                        ModelState.AddModelError(error.Key, error.Value);
                    }
                }
            }

            return BadRequest(ModelState);
        }

        public async Task<IActionResult> Edit([FromBody] ToDoItem task, [FromQuery] TimeOnly? dueTime)
        {
            if (ModelState.IsValid)
            {
                var taskResult = await _tasksService.EditTaskAsync(User, task, dueTime);
                if (taskResult.ValidationResult.IsValid)
                {
                    return CreatedAtAction(nameof(Create), taskResult.Task);
                }
                else
                {
                    foreach (var error in taskResult.ValidationResult.Errors)
                    {
                        ModelState.AddModelError(error.Key, error.Value);
                    }
                }
            }

            return BadRequest(ModelState);
        }

        public async Task<IActionResult> Delete([FromBody] int id)
        {
            var taskResult = await _tasksService.DeleteTaskAsync(User, id);
            if (taskResult.IsSuccess)
            {
                return Ok(taskResult.Task);
            }

            return NotFound();
        }
    }
}

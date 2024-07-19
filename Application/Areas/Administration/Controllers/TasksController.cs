using Application.Data;
using Application.Enums;
using Application.Models;
using Application.Scripts;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace Application.Areas.Administration.Controllers
{
    [Area("Administration")]
    [Route("admin/tasks")]
    public class TasksController(ApplicationDbContext context, UserManager<User> userManager) : Controller
    {
        private readonly ApplicationDbContext _context = context;
        private readonly UserManager<User> _userManager = userManager;

        [Route("")]
        public async Task<IActionResult> Index([FromQuery] TasksFilterEnum? tasksFilter, [FromQuery] TaskStatusFilterEnum? taskStatusFilter, [FromQuery] int? taskPriorityFilter)
        {
            var tasks = await _context.ToDoItems.Where(t => t.User == _userManager.GetUserId(User)).OrderBy(t => t.DueDate).ToListAsync();

            if (tasksFilter.HasValue)
            {
                switch (tasksFilter.Value)
                {
                    case TasksFilterEnum.Status:
                        tasks = [.. tasks.OrderBy(t => t.IsCompleted)];
                        break;

                    case TasksFilterEnum.Priority:
                        tasks = [.. tasks.OrderByDescending(t => t.Priority)];
                        break;

                    case TasksFilterEnum.User:
                        tasks = [.. tasks.OrderBy(t => t.User)];
                        break;
                }
            }

            if (taskStatusFilter.HasValue)
            {
                switch (taskStatusFilter.Value)
                {
                    case TaskStatusFilterEnum.Ongoing:
                        tasks = [.. tasks.Where(t => !t.IsCompleted)];
                        break;

                    case TaskStatusFilterEnum.Completed:
                        tasks = [.. tasks.Where(t => t.IsCompleted)];
                        break;
                }
            }

            if (taskPriorityFilter is not null)
            {
                if (taskPriorityFilter != 0)
                {
                    if (taskPriorityFilter == -1)
                        tasks = [.. tasks.Where(t => t.Priority == 0)];
                    else
                        tasks = [.. tasks.Where(t => t.Priority == taskPriorityFilter)];
                }
            }

            foreach (var task in tasks)
            {
                task.PriorityNavigation = await _context.Priorities.FirstOrDefaultAsync(p => p.Id == task.Priority);
                task.UserNavigation = await _userManager.FindByIdAsync(task.User);
            }

            var priorities = SelectListModifier.InsertInitialItems(
                new SelectList(_context.Priorities.OrderBy(p => p.Level), "Id", "Level"), "Без приоритета", "Приоритет задачи", "-1");

            ViewData["Priorities"] = SelectListModifier.InsertSelectItem(priorities, 1, "Любой приоритет", "0");

            return View(tasks);
        }

        [Route("details")]
        public async Task<IActionResult> Details([FromQuery] int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var task = await _context.ToDoItems.FirstOrDefaultAsync(i => i.Id == id);
            if (task == null)
            {
                return NotFound();
            }

            task.PriorityNavigation = await _context.Priorities.FirstOrDefaultAsync(p => p.Id == task.Priority);
            task.UserNavigation = await _userManager.FindByIdAsync(task.User);
            if (task.DueDate.HasValue)
            {
                ViewData["DueTime"] = TimeOnly.FromDateTime(task.DueDate.Value);
            }

            return View(task);
        }

        [Route("create")]
        public IActionResult Create()
        {
            ViewData["Priorities"] = SelectListModifier.InsertInitialItems(
                new SelectList(_context.Priorities.OrderBy(p => p.Level), "Id", "Level"), "Без приоритета", "Выберите приоритет", "0");

            ViewData["Users"] = SelectListModifier.InsertPickItem(
                new SelectList(_userManager.Users.OrderBy(u => u.UserName), "Id", "UserName"), "Выберите пользователя");

            return View();
        }

        [Route("create")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([FromForm] TimeOnly? dueTime, [Bind("Id,Title,Description,IsCompleted,DueDate,Priority,User")] ToDoItem task)
        {
            if (string.IsNullOrEmpty(task.Title))
            {
                ModelState.AddModelError("Title", "Обязательно для заполнения");
            }

            if (await _userManager.FindByIdAsync(task.User) is null)
            {
                ModelState.AddModelError("User", "Обязательно для заполнения");
            }

            if (ModelState.IsValid)
            {
                if (task.DueDate.HasValue)
                {
                    if (dueTime.HasValue)
                        task.DueDate = new DateTime(DateOnly.FromDateTime(task.DueDate.Value), dueTime.Value, DateTimeKind.Utc);
                    else
                        task.DueDate = new DateTime(DateOnly.FromDateTime(task.DueDate.Value), new TimeOnly(), DateTimeKind.Utc);
                }
                else if (dueTime.HasValue)
                    task.DueDate = new DateTime(DateOnly.FromDateTime(DateTime.Now), dueTime.Value, DateTimeKind.Utc);

                _context.Add(task);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            ViewData["Priorities"] = SelectListModifier.InsertInitialItems(
                new SelectList(_context.Priorities.OrderBy(p => p.Level), "Id", "Level"), "Без приоритета", "Выберите приоритет", "0");

            ViewData["Users"] = SelectListModifier.InsertPickItem(
                new SelectList(_userManager.Users.OrderBy(u => u.UserName), "Id", "UserName"), "Выберите пользователя");

            return View(task);
        }

        [Route("edit")]
        public async Task<IActionResult> Edit([FromQuery] int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var item = await _context.ToDoItems.FirstOrDefaultAsync(i => i.Id == id);
            if (item == null)
            {
                return NotFound();
            }

            if (item.DueDate.HasValue)
            {
                ViewData["DueTime"] = TimeOnly.FromDateTime(item.DueDate.Value);
            }

            ViewData["Priorities"] = SelectListModifier.InsertInitialItems(
                new SelectList(_context.Priorities.OrderBy(p => p.Level), "Id", "Level"), "Без приоритета", "Выберите приоритет", "0");

            ViewData["Users"] = SelectListModifier.InsertPickItem(
                new SelectList(_userManager.Users.OrderBy(u => u.UserName), "Id", "UserName"), "Выберите пользователя");

            ViewData["PreviousPage"] = Request.Headers.Referer.ToString();

            return View(item);
        }

        [Route("edit")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit([FromForm] TimeOnly? dueTime, [Bind("Id,Title,Description,IsCompleted,DueDate,Priority,User")] ToDoItem task)
        {
            if (string.IsNullOrEmpty(task.Title))
            {
                ModelState.AddModelError("Title", "Обязательно для заполнения");
            }

            if (await _userManager.FindByIdAsync(task.User) is null)
            {
                ModelState.AddModelError("User", "Обязательно для заполнения");
            }

            if (ModelState.IsValid)
            {
                if (task.DueDate.HasValue)
                {
                    if (dueTime.HasValue)
                        task.DueDate = new DateTime(DateOnly.FromDateTime(task.DueDate.Value), dueTime.Value, DateTimeKind.Utc);
                    else
                        task.DueDate = new DateTime(DateOnly.FromDateTime(task.DueDate.Value), new TimeOnly(), DateTimeKind.Utc);
                }
                else if (dueTime.HasValue)
                    task.DueDate = new DateTime(DateOnly.FromDateTime(DateTime.Now), dueTime.Value, DateTimeKind.Utc);

                try
                {
                    _context.Update(task);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ToDoItemExists(task.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }

            ViewData["Priorities"] = SelectListModifier.InsertInitialItems(
                new SelectList(_context.Priorities.OrderBy(p => p.Level), "Id", "Level"), "Без приоритета", "Выберите приоритет", "0");

            ViewData["Users"] = SelectListModifier.InsertPickItem(
                new SelectList(_userManager.Users.OrderBy(u => u.UserName), "Id", "UserName"), "Выберите пользователя");

            return View(task);
        }

        [Route("delete")]
        public async Task<IActionResult> Delete([FromQuery] int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var task = await _context.ToDoItems.FirstOrDefaultAsync(m => m.Id == id);
            if (task == null)
            {
                return NotFound();
            }

            ViewData["PreviousPage"] = Request.Headers.Referer.ToString();

            return View(task);
        }

        [Route("delete")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var task = await _context.ToDoItems.FirstOrDefaultAsync(i => i.Id == id);

            if (task is not null)
            {
                _context.ToDoItems.Remove(task);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ToDoItemExists(int id)
        {
            return _context.ToDoItems.Any(e => e.Id == id);
        }
    }
}

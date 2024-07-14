using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Application.Data;
using Application.Models;
using Application.Scripts;

namespace Application.Controllers
{
    [Route("tasks")]
    public class TasksController(ApplicationDbContext context) : Controller
    {
        private readonly ApplicationDbContext _context = context;

        [Route("")]
        public async Task<IActionResult> Index()
        {
            var tasks = await _context.ToDoItems.ToListAsync();

            foreach (var task in tasks)
            {
                task.PriorityNavigation = await _context.Priorities.FirstOrDefaultAsync(p => p.Id == task.Priority);
            }

            return View(tasks);
        }

        [Route("details")]
        public async Task<IActionResult> Details([FromQuery] int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var toDoItem = await _context.ToDoItems
                .FirstOrDefaultAsync(m => m.Id == id);
            if (toDoItem == null)
            {
                return NotFound();
            }

            if (toDoItem.DueDate.HasValue)
            {
                ViewData["DueTime"] = TimeOnly.FromDateTime(toDoItem.DueDate.Value);
            }

            ViewData["Priorities"] = SelectListModifier.InsertSelectItems(
                new SelectList(_context.Priorities.OrderBy(p => p.Level), "Id", "Level"), "Без приоритета", "Выберите приоритет");

            return View(toDoItem);
        }

        [Route("create")]
        public IActionResult Create()
        {
            ViewData["Priorities"] = SelectListModifier.InsertSelectItems(
                new SelectList(_context.Priorities.OrderBy(p => p.Level), "Id", "Level"), "Без приоритета", "Выберите приоритет");

            return View();
        }

        [Route("create")]
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([FromForm] TimeOnly? dueTime, [Bind("Id,Title,Description,IsCompleted,DueDate,Priority,User")] ToDoItem toDoItem)
        {
            if (ModelState.IsValid)
            {
                if (toDoItem.DueDate.HasValue)
                {
                    if (dueTime.HasValue)
                        toDoItem.DueDate = new DateTime(DateOnly.FromDateTime(toDoItem.DueDate.Value), dueTime.Value, DateTimeKind.Utc);
                    else
                        toDoItem.DueDate = new DateTime(DateOnly.FromDateTime(toDoItem.DueDate.Value), new TimeOnly(), DateTimeKind.Utc);
                }
                else if (dueTime.HasValue)
                    toDoItem.DueDate = new DateTime(DateOnly.FromDateTime(DateTime.Now), dueTime.Value, DateTimeKind.Utc);

                _context.Add(toDoItem);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            ViewData["Priorities"] = new SelectList(_context.Priorities.OrderBy(p => p.Level), "Id", "Level");

            ViewData["Users"] = new SelectList(_context.Users, "Id", "Name");

            return View(toDoItem);
        }

        [Route("edit")]
        public async Task<IActionResult> Edit([FromQuery] int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var toDoItem = await _context.ToDoItems.FindAsync(id);
            if (toDoItem == null)
            {
                return NotFound();
            }

            if (toDoItem.DueDate.HasValue)
            {
                ViewData["DueTime"] = TimeOnly.FromDateTime(toDoItem.DueDate.Value);
            }

            ViewData["Priorities"] = SelectListModifier.InsertSelectItems(
                new SelectList(_context.Priorities.OrderBy(p => p.Level), "Id", "Level"), "Без приоритета", "Выберите приоритет");

            ViewData["PreviousPage"] = Request.Headers.Referer.ToString();

            return View(toDoItem);
        }

        [Route("edit")]
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit([FromForm] int id, [FromForm] TimeOnly? dueTime, [Bind("Id,Title,Description,IsCompleted,DueDate,Priority,User")] ToDoItem toDoItem)
        {
            if (id != toDoItem.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                if (toDoItem.DueDate.HasValue)
                {
                    if (dueTime.HasValue)
                        toDoItem.DueDate = new DateTime(DateOnly.FromDateTime(toDoItem.DueDate.Value), dueTime.Value, DateTimeKind.Utc);
                    else
                        toDoItem.DueDate = new DateTime(DateOnly.FromDateTime(toDoItem.DueDate.Value), new TimeOnly(), DateTimeKind.Utc);
                }
                else if (dueTime.HasValue)
                    toDoItem.DueDate = new DateTime(DateOnly.FromDateTime(DateTime.Now), dueTime.Value, DateTimeKind.Utc);

                try
                {
                    _context.Update(toDoItem);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ToDoItemExists(toDoItem.Id))
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
            return View(toDoItem);
        }

        [Route("delete")]
        public async Task<IActionResult> Delete([FromQuery] int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var toDoItem = await _context.ToDoItems
                .FirstOrDefaultAsync(m => m.Id == id);
            if (toDoItem == null)
            {
                return NotFound();
            }

            ViewData["PreviousPage"] = Request.Headers.Referer.ToString();

            return View(toDoItem);
        }

        [Route("delete")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var toDoItem = await _context.ToDoItems.FindAsync(id);
            if (toDoItem != null)
            {
                _context.ToDoItems.Remove(toDoItem);
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

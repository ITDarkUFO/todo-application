using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Application.Data;
using Application.Models;

namespace Application.Areas.Administration.Controllers
{
    [Area("Administration")]
    [Route("admin/priorities")]
    public class PrioritiesController(ApplicationDbContext context) : Controller
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

            return View(priorities);
        }

        [Route("create")]
        public IActionResult Create()
        {
            return View();
        }

        [Route("create")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Level")] Priority priority)
        {
            if (await _context.Priorities.AsNoTracking()
                .AnyAsync(p => p.Level == priority.Level))
                ModelState.AddModelError("Level", $"Уровень приоритета {priority.Level} уже существует.");

            if (priority.Level < 0)
                ModelState.AddModelError("Level", "Уровень приоритета не может быть меньше нуля.");

            if (ModelState.IsValid)
            {
                _context.Add(priority);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            return View(priority);
        }

        [Route("edit")]
        public async Task<IActionResult> Edit([FromQuery] int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var priority = await _context.Priorities.FindAsync(id);
            if (priority == null)
            {
                return NotFound();
            }

            ViewData["PreviousPage"] = Request.Headers.Referer.ToString();

            return View(priority);
        }

        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Route("edit")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit([FromForm] int id, [Bind("Id,Level")] Priority priority)
        {
            if (id != priority.Id)
                return NotFound();

            if (await _context.Priorities.AsNoTracking()
                .AnyAsync(p => p.Level == priority.Level && p.Id != priority.Id))
                ModelState.AddModelError("Level", $"Level {priority.Level} уже существует.");

            if (priority.Level < 0)
                ModelState.AddModelError("Level", "Уровень приоритета не может быть меньше нуля.");

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(priority);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PriorityExists(priority.Id))
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

            return View(priority);
        }

        [Route("details")]
        public async Task<IActionResult> Details([FromQuery] int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var priority = await _context.Priorities.FindAsync(id);
            if (priority == null)
            {
                return NotFound();
            }

            priority.ToDoItems = await _context.ToDoItems.Where(i => i.Priority == priority.Id).ToListAsync();

            return View(priority);
        }

        [Route("delete")]
        public async Task<IActionResult> Delete([FromQuery] int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var priority = await _context.Priorities
                .FirstOrDefaultAsync(m => m.Id == id);
            if (priority == null)
            {
                return NotFound();
            }

            ViewData["PreviousPage"] = Request.Headers.Referer.ToString();

            return View(priority);
        }

        [Route("delete")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed([FromForm] int id)
        {
            var priority = await _context.Priorities.FindAsync(id);
            if (priority != null)
            {
                _context.Priorities.Remove(priority);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PriorityExists(int id)
        {
            return _context.Priorities.Any(e => e.Id == id);
        }
    }
}

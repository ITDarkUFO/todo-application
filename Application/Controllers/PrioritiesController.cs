using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Application.Data;
using Application.Models;
using Npgsql;

namespace Application.Controllers
{
    public class PrioritiesController(ApplicationDbContext context) : Controller
    {
        private readonly ApplicationDbContext _context = context;

        // GET: Priorities
        public async Task<IActionResult> Index()
        {
            return View(await _context.Priorities.OrderBy(p => p.Level).ToListAsync());
        }

        // GET: Priorities/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Priorities/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Level")] Priority priority)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    _context.Add(priority);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateException ex)
                {
                    var innerException = ex.InnerException;

                    if (innerException is PostgresException pgEx &&
                        pgEx.SqlState == PostgresErrorCodes.UniqueViolation)
                    {
                        switch (pgEx.SqlState)
                        {
                            case PostgresErrorCodes.UniqueViolation:
                                ModelState.AddModelError("Level", "Level должен быть уникальным.");
                                return BadRequest(ModelState);
                        }
                    }
                }
            }

            return View(priority);
        }

        // GET: Priorities/Edit/5
        public async Task<IActionResult> Edit(int? id)
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
            return View(priority);
        }

        // POST: Priorities/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Level")] Priority priority)
        {
            if (id != priority.Id)
            {
                return NotFound();
            }

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
                catch (DbUpdateException ex)
                {
                    var innerException = ex.InnerException;

                    if (innerException is PostgresException pgEx &&
                        pgEx.SqlState == PostgresErrorCodes.UniqueViolation)
                    {
                        switch (pgEx.SqlState)
                        {
                            case PostgresErrorCodes.UniqueViolation:
                                ModelState.AddModelError("Level", "Level должен быть уникальным.");
                                return BadRequest(ModelState);
                        }
                    }
                }

                return RedirectToAction(nameof(Index));
            }

            return View(priority);
        }

        // GET: Priorities/Delete/5
        public async Task<IActionResult> Delete(int? id)
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

            return View(priority);
        }

        // POST: Priorities/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
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

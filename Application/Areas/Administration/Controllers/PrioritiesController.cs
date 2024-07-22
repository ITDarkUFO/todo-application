using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Application.Data;
using Application.Models;
using Application.Interfaces;
using Application.Utilities;
using Microsoft.AspNetCore.Authorization;

namespace Application.Areas.Administration.Controllers
{
    [Authorize(Roles = "admin")]
    [Area("Administration")]
    [Route("admin/priorities")]
    public class PrioritiesController(IPrioritiesService priorityService) : Controller
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

            return View(priorities);
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

            return View(priorityResult.Priority);
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
            if (ModelState.IsValid)
            {
                var priorityResult = await _priorityService.CreatePriorityAsync(priority);

                if (priorityResult.ValidationResult.IsValid)
                {
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    foreach (var error in priorityResult.ValidationResult.Errors)
                    {
                        ModelState.AddModelError(error.Key, error.Value);
                    }
                }
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

            var priorityResult = await _priorityService.GetPriorityByIdAsync(id.Value);
            if (!priorityResult.IsSuccess)
            {
                return NotFound();
            }

            ViewData["PreviousPage"] = Request.Headers.Referer.ToString();

            return View(priorityResult.Priority);
        }

        [Route("edit")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit([Bind("Id,Level")] Priority priority)
        {
            if (ModelState.IsValid)
            {
                var priorityResult = await _priorityService.EditPriorityAsync(priority);

                if (priorityResult.ValidationResult.IsValid)
                {
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    foreach (var error in priorityResult.ValidationResult.Errors)
                    {
                        ModelState.AddModelError(error.Key, error.Value);
                    }
                }
            }

            return View(priority);
        }

        [Route("delete")]
        public async Task<IActionResult> Delete([FromQuery] int? id)
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

            ViewData["PreviousPage"] = Request.Headers.Referer.ToString();

            return View(priorityResult.Priority);
        }

        [Route("delete")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed([FromForm] int id)
        {
            var priorityResult = await _priorityService.DeletePriorityAsync(id);
            return RedirectToAction(nameof(Index));
        }
    }
}

using System;
using System.ComponentModel;
using Application.Data;
using Application.Enums;
using Application.Interfaces;
using Application.Models;
using Application.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace Application.Controllers
{
    [Route("tasks")]
    public class TasksController(ITasksService tasksService, IPrioritiesService priorityService) : Controller
    {
        private readonly ITasksService _tasksService = tasksService;
        private readonly IPrioritiesService _priorityService = priorityService;

        [Route("")]
        public async Task<IActionResult> Index([FromQuery] TasksSorterEnum? tasksSorter, [FromQuery] TaskStatusFilterEnum? taskStatusFilter, [FromQuery] int? taskPriorityFilter)
        {
            var tasks = await _tasksService.GetUserTaskList(User, tasksSorter, taskStatusFilter, taskPriorityFilter);

            var prioritiesList = (await _priorityService.GetPriorityListAsync()).OrderBy(p => p.Level);
            var priorities = SelectListModifier.InsertInitialItems(
                new SelectList(prioritiesList, "Id", "Level"), "Без приоритета", "Приоритет задачи", "-1");
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

            var taskResult = await _tasksService.GetTaskByIdAsync(User, id.Value);
            if (!taskResult.IsSuccess)
            {
                return NotFound();
            }

            if (taskResult.Task!.DueDate.HasValue)
            {
                ViewData["DueTime"] = TimeOnly.FromDateTime(taskResult.Task.DueDate.Value);
            }

            var prioritiesList = (await _priorityService.GetPriorityListAsync()).OrderBy(p => p.Level);
            ViewData["Priorities"] = SelectListModifier.InsertInitialItems(
                new SelectList(prioritiesList, "Id", "Level"), "Без приоритета", "Выберите приоритет", "0");

            return View(taskResult.Task);
        }

        [Route("create")]
        public async Task<IActionResult> Create()
        {
            var prioritiesList = (await _priorityService.GetPriorityListAsync()).OrderBy(p => p.Level);
            ViewData["Priorities"] = SelectListModifier.InsertInitialItems(
                new SelectList(prioritiesList, "Id", "Level"), "Без приоритета", "Выберите приоритет", "0");

            return View();
        }

        [Route("create")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([FromForm] TimeOnly? dueTime, [Bind("Id,Title,Description,IsCompleted,DueDate,Priority,User")] ToDoItem task)
        {
            if (ModelState.IsValid)
            {
                var taskResult = await _tasksService.CreateTaskAsync(User, task, dueTime);

                if (taskResult.ValidationResult.IsValid)
                {
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    foreach (var error in taskResult.ValidationResult.Errors)
                    {
                        ModelState.AddModelError(error.Key, error.Value);
                    }
                }
            }

            var prioritiesList = (await _priorityService.GetPriorityListAsync()).OrderBy(p => p.Level);
            ViewData["Priorities"] = SelectListModifier.InsertInitialItems(
                new SelectList(prioritiesList, "Id", "Level"), "Без приоритета", "Выберите приоритет", "0");

            return View(task);
        }

        [Route("edit")]
        public async Task<IActionResult> Edit([FromQuery] int? id)
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

            if (taskResult.Task!.DueDate.HasValue)
            {
                ViewData["DueTime"] = TimeOnly.FromDateTime(taskResult.Task.DueDate.Value);
            }

            var prioritiesList = (await _priorityService.GetPriorityListAsync()).OrderBy(p => p.Level);
            ViewData["Priorities"] = SelectListModifier.InsertInitialItems(
                new SelectList(prioritiesList, "Id", "Level"), "Без приоритета", "Выберите приоритет", "0");

            ViewData["PreviousPage"] = Request.Headers.Referer.ToString();

            return View(taskResult.Task);
        }

        [Route("edit")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit([FromForm] TimeOnly? dueTime, [Bind("Id,Title,Description,IsCompleted,DueDate,Priority,User")] ToDoItem task)
        {
            if (ModelState.IsValid)
            {
                var taskResult = await _tasksService.EditTaskAsync(User, task, dueTime);
                if (taskResult.ValidationResult.IsValid)
                {
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    foreach (var error in taskResult.ValidationResult.Errors)
                    {
                        ModelState.AddModelError(error.Key, error.Value);
                    }
                }
            }

            var prioritiesList = (await _priorityService.GetPriorityListAsync()).OrderBy(p => p.Level);
            ViewData["Priorities"] = SelectListModifier.InsertInitialItems(
                new SelectList(prioritiesList, "Id", "Level"), "Без приоритета", "Выберите приоритет", "0");

            return View(task);
        }

        [Route("delete")]
        public async Task<IActionResult> Delete([FromQuery] int? id)
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

            ViewData["PreviousPage"] = Request.Headers.Referer.ToString();

            return View(taskResult.Task);
        }

        [Route("delete")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var taskResult = await _tasksService.DeleteTaskAsync(User, id);
            return RedirectToAction(nameof(Index));
        }
    }
}

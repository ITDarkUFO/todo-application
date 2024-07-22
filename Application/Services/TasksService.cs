using Application.Data;
using Application.Enums;
using Application.Interfaces;
using Application.Models;
using Application.Utilities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Security.Claims;

namespace Application.Services
{
    public class TasksService(ApplicationDbContext context, UserManager<User> userManager) : ITasksService
    {
        private readonly ApplicationDbContext _context = context;
        private readonly UserManager<User> _userManager = userManager;

        public async Task<List<ToDoItem>?> GetUserTaskList(ClaimsPrincipal principal,
            TasksSorterEnum? tasksSorter = null, TaskStatusFilterEnum? taskStatusFilter = null, int? taskPriorityFilter = null)
        {
            List<ToDoItem> tasks;

            User? user = await _userManager.GetUserAsync(principal);
            if (user is not null)
                tasks = await _context.ToDoItems.Where(t => t.User == user.Id).OrderBy(t => t.DueDate).ToListAsync();
            else
                return null;

            tasks = FilterTasks(tasks, tasksSorter, taskStatusFilter, taskPriorityFilter);

            foreach (var task in tasks)
            {
                task.PriorityNavigation = await _context.Priorities.FirstOrDefaultAsync(p => p.Id == task.Priority);
                task.UserNavigation = await _userManager.FindByIdAsync(task.User);
            }

            return tasks;
        }

        public async Task<List<ToDoItem>?> GetTaskListAsync(ClaimsPrincipal principal,
            TasksSorterEnum? tasksSorter = null, TaskStatusFilterEnum? taskStatusFilter = null, int? taskPriorityFilter = null, string? taskUserFilter = null)
        {
            List<ToDoItem> tasks;

            if (principal.IsInRole("admin"))
            {
                tasks = await _context.ToDoItems.OrderBy(t => t.DueDate).ToListAsync();

                if (!string.IsNullOrEmpty(taskUserFilter))
                {
                    var filteredUser = await _userManager.FindByIdAsync(taskUserFilter);

                    if (filteredUser is not null)
                        tasks = tasks.Where(t => t.User == filteredUser.Id).ToList();
                    else
                        return null;
                }
            }
            else
            {
                User? user = await _userManager.GetUserAsync(principal);
                if (user is not null)
                    tasks = await _context.ToDoItems.Where(t => t.User == user.Id).OrderBy(t => t.DueDate).ToListAsync();
                else
                    return null;
            }

            tasks = FilterTasks(tasks, tasksSorter, taskStatusFilter, taskPriorityFilter);

            foreach (var task in tasks)
            {
                task.PriorityNavigation = await _context.Priorities.FirstOrDefaultAsync(p => p.Id == task.Priority);
                task.UserNavigation = await _userManager.FindByIdAsync(task.User);
            }

            return tasks;
        }

        public async Task<TaskResult> GetTaskByIdAsync(ClaimsPrincipal principal, int id)
        {
            var task = await _context.ToDoItems.FirstOrDefaultAsync(m => m.Id == id);
            if (task is null)
            {
                return new();
            }

            task.PriorityNavigation = await _context.Priorities.FirstOrDefaultAsync(p => p.Id == task.Priority);
            task.UserNavigation = await _userManager.FindByIdAsync(task.User);

            User? user = await _userManager.GetUserAsync(principal);

            if (!principal.IsInRole("admin") && user is not null && task.User != user.Id)
            {
                return new();
            }

            return new() { Task = task };
        }

        public async Task<TaskResult> CreateTaskAsync(ClaimsPrincipal principal, ToDoItem task, TimeOnly? dueTime = null)
        {
            var validationResult = new ValidationResult();

            User? user = await _userManager.GetUserAsync(principal);
            if (!principal.IsInRole("admin") && user is not null && task.User != user.Id)
            {
                validationResult.AddError("", "Произошла неизвестная ошибка. Пожалуйста, попробуйте снова");
                return new() { Task = task, ValidationResult = validationResult };
            }

            if (string.IsNullOrEmpty(task.Title))
            {
                validationResult.AddError("Title", "Обязательно для заполнения");
            }

            if (await _userManager.FindByIdAsync(task.User) is null)
            {
                validationResult.AddError("User", "Обязательно для заполнения");
            }

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

            return new() { Task = task, ValidationResult = validationResult };
        }

        public async Task<TaskResult> EditTaskAsync(ClaimsPrincipal principal, ToDoItem task, TimeOnly? dueTime = null)
        {
            var validationResult = new ValidationResult();

            User? user = await _userManager.GetUserAsync(principal);
            if (!principal.IsInRole("admin") && user is not null && task.User != user.Id)
            {
                validationResult.AddError("", "Произошла неизвестная ошибка. Пожалуйста, попробуйте снова");
                return new() { Task = task, ValidationResult = validationResult };
            }

            if (!await ToDoItemExists(task.Id))
            {
                validationResult.AddError("Id", "Некорректный ID задачи");
            }

            if (string.IsNullOrEmpty(task.Title))
            {
                validationResult.AddError("Title", "Обязательно для заполнения");
            }

            if (await _userManager.FindByIdAsync(task.User) is null)
            {
                validationResult.AddError("User", "Обязательно для заполнения");
            }

            if (task.DueDate.HasValue)
            {
                if (dueTime.HasValue)
                    task.DueDate = new DateTime(DateOnly.FromDateTime(task.DueDate.Value), dueTime.Value, DateTimeKind.Utc);
                else
                    task.DueDate = new DateTime(DateOnly.FromDateTime(task.DueDate.Value), new TimeOnly(), DateTimeKind.Utc);
            }
            else if (dueTime.HasValue)
                task.DueDate = new DateTime(DateOnly.FromDateTime(DateTime.Now), dueTime.Value, DateTimeKind.Utc);

            if (validationResult.IsValid)
            {
                try
                {
                    _context.Update(task);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!await ToDoItemExists(task.Id))
                    {
                        validationResult.AddError("Id", "Некорректный ID задачи");
                    }
                    else
                    {
                        validationResult.AddError("", "Произошла ошибка при попытке обновить данные. Пожалуйста, попробуйте снова");
                    }
                }
            }

            return new() { Task = task, ValidationResult = validationResult };
        }
        public async Task<TaskResult> DeleteTaskAsync(ClaimsPrincipal principal, int id)
        {
            var validationResult = new ValidationResult();

            ToDoItem? task = await _context.ToDoItems.FirstOrDefaultAsync(t => t.Id == id);
            if (task is null)
            {
                return new();
            }

            User? user = await _userManager.GetUserAsync(principal);
            if (!principal.IsInRole("admin") && user is not null && task.User != user.Id)
            {
                validationResult.AddError("", "Произошла неизвестная ошибка. Пожалуйста, попробуйте снова");
            }

            _context.ToDoItems.Remove(task);

            await _context.SaveChangesAsync();
            return new() { Task = task, ValidationResult = validationResult };
        }

        private static List<ToDoItem> FilterTasks(List<ToDoItem> tasks, TasksSorterEnum? tasksSorter, TaskStatusFilterEnum? taskStatusFilter, int? taskPriorityFilter)
        {
            if (tasksSorter.HasValue)
            {
                switch (tasksSorter.Value)
                {
                    case TasksSorterEnum.Status:
                        tasks = [.. tasks.OrderBy(t => t.IsCompleted)];
                        break;

                    case TasksSorterEnum.Priority:
                        tasks = [.. tasks.OrderByDescending(t => t.Priority)];
                        break;

                    case TasksSorterEnum.User:
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

            if (taskPriorityFilter.HasValue)
            {
                if (taskPriorityFilter != 0)
                {
                    if (taskPriorityFilter == -1)
                        tasks = [.. tasks.Where(t => t.Priority == 0)];
                    else
                        tasks = [.. tasks.Where(t => t.Priority == taskPriorityFilter)];
                }
            }

            return tasks;
        }

        private async Task<bool> ToDoItemExists(int id)
        {
            return await _context.ToDoItems.AnyAsync(e => e.Id == id);
        }
    }
}

using Application.Interfaces;
using Application.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace Application.Controllers
{
    [AllowAnonymous]
    public class HomeController(ILogger<HomeController> logger, SignInManager<User> signInManager, ITasksService tasksService) : Controller
    {
        private readonly ILogger<HomeController> _logger = logger;
        private readonly SignInManager<User> _signInManager = signInManager;
        private readonly ITasksService _tasksService = tasksService;

        public async Task<IActionResult> Index()
        {
            if (_signInManager.IsSignedIn(User))
            {
                var tasks = await _tasksService.GetUserTaskList(User);
                if (tasks is not null)
                {
                    tasks = tasks.Where(t => t.DueDate is not null
                    && DateOnly.FromDateTime(t.DueDate.Value) == DateOnly.FromDateTime(DateTime.UtcNow)).ToList();
                    return View(tasks);
                }
            }
            return View();
        }
    }
}

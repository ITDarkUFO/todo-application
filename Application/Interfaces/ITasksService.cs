using Application.Enums;
using Application.Models;
using Application.Utilities;
using System.Security.Claims;

namespace Application.Interfaces
{
    public interface ITasksService
    {
        Task<List<ToDoItem>?> GetUserTaskList(ClaimsPrincipal principal,
            TasksSorterEnum? tasksSorter = null, TaskStatusFilterEnum? taskStatusFilter = null, int? taskPriorityFilter = null);

        Task<List<ToDoItem>?> GetTaskListAsync(ClaimsPrincipal principal,
            TasksSorterEnum? tasksSorter = null, TaskStatusFilterEnum? taskStatusFilter = null, int? taskPriorityFilter = null, string? taskUserFilter = null);

        Task<TaskResult> GetTaskByIdAsync(ClaimsPrincipal principal, int id);

        Task<TaskResult> CreateTaskAsync(ClaimsPrincipal principal, ToDoItem task, TimeOnly? dueTime = null);

        Task<TaskResult> EditTaskAsync(ClaimsPrincipal principal, ToDoItem task, TimeOnly? dueTime = null);

        Task<TaskResult> DeleteTaskAsync(ClaimsPrincipal principal, int id);
    }
}

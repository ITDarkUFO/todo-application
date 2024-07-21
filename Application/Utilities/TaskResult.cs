using Application.Models;

namespace Application.Utilities
{
    public class TaskResult
    {
        public ValidationResult ValidationResult { get; set; } = new();

        public ToDoItem? Task { get; set; }

        public bool IsSuccess => ValidationResult.IsValid && Task is not null;
    }
}

using Application.Models;

namespace Application.Utilities
{
    public class PriorityResult
    {
        public ValidationResult ValidationResult { get; set; } = new();

        public Priority? Priority { get; set; }

        public bool IsSuccess => ValidationResult.IsValid && Priority is not null;
    }
}

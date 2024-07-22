using Application.Models;

namespace Application.Utilities
{
    public class UserResult
    {
        public ValidationResult ValidationResult { get; set; } = new();

        public User? User { get; set; }

        public bool IsSuccess => ValidationResult.IsValid && User is not null;
    }
}

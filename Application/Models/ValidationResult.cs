namespace Application.Models
{
    public class ValidationResult
    {
        public Dictionary<string, string> Errors { get; } = [];

        public bool IsValid => Errors.Count == 0;

        public void AddError(string key, string message)
        {
            Errors.TryAdd(key, message);
        }
    }
}

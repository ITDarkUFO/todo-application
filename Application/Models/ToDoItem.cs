namespace Application.Models
{
    public class ToDoItem
    {
        public int Id { get; set; }

        public string Title { get; set; } = default!;

        public string? Description { get; set; } = default!;

        public bool IsCompleted { get; set; }

        public DateTime? DueDate { get; set; }

        public Priority Priority { get; set; } = default!;

        public User User { get; set; } = default!;
    }
}

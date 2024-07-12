namespace Application.Models
{
    public class ToDoItem
    {
        public int Id { get; set; }

        public string Title { get; set; } = null!;

        public string? Description { get; set; } = null!;

        public bool IsCompleted { get; set; }

        public DateTime? DueDate { get; set; }

        public int Priority { get; set; }

        public int User { get; set; }

        public virtual Priority? PriorityNavigation { get; set; }

        public virtual User? UserNavigation { get; set; }

    }
}

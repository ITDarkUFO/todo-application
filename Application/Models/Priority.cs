namespace Application.Models
{
    public class Priority
    {
        public int Id { get; set; }
        
        public int Level { get; set; }

        public List<ToDoItem>? ToDoItems { get; set; } = default;
    }
}

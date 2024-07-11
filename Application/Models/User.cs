namespace Application.Models
{
    public class User
    {
        public int Id { get; set; }

        public string Name { get; set; } = default!;

        public List<ToDoItem>? ToDoItems { get; set; } = default;
    }
}

using Microsoft.AspNetCore.Identity;

namespace Application.Models
{
    public class User : IdentityUser
    {
        public List<ToDoItem>? ToDoItems { get; set; } = default;
    }
}

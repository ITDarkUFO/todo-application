using System.ComponentModel.DataAnnotations;

namespace Application.Dtos
{
    public class UserEditDto
    {
        [Required]
        public string Id { get; set; } = null!;

        [Required(ErrorMessage = "Обязательно для заполнения")]
        public string UserName { get; set; } = null!;

        [Required(ErrorMessage = "Обязательно для заполнения")]
        [EmailAddress(ErrorMessage = "Введите корректный адрес электронной почты")]
        public string Email { get; set; } = null!;

        public string? CurrentPassword { get; set; } = null;

        public string? Password { get; set; } = null!;

        public string? ConfirmPassword { get; set; } = null!;
    }
}

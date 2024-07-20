using System.ComponentModel.DataAnnotations;

namespace Application.Dtos
{
    public class UserRegistrationDto
    {
        [Required(ErrorMessage = "Обязательно для заполнения")]
        public string UserName { get; set; } = null!;

        [Required(ErrorMessage = "Обязательно для заполнения")]
        [EmailAddress(ErrorMessage = "Введите корректный адрес электронной почты")]
        public string Email { get; set; } = null!;

        [Required(ErrorMessage = "Обязательно для заполнения")]
        public string Password { get; set; } = null!;

        [Required(ErrorMessage = "Обязательно для заполнения")]
        public string ConfirmPassword { get; set; } = null!;
    }
}

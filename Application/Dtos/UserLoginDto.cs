﻿using System.ComponentModel.DataAnnotations;

namespace Application.Dtos
{
    public class UserLoginDto
    {
        [Required(ErrorMessage = "Обязательно для заполнения")]
        public string UserName { get; set; } = null!;

        [Required(ErrorMessage = "Обязательно для заполнения")]
        public string Password { get; set; } = null!;

        public bool RememberMe { get; set; } = false;
    }
}

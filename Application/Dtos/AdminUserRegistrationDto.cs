namespace Application.Dtos
{
    public class AdminUserRegistrationDto : UserRegistrationDto
    {
        public bool IsSuperUser { get; set; }
    }
}

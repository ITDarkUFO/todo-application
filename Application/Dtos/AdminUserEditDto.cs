namespace Application.Dtos
{
    public class AdminUserEditDto : UserEditDto
    {
        public bool IsSuperUser { get; set; } = false;
    }
}

using Application.Dtos;
using Application.Models;
using Application.Utilities;
using System.Security.Claims;

namespace Application.Interfaces
{
    public interface IUsersService
    {
        Task<List<User>?> GetUsersListAsync();

        Task<UserResult> GetUserAsync(ClaimsPrincipal principal);

        Task<UserResult> GetUserByIdAsync(ClaimsPrincipal principal, string id);

        Task<UserResult> EditUserAsync(UserEditDto request);

        Task<UserResult> DeleteUserAsync(string id);

        Task<UserResult> RegisterUserAsync(UserRegistrationDto request);

        Task<UserResult> LoginUserAsync(UserLoginDto request);

        Task LogoutAsync();

        Task<bool> IsUserInRole(User user, string role);

        Task<bool> IsUserAdmin(User user) => IsUserInRole(user, "admin");
    }
}

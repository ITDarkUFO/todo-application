using Application.Data;
using Application.Dtos;
using Application.Interfaces;
using Application.Models;
using Application.Utilities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.Text.RegularExpressions;

namespace Application.Services
{
    public partial class UsersService(ApplicationDbContext context, UserManager<User> userManager, SignInManager<User> signInManager) : IUsersService
    {
        [GeneratedRegex("^[a-zA-Z0-9]+(?:\\.[a-zA-Z0-9]+)*@[a-zA-Z0-9]+(?:\\.[a-zA-Z0-9]+)*$")]
        private static partial Regex emailRegex();

        private readonly ApplicationDbContext _context = context;
        private readonly UserManager<User> _userManager = userManager;
        private readonly SignInManager<User> _signInManager = signInManager;
        private readonly PasswordHasher<User> _passwordHasher = new();

        public async Task<List<User>?> GetUsersListAsync()
        {
            var users = await _userManager.Users.ToListAsync();

            foreach (var user in users)
            {
                user.ToDoItems = await _context.ToDoItems.Where(i => i.User == user.Id).ToListAsync();
            }

            return users;
        }

        public async Task<UserResult> GetUserAsync(ClaimsPrincipal principal)
        {
            var user = await _userManager.GetUserAsync(principal);
            if (user is null)
            {
                return new();
            }

            user.ToDoItems = await _context.ToDoItems.Where(i => i.User == user.Id).ToListAsync();

            foreach (var item in user.ToDoItems)
            {
                item.PriorityNavigation = await _context.Priorities.FirstOrDefaultAsync(p => p.Id == item.Priority);
            }
            
            return new() { User = user };
        }

        public async Task<UserResult> GetUserByIdAsync(ClaimsPrincipal principal, string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user is null)
            {
                return new();
            }

            user.ToDoItems = await _context.ToDoItems.Where(i => i.User == user.Id).ToListAsync();

            foreach (var item in user.ToDoItems)
            {
                item.PriorityNavigation = await _context.Priorities.FirstOrDefaultAsync(p => p.Id == item.Priority);
            }

            return new() { User = user };
        }

        public async Task<UserResult> RegisterUserAsync(UserRegistrationDto request)
        {
            ValidationResult validationResult = new();

            if (!emailRegex().IsMatch(request.Email))
                validationResult.AddError("Email", "Некорректный адрес электронной почты");

            if (await _userManager.FindByEmailAsync(request.Email) is not null)
                validationResult.AddError("Email", "Почта уже используется");

            if (await _userManager.FindByNameAsync(request.UserName) is not null)
                validationResult.AddError("UserName", "Данное имя уже занято");

            if (request.Password != request.ConfirmPassword)
                validationResult.AddError("ConfirmPassword", "Пароли не совпадают");

            if (validationResult.IsValid)
            {
                User newUser = new()
                {
                    Email = request.Email,
                    NormalizedEmail = request.Email.Normalize().ToUpperInvariant(),
                    UserName = request.UserName,
                    NormalizedUserName = request.UserName.Normalize().ToUpperInvariant()
                };

                newUser.PasswordHash = _passwordHasher.HashPassword(newUser, request.Password);

                var result = await _userManager.CreateAsync(newUser);

                if (result.Succeeded)
                {
                    if (request is AdminUserRegistrationDto adminRequest)
                    {
                        if (adminRequest.IsSuperUser)
                            await _userManager.AddToRoleAsync(newUser, "admin");
                    }

                    return new() { User = newUser };
                }
                else
                {
                    foreach (var error in result.Errors)
                    {
                        validationResult.AddError("", error.Description);
                    }
                }
            }

            return new() { ValidationResult = validationResult };
        }

        public async Task<UserResult> LoginUserAsync(UserLoginDto request)
        {
            ValidationResult validationResult = new();

            User? user = await _userManager.FindByNameAsync(request.UserName);

            if (user is null)
                validationResult.AddError("UserName", "Данный пользователь не найден");

            if (validationResult.IsValid)
            {
                var signInResult = await _signInManager.PasswordSignInAsync(user!, request.Password, request.RememberMe, false);

                if (signInResult.Succeeded)
                {
                    return new() { User = user };
                }
                else
                {
                    validationResult.AddError("Password", "Неправильный пароль");
                }
            }

            return new() { ValidationResult = validationResult };
        }

        public async Task<UserResult> EditUserAsync(UserEditDto request)
        {
            ValidationResult validationResult = new();

            var user = await _userManager.FindByIdAsync(request.Id);
            if (user is null)
            {
                return new();
            }

            if (!emailRegex().IsMatch(request.Email))
                validationResult.AddError("Email", "Некорректный адрес электронной почты");

            if (await _userManager.FindByEmailAsync(request.Email) is not null
                && !user.NormalizedEmail!.Equals(request.Email.Normalize(), StringComparison.InvariantCultureIgnoreCase))
                validationResult.AddError("Email", "Почта уже используется");

            if (await _userManager.FindByNameAsync(request.UserName) is not null
                && !user.NormalizedUserName!.Equals(request.UserName.Normalize(), StringComparison.InvariantCultureIgnoreCase))
                validationResult.AddError("UserName", "Данное имя уже занято");

            if (request.Password != request.ConfirmPassword)
                validationResult.AddError("ConfirmPassword", "Пароли не совпадают");

            //if (request.Password != null && request.CurrentPassword == null)
            //    validationResult.AddError("CurrentPassword", "Укажите текущий пароль");

            //if (request.CurrentPassword != null && request.Password != null)
            //    if (!await _userManager.CheckPasswordAsync(userResult, request.CurrentPassword))
            //        validationResult.AddError("CurrentPassword", "Неверный пароль");

            if (validationResult.IsValid)
            {
                if (!user.NormalizedEmail!.Equals(request.Email.Normalize(), StringComparison.InvariantCultureIgnoreCase))
                {
                    // INFO: Токен должен отправляться на новую почту, но так как это тестовое задание, он используется напрямую
                    var changeEmailToken = await _userManager.GenerateChangeEmailTokenAsync(user, request.Email);
                    await _userManager.ChangeEmailAsync(user, request.Email, changeEmailToken);
                }

                if (!user.NormalizedUserName!.Equals(request.UserName.Normalize(), StringComparison.InvariantCultureIgnoreCase))
                    await _userManager.SetUserNameAsync(user, request.UserName);

                if (request.Password != null)
                {
                    // INFO: Токен должен отправиться на почту, но так как это тестовое задание, он используется напрямую
                    var changePasswordToken = await _userManager.GeneratePasswordResetTokenAsync(user);
                    await _userManager.ResetPasswordAsync(user, changePasswordToken, request.Password);
                }

                if (request is AdminUserEditDto adminRequest)
                {
                    if (adminRequest.IsSuperUser)
                        await _userManager.AddToRoleAsync(user, "admin");
                    else
                        await _userManager.RemoveFromRoleAsync(user, "admin");
                }

                return new() { User = user };
            }

            return new() { ValidationResult = validationResult };
        }

        public async Task<UserResult> DeleteUserAsync(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user != null)
            {
                await _context.ToDoItems.Where(t => t.User == user.Id).ExecuteDeleteAsync();
                await _userManager.DeleteAsync(user);
            }

            return new() { User = user };
        }

        public async Task LogoutAsync()
        {
            await _signInManager.SignOutAsync();
        }

        public async Task<bool> IsUserInRole(User user, string role)
        {
            return await _userManager.IsInRoleAsync(user, role);
        }
    }
}

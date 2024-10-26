using FastEndpointAuth.Model;
using Microsoft.AspNetCore.Identity;

namespace FastEndpointAuth.Interfaces;

public interface IAuthRepository
{
    Task<RoleResult> AddRole(string role);
    Task<IdentityResult> AddUserAsync(IdentityUser user, string password);
    Task<RoleResult> AssignRole(UserRole model);
    Task<LogInResult> LoginUserAsync(LogIn model);
}

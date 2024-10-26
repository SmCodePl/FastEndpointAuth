
using FastEndpointAuth.Interfaces;
using FluentValidation.Results;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Claims;
using System.Text;

namespace FastEndpointAuth.Repository;

public class AuthRepository(UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager, IConfiguration configuration)
    : IAuthRepository
{
    // Register new user 
    public async Task<IdentityResult> AddUserAsync(IdentityUser user, string password)
    {
        return await userManager.CreateAsync(user, password);
    }
    
    // Add New role
    public async Task<RoleResult> AddRole(string role)
    {
        if(string.IsNullOrEmpty(role))
        {
            return new RoleResult(HttpStatusCode.BadRequest, "Role cannot be empty");
        }

        if (!await roleManager.RoleExistsAsync(role))
        {
            var result = await roleManager.CreateAsync(new IdentityRole(role));
            
            if (result.Succeeded)
            {
                return new RoleResult(HttpStatusCode.OK, $"Role:{role} has been added");
            }
            
            return new RoleResult(HttpStatusCode.BadRequest, $"Role: {role} not added exception:{result.Errors?.FirstOrDefault()?.Description}");
        }
        
        return new RoleResult(HttpStatusCode.BadRequest, $"Role: {role} already exists");
    }


    public async Task<RoleResult> AssignRole(UserRole model)
    {
        if (string.IsNullOrEmpty(model.UserName))
        {
            return new RoleResult(HttpStatusCode.NotFound, "User name cannot be empty");
        }

        if (string.IsNullOrEmpty(model.Role))
        {
            return new RoleResult(HttpStatusCode.NotFound, "Role cannot be empty");
        }
        
        var user = await userManager.FindByNameAsync(model.UserName);

        if (user == null)
        {
            return new RoleResult(HttpStatusCode.NotFound, $"User: {model.UserName} not found");
        }

       
        var result = await userManager.AddToRoleAsync(user, model.Role);

        if (result.Succeeded)
        {
            return new RoleResult(HttpStatusCode.OK, $"Role: {model.Role} has been assigned to user: {model.UserName}");
        }

        return new RoleResult(HttpStatusCode.BadRequest, result.Errors?.FirstOrDefault()?.Description ?? string.Empty);
    }

    // Login user
    public async Task<LogInResult> LoginUserAsync(LogIn model)
    {
        var user = await userManager.FindByNameAsync(model.UserName);
        
        if (user != null && await userManager.CheckPasswordAsync(user, model.Password))
        {
            var userRoles = await userManager.GetRolesAsync(user);
            var authClaims = new List<Claim>
            {
                    new Claim(JwtRegisteredClaimNames.Sub, user.UserName!),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            authClaims.AddRange(userRoles.Select(role => new Claim(ClaimTypes.Role, role)));
           
            var token = new JwtSecurityToken(
                    issuer: configuration["Jwt:Issuer"],
                    expires: DateTime.Now.AddMinutes(double.Parse(configuration["Jwt:ExpiryMinutes"]!)),
                    claims: authClaims,
                    signingCredentials: new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:Key"]!)),
                    SecurityAlgorithms.HmacSha256));
          
            return new LogInResult(HttpStatusCode.OK, "OK",new JwtSecurityTokenHandler().WriteToken(token));
        }
        if(user is null)
        {
            return new LogInResult(HttpStatusCode.NotFound, "User does not exist", string.Empty);
        }

        return new LogInResult(HttpStatusCode.BadRequest, "User not authorized",string.Empty);
    }
}


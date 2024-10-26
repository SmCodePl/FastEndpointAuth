using FastEndpointAuth.Data;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using FastEndpointAuth.Interfaces;
using FastEndpointAuth.Repository;

namespace FastEndpointAuth.Services;

public static class ServiceRegistration
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration, bool IsDevelopment)
    {
        // Add Infrastructure services Here like DbContext, Identity, etc.
        string connectionString = configuration.GetConnectionString("AuthDb") ?? string.Empty;

        if (string.IsNullOrEmpty(connectionString))
        {
            throw new Exception("Connection string is empty");
        }

        services.AddDbContext<AuthDbContext>(options =>
        {
           
            options.UseSqlServer(connectionString);
        });

        services.AddIdentity<IdentityUser, IdentityRole>(options =>
        {
            options.Password.RequireDigit = false;
            options.Password.RequiredLength = 6;
            options.Password.RequireLowercase = false;
            options.Password.RequireNonAlphanumeric = false;
            options.Password.RequireUppercase = false;
        })
        .AddEntityFrameworkStores<AuthDbContext>()
        .AddDefaultTokenProviders();

        services
            .AddAuthenticationCookie(validFor: TimeSpan.FromMinutes(60))
            .AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            })
           //.AddCookie(options => options.SlidingExpiration = true)
           .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = false,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = configuration["Jwt:Issuer"],
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:Key"]!))
                };
            });

        services.AddAuthorization(options =>
        {
            options.AddPolicy("AdminPolicy", policy => policy.RequireRole("Admin"));
            options.AddPolicy("UserPolicy", policy => policy.RequireRole("User"));
        });
           
        services.AddDomainServices();
        
        return services;
    }

    public static IServiceCollection AddDomainServices(this IServiceCollection services)
    {
        // Add Domain services Here like Repositories, Services, etc.
       
        services.AddAuthorization()
                .AddFastEndpoints();
        
        services.AddTransient<IAuthRepository, AuthRepository>();
        
        return services;
    }
}

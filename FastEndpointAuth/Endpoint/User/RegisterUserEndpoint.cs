
using FastEndpointAuth.Interfaces;

namespace FastEndpointAuth.Endpoint.User
{

    public record RegisterRequest(string UserName, string email,string Password);
    public record RegisterResponse(int ResultCode, string UserName, string Message);
    public class RegisterUserEndpoint(IAuthRepository repository) : Endpoint<RegisterRequest, RegisterResponse>
    {
        public override void Configure()
        {
            Post("/user/register");
            AllowAnonymous();
        }
        public override async Task HandleAsync(RegisterRequest request, CancellationToken cancellationToken)
        {
            var user = new IdentityUser { UserName = request.UserName,Email = request.email };
            var result = await repository.AddUserAsync(user, request.Password);
            
            
            await SendAsync(new RegisterResponse(
                ResultCode: result.Succeeded ? 200 : 400,
                request.UserName,
                Message: result.Succeeded ? "User registered" : result.Errors?.FirstOrDefault()?.Description ?? string.Empty
            ));
        }
    }
}
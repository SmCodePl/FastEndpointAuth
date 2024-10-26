using FastEndpointAuth.Interfaces;
using System.Net;


namespace FastEndpointAuth.Endpoint.User.LogIn
{
    public record LogInRequest(string UserName, string Password);
    public record LogInResponse(HttpStatusCode ResultCode,string UserName,string message,string Token);
    public class LogInEndpoint(IAuthRepository repository) : Endpoint<LogInRequest, LogInResponse>
    {
        public override void Configure()
        {
            Post("/user/login");
            AllowAnonymous();
        }

        public override async Task HandleAsync(LogInRequest request, CancellationToken cancelationToken)
        {
           
            var result = await repository.LoginUserAsync(new Model.LogIn(request.UserName,request.Password));
            
            await SendAsync(new LogInResponse(
                ResultCode: result.ResultCode,
                request.UserName,
                message: result.message,
                Token: result.token
                ));
        }
    }
}

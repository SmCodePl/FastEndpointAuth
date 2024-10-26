using FastEndpointAuth.Interfaces;
using System.Net;

namespace FastEndpointAuth.Endpoint.Role.AssignRole;

public record AssignRequest(string UserName, string RoleName);
public record AssignRespond(HttpStatusCode ResultCode, string Message);
public class AssignRoleEndpoint(IAuthRepository repository) : Endpoint<AssignRequest, AssignRespond>
{
    public override void Configure()
    {
        Post("role/assign");
        AllowAnonymous();
        //Roles("Admin");
    }
    public override async Task HandleAsync(AssignRequest request, CancellationToken cancelationToken)
    {
        var userRole = new UserRole(request.UserName, request.RoleName);
        
        var result = await repository.AssignRole(userRole);

        await SendAsync(new AssignRespond(
            ResultCode: result.ResultCode,
            Message: result.Message
        ));
    }
}

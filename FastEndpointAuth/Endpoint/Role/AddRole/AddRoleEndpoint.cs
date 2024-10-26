using FastEndpointAuth.Interfaces;

namespace FastEndpointAuth.Endpoint.Role.AddRole;

public class AddRoleEndpoint(IAuthRepository repository) : Endpoint<AddRoleRequest, Response>
{
    public override void Configure()
    {
        Post("/role/add");
        AllowAnonymous();

    }
    public override async Task HandleAsync(AddRoleRequest request, CancellationToken cancelationToken)
    {
     
      var result = await repository.AddRole(request.RoleName);
        
        await SendAsync(new Response(
            ResultCode: result.ResultCode,
            Message: result.Message
            ));
    }
}

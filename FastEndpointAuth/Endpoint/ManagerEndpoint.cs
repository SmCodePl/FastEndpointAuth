using Microsoft.AspNetCore.Authorization;

namespace FastEndpointAuth.Endpoint;

//Configuration with Attributes 
[HttpGet("/request/manager")]
[Authorize(Roles ="Admin,Manager")]
public class ManagerEndpoint : EndpointWithoutRequest<ManagerResult>
{
    public override async Task HandleAsync(CancellationToken cancelationToken)
    {
        await SendAsync(new ManagerResult("Hello Manager!"));
    }
}

public record ManagerResult(string Message);
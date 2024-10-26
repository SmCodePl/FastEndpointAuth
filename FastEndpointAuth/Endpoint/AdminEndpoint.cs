namespace FastEndpointAuth.Endpoint;

public record AdminResponse(string Message);
public class AdminEndpoint : Ep.NoReq.Res<AdminResponse>
{
    public override void Configure()
    {
        Get("/request/admin");
        Roles("Admin");
    }

    public override async Task HandleAsync(CancellationToken cancelationToken)
    {
        await SendAsync( new AdminResponse( "Hello Admin!"));
    }
}

using Microsoft.AspNetCore.Http.HttpResults;


namespace FastEndpointAuth.Endpoint;

public record UserRequest(string UserName, int OperationType);
public record UserResponse(int ResultCode, string Message);
public class UserEndpoint : Endpoint<UserRequest,Results<Ok<UserResponse>,NotFound,ProblemDetails>>
{
    public override void Configure()
    {
        Get("/request/user");
        //AllowAnonymous();
        Roles("Admin","User");
    }
    public override async Task<Results<Ok<UserResponse>, NotFound, ProblemDetails>> ExecuteAsync
        (UserRequest request, CancellationToken cancelationToken)
    {
        await Task.CompletedTask;

        if(request.UserName == "User")
        {
            if(request.OperationType == 0)
            {
                return TypedResults.NotFound();
            }
            if(request.OperationType == 1)
            {
                AddError(r => r.OperationType, "Value ha to be different then 1 ");
                return new FastEndpoints.ProblemDetails(ValidationFailures);
            }
           
            return TypedResults.Ok(new UserResponse(request.OperationType, "Hello User!"));
        }
        else
        {
            AddError(r=> r.UserName,"User not found");
            return new FastEndpoints.ProblemDetails(ValidationFailures);
        }
    }
}

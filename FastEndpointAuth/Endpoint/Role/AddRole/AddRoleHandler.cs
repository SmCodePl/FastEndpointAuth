using LanguageExt;
using LanguageExt.Common;
using System.Net;

namespace FastEndpointAuth.Endpoint.Role.AddRole;

public record AddRoleRequest(string RoleName);
public record Response(HttpStatusCode ResultCode, string Message);

public class RoleValidator
{
    public static Validation<Error,bool> ValidateRole(string RoleName)
    {
        
        if (string.IsNullOrEmpty(RoleName))
        {
            return Error.New("RoleName is required");
        }

        return true;
    }
}

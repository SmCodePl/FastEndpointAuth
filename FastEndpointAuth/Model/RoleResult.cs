using Microsoft.AspNetCore.Identity;
using System.Net;

namespace FastEndpointAuth.Model;

public record RoleResult(HttpStatusCode ResultCode,string Message);


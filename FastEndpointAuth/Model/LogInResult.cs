using System.Net;

namespace FastEndpointAuth.Model;

public record LogInResult(HttpStatusCode ResultCode,string message,string token);

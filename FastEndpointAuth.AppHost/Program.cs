var builder = DistributedApplication.CreateBuilder(args);

builder.AddProject<Projects.FastEndpointAuth>("fastendpointauth");

builder.Build().Run();

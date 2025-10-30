using ApiGateway.Presentation.Middlewares;
using SharedLibrarySolution.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddReverseProxy()
    .LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"));

var app = builder.Build();

// Custom middleware
app.UseMiddleware<CorrelationIdMiddleware>();
app.UseSharedLogging();
app.UseSharedPolicies(); // shared middleware cho Gateway có global exception

// Swagger UI đa endpoint
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    var endpoints = builder.Configuration.GetSection("SwaggerEndpoints")
        .Get<List<SwaggerEndpoint>>() ?? new List<SwaggerEndpoint>();

    foreach (var endpoint in endpoints)
    {
        // Nếu user truy cập gateway ở port 8888, giữ nguyên URL gateway
        // => load đúng swagger.json qua YARP
        c.SwaggerEndpoint(endpoint.Url, endpoint.Name);
    }

    c.DocumentTitle = "E-Commerce Gateway API Docs";
    c.RoutePrefix = "swagger"; // => http://localhost:8888/swagger
});

// Reverse proxy
app.MapReverseProxy();

app.Run();

public class SwaggerEndpoint
{
    public string Name { get; set; } = string.Empty;
    public string Url { get; set; } = string.Empty;
}
using System.Reflection;
using Api.DTOs.Common;
using Api.Extensions;
using Api.Middleware;
using Microsoft.AspNetCore.Mvc;

var applicationBuilder = WebApplication.CreateBuilder(args);

// Register application services
applicationBuilder.Services.AddApplicationServices(applicationBuilder.Configuration);

// Configure API controllers with JSON options
applicationBuilder.Services.AddControllers()
    .AddJsonOptions(jsonOptions =>
    {
        jsonOptions.JsonSerializerOptions.PropertyNamingPolicy = null;
    })
    .ConfigureApiBehaviorOptions(apiBehaviorOptions =>
    {
        apiBehaviorOptions.InvalidModelStateResponseFactory = actionContext =>
            new BadRequestObjectResult(new ErrorResponseDto
            {
                Code = "ORG-VAL-001",
                Message = "The request body is invalid.",
                Details = actionContext.ModelState
                    .Where(entry => entry.Value?.Errors.Count > 0)
                    .SelectMany(entry => entry.Value!.Errors.Select(error => new ErrorDetailDto
                    {
                        Field = entry.Key.TrimStart('$', '.'),
                        Message = string.IsNullOrWhiteSpace(error.ErrorMessage)
                            ? "The supplied value is invalid."
                            : error.ErrorMessage
                    }))
            });
    });

// Configure Swagger/OpenAPI documentation
applicationBuilder.Services.AddEndpointsApiExplorer();
applicationBuilder.Services.AddSwaggerGen(swaggerOptions =>
{
    swaggerOptions.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "Organization API",
        Version = "v1",
        Description = "RESTful API following organization development standards"
    });

    // Include XML documentation comments in Swagger
    var assemblyXmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var assemblyXmlPath = Path.Combine(AppContext.BaseDirectory, assemblyXmlFile);
    if (File.Exists(assemblyXmlPath))
    {
        swaggerOptions.IncludeXmlComments(assemblyXmlPath);
    }
});

// Configure CORS for frontend development
applicationBuilder.Services.AddCors(corsOptions =>
{
    corsOptions.AddDefaultPolicy(corsPolicy =>
    {
        corsPolicy
            .WithOrigins(
                "http://localhost:5173",
                "http://localhost:3000"
            )
            .AllowAnyHeader()
            .AllowAnyMethod()
            .WithExposedHeaders("X-Transaction-Id");
    });
});

var webApplication = applicationBuilder.Build();

// Enable Swagger in development
if (webApplication.Environment.IsDevelopment())
{
    webApplication.UseSwagger();
    webApplication.UseSwaggerUI(swaggerUiOptions =>
    {
        swaggerUiOptions.SwaggerEndpoint("/swagger/v1/swagger.json", "Organization API v1");
    });
}

// Configure middleware pipeline
webApplication.UseCors();
webApplication.UseTransactionId();
webApplication.UseAuthorization();
webApplication.MapControllers();

// Redirect root URL to Swagger UI for developer convenience
webApplication.MapGet("/", () => Results.Redirect("/swagger"));

webApplication.Run();

// Make Program class accessible for integration tests
public partial class Program { }

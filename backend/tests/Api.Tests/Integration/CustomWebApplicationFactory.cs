using Api.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Api.Tests.Integration;

/// <summary>
/// Custom web application factory for integration testing.
/// Configures the test server with testing environment settings.
/// </summary>
public sealed class CustomWebApplicationFactory : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder webHostBuilder)
    {
        webHostBuilder.UseEnvironment("Testing");
        var databaseName = $"IntegrationTestDb_{Guid.NewGuid()}";

        webHostBuilder.ConfigureServices(serviceCollection =>
        {
            var existingDbDescriptor = serviceCollection.SingleOrDefault(
                descriptor => descriptor.ServiceType == typeof(DbContextOptions<AppDbContext>));

            if (existingDbDescriptor is not null)
            {
                serviceCollection.Remove(existingDbDescriptor);
            }

            serviceCollection.AddDbContext<AppDbContext>(databaseOptions =>
                databaseOptions.UseInMemoryDatabase(databaseName));
        });
    }
}

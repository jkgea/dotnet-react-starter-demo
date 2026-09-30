using Api.Configuration;
using Api.Data;
using Api.Repositories;
using Api.Services;
using Microsoft.EntityFrameworkCore;

namespace Api.Extensions;

/// <summary>
/// Extension methods for configuring application services in the DI container.
/// Centralizes service registration following organization standards.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Registers all application services, repositories, and dependencies.
    /// Call this method in Program.cs to configure the service container.
    /// </summary>
    /// <param name="services">The service collection to configure.</param>
    /// <param name="configuration">Application configuration containing database settings.</param>
    /// <returns>The service collection for method chaining.</returns>
    public static IServiceCollection AddApplicationServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException("Connection string 'DefaultConnection' is not configured.");

        services.AddDbContext<AppDbContext>(databaseOptions =>
            databaseOptions.UseSqlServer(connectionString));

        services.AddOptions<UserPreferencesOptions>()
            .Bind(configuration.GetSection("UserPreferences"))
            .Validate(
                options => options.StarterUserId != Guid.Empty,
                "UserPreferences:StarterUserId must be a non-empty GUID.")
            .ValidateOnStart();
        services.AddScoped<IUserPreferenceRepository, UserPreferenceRepository>();
        services.AddScoped<IUserPreferenceService, UserPreferenceService>();

        return services;
    }
}

using Api.Domain;
using Microsoft.EntityFrameworkCore;

namespace Api.Data;

/// <summary>
/// Entity Framework database context for application data.
/// </summary>
public sealed class AppDbContext : DbContext
{
    /// <summary>
    /// Initializes a new instance of the application database context.
    /// </summary>
    /// <param name="options">Database context options.</param>
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    /// <summary>
    /// Gets the user preference records.
    /// </summary>
    public DbSet<UserPreference> UserPreferences => Set<UserPreference>();

    /// <inheritdoc />
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        var userPreference = modelBuilder.Entity<UserPreference>();

        userPreference.ToTable("UserPreferences");
        userPreference.HasKey(preference => preference.UserPreferenceId);
        userPreference.HasIndex(preference => preference.UserId).IsUnique();
        userPreference.Property(preference => preference.DisplayTheme).HasMaxLength(10).IsRequired();
        userPreference.Property(preference => preference.EmailNotificationIndicator).HasDefaultValue(true);
        userPreference.Property(preference => preference.InAppNotificationIndicator).HasDefaultValue(true);
        userPreference.Property(preference => preference.CreatedDate).HasDefaultValueSql("SYSUTCDATETIME()");
        userPreference.Property(preference => preference.UpdatedDate).HasDefaultValueSql("SYSUTCDATETIME()");
    }
}

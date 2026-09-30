namespace Api.Configuration;

/// <summary>
/// Configuration for the temporary starter user preference identity.
/// </summary>
public sealed class UserPreferencesOptions
{
    /// <summary>
    /// Gets or sets the server-owned user identifier used until authentication is introduced.
    /// </summary>
    public Guid StarterUserId { get; set; }
}

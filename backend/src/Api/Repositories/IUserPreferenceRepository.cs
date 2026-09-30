using Api.Domain;

namespace Api.Repositories;

/// <summary>
/// Provides persistence operations for user preferences.
/// </summary>
public interface IUserPreferenceRepository
{
    /// <summary>
    /// Gets preferences for a user, if a record exists.
    /// </summary>
    Task<UserPreference?> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken);

    /// <summary>
    /// Creates or updates preferences for a user.
    /// </summary>
    Task<UserPreference> UpsertAsync(UserPreference preference, CancellationToken cancellationToken);
}

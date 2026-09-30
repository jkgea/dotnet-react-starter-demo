using Api.DTOs.UserPreferences;

namespace Api.Services;

/// <summary>
/// Provides user preference business operations.
/// </summary>
public interface IUserPreferenceService
{
    /// <summary>
    /// Gets saved preferences or application defaults for the starter user.
    /// </summary>
    Task<UserPreferenceDto> GetAsync(CancellationToken cancellationToken);

    /// <summary>
    /// Validates and saves preferences for the starter user.
    /// </summary>
    Task<UserPreferenceDto> UpdateAsync(
        UpdateUserPreferenceDto preference,
        CancellationToken cancellationToken);
}

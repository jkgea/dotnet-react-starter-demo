using Api.Configuration;
using Api.Domain;
using Api.DTOs.UserPreferences;
using Api.Repositories;
using Microsoft.Extensions.Options;

namespace Api.Services;

/// <summary>
/// Applies defaults and validation to user preference operations.
/// </summary>
public sealed class UserPreferenceService : IUserPreferenceService
{
    public const string LightTheme = "light";
    public const string DarkTheme = "dark";

    private readonly IUserPreferenceRepository _repository;
    private readonly Guid _starterUserId;

    /// <summary>
    /// Initializes a new instance of the user preference service.
    /// </summary>
    public UserPreferenceService(
        IUserPreferenceRepository repository,
        IOptions<UserPreferencesOptions> options)
    {
        _repository = repository;
        _starterUserId = options.Value.StarterUserId;
    }

    /// <inheritdoc />
    public async Task<UserPreferenceDto> GetAsync(CancellationToken cancellationToken)
    {
        var preference = await _repository.GetByUserIdAsync(_starterUserId, cancellationToken);

        return preference is null
            ? CreateDefaultPreference()
            : MapToDto(preference);
    }

    /// <inheritdoc />
    public async Task<UserPreferenceDto> UpdateAsync(
        UpdateUserPreferenceDto preference,
        CancellationToken cancellationToken)
    {
        var normalizedTheme = preference.DisplayTheme.Trim().ToLowerInvariant();

        if (normalizedTheme is not LightTheme and not DarkTheme)
        {
            throw new ArgumentException("Display theme must be either 'light' or 'dark'.", nameof(preference));
        }

        var currentTimestamp = DateTime.UtcNow;
        var savedPreference = await _repository.UpsertAsync(
            new UserPreference
            {
                UserId = _starterUserId,
                DisplayTheme = normalizedTheme,
                EmailNotificationIndicator = preference.EmailNotificationIndicator,
                InAppNotificationIndicator = preference.InAppNotificationIndicator,
                CreatedDate = currentTimestamp,
                UpdatedDate = currentTimestamp
            },
            cancellationToken);

        return MapToDto(savedPreference);
    }

    private static UserPreferenceDto CreateDefaultPreference()
    {
        return new UserPreferenceDto
        {
            DisplayTheme = LightTheme,
            EmailNotificationIndicator = true,
            InAppNotificationIndicator = true
        };
    }

    private static UserPreferenceDto MapToDto(UserPreference preference)
    {
        return new UserPreferenceDto
        {
            DisplayTheme = preference.DisplayTheme,
            EmailNotificationIndicator = preference.EmailNotificationIndicator,
            InAppNotificationIndicator = preference.InAppNotificationIndicator
        };
    }
}

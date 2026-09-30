using System.Text.Json.Serialization;

namespace Api.DTOs.UserPreferences;

/// <summary>
/// Represents a user's display and notification preferences.
/// </summary>
public sealed class UserPreferenceDto
{
    /// <summary>
    /// Gets or sets the selected display theme.
    /// </summary>
    [JsonPropertyName("displayTheme")]
    public required string DisplayTheme { get; set; }

    /// <summary>
    /// Gets or sets whether email notifications are enabled.
    /// </summary>
    [JsonPropertyName("emailNotificationIndicator")]
    public bool EmailNotificationIndicator { get; set; }

    /// <summary>
    /// Gets or sets whether in-app notifications are enabled.
    /// </summary>
    [JsonPropertyName("inAppNotificationIndicator")]
    public bool InAppNotificationIndicator { get; set; }
}

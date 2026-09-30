using System.Text.Json.Serialization;

namespace Api.DTOs.UserPreferences;

/// <summary>
/// Represents changes to a user's display and notification preferences.
/// </summary>
public sealed class UpdateUserPreferenceDto
{
    /// <summary>
    /// Gets or sets the selected display theme. Supported values are light and dark.
    /// </summary>
    [JsonPropertyName("displayTheme")]
    public string DisplayTheme { get; set; } = string.Empty;

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

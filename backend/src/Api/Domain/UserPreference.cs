namespace Api.Domain;

/// <summary>
/// Stores display and notification preferences for a user.
/// </summary>
public sealed class UserPreference
{
    /// <summary>
    /// Gets or sets the unique preference record identifier.
    /// </summary>
    public int UserPreferenceId { get; set; }

    /// <summary>
    /// Gets or sets the user associated with these preferences.
    /// </summary>
    public Guid UserId { get; set; }

    /// <summary>
    /// Gets or sets the selected display theme.
    /// </summary>
    public required string DisplayTheme { get; set; }

    /// <summary>
    /// Gets or sets whether email notifications are enabled.
    /// </summary>
    public bool EmailNotificationIndicator { get; set; }

    /// <summary>
    /// Gets or sets whether in-app notifications are enabled.
    /// </summary>
    public bool InAppNotificationIndicator { get; set; }

    /// <summary>
    /// Gets or sets when the preferences were created.
    /// </summary>
    public DateTime CreatedDate { get; set; }

    /// <summary>
    /// Gets or sets when the preferences were last updated.
    /// </summary>
    public DateTime UpdatedDate { get; set; }
}

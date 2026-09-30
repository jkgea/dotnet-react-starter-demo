using Api.Configuration;
using Api.Domain;
using Api.DTOs.UserPreferences;
using Api.Repositories;
using Api.Services;
using Microsoft.Extensions.Options;
using Moq;
using Xunit;

namespace Api.Tests.Services;

public sealed class UserPreferenceServiceTests
{
    private readonly Guid _starterUserId = Guid.NewGuid();
    private readonly Mock<IUserPreferenceRepository> _repositoryMock;
    private readonly UserPreferenceService _service;

    public UserPreferenceServiceTests()
    {
        _repositoryMock = new Mock<IUserPreferenceRepository>();
        _service = new UserPreferenceService(
            _repositoryMock.Object,
            Options.Create(new UserPreferencesOptions { StarterUserId = _starterUserId }));
    }

    #region GetAsync Tests

    [Fact]
    public async Task GetAsync_WhenPreferenceDoesNotExist_ReturnsDefaults()
    {
        // Arrange
        _repositoryMock
            .Setup(repository => repository.GetByUserIdAsync(_starterUserId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((UserPreference?)null);

        // Act
        var result = await _service.GetAsync(CancellationToken.None);

        // Assert
        Assert.Equal(UserPreferenceService.LightTheme, result.DisplayTheme);
        Assert.True(result.EmailNotificationIndicator);
        Assert.True(result.InAppNotificationIndicator);
    }

    [Fact]
    public async Task GetAsync_WhenPreferenceExists_ReturnsStoredNotificationBooleans()
    {
        // Arrange
        _repositoryMock
            .Setup(repository => repository.GetByUserIdAsync(_starterUserId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(CreatePreference(
                displayTheme: UserPreferenceService.DarkTheme,
                emailNotificationIndicator: false,
                inAppNotificationIndicator: true));

        // Act
        var result = await _service.GetAsync(CancellationToken.None);

        // Assert
        Assert.Equal(UserPreferenceService.DarkTheme, result.DisplayTheme);
        Assert.False(result.EmailNotificationIndicator);
        Assert.True(result.InAppNotificationIndicator);
    }

    #endregion

    #region UpdateAsync Tests

    [Fact]
    public async Task UpdateAsync_WithValidPreference_NormalizesAndPersistsValues()
    {
        // Arrange
        UserPreference? capturedPreference = null;
        var request = new UpdateUserPreferenceDto
        {
            DisplayTheme = " DARK ",
            EmailNotificationIndicator = false,
            InAppNotificationIndicator = true
        };
        _repositoryMock
            .Setup(repository => repository.UpsertAsync(It.IsAny<UserPreference>(), It.IsAny<CancellationToken>()))
            .Callback<UserPreference, CancellationToken>((preference, _) => capturedPreference = preference)
            .ReturnsAsync((UserPreference preference, CancellationToken _) => preference);

        // Act
        var result = await _service.UpdateAsync(request, CancellationToken.None);

        // Assert
        Assert.NotNull(capturedPreference);
        Assert.Equal(_starterUserId, capturedPreference.UserId);
        Assert.Equal(UserPreferenceService.DarkTheme, capturedPreference.DisplayTheme);
        Assert.False(capturedPreference.EmailNotificationIndicator);
        Assert.True(capturedPreference.InAppNotificationIndicator);
        Assert.Equal(UserPreferenceService.DarkTheme, result.DisplayTheme);
    }

    [Fact]
    public async Task UpdateAsync_WithInvalidTheme_ThrowsAndDoesNotPersist()
    {
        // Arrange
        var request = new UpdateUserPreferenceDto
        {
            DisplayTheme = "high-contrast",
            EmailNotificationIndicator = true,
            InAppNotificationIndicator = false
        };

        // Act
        var exception = await Assert.ThrowsAsync<ArgumentException>(
            () => _service.UpdateAsync(request, CancellationToken.None));

        // Assert
        Assert.Contains("light", exception.Message);
        _repositoryMock.Verify(
            repository => repository.UpsertAsync(It.IsAny<UserPreference>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }

    #endregion

    private UserPreference CreatePreference(
        string displayTheme,
        bool emailNotificationIndicator,
        bool inAppNotificationIndicator)
    {
        return new UserPreference
        {
            UserId = _starterUserId,
            DisplayTheme = displayTheme,
            EmailNotificationIndicator = emailNotificationIndicator,
            InAppNotificationIndicator = inAppNotificationIndicator,
            CreatedDate = DateTime.UtcNow,
            UpdatedDate = DateTime.UtcNow
        };
    }
}

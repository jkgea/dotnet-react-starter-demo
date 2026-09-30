using Api.Data;
using Api.Domain;
using Api.Repositories;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Api.Tests.Repositories;

public sealed class UserPreferenceRepositoryTests : IDisposable
{
    private readonly AppDbContext _context;
    private readonly UserPreferenceRepository _repository;

    public UserPreferenceRepositoryTests()
    {
        var databaseOptions = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        _context = new AppDbContext(databaseOptions);
        _repository = new UserPreferenceRepository(_context);
    }

    #region GetByUserIdAsync Tests

    [Fact]
    public async Task GetByUserIdAsync_WhenPreferenceExists_ReturnsUntrackedPreference()
    {
        // Arrange
        var preference = CreatePreference();
        _context.UserPreferences.Add(preference);
        await _context.SaveChangesAsync();
        _context.ChangeTracker.Clear();

        // Act
        var result = await _repository.GetByUserIdAsync(preference.UserId, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(UserPreferenceServiceValues.DarkTheme, result.DisplayTheme);
        Assert.False(result.EmailNotificationIndicator);
        Assert.True(result.InAppNotificationIndicator);
        Assert.Empty(_context.ChangeTracker.Entries());
    }

    #endregion

    #region UpsertAsync Tests

    [Fact]
    public async Task UpsertAsync_WhenPreferenceDoesNotExist_CreatesPreference()
    {
        // Arrange
        var preference = CreatePreference();

        // Act
        var result = await _repository.UpsertAsync(preference, CancellationToken.None);

        // Assert
        Assert.True(result.UserPreferenceId > 0);
        var storedPreference = await _context.UserPreferences.SingleAsync();
        Assert.Equal(preference.UserId, storedPreference.UserId);
        Assert.False(storedPreference.EmailNotificationIndicator);
        Assert.True(storedPreference.InAppNotificationIndicator);
    }

    [Fact]
    public async Task UpsertAsync_WhenPreferenceExists_UpdatesValuesAndPreservesCreatedDate()
    {
        // Arrange
        var originalCreatedDate = DateTime.UtcNow.AddDays(-1);
        var existingPreference = CreatePreference(createdDate: originalCreatedDate);
        _context.UserPreferences.Add(existingPreference);
        await _context.SaveChangesAsync();
        var updatedDate = DateTime.UtcNow;
        var update = CreatePreference(
            userId: existingPreference.UserId,
            displayTheme: UserPreferenceServiceValues.LightTheme,
            emailNotificationIndicator: true,
            inAppNotificationIndicator: false,
            createdDate: updatedDate,
            updatedDate: updatedDate);

        // Act
        var result = await _repository.UpsertAsync(update, CancellationToken.None);

        // Assert
        Assert.Equal(existingPreference.UserPreferenceId, result.UserPreferenceId);
        Assert.Equal(UserPreferenceServiceValues.LightTheme, result.DisplayTheme);
        Assert.True(result.EmailNotificationIndicator);
        Assert.False(result.InAppNotificationIndicator);
        Assert.Equal(originalCreatedDate, result.CreatedDate);
        Assert.Equal(updatedDate, result.UpdatedDate);
        Assert.Equal(1, await _context.UserPreferences.CountAsync());
    }

    #endregion

    public void Dispose()
    {
        _context.Dispose();
    }

    private static UserPreference CreatePreference(
        Guid? userId = null,
        string displayTheme = UserPreferenceServiceValues.DarkTheme,
        bool emailNotificationIndicator = false,
        bool inAppNotificationIndicator = true,
        DateTime? createdDate = null,
        DateTime? updatedDate = null)
    {
        return new UserPreference
        {
            UserId = userId ?? Guid.NewGuid(),
            DisplayTheme = displayTheme,
            EmailNotificationIndicator = emailNotificationIndicator,
            InAppNotificationIndicator = inAppNotificationIndicator,
            CreatedDate = createdDate ?? DateTime.UtcNow,
            UpdatedDate = updatedDate ?? DateTime.UtcNow
        };
    }

    private static class UserPreferenceServiceValues
    {
        public const string LightTheme = "light";
        public const string DarkTheme = "dark";
    }
}

using System.Net;
using System.Net.Http.Json;
using System.Text;
using Api.Data;
using Api.DTOs.Common;
using Api.DTOs.UserPreferences;
using Api.Tests.Integration;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Api.Tests.Controllers;

public sealed class UserPreferencesControllerTests : IClassFixture<CustomWebApplicationFactory>, IDisposable
{
    private const string EndpointPath = "/v1/user-preferences";
    private readonly HttpClient _testClient;

    public UserPreferencesControllerTests(CustomWebApplicationFactory applicationFactory)
    {
        using var serviceScope = applicationFactory.Services.CreateScope();
        var databaseContext = serviceScope.ServiceProvider.GetRequiredService<AppDbContext>();
        databaseContext.Database.EnsureDeleted();
        databaseContext.Database.EnsureCreated();

        _testClient = applicationFactory.CreateClient();
    }

    #region GetAsync Tests

    [Fact]
    public async Task GetAsync_WhenPreferenceDoesNotExist_ReturnsDefaultsWithMetadataAndSelfLink()
    {
        // Arrange
        var requestStartedDate = DateTime.UtcNow;

        // Act
        var httpResponse = await _testClient.GetAsync(EndpointPath);
        var response = await httpResponse.Content.ReadFromJsonAsync<ItemResponseDto<UserPreferenceDto>>();

        // Assert
        Assert.Equal(HttpStatusCode.OK, httpResponse.StatusCode);
        Assert.NotNull(response);
        Assert.Equal("light", response.Item.DisplayTheme);
        Assert.True(response.Item.EmailNotificationIndicator);
        Assert.True(response.Item.InAppNotificationIndicator);
        Assert.True(response.Metadata.Timestamp >= requestStartedDate);
        Assert.True(Guid.TryParse(response.Metadata.TransactionId, out _));
        Assert.Equal($"http://localhost{EndpointPath}", response.Links.Self);
        Assert.Null(response.Links.Next);
        Assert.Null(response.Links.Prev);
    }

    #endregion

    #region UpdateAsync Tests

    [Fact]
    public async Task UpdateAsync_WithValidPreference_PersistsThemeAndNotificationBooleans()
    {
        // Arrange
        var request = new UpdateUserPreferenceDto
        {
            DisplayTheme = "dark",
            EmailNotificationIndicator = false,
            InAppNotificationIndicator = true
        };

        // Act
        var updateResponse = await _testClient.PutAsJsonAsync(EndpointPath, request);
        var getResponse = await _testClient.GetFromJsonAsync<ItemResponseDto<UserPreferenceDto>>(EndpointPath);

        // Assert
        Assert.Equal(HttpStatusCode.OK, updateResponse.StatusCode);
        Assert.NotNull(getResponse);
        Assert.Equal("dark", getResponse.Item.DisplayTheme);
        Assert.False(getResponse.Item.EmailNotificationIndicator);
        Assert.True(getResponse.Item.InAppNotificationIndicator);
    }

    [Fact]
    public async Task UpdateAsync_WithInvalidTheme_ReturnsValidationErrorEnvelope()
    {
        // Arrange
        var request = new UpdateUserPreferenceDto
        {
            DisplayTheme = "sepia",
            EmailNotificationIndicator = true,
            InAppNotificationIndicator = true
        };

        // Act
        var httpResponse = await _testClient.PutAsJsonAsync(EndpointPath, request);
        var response = await httpResponse.Content.ReadFromJsonAsync<ErrorResponseDto>();

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, httpResponse.StatusCode);
        Assert.NotNull(response);
        Assert.Equal("ORG-VAL-001", response.Code);
        Assert.Equal("The user preferences request is invalid.", response.Message);
        var detail = Assert.Single(response.Details!);
        Assert.Equal("displayTheme", detail.Field);
        Assert.Contains("light", detail.Message);
    }

    [Fact]
    public async Task UpdateAsync_WithMalformedJson_ReturnsValidationErrorEnvelope()
    {
        // Arrange
        using var malformedContent = new StringContent(
            "{\"displayTheme\":",
            Encoding.UTF8,
            "application/json");

        // Act
        var httpResponse = await _testClient.PutAsync(EndpointPath, malformedContent);
        var response = await httpResponse.Content.ReadFromJsonAsync<ErrorResponseDto>();

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, httpResponse.StatusCode);
        Assert.NotNull(response);
        Assert.Equal("ORG-VAL-001", response.Code);
        Assert.Equal("The request body is invalid.", response.Message);
        Assert.NotEmpty(response.Details!);
    }

    #endregion

    public void Dispose()
    {
        _testClient.Dispose();
    }
}

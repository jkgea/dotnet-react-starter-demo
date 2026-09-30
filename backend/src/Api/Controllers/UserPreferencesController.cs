using Api.DTOs.Common;
using Api.DTOs.UserPreferences;
using Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

/// <summary>
/// Manages display and notification preferences for the current starter user.
/// </summary>
[Route("v1/user-preferences")]
[ApiController]
[Produces("application/json")]
public sealed class UserPreferencesController : ControllerBase
{
    private readonly IUserPreferenceService _service;

    /// <summary>
    /// Initializes a new instance of the user preferences controller.
    /// </summary>
    public UserPreferencesController(IUserPreferenceService service)
    {
        _service = service;
    }

    /// <summary>
    /// Gets preferences for the current starter user.
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(ItemResponseDto<UserPreferenceDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<ItemResponseDto<UserPreferenceDto>>> GetAsync(
        CancellationToken cancellationToken)
    {
        var preference = await _service.GetAsync(cancellationToken);
        return Ok(CreateEnvelope(preference));
    }

    /// <summary>
    /// Saves preferences for the current starter user.
    /// </summary>
    [HttpPut]
    [ProducesResponseType(typeof(ItemResponseDto<UserPreferenceDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<ItemResponseDto<UserPreferenceDto>>> UpdateAsync(
        [FromBody] UpdateUserPreferenceDto preference,
        CancellationToken cancellationToken)
    {
        try
        {
            var updatedPreference = await _service.UpdateAsync(preference, cancellationToken);
            return Ok(CreateEnvelope(updatedPreference));
        }
        catch (ArgumentException exception)
        {
            return BadRequest(new ErrorResponseDto
            {
                Code = "ORG-VAL-001",
                Message = "The user preferences request is invalid.",
                Details =
                [
                    new ErrorDetailDto
                    {
                        Field = "displayTheme",
                        Message = exception.Message
                    }
                ]
            });
        }
    }

    private ItemResponseDto<UserPreferenceDto> CreateEnvelope(UserPreferenceDto preference)
    {
        var currentTimestamp = DateTime.UtcNow;
        var transactionId = HttpContext.Items["TransactionId"]?.ToString() ?? Guid.NewGuid().ToString();

        return new ItemResponseDto<UserPreferenceDto>
        {
            Item = preference,
            Metadata = new MetadataDto
            {
                Timestamp = currentTimestamp,
                TransactionId = transactionId,
                TotalCount = null
            },
            Links = new LinksDto
            {
                Self = $"{Request.Scheme}://{Request.Host}{Request.PathBase}{Request.Path}",
                Next = null,
                Prev = null
            }
        };
    }
}

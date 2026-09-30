using System.Data;
using Api.Data;
using Api.Domain;
using Microsoft.EntityFrameworkCore;

namespace Api.Repositories;

/// <summary>
/// Stores user preferences using Entity Framework Core.
/// </summary>
public sealed class UserPreferenceRepository : IUserPreferenceRepository
{
    private readonly AppDbContext _context;

    /// <summary>
    /// Initializes a new instance of the user preference repository.
    /// </summary>
    public UserPreferenceRepository(AppDbContext context)
    {
        _context = context;
    }

    /// <inheritdoc />
    public Task<UserPreference?> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken)
    {
        return _context.UserPreferences
            .AsNoTracking()
            .SingleOrDefaultAsync(preference => preference.UserId == userId, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<UserPreference> UpsertAsync(
        UserPreference preference,
        CancellationToken cancellationToken)
    {
        if (!_context.Database.IsRelational())
        {
            return await SavePreferenceAsync(preference, cancellationToken);
        }

        await using var transaction = await _context.Database.BeginTransactionAsync(
            IsolationLevel.Serializable,
            cancellationToken);
        var savedPreference = await SavePreferenceAsync(preference, cancellationToken);
        await transaction.CommitAsync(cancellationToken);

        return savedPreference;
    }

    private async Task<UserPreference> SavePreferenceAsync(
        UserPreference preference,
        CancellationToken cancellationToken)
    {
        var existingPreference = await _context.UserPreferences
            .SingleOrDefaultAsync(
                candidate => candidate.UserId == preference.UserId,
                cancellationToken);

        if (existingPreference is null)
        {
            _context.UserPreferences.Add(preference);
            existingPreference = preference;
        }
        else
        {
            existingPreference.DisplayTheme = preference.DisplayTheme;
            existingPreference.EmailNotificationIndicator = preference.EmailNotificationIndicator;
            existingPreference.InAppNotificationIndicator = preference.InAppNotificationIndicator;
            existingPreference.UpdatedDate = preference.UpdatedDate;
        }

        await _context.SaveChangesAsync(cancellationToken);
        return existingPreference;
    }
}

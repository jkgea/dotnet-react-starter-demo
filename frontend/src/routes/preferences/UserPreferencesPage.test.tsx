import { render, screen, waitFor } from '@testing-library/react';
import userEvent from '@testing-library/user-event';
import { updateUserPreferences } from '../../lib/api';
import { preferenceContent } from '../../lib/content';
import type { SingleItemEnvelope, UserPreference } from '../../lib/types';
import { UserPreferencesPage } from './UserPreferencesPage';

jest.mock('../../lib/api', () => ({
  updateUserPreferences: jest.fn(),
}));

const updateUserPreferencesMock = jest.mocked(updateUserPreferences);
const defaultPreference: UserPreference = {
  displayTheme: 'light',
  emailNotificationIndicator: true,
  inAppNotificationIndicator: true,
};

describe('UserPreferencesPage', () => {
  test('UserPreferencesPage_WhenControlsChangeAndSaveSucceeds_PersistsDraftAndReportsSuccess', async () => {
    // Arrange
    const user = userEvent.setup();
    const onPreferenceSaved = jest.fn();
    const savedPreference: UserPreference = {
      displayTheme: 'dark',
      emailNotificationIndicator: false,
      inAppNotificationIndicator: false,
    };
    updateUserPreferencesMock.mockResolvedValue(createEnvelope(savedPreference));
    render(
      <UserPreferencesPage
        preference={defaultPreference}
        loadingIndicator={false}
        loadErrorIndicator={false}
        onPreferenceSaved={onPreferenceSaved}
      />
    );

    // Act
    await user.click(screen.getByRole('button', { name: preferenceContent.darkThemeLabel }));
    await user.click(screen.getByRole('switch', { name: preferenceContent.emailLabel }));
    await user.click(screen.getByRole('switch', { name: preferenceContent.inAppLabel }));
    await user.click(screen.getByRole('button', { name: preferenceContent.saveLabel }));

    // Assert
    await waitFor(() => expect(updateUserPreferencesMock).toHaveBeenCalledWith(savedPreference));
    expect(onPreferenceSaved).toHaveBeenCalledWith(savedPreference);
    expect(screen.getByRole('status')).toHaveTextContent(preferenceContent.savedMessage);
  });

  test('UserPreferencesPage_WhenSaveFails_ShowsErrorAndDoesNotReportSavedPreference', async () => {
    // Arrange
    const user = userEvent.setup();
    const onPreferenceSaved = jest.fn();
    updateUserPreferencesMock.mockRejectedValue(new Error('Network unavailable'));
    render(
      <UserPreferencesPage
        preference={defaultPreference}
        loadingIndicator={false}
        loadErrorIndicator={false}
        onPreferenceSaved={onPreferenceSaved}
      />
    );

    // Act
    await user.click(screen.getByRole('button', { name: preferenceContent.saveLabel }));

    // Assert
    expect(await screen.findByRole('alert')).toHaveTextContent(
      `${preferenceContent.saveErrorTitle}: Network unavailable`
    );
    expect(onPreferenceSaved).not.toHaveBeenCalled();
  });
});

function createEnvelope(item: UserPreference): SingleItemEnvelope<UserPreference> {
  return {
    item,
    metadata: {
      timestamp: '2026-09-30T12:00:00Z',
      transactionId: 'test-transaction',
    },
    links: {
      self: '/v1/user-preferences',
    },
  };
}
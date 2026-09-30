import { render, screen } from '@testing-library/react';
import { ApplicationShell } from './App';
import { getUserPreferences } from './lib/api';
import { preferenceContent } from './lib/content';
import type { SingleItemEnvelope, UserPreference } from './lib/types';

jest.mock('./lib/api', () => ({
  getUserPreferences: jest.fn(),
  updateUserPreferences: jest.fn(),
}));

const getUserPreferencesMock = jest.mocked(getUserPreferences);

describe('ApplicationShell preferences', () => {
  beforeEach(() => {
    window.history.pushState({}, '', '/preferences');
    delete document.documentElement.dataset.theme;
  });

  test('ApplicationShell_WhilePreferencesLoad_ShowsLoadingStateThenAppliesLoadedTheme', async () => {
    // Arrange
    getUserPreferencesMock.mockResolvedValue(createEnvelope({
      displayTheme: 'dark',
      emailNotificationIndicator: false,
      inAppNotificationIndicator: true,
    }));

    // Act
    render(<ApplicationShell />);

    // Assert
    expect(screen.getByRole('status', { name: 'Loading content' })).toBeInTheDocument();
    expect(await screen.findByRole('heading', { name: preferenceContent.pageTitle })).toBeInTheDocument();
    expect(document.documentElement).toHaveAttribute('data-theme', 'dark');
    expect(screen.getByRole('button', { name: preferenceContent.darkThemeLabel })).toHaveAttribute(
      'aria-pressed',
      'true'
    );
    expect(screen.getByRole('switch', { name: preferenceContent.emailLabel })).toHaveAttribute(
      'aria-checked',
      'false'
    );
  });

  test('ApplicationShell_WhenPreferencesLoadFails_ShowsDefaultsAndErrorStatus', async () => {
    // Arrange
    getUserPreferencesMock.mockRejectedValue(new Error('Service unavailable'));

    // Act
    render(<ApplicationShell />);

    // Assert
    expect(await screen.findByText(preferenceContent.loadErrorTitle)).toBeInTheDocument();
    expect(document.documentElement).toHaveAttribute('data-theme', 'light');
    expect(screen.getByRole('button', { name: preferenceContent.lightThemeLabel })).toHaveAttribute(
      'aria-pressed',
      'true'
    );
    expect(screen.getByRole('switch', { name: preferenceContent.emailLabel })).toHaveAttribute(
      'aria-checked',
      'true'
    );
    expect(screen.getByRole('switch', { name: preferenceContent.inAppLabel })).toHaveAttribute(
      'aria-checked',
      'true'
    );
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
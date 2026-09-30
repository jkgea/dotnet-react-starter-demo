import { useEffect, useState, type FormEvent } from 'react';
import { updateUserPreferences } from '../../lib/api';
import { preferenceContent } from '../../lib/content';
import type { DisplayTheme, UserPreference } from '../../lib/types';
import { LoadingSpinner } from '../../components/LoadingSpinner';

interface UserPreferencesPageProps {
  preference: UserPreference;
  loadingIndicator: boolean;
  loadErrorIndicator: boolean;
  onPreferenceSaved: (preference: UserPreference) => void;
}

type SaveState = 'idle' | 'saving' | 'saved' | 'error';

/** Allows the current user to manage display and notification preferences. */
export function UserPreferencesPage({
  preference,
  loadingIndicator,
  loadErrorIndicator,
  onPreferenceSaved,
}: UserPreferencesPageProps) {
  const [draftPreference, setDraftPreference] = useState(preference);
  const [saveState, setSaveState] = useState<SaveState>('idle');
  const [saveError, setSaveError] = useState('');

  useEffect(() => {
    setDraftPreference(preference);
  }, [preference]);

  function selectTheme(displayTheme: DisplayTheme) {
    setDraftPreference((currentPreference) => ({
      ...currentPreference,
      displayTheme,
    }));
    setSaveState('idle');
  }

  function toggleNotification(
    fieldName: 'emailNotificationIndicator' | 'inAppNotificationIndicator'
  ) {
    setDraftPreference((currentPreference) => ({
      ...currentPreference,
      [fieldName]: !currentPreference[fieldName],
    }));
    setSaveState('idle');
  }

  async function savePreferences(event: FormEvent<HTMLFormElement>) {
    event.preventDefault();
    setSaveState('saving');
    setSaveError('');

    try {
      const response = await updateUserPreferences(draftPreference);
      onPreferenceSaved(response.item);
      setSaveState('saved');
    } catch (error) {
      setSaveError(error instanceof Error ? error.message : preferenceContent.saveErrorTitle);
      setSaveState('error');
    }
  }

  if (loadingIndicator) {
    return <LoadingSpinner />;
  }

  return (
    <div className="mx-auto max-w-3xl space-y-8">
      <div>
        <h2 className="text-3xl font-bold text-[var(--color-brand-text)]">
          {preferenceContent.pageTitle}
        </h2>
        <p className="mt-2 text-[var(--color-muted-text)]">
          {preferenceContent.pageDescription}
        </p>
      </div>

      {loadErrorIndicator && (
        <div
          className="border-l-4 border-amber-500 bg-[var(--color-surface)] px-4 py-3 text-[var(--color-brand-text)]"
          role="status"
        >
          <p className="font-semibold">{preferenceContent.loadErrorTitle}</p>
          <p className="text-sm text-[var(--color-muted-text)]">
            {preferenceContent.loadErrorDescription}
          </p>
        </div>
      )}

      <form className="space-y-8" onSubmit={savePreferences}>
        <section className="border-b border-[var(--color-border)] pb-8">
          <h3 className="text-lg font-semibold text-[var(--color-brand-text)]">
            {preferenceContent.appearanceHeading}
          </h3>
          <p className="mt-1 text-sm text-[var(--color-muted-text)]">
            {preferenceContent.appearanceDescription}
          </p>

          <div
            className="mt-5 inline-flex border border-[var(--color-border)] bg-[var(--color-surface-muted)] p-1"
            role="group"
            aria-label={preferenceContent.appearanceHeading}
          >
            {(['light', 'dark'] as const).map((theme) => {
              const selectedIndicator = draftPreference.displayTheme === theme;
              const label = theme === 'light'
                ? preferenceContent.lightThemeLabel
                : preferenceContent.darkThemeLabel;

              return (
                <button
                  key={theme}
                  className={`min-w-24 px-5 py-2 text-sm font-semibold transition-colors ${
                    selectedIndicator
                      ? 'bg-[var(--color-brand-primary)] text-white shadow-sm'
                      : 'text-[var(--color-muted-text)] hover:text-[var(--color-brand-text)]'
                  }`}
                  type="button"
                  aria-pressed={selectedIndicator}
                  disabled={saveState === 'saving'}
                  onClick={() => selectTheme(theme)}
                >
                  {label}
                </button>
              );
            })}
          </div>
        </section>

        <section className="space-y-5 border-b border-[var(--color-border)] pb-8">
          <div>
            <h3 className="text-lg font-semibold text-[var(--color-brand-text)]">
              {preferenceContent.notificationsHeading}
            </h3>
            <p className="mt-1 text-sm text-[var(--color-muted-text)]">
              {preferenceContent.notificationsDescription}
            </p>
          </div>

          <NotificationToggle
            checkedIndicator={draftPreference.emailNotificationIndicator}
            description={preferenceContent.emailDescription}
            disabledIndicator={saveState === 'saving'}
            label={preferenceContent.emailLabel}
            onToggle={() => toggleNotification('emailNotificationIndicator')}
          />
          <NotificationToggle
            checkedIndicator={draftPreference.inAppNotificationIndicator}
            description={preferenceContent.inAppDescription}
            disabledIndicator={saveState === 'saving'}
            label={preferenceContent.inAppLabel}
            onToggle={() => toggleNotification('inAppNotificationIndicator')}
          />
        </section>

        <div className="flex min-h-11 items-center gap-4">
          <button
            className="bg-[var(--color-brand-primary)] px-5 py-2.5 font-semibold text-white hover:brightness-90 disabled:cursor-not-allowed disabled:opacity-60"
            type="submit"
            disabled={saveState === 'saving'}
          >
            {saveState === 'saving'
              ? preferenceContent.savingLabel
              : preferenceContent.saveLabel}
          </button>
          {saveState === 'saved' && (
            <p className="text-sm font-medium text-green-700" role="status">
              {preferenceContent.savedMessage}
            </p>
          )}
          {saveState === 'error' && (
            <p className="text-sm font-medium text-red-700" role="alert">
              {preferenceContent.saveErrorTitle}: {saveError}
            </p>
          )}
        </div>
      </form>
    </div>
  );
}

interface NotificationToggleProps {
  checkedIndicator: boolean;
  description: string;
  disabledIndicator: boolean;
  label: string;
  onToggle: () => void;
}

function NotificationToggle({
  checkedIndicator,
  description,
  disabledIndicator,
  label,
  onToggle,
}: NotificationToggleProps) {
  return (
    <div className="flex items-center justify-between gap-6 bg-[var(--color-surface)] px-4 py-4 shadow-sm">
      <div>
        <p className="font-medium text-[var(--color-brand-text)]">{label}</p>
        <p className="mt-1 text-sm text-[var(--color-muted-text)]">{description}</p>
      </div>
      <button
        className={`relative h-7 w-12 shrink-0 rounded-full border transition-colors ${
          checkedIndicator
            ? 'border-[var(--color-brand-primary)] bg-[var(--color-brand-primary)]'
            : 'border-[var(--color-border)] bg-[var(--color-surface-muted)]'
        }`}
        type="button"
        role="switch"
        aria-checked={checkedIndicator}
        aria-label={label}
        disabled={disabledIndicator}
        onClick={onToggle}
      >
        <span
          className={`absolute top-0.5 h-5 w-5 rounded-full bg-white shadow transition-transform ${
            checkedIndicator ? 'translate-x-5' : 'translate-x-0.5'
          }`}
        />
      </button>
    </div>
  );
}
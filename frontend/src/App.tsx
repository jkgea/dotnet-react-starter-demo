import { useEffect, useState } from 'react';
import { BrowserRouter, Routes, Route, Link } from 'react-router-dom';
import { SystemHealthPage } from './routes/health/HealthPage';
import { UserPreferencesPage } from './routes/preferences/UserPreferencesPage';
import { getUserPreferences } from './lib/api';
import { preferenceContent } from './lib/content';
import type { UserPreference } from './lib/types';

const defaultPreference: UserPreference = {
  displayTheme: 'light',
  emailNotificationIndicator: true,
  inAppNotificationIndicator: true,
};

/**
 * Main application shell with routing and navigation.
 */
export function ApplicationShell() {
  const [preference, setPreference] = useState<UserPreference>(defaultPreference);
  const [preferenceLoadingIndicator, setPreferenceLoadingIndicator] = useState(true);
  const [preferenceLoadErrorIndicator, setPreferenceLoadErrorIndicator] = useState(false);

  useEffect(() => {
    let activeIndicator = true;

    getUserPreferences()
      .then((response) => {
        if (activeIndicator) {
          setPreference(response.item);
        }
      })
      .catch(() => {
        if (activeIndicator) {
          setPreferenceLoadErrorIndicator(true);
        }
      })
      .finally(() => {
        if (activeIndicator) {
          setPreferenceLoadingIndicator(false);
        }
      });

    return () => {
      activeIndicator = false;
    };
  }, []);

  useEffect(() => {
    document.documentElement.dataset.theme = preference.displayTheme;
  }, [preference.displayTheme]);

  return (
    <BrowserRouter>
      <div className="min-h-screen flex flex-col bg-[var(--color-brand-content)] text-[var(--color-brand-text)]">
        <header className="bg-[var(--color-brand-header)] px-6 py-4 shadow-md">
          <nav className="max-w-6xl mx-auto flex items-center justify-between">
            <h1 className="text-white text-xl font-bold tracking-tight">
              Organization Starter
            </h1>
            <ul className="flex gap-6">
              <li>
                <Link
                  to="/"
                  className="text-white hover:text-blue-100 transition-colors"
                >
                  Home
                </Link>
              </li>
              <li>
                <Link
                  to="/health"
                  className="text-white hover:text-blue-100 transition-colors"
                >
                  System Health
                </Link>
              </li>
              <li>
                <Link
                  to="/preferences"
                  className="text-white hover:text-blue-100 transition-colors"
                >
                  {preferenceContent.navigationLabel}
                </Link>
              </li>
            </ul>
          </nav>
        </header>

        <main className="flex-1 max-w-6xl mx-auto w-full px-6 py-8">
          <Routes>
            <Route path="/" element={<WelcomeLanding />} />
            <Route path="/health" element={<SystemHealthPage />} />
            <Route
              path="/preferences"
              element={
                <UserPreferencesPage
                  preference={preference}
                  loadingIndicator={preferenceLoadingIndicator}
                  loadErrorIndicator={preferenceLoadErrorIndicator}
                  onPreferenceSaved={setPreference}
                />
              }
            />
          </Routes>
        </main>

        <footer className="bg-[var(--color-surface-muted)] px-6 py-4 text-center text-sm text-[var(--color-muted-text)]">
          <p>Organization Starter Template &copy; {new Date().getFullYear()}</p>
        </footer>
      </div>
    </BrowserRouter>
  );
}

/** Landing page component */
function WelcomeLanding() {
  return (
    <div className="text-center py-12">
      <h2 className="text-3xl font-bold text-[var(--color-brand-text)] mb-4">
        Welcome to the Starter Template
      </h2>
      <p className="text-lg text-gray-600 mb-8 max-w-2xl mx-auto">
        This application demonstrates the organization development standards
        with a .NET 8 backend and React 18 frontend.
      </p>
      <Link
        to="/health"
        className="inline-block bg-[var(--color-brand-primary)] text-white px-6 py-3 rounded-lg font-medium hover:bg-blue-700 transition-colors"
      >
        Check System Health
      </Link>
    </div>
  );
}

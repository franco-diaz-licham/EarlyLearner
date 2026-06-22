import { UilMicrosoft, UilShieldCheck } from '@iconscout/react-unicons';
import { useEffect } from 'react';
import { Navigate, useSearchParams } from 'react-router-dom';
import LightLogo from '../../../assets/logo.png';
import { AppButton } from '../../../shared/ui/AppButton';
import { useAuth } from '../hooks/useAuth';

const getSafeReturnPath = (value: string | null) => {
  if (!value || !value.startsWith('/') || value.startsWith('//')) return '/home';
  return value;
};

export const LoginPage = () => {
  const [searchParams] = useSearchParams();
  const { isAuthenticated, isAuthReady, interactionInProgress, login, createAccount, routeToAccessNotEnabled } = useAuth();
  const accessNotEnabled = routeToAccessNotEnabled || searchParams.get('access') === 'not-enabled';
  const returnTo = getSafeReturnPath(searchParams.get('returnTo'));

  useEffect(() => {
    if (isAuthenticated) return;
    document.title = 'Sign in | EarlyLearner';
  }, [isAuthenticated]);

  if (isAuthenticated) return <Navigate to={returnTo} replace />;

  const handleLogin = () => {
    void login();
  };

  const handleCreateAccount = () => {
    void createAccount();
  };

  return (
    <main className="flex min-h-screen items-center justify-center bg-brand-background px-4 py-8">
      <section className="grid w-full max-w-6xl overflow-hidden rounded-2xl border border-brand-border bg-white shadow-app-card lg:grid-cols-[1.05fr_0.95fr]">
        <div className="flex flex-col justify-between bg-brand-surface-soft p-8 sm:p-12">
          <div className="flex items-center gap-3">
            <img src={LightLogo} alt="EarlyLearner logo" className="h-11 w-11" />
            <span className="text-xl font-extrabold text-brand-heading">EarlyLearner</span>
          </div>

          <div className="max-w-lg">
            <p className="text-sm font-extrabold uppercase tracking-wide text-brand-primary">Secure family learning workspace</p>
            <h1 className="mt-4 text-4xl font-extrabold leading-tight text-brand-heading sm:text-5xl">Welcome back</h1>
            <p className="mt-5 text-lg leading-8 text-brand-text">Sign in with your Microsoft account to continue planning, recording observations, and tracking school readiness.</p>
          </div>
        </div>

        <div className="flex  flex-col justify-center p-8 sm:p-12">
          <div className="mx-auto w-full max-w-sm">
            <div className="mb-8 flex h-14 w-14 items-center justify-center rounded-md bg-brand-primary-soft text-brand-primary">
              <UilShieldCheck aria-hidden="true" className="h-8 w-8" />
            </div>

            <h2 className="text-2xl font-extrabold text-brand-heading">Log in or create account</h2>
            <p className="mt-3 text-sm leading-6 text-brand-muted">Authentication is handled by Microsoft Entra ID.</p>

            {accessNotEnabled && <div className="mt-6 rounded-md border border-brand-error/30 bg-brand-coral-50 px-4 py-3 text-sm font-semibold text-brand-error">Access is not enabled for this Microsoft tenant.</div>}

            <div className="mt-8 flex flex-col gap-3">
              <AppButton className="w-full" disabled={!isAuthReady || interactionInProgress} onClick={handleLogin}>
                <UilMicrosoft aria-hidden="true" className="h-5 w-5" />
                {interactionInProgress || !isAuthReady ? 'Connecting...' : 'Log in with Microsoft'}
              </AppButton>

              <div className="flex items-center gap-3 ">
                <span className="h-px flex-1 bg-brand-border" />
                <span className="text-xs font-extrabold uppercase text-brand-muted">or</span>
                <span className="h-px flex-1 bg-brand-border" />
              </div>

              <AppButton className="w-full" variant="secondary" disabled={!isAuthReady || interactionInProgress} onClick={handleCreateAccount}>
                Create account
              </AppButton>
            </div>
          </div>
        </div>
      </section>
    </main>
  );
};

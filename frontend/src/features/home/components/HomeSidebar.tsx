import { Link } from 'react-router-dom';
import { UilBookOpen, UilCalendarAlt, UilShieldCheck, UilUsersAlt } from '@iconscout/react-unicons';
import type { HomeModel } from '../types/home.types';

interface HomeSidebarProps {
  home?: HomeModel;
  isLoading: boolean;
}

const formatAge = (dateOfBirth: string) => {
  const birthDate = new Date(dateOfBirth);
  const today = new Date();
  let years = today.getFullYear() - birthDate.getFullYear();
  const monthDelta = today.getMonth() - birthDate.getMonth();
  if (monthDelta < 0 || (monthDelta === 0 && today.getDate() < birthDate.getDate())) years -= 1;
  return years <= 0 ? 'Under 1' : `${String(years)} year${years === 1 ? '' : 's'}`;
};

export const HomeSidebar = ({ home, isLoading }: HomeSidebarProps) => {
  const metrics = home?.metrics ?? [];
  const coverage = home?.outcomeCoverage;
  const children = home?.children ?? [];

  return (
    <div className="space-y-5">
      <section className="rounded-md bg-white p-5 shadow-app-card">
        <div className="flex items-center gap-3">
          <div className="flex h-10 w-10 items-center justify-center rounded-md bg-brand-sky-50 text-brand-sky-500">
            <UilShieldCheck aria-hidden="true" size={20} />
          </div>
          <div>
            <h2 className="text-base font-bold text-brand-heading">Learning Coverage</h2>
            <p className="text-sm text-brand-muted">Outcome use over the last seven days.</p>
          </div>
        </div>

        <div className="mt-5 grid grid-cols-3 gap-2">
          <div className="rounded-md bg-brand-surface-soft p-3 text-center">
            <p className="text-2xl font-bold text-brand-heading">{isLoading ? '-' : (coverage?.activeOutcomeCount ?? 0)}</p>
            <p className="mt-1 text-xs font-semibold text-brand-muted">Active</p>
          </div>
          <div className="rounded-md bg-brand-mint-50 p-3 text-center">
            <p className="text-2xl font-bold text-brand-heading">{isLoading ? '-' : (coverage?.touchedThisWeekCount ?? 0)}</p>
            <p className="mt-1 text-xs font-semibold text-brand-muted">Touched</p>
          </div>
          <div className="rounded-md bg-brand-yellow-50 p-3 text-center">
            <p className="text-2xl font-bold text-brand-heading">{isLoading ? '-' : (coverage?.untouchedActiveOutcomeCount ?? 0)}</p>
            <p className="mt-1 text-xs font-semibold text-brand-muted">Untouched</p>
          </div>
        </div>

        <Link className="mt-4 inline-flex text-sm font-semibold text-brand-primary hover:text-brand-primary-hover" to="/learning">
          Manage outcomes
        </Link>
      </section>

      <section className="rounded-md bg-white p-5 shadow-app-card">
        <div className="flex items-center gap-3">
          <div className="flex h-10 w-10 items-center justify-center rounded-md bg-brand-primary-soft text-brand-primary">
            <UilCalendarAlt aria-hidden="true" size={20} />
          </div>
          <div>
            <h2 className="text-base font-bold text-brand-heading">Household Pulse</h2>
            <p className="text-sm text-brand-muted">Small operational numbers for today.</p>
          </div>
        </div>

        <div className="mt-5 space-y-3">
          {metrics.map((metric) => (
            <div className="flex items-start justify-between gap-4 rounded-md border border-brand-border p-3" key={metric.label}>
              <div>
                <p className="text-sm font-bold text-brand-heading">{metric.label}</p>
                <p className="mt-1 text-xs leading-5 text-brand-muted">{metric.detail}</p>
              </div>
              <p className="text-2xl font-bold text-brand-heading">{metric.value}</p>
            </div>
          ))}
        </div>
      </section>

      <section className="rounded-md bg-white p-5 shadow-app-card">
        <div className="flex items-center gap-3">
          <div className="flex h-10 w-10 items-center justify-center rounded-md bg-brand-lavender-50 text-brand-lavender-500">
            <UilUsersAlt aria-hidden="true" size={20} />
          </div>
          <div>
            <h2 className="text-base font-bold text-brand-heading">Children</h2>
            <p className="text-sm text-brand-muted">{children.length} active in this household</p>
          </div>
        </div>

        <div className="mt-5 space-y-3">
          {children.length === 0 ? (
            <p className="text-sm text-brand-muted">Add children from the household page to start recording learning.</p>
          ) : (
            children.map((child) => (
              <div className="flex items-center gap-3 rounded-md bg-brand-surface-soft p-3" key={child.childId}>
                <div className="flex h-9 w-9 items-center justify-center rounded-md bg-white text-brand-primary">
                  <UilBookOpen aria-hidden="true" size={18} />
                </div>
                <div>
                  <p className="text-sm font-bold text-brand-heading">{child.givenName}</p>
                  <p className="text-xs text-brand-muted">{formatAge(child.dateOfBirth)}</p>
                </div>
              </div>
            ))
          )}
        </div>
      </section>
    </div>
  );
};

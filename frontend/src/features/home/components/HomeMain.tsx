import { Link } from 'react-router-dom';
import { UilBookOpen, UilCalendarAlt, UilFileAlt, UilPlus } from '@iconscout/react-unicons';
import { AppHero } from './AppHero';
import { AppStatusBadge } from '../../../shared/ui/AppStatusBadge';
import type { HomeModel, HomeRecentMomentModel } from '../types/home.types';

interface HomeMainProps {
  home?: HomeModel;
  isLoading: boolean;
}

const formatDate = (value: string) =>
  new Intl.DateTimeFormat('en-AU', {
    day: '2-digit',
    month: 'short'
  }).format(new Date(value));

const getKindLabel = (kind: string) => {
  if (kind === 'activity') return 'Activity';
  if (kind === 'observation') return 'Observation';
  if (kind === 'reading') return 'Reading';
  if (kind === 'routine') return 'Routine';
  return kind;
};

const getKindTone = (kind: string) => {
  if (kind === 'routine') return 'neutral';
  if (kind === 'observation') return 'success';
  return 'warning';
};

const RecentMoment = ({ moment }: { moment: HomeRecentMomentModel }) => (
  <article className="grid grid-cols-[44px_minmax(0,1fr)] gap-4 rounded-md border border-brand-border bg-white p-4">
    <div className="flex h-11 w-11 items-center justify-center rounded-md bg-rose-100 text-brand-primary">
      <UilBookOpen aria-hidden="true" size={20} />
    </div>
    <div className="min-w-0">
      <div className="flex flex-wrap items-start justify-between gap-2">
        <div>
          <h3 className="font-bold text-brand-heading">{moment.title}</h3>
          <p className="mt-1 text-xs font-semibold text-brand-muted">
            {moment.childName} · {formatDate(moment.logDate)}
          </p>
        </div>
        <AppStatusBadge tone={getKindTone(moment.kind)}>{getKindLabel(moment.kind)}</AppStatusBadge>
      </div>
      <p className="mt-2 line-clamp-2 text-sm leading-6 text-brand-muted">{moment.notes}</p>
      {moment.outcomeNames.length > 0 ? (
        <div className="mt-3 flex flex-wrap gap-2">
          {moment.outcomeNames.slice(0, 3).map((outcome) => (
            <span className="rounded-md bg-brand-surface-soft px-2.5 py-1 text-xs font-semibold text-brand-muted" key={outcome}>
              {outcome}
            </span>
          ))}
        </div>
      ) : null}
    </div>
  </article>
);

export const HomeMain = ({ home, isLoading }: HomeMainProps) => {
  const today = home?.today;
  const recentMoments = home?.recentMoments ?? [];

  return (
    <div className="space-y-5">
      <AppHero />

      <section className="rounded-md bg-white p-5 shadow-app-card">
        <div className="flex flex-wrap items-start justify-between gap-3">
          <div>
            <h2 className="text-base font-bold text-brand-heading">Today</h2>
            <p className="mt-1 text-sm text-brand-muted">A quick pulse of learning captured today.</p>
          </div>
          <Link className="inline-flex min-h-10 items-center justify-center gap-2 rounded-md bg-brand-primary px-4 text-sm font-semibold text-white hover:bg-brand-primary-hover" to="/learning">
            <UilPlus aria-hidden="true" className="h-4 w-4" />
            Add log
          </Link>
        </div>

        <div className="mt-5 grid gap-3 sm:grid-cols-3">
          <div className="rounded-md bg-brand-primary-soft p-4">
            <p className="text-3xl font-bold text-brand-heading">{isLoading ? '-' : (today?.dailyLogCount ?? 0)}</p>
            <p className="mt-1 text-sm font-semibold text-brand-muted">Daily logs</p>
          </div>
          <div className="rounded-md bg-brand-sky-50 p-4">
            <p className="text-3xl font-bold text-brand-heading">{isLoading ? '-' : (today?.learningMomentCount ?? 0)}</p>
            <p className="mt-1 text-sm font-semibold text-brand-muted">Learning moments</p>
          </div>
          <div className="rounded-md bg-brand-mint-50 p-4">
            <p className="text-3xl font-bold text-brand-heading">{isLoading ? '-' : (today?.childrenObservedCount ?? 0)}</p>
            <p className="mt-1 text-sm font-semibold text-brand-muted">Children observed</p>
          </div>
        </div>
      </section>

      <section className="rounded-md bg-white p-5 shadow-app-card">
        <div className="flex flex-wrap items-center justify-between gap-3">
          <div>
            <h2 className="text-base font-bold text-brand-heading">Recent Learning</h2>
            <p className="mt-1 text-sm text-brand-muted">Latest moments from the household learning stream.</p>
          </div>
          <Link className="text-sm font-semibold text-brand-primary hover:text-brand-primary-hover" to="/learning">
            View all
          </Link>
        </div>

        <div className="mt-5 space-y-3">
          {recentMoments.length === 0 ? (
            <div className="flex items-center gap-4 rounded-md border border-dashed border-brand-border p-5">
              <div className="flex h-12 w-12 items-center justify-center rounded-md bg-brand-surface-soft text-brand-muted">
                <UilFileAlt aria-hidden="true" size={22} />
              </div>
              <div>
                <h3 className="font-bold text-brand-heading">No learning moments yet</h3>
                <p className="text-sm text-brand-muted">Add the first learning log when something worth remembering happens.</p>
              </div>
            </div>
          ) : (
            recentMoments.map((moment) => <RecentMoment key={moment.learningMomentId} moment={moment} />)
          )}
        </div>
      </section>

      <section className="flex items-center gap-4 rounded-md bg-white p-5 shadow-app-card">
        <div className="flex h-14 w-14 items-center justify-center rounded-md bg-orange-100 text-orange-500">
          <UilCalendarAlt aria-hidden="true" size={24} />
        </div>
        <div>
          <h2 className="font-bold text-brand-heading">Use Home for the pulse, Learning for the work</h2>
          <p className="text-sm text-brand-muted">The dashboard highlights what changed. The Learning page manages logs, outcomes, filters, and history.</p>
        </div>
      </section>
    </div>
  );
};

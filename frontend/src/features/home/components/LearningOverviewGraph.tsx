import { Link } from 'react-router-dom';
import { UilFileAlt } from '@iconscout/react-unicons';
import type { HomeOutcomeCoverageModel, HomeRecentMomentModel } from '../types/home.types';

interface LearningOverviewGraphProps {
  isLoading: boolean;
  outcomeCoverage?: HomeOutcomeCoverageModel;
  recentMoments: HomeRecentMomentModel[];
}

const learningKindClassNames: Record<string, string> = {
  activity: 'bg-brand-primary',
  observation: 'bg-brand-success',
  reading: 'bg-brand-info',
  routine: 'bg-brand-warning'
};

const getKindLabel = (kind: string) => {
  if (kind === 'activity') return 'Activity';
  if (kind === 'observation') return 'Observation';
  if (kind === 'reading') return 'Reading';
  if (kind === 'routine') return 'Routine';
  return kind;
};

export const LearningOverviewGraph = ({ isLoading, outcomeCoverage, recentMoments }: LearningOverviewGraphProps) => {
  const totalRecentMoments = recentMoments.length;
  const touchedOutcomePercent = outcomeCoverage?.activeOutcomeCount ? Math.round((outcomeCoverage.touchedThisWeekCount / outcomeCoverage.activeOutcomeCount) * 100) : 0;
  const untouchedOutcomePercent = outcomeCoverage?.activeOutcomeCount ? Math.round((outcomeCoverage.untouchedActiveOutcomeCount / outcomeCoverage.activeOutcomeCount) * 100) : 0;
  const learningKindCounts = ['activity', 'observation', 'reading', 'routine'].map((kind) => ({
    count: recentMoments.filter((moment) => moment.kind === kind).length,
    kind
  }));

  return (
    <div className="rounded-md bg-white p-5 shadow-app-card">
      <div className="flex flex-wrap items-center justify-between gap-3">
        <div>
          <h2 className="text-base font-bold text-brand-heading">Learning Overview</h2>
          <p className="mt-1 text-sm text-brand-muted">A quick read of learning mix and outcome coverage.</p>
        </div>
        <Link className="text-sm font-semibold text-brand-primary hover:text-brand-primary-hover" to="/learning">
          View all
        </Link>
      </div>

      {totalRecentMoments === 0 ? (
        <div className="mt-5 flex items-center gap-4 rounded-md border border-dashed border-brand-border p-5">
          <div className="flex h-12 w-12 items-center justify-center rounded-md bg-brand-surface-soft text-brand-muted">
            <UilFileAlt aria-hidden="true" size={22} />
          </div>
          <div>
            <h3 className="font-bold text-brand-heading">No learning overview yet</h3>
            <p className="text-sm text-brand-muted">Add learning logs to build a household learning pulse.</p>
          </div>
        </div>
      ) : (
        <div className="mt-5 grid gap-4 lg:grid-cols-[minmax(0,1fr)_18rem]">
          <div className="rounded-md border border-brand-border p-4">
            <div className="flex items-center justify-between gap-3">
              <h3 className="font-bold text-brand-heading">Learning Mix</h3>
              <span className="text-sm font-semibold text-brand-muted">{totalRecentMoments} moments</span>
            </div>
            <div className="mt-4 space-y-4">
              {learningKindCounts.map(({ count, kind }) => {
                const width = totalRecentMoments ? Math.round((count / totalRecentMoments) * 100) : 0;

                return (
                  <div key={kind}>
                    <div className="mb-1 flex items-center justify-between text-sm">
                      <span className="font-semibold text-brand-heading">{getKindLabel(kind)}</span>
                      <span className="font-semibold text-brand-muted">{count}</span>
                    </div>
                    <div className="h-3 overflow-hidden rounded-full bg-brand-surface-muted">
                      <div className={`h-full rounded-full ${learningKindClassNames[kind]}`} style={{ width: `${width.toString()}%` }} />
                    </div>
                  </div>
                );
              })}
            </div>
          </div>

          <div className="rounded-md border border-brand-border p-4">
            <h3 className="font-bold text-brand-heading">Outcome Coverage</h3>
            <div className="mt-4 grid grid-cols-2 gap-3">
              <div className="rounded-md bg-brand-mint-50 p-3">
                <p className="text-2xl font-bold text-brand-heading">{isLoading ? '-' : (outcomeCoverage?.touchedThisWeekCount ?? 0)}</p>
                <p className="mt-1 text-xs font-semibold text-brand-muted">Used this week</p>
              </div>
              <div className="rounded-md bg-brand-yellow-50 p-3">
                <p className="text-2xl font-bold text-brand-heading">{isLoading ? '-' : (outcomeCoverage?.untouchedActiveOutcomeCount ?? 0)}</p>
                <p className="mt-1 text-xs font-semibold text-brand-muted">Not used</p>
              </div>
            </div>
            <div className="mt-4 overflow-hidden rounded-full bg-brand-surface-muted">
              <div className="flex h-3">
                <div className="bg-brand-success" style={{ width: `${touchedOutcomePercent.toString()}%` }} />
                <div className="bg-brand-warning" style={{ width: `${untouchedOutcomePercent.toString()}%` }} />
              </div>
            </div>
            <p className="mt-3 text-sm font-semibold text-brand-muted">{outcomeCoverage?.activeOutcomeCount ?? 0} active outcomes</p>
          </div>
        </div>
      )}
    </div>
  );
};


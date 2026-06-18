import { useState } from 'react';
import { UilBookOpen, UilCalendarAlt, UilPlus } from '@iconscout/react-unicons';
import { InputTextarea } from 'primereact/inputtextarea';
import { AppButton } from '../../../shared/ui/AppButton';
import { AppCard } from '../../../shared/ui/AppCard';
import { AppStatusBadge } from '../../../shared/ui/AppStatusBadge';

const weeklyFocus = [
  { title: 'Butterfly life cycle poster', outcome: 'Science, sequencing and fine motor', status: 'Ready' },
  { title: 'Count the snack bowls', outcome: 'Early numeracy and routines', status: 'Simple' },
  { title: 'Story retell with pictures', outcome: 'Communication and early literacy', status: 'Shared' }
] as const;

const goals = [
  { title: 'Use longer sentences during play', type: 'Short term', dateRange: 'This week' },
  { title: 'Build confidence joining group activities', type: 'Long term', dateRange: 'June to August' }
] as const;

export const PlanningPage = () => {
  const [selectedFocus, setSelectedFocus] = useState<string>(weeklyFocus[0].title);
  const [parentNote, setParentNote] = useState('');

  return (
    <section aria-labelledby="planning-title" className="space-y-5">
      <div className="grid gap-5 xl:grid-cols-[minmax(0,1fr)_360px]">
        <div className="space-y-5">
          <AppCard>
            <div className="flex flex-col gap-4 sm:flex-row sm:items-start sm:justify-between">
              <div>
                <p className="text-sm font-semibold text-brand-muted">Planning</p>
                <h1 id="planning-title" className="mt-1 text-3xl font-bold text-brand-heading">
                  Shape the week around Sophia
                </h1>
                <p className="mt-2 max-w-2xl text-brand-muted">Choose a gentle focus, keep goals visible, and turn everyday moments into learning opportunities.</p>
              </div>
              <AppButton icon={<UilPlus aria-hidden="true" className="h-5 w-5" />} label="New plan" />
            </div>
          </AppCard>

          <AppCard title="This Week's Plan">
            <div className="grid gap-3 md:grid-cols-3">
              {weeklyFocus.map((item) => {
                const isSelected = item.title === selectedFocus;

                return (
                  <button
                    className={`rounded-md border p-4 text-left transition ${isSelected ? 'border-brand-primary bg-brand-primary-soft' : 'border-brand-border bg-white hover:bg-brand-surface-muted'}`}
                    key={item.title}
                    type="button"
                    onClick={() => {
                      setSelectedFocus(item.title);
                    }}
                  >
                    <div className="flex items-center justify-between gap-3">
                      <UilBookOpen aria-hidden="true" className="h-5 w-5 text-brand-primary" />
                      <AppStatusBadge tone={isSelected ? 'warning' : 'neutral'}>{item.status}</AppStatusBadge>
                    </div>
                    <h2 className="mt-4 font-bold text-brand-heading">{item.title}</h2>
                    <p className="mt-2 text-sm leading-6 text-brand-muted">{item.outcome}</p>
                  </button>
                );
              })}
            </div>
          </AppCard>

          <AppCard title="Parent Note">
            <InputTextarea
              autoResize
              className="min-h-32 w-full rounded-md border border-brand-border px-4 py-3 text-brand-text placeholder:text-brand-muted"
              placeholder="Add what worked, what felt hard, or what Sophia asked about..."
              rows={5}
              value={parentNote}
              onChange={(event) => {
                setParentNote(event.target.value);
              }}
            />
            <div className="mt-4 flex justify-end">
              <AppButton disabled={!parentNote.trim()} label="Save note" />
            </div>
          </AppCard>
        </div>

        <aside className="space-y-5">
          <AppCard title="Active Goals">
            <div className="space-y-4">
              {goals.map((goal) => (
                <article className="rounded-md border border-brand-border p-4" key={goal.title}>
                  <div className="flex items-center justify-between gap-3">
                    <AppStatusBadge>{goal.type}</AppStatusBadge>
                    <span className="flex items-center gap-1 text-xs font-semibold text-brand-muted">
                      <UilCalendarAlt aria-hidden="true" className="h-4 w-4" />
                      {goal.dateRange}
                    </span>
                  </div>
                  <h2 className="mt-3 font-bold text-brand-heading">{goal.title}</h2>
                </article>
              ))}
            </div>
          </AppCard>
        </aside>
      </div>
    </section>
  );
};

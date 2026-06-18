import { useState } from 'react';
import { UilShieldCheck } from '@iconscout/react-unicons';
import { Checkbox } from 'primereact/checkbox';
import { ProgressBar } from 'primereact/progressbar';
import { AppButton } from '../../../shared/ui/AppButton';
import { AppCard } from '../../../shared/ui/AppCard';
import { AppStatusBadge } from '../../../shared/ui/AppStatusBadge';

const readinessAreas = [
  { name: 'Communication', detail: 'Uses words, gestures and stories to share ideas.', status: 'Growing' },
  { name: 'Social skills', detail: 'Joins play, takes turns and notices others.', status: 'Growing' },
  { name: 'Emotional regulation', detail: 'Names feelings and accepts help to reset.', status: 'Emerging' },
  { name: 'Early literacy', detail: 'Enjoys books, sounds, marks and retelling.', status: 'Growing' },
  { name: 'Fine motor skills', detail: 'Builds hand strength through drawing, cutting and play.', status: 'Emerging' }
] as const;

export const ReadinessPage = () => {
  const [selectedAreas, setSelectedAreas] = useState<string[]>(['Communication', 'Social skills', 'Early literacy']);
  const progress = Math.round((selectedAreas.length / readinessAreas.length) * 100);

  const toggleArea = (area: string) => {
    setSelectedAreas((current) => (current.includes(area) ? current.filter((item) => item !== area) : [...current, area]));
  };

  return (
    <section aria-labelledby="readiness-title" className="space-y-5">
      <AppCard>
        <div className="flex flex-col gap-4 sm:flex-row sm:items-start sm:justify-between">
          <div>
            <p className="text-sm font-semibold text-brand-muted">Readiness</p>
            <h1 id="readiness-title" className="mt-1 text-3xl font-bold text-brand-heading">
              Notice growth without rushing it
            </h1>
            <p className="mt-2 max-w-2xl text-brand-muted">Track the readiness outcomes that matter this season and choose one small thing to support next.</p>
          </div>
          <AppButton icon={<UilShieldCheck aria-hidden="true" className="h-5 w-5" />} label="Review profile" />
        </div>
      </AppCard>

      <div className="grid gap-5 xl:grid-cols-[minmax(0,1fr)_360px]">
        <AppCard title="Readiness Areas">
          <div className="space-y-3">
            {readinessAreas.map((area) => {
              const isSelected = selectedAreas.includes(area.name);

              return (
                <label className="flex cursor-pointer items-start gap-4 rounded-md border border-brand-border p-4 hover:bg-brand-surface-muted" key={area.name}>
                  <Checkbox
                    checked={isSelected}
                    inputId={area.name}
                    onChange={() => {
                      toggleArea(area.name);
                    }}
                  />
                  <span className="min-w-0 flex-1">
                    <span className="flex items-center justify-between gap-3">
                      <span className="font-bold text-brand-heading">{area.name}</span>
                      <AppStatusBadge tone={area.status === 'Growing' ? 'success' : 'warning'}>{area.status}</AppStatusBadge>
                    </span>
                    <span className="mt-1 block text-sm leading-6 text-brand-muted">{area.detail}</span>
                  </span>
                </label>
              );
            })}
          </div>
        </AppCard>

        <aside className="space-y-5">
          <AppCard title="Profile Snapshot">
            <p className="text-sm text-brand-muted">Selected outcomes</p>
            <p className="mt-2 text-4xl font-bold text-brand-heading">
              {selectedAreas.length}/{readinessAreas.length}
            </p>
            <ProgressBar className="mt-4" value={progress} />
            <p className="mt-4 text-sm leading-6 text-brand-muted">School readiness is a journey, not a race. Pick one area, then support it through play.</p>
          </AppCard>

          <AppCard title="Suggested Next Step">
            <h2 className="text-lg font-bold text-brand-heading">Invite Sophia to explain her drawing</h2>
            <p className="mt-2 text-sm leading-6 text-brand-muted">Great for communication, confidence and early literacy.</p>
            <AppButton className="mt-4" label="Use this idea" />
          </AppCard>
        </aside>
      </div>
    </section>
  );
};

import { useState } from 'react';
import type { SelectButtonChangeEvent } from 'primereact/selectbutton';
import { UilBookOpen, UilCalendarAlt, UilClipboardNotes } from '@iconscout/react-unicons';
import { InputTextarea } from 'primereact/inputtextarea';
import { SelectButton } from 'primereact/selectbutton';
import { AppButton } from '../../../shared/ui/AppButton';
import { AppCard } from '../../../shared/ui/AppCard';
import { AppStatusBadge } from '../../../shared/ui/AppStatusBadge';

const logTypes = ['Moment', 'Reading', 'Routine'];

const recentLogs = [
  { title: 'Butterfly craft', detail: 'Sophia described each wing and chose matching colours.', type: 'Moment', count: '1 activity' },
  { title: 'Bedtime reading', detail: 'Read together and paused to guess what would happen next.', type: 'Reading', count: '2 entries' },
  { title: 'Packed bag together', detail: 'Practised sequencing: lunchbox, hat, drink bottle.', type: 'Routine', count: '1 routine' }
] as const;

export const LearningPage = () => {
  const [logType, setLogType] = useState(logTypes[0]);
  const [note, setNote] = useState('');

  const handleLogTypeChange = (event: SelectButtonChangeEvent) => {
    if (typeof event.value === 'string') setLogType(event.value);
  };

  return (
    <section aria-labelledby="learning-title" className="space-y-5">
      <AppCard>
        <div className="flex flex-col gap-4 sm:flex-row sm:items-start sm:justify-between">
          <div>
            <p className="text-sm font-semibold text-brand-muted">Learning</p>
            <h1 id="learning-title" className="mt-1 text-3xl font-bold text-brand-heading">
              Capture the small moments
            </h1>
            <p className="mt-2 max-w-2xl text-brand-muted">Record activities, reading and routines while they are fresh, then use them to shape the next plan.</p>
          </div>
          <AppButton icon={<UilClipboardNotes aria-hidden="true" className="h-5 w-5" />} label="Add log" />
        </div>
      </AppCard>

      <div className="grid gap-5 xl:grid-cols-[minmax(0,1fr)_360px]">
        <AppCard title="Today's Learning Log">
          <div className="space-y-4">
            <SelectButton options={logTypes} value={logType} onChange={handleLogTypeChange} />
            <InputTextarea
              autoResize
              className="min-h-36 w-full rounded-md border border-brand-border px-4 py-3 text-brand-text placeholder:text-brand-muted"
              placeholder="What happened? What did Sophia try, say, notice, or enjoy?"
              rows={6}
              value={note}
              onChange={(event) => {
                setNote(event.target.value);
              }}
            />
            <div className="flex justify-end">
              <AppButton disabled={!note.trim()} label={`Save ${logType.toLowerCase()}`} />
            </div>
          </div>
        </AppCard>

        <aside className="space-y-5">
          <AppCard title="Today at a Glance">
            <div className="grid grid-cols-3 gap-3 text-center">
              <div className="rounded-md bg-brand-surface-muted p-3">
                <p className="text-2xl font-bold text-brand-heading">3</p>
                <p className="text-xs font-semibold text-brand-muted">Activities</p>
              </div>
              <div className="rounded-md bg-brand-surface-muted p-3">
                <p className="text-2xl font-bold text-brand-heading">2</p>
                <p className="text-xs font-semibold text-brand-muted">Reading</p>
              </div>
              <div className="rounded-md bg-brand-surface-muted p-3">
                <p className="text-2xl font-bold text-brand-heading">1</p>
                <p className="text-xs font-semibold text-brand-muted">Routine</p>
              </div>
            </div>
          </AppCard>
        </aside>
      </div>

      <AppCard title="Recent Learning">
        <div className="grid gap-4 lg:grid-cols-3">
          {recentLogs.map((log) => (
            <article className="rounded-md border border-brand-border p-4" key={log.title}>
              <div className="flex items-center justify-between gap-3">
                <div className="flex h-10 w-10 items-center justify-center rounded-md bg-brand-primary-soft text-brand-primary">
                  {log.type === 'Reading' ? <UilBookOpen aria-hidden="true" className="h-5 w-5" /> : <UilCalendarAlt aria-hidden="true" className="h-5 w-5" />}
                </div>
                <AppStatusBadge>{log.type}</AppStatusBadge>
              </div>
              <h2 className="mt-4 font-bold text-brand-heading">{log.title}</h2>
              <p className="mt-2 text-sm leading-6 text-brand-muted">{log.detail}</p>
              <p className="mt-4 text-xs font-semibold text-brand-muted">{log.count}</p>
            </article>
          ))}
        </div>
      </AppCard>
    </section>
  );
};

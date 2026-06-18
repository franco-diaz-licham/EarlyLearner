import { UilBookOpen, UilCalendarAlt, UilShieldCheck } from '@iconscout/react-unicons';
import { AppStatusBadge } from '../../../shared/ui/AppStatusBadge';

const readinessAreas = [
  { label: 'Communication', status: 'Growing', tone: 'success' },
  { label: 'Social skills', status: 'Growing', tone: 'success' },
  { label: 'Emotional regulation', status: 'Emerging', tone: 'warning' },
  { label: 'Early literacy', status: 'Growing', tone: 'success' },
  { label: 'Fine motor skills', status: 'Emerging', tone: 'warning' }
] as const;

const events = [
  { month: 'May', day: '23', title: 'Library story time', detail: 'Friday, 10:30am' },
  { month: 'May', day: '28', title: 'Playdate with Mia', detail: 'Wednesday, 9:30am' },
  { month: 'Jun', day: '05', title: 'Zoo excursion', detail: 'Thursday, 9:00am' }
];
export const HomeSidebar = () => {
  return (
    <div className="space-y-5">
      <section className="rounded-md bg-white p-6 shadow-app-card">
        <div className="flex items-center justify-between">
          <h2 className="text-base font-bold text-brand-heading">School Readiness</h2>
          <button className="text-sm font-semibold text-[#ef7676]" type="button">
            View all
          </button>
        </div>
        <div className="mt-5 space-y-4">
          {readinessAreas.map((area) => (
            <div className="flex items-center justify-between gap-3" key={area.label}>
              <div className="flex items-center gap-3">
                <div className="flex h-8 w-8 items-center justify-center rounded-md bg-brand-sky-50 text-brand-sky-500">
                  <UilShieldCheck aria-hidden="true" size={17} />
                </div>
                <span className="text-sm font-semibold text-brand-heading">{area.label}</span>
              </div>
              <AppStatusBadge tone={area.tone}>{area.status}</AppStatusBadge>
            </div>
          ))}
        </div>
        <p className="mt-6 text-sm text-brand-muted">School readiness is a journey, not a race.</p>
      </section>

      <section className="rounded-md bg-white p-6 shadow-app-card">
        <div className="flex items-center justify-between">
          <h2 className="text-base font-bold text-brand-heading">Upcoming Events</h2>
          <button className="text-sm font-semibold text-[#ef7676]" type="button">
            View calendar
          </button>
        </div>
        <div className="mt-5 divide-y divide-brand-border">
          {events.map((event) => (
            <article className="flex items-center gap-4 py-4 first:pt-0 last:pb-0" key={`${event.month}-${event.day}`}>
              <div className="w-14 rounded-md bg-rose-50 py-2 text-center">
                <p className="text-xs font-bold uppercase text-[#ef7676]">{event.month}</p>
                <p className="text-xl font-bold text-brand-heading">{event.day}</p>
              </div>
              <div>
                <h3 className="font-bold text-brand-heading">{event.title}</h3>
                <p className="text-sm text-brand-muted">{event.detail}</p>
              </div>
            </article>
          ))}
        </div>
      </section>

      <section className="rounded-md bg-[#f5efff] p-6 shadow-app-card">
        <div className="flex items-center gap-3">
          <div className="flex h-12 w-12 items-center justify-center rounded-md bg-white/80 text-brand-lavender-500">
            <UilBookOpen aria-hidden="true" size={22} />
          </div>
          <h2 className="text-base font-bold text-brand-heading">Suggested Next Activity</h2>
        </div>
        <h3 className="mt-6 text-xl font-bold leading-7 text-brand-heading">Create a butterfly life cycle poster</h3>
        <p className="mt-3 text-sm leading-6 text-brand-muted">Great for science, talking and fine motor skills.</p>
        <button className="mt-5 rounded-md bg-white/80 px-6 py-3 text-sm font-bold text-brand-lavender-500" type="button">
          See idea
        </button>
      </section>

      <section className="rounded-md bg-white p-6 shadow-app-card">
        <div className="flex items-center gap-4">
          <div className="flex h-11 w-11 items-center justify-center rounded-md bg-brand-sky-50 text-brand-sky-500">
            <UilCalendarAlt aria-hidden="true" size={21} />
          </div>
          <div>
            <h2 className="font-bold text-brand-heading">Today feels open</h2>
            <p className="text-sm text-brand-muted">Add one small moment when it happens.</p>
          </div>
        </div>
      </section>
    </div>
  );
};

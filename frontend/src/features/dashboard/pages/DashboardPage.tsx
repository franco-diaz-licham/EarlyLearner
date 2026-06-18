import { UilBookOpen, UilCalendarAlt, UilEstate, UilFileAlt, UilPlus, UilShieldCheck } from '@iconscout/react-unicons';
import dashboardHero from '../../../assets/dashboard-hero.png';
import { AppButton } from '../../../shared/ui/AppButton';
import { AppStatusBadge } from '../../../shared/ui/AppStatusBadge';

const readinessAreas = [
  { label: 'Communication', status: 'Growing', tone: 'success' },
  { label: 'Social skills', status: 'Growing', tone: 'success' },
  { label: 'Emotional regulation', status: 'Emerging', tone: 'warning' },
  { label: 'Early literacy', status: 'Growing', tone: 'success' },
  { label: 'Fine motor skills', status: 'Emerging', tone: 'warning' }
] as const;

const focusItems = [
  { title: 'Exploring insects', detail: 'Science & discovery', iconClassName: 'bg-green-100 text-green-700' },
  { title: 'Counting & numbers', detail: 'Numeracy', iconClassName: 'bg-lime-100 text-lime-700' },
  { title: 'Scissor skills', detail: 'Fine motor', iconClassName: 'bg-amber-100 text-amber-700' }
];

const moments = [
  { title: 'Butterfly craft', detail: 'Sophia loved making a butterfly using coloured paper and glue.', when: 'Today', tone: 'warning' },
  { title: 'Backyard bug hunt', detail: 'We looked for bugs and talked about their colours and shapes.', when: 'Yesterday', tone: 'success' },
  { title: 'The Very Hungry Caterpillar', detail: 'We read together and counted the yummy foods.', when: 'Yesterday', tone: 'warning' }
] as const;

const events = [
  { month: 'May', day: '23', title: 'Library story time', detail: 'Friday, 10:30am' },
  { month: 'May', day: '28', title: 'Playdate with Mia', detail: 'Wednesday, 9:30am' },
  { month: 'Jun', day: '05', title: 'Zoo excursion', detail: 'Thursday, 9:00am' }
];

const interests = ['Butterflies', 'Bugs & insects', 'Arts & crafts'];

export const DashboardPage = () => (
  <section aria-labelledby="dashboard-title" className="space-y-5">
    <header className="flex items-center justify-between rounded-md border border-white/80 bg-white/80 px-4 py-3 shadow-app-card backdrop-blur">
      <div className="flex items-center gap-3">
        <div className="flex h-10 w-10 items-center justify-center rounded-md bg-rose-100 text-rose-500">
          <UilShieldCheck aria-hidden="true" size={22} />
        </div>
        <div>
          <p className="text-base font-bold text-brand-heading">EarlyLearner</p>
          <p className="text-xs text-brand-muted">Sophia, 4y 2m</p>
        </div>
      </div>
      <AppButton className="bg-[#ef7676] hover:bg-[#dc5f5f]">
        <UilPlus aria-hidden="true" size={18} />
        Log a moment
      </AppButton>
    </header>

    <div className="grid gap-5 xl:grid-cols-[minmax(0,1fr)_390px]">
      <div className="space-y-5">
        <section className="relative min-h-[280px] overflow-hidden rounded-md bg-white shadow-app-card">
          <img alt="" className="absolute inset-0 h-full w-full object-cover" src={dashboardHero} />
          <div className="absolute inset-0 bg-gradient-to-r from-white via-white/80 to-white/5" />
          <div className="relative max-w-xl px-6 py-10 sm:px-10">
            <h1 id="dashboard-title" className="text-4xl font-bold leading-tight text-brand-heading sm:text-5xl">
              Good morning, Franco!
            </h1>
            <p className="mt-4 max-w-sm text-lg leading-8 text-brand-muted">Sophia is curious, creative and growing every day.</p>
          </div>
        </section>

        <div className="grid gap-5 lg:grid-cols-[310px_minmax(0,1fr)]">
          <div className="space-y-5">
            <section className="rounded-md bg-white p-6 shadow-app-card">
              <div className="flex items-center justify-between">
                <h2 className="text-base font-bold text-brand-heading">This Week's Focus</h2>
                <button className="text-sm font-semibold text-[#ef7676]" type="button">
                  Edit
                </button>
              </div>
              <div className="mt-6 space-y-5">
                {focusItems.map((item) => (
                  <div className="flex items-center gap-4" key={item.title}>
                    <div className={`flex h-10 w-10 items-center justify-center rounded-md ${item.iconClassName}`}>
                      <UilEstate aria-hidden="true" size={20} />
                    </div>
                    <div>
                      <p className="font-semibold text-brand-heading">{item.title}</p>
                      <p className="text-sm text-brand-muted">{item.detail}</p>
                    </div>
                  </div>
                ))}
              </div>
            </section>

            <section className="rounded-md bg-white p-6 shadow-app-card">
              <div className="flex items-center justify-between">
                <h2 className="text-base font-bold text-brand-heading">Child Interests</h2>
                <button className="text-sm font-semibold text-[#ef7676]" type="button">
                  View all
                </button>
              </div>
              <div className="mt-5 space-y-3">
                {interests.map((interest) => (
                  <div className="rounded-md bg-[#fff2db] px-4 py-3 text-sm font-semibold text-brand-heading" key={interest}>
                    {interest}
                  </div>
                ))}
              </div>
            </section>
          </div>

          <section className="rounded-md bg-white p-6 shadow-app-card">
            <div className="flex items-center justify-between">
              <h2 className="text-base font-bold text-brand-heading">Recent Moments</h2>
              <button className="text-sm font-semibold text-[#ef7676]" type="button">
                View all
              </button>
            </div>
            <div className="mt-4 flex flex-wrap gap-2">
              {['All', 'Activities', 'Outings', 'Reading'].map((filter) => (
                <button className="rounded-full bg-brand-surface-muted px-4 py-2 text-xs font-semibold text-brand-muted first:bg-rose-100 first:text-[#ef7676]" key={filter} type="button">
                  {filter}
                </button>
              ))}
            </div>
            <div className="mt-6 space-y-5">
              {moments.map((moment) => (
                <article className="grid grid-cols-[44px_minmax(0,1fr)_auto] items-start gap-4" key={moment.title}>
                  <div className="flex h-11 w-11 items-center justify-center rounded-md bg-rose-100 text-[#ef7676]">
                    <UilBookOpen aria-hidden="true" size={20} />
                  </div>
                  <div>
                    <h3 className="font-bold text-brand-heading">{moment.title}</h3>
                    <p className="mt-1 max-w-md text-sm leading-6 text-brand-muted">{moment.detail}</p>
                  </div>
                  <div className="flex items-start gap-3">
                    <div className="text-right">
                      <p className="text-xs text-brand-muted">{moment.when}</p>
                      <div className="mt-2">
                        <AppStatusBadge tone={moment.tone}>Activity</AppStatusBadge>
                      </div>
                    </div>
                  </div>
                </article>
              ))}
            </div>
            <button className="mx-auto mt-6 block rounded-full bg-brand-surface-muted px-10 py-3 text-sm font-semibold text-brand-muted" type="button">
              See more moments
            </button>
          </section>
        </div>

        <section className="flex items-center gap-4 rounded-md bg-white p-5 shadow-app-card">
          <div className="flex h-14 w-14 items-center justify-center rounded-md bg-orange-100 text-orange-500">
            <UilFileAlt aria-hidden="true" size={24} />
          </div>
          <div>
            <h2 className="font-bold text-brand-heading">Quick Notes</h2>
            <p className="text-sm text-brand-muted">Jot down a thought, a plan, or something Sophia said or did.</p>
          </div>
        </section>
      </div>

      <aside className="space-y-5">
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
                  <div className="flex h-8 w-8 items-center justify-center rounded-md bg-blue-50 text-brand-blue-500">
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
            <div className="flex h-12 w-12 items-center justify-center rounded-md bg-white/80 text-brand-purple">
              <UilBookOpen aria-hidden="true" size={22} />
            </div>
            <h2 className="text-base font-bold text-brand-heading">Suggested Next Activity</h2>
          </div>
          <h3 className="mt-6 text-xl font-bold leading-7 text-brand-heading">Create a butterfly life cycle poster</h3>
          <p className="mt-3 text-sm leading-6 text-brand-muted">Great for science, talking and fine motor skills.</p>
          <button className="mt-5 rounded-md bg-white/80 px-6 py-3 text-sm font-bold text-brand-purple" type="button">
            See idea
          </button>
        </section>

        <section className="rounded-md bg-white p-6 shadow-app-card">
          <div className="flex items-center gap-4">
            <div className="flex h-11 w-11 items-center justify-center rounded-md bg-blue-50 text-brand-blue-500">
              <UilCalendarAlt aria-hidden="true" size={21} />
            </div>
            <div>
              <h2 className="font-bold text-brand-heading">Today feels open</h2>
              <p className="text-sm text-brand-muted">Add one small moment when it happens.</p>
            </div>
          </div>
        </section>
      </aside>
    </div>
  </section>
);

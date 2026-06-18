import { UilBookOpen, UilEstate, UilFileAlt } from '@iconscout/react-unicons';
import { AppHero } from './AppHero';
import { AppStatusBadge } from '../../../shared/ui/AppStatusBadge';

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

const interests = ['Butterflies', 'Bugs & insects', 'Arts & crafts'];

export const HomeMain = () => {
  return (
    <div className="space-y-5">
      <AppHero />
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
  );
};

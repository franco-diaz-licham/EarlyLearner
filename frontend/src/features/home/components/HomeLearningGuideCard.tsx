import { UilCalendarAlt } from '@iconscout/react-unicons';

export const HomeLearningGuideCard = () => {
  return (
    <section className="flex items-center gap-4 rounded-md bg-white p-5 shadow-app-card">
      <div className="flex h-14 w-14 items-center justify-center rounded-md bg-orange-100 text-orange-500">
        <UilCalendarAlt aria-hidden="true" size={24} />
      </div>
      <div>
        <h2 className="font-bold text-brand-heading">Use Home for the pulse, Learning for the work</h2>
        <p className="text-sm text-brand-muted">The dashboard highlights what changed. The Learning page manages logs, outcomes, filters, and history.</p>
      </div>
    </section>
  );
};

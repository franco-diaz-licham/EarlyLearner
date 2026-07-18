import { Link } from 'react-router-dom';
import { UilClipboardNotes, UilEstate, UilShieldCheck } from '@iconscout/react-unicons';

const quickActions = [
  {
    icon: <UilEstate aria-hidden="true" className="h-5 w-5" />,
    label: 'Manage household',
    to: '/households'
  },
  {
    icon: <UilClipboardNotes aria-hidden="true" className="h-5 w-5" />,
    label: 'View logs',
    to: '/learning'
  },
  {
    icon: <UilShieldCheck aria-hidden="true" className="h-5 w-5" />,
    label: 'Manage outcomes',
    to: '/outcomes'
  }
];

export const HomeQuickActions = () => {
  return (
    <section aria-label="Home quick actions" className="grid gap-3 sm:grid-cols-3">
      {quickActions.map((action) => (
        <Link className="inline-flex min-h-12 items-center justify-center gap-2 rounded-md bg-white px-4 text-sm font-semibold text-brand-heading shadow-app-card hover:text-brand-primary" key={action.label} to={action.to}>
          {action.icon}
          {action.label}
        </Link>
      ))}
    </section>
  );
};

import { UilUsersAlt } from '@iconscout/react-unicons';
import { AppAvatar } from '../../../shared/ui/AppAvatar';
import type { HomeChildModel } from '../types/home.types';

interface ChildrenCardProps {
  children: HomeChildModel[];
}

const getChildInitials = (givenName: string): string => givenName.slice(0, 2);

const formatAge = (dateOfBirth: string) => {
  const birthDate = new Date(dateOfBirth);
  const today = new Date();
  let years = today.getFullYear() - birthDate.getFullYear();
  const monthDelta = today.getMonth() - birthDate.getMonth();
  if (monthDelta < 0 || (monthDelta === 0 && today.getDate() < birthDate.getDate())) years -= 1;
  return years <= 0 ? 'Under 1' : `${String(years)} year${years === 1 ? '' : 's'}`;
};

export const ChildrenCard = ({ children }: ChildrenCardProps) => {
  return (
    <div className="rounded-md bg-white p-5 shadow-app-card">
      <div className="flex items-center gap-3">
        <div className="flex h-10 w-10 items-center justify-center rounded-md bg-brand-lavender-50 text-brand-lavender-500">
          <UilUsersAlt aria-hidden="true" size={20} />
        </div>
        <div>
          <h2 className="text-base font-bold text-brand-heading">Children</h2>
          <p className="text-sm text-brand-muted">{children.length} active in this household</p>
        </div>
      </div>

      <div className="mt-5 space-y-3">
        {children.length === 0 ? (
          <p className="text-sm text-brand-muted">Add children from the household page to start recording learning.</p>
        ) : (
          children.map((child) => (
            <div className="flex items-center gap-3 rounded-md bg-brand-surface-soft p-3" key={child.childId}>
              <AppAvatar alt={child.givenName} className="shrink-0 [&>div:first-child]:h-9 [&>div:first-child]:w-9" initials={getChildInitials(child.givenName)} readOnly size="sm" storedFileId={child.avatarStoredFileId} />
              <div>
                <p className="text-sm font-bold text-brand-heading">{child.givenName}</p>
                <p className="text-xs text-brand-muted">{formatAge(child.dateOfBirth)}</p>
              </div>
            </div>
          ))
        )}
      </div>
    </div>
  );
};

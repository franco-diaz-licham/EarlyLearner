import { UilBookOpen, UilCalendarAlt, UilEditAlt, UilTrashAlt, UilUserPlus, UilUsersAlt } from '@iconscout/react-unicons';
import { AppIconButton } from '../../../shared/ui/AppIconButton';
import { AppStatusBadge } from '../../../shared/ui/AppStatusBadge';
import type { ChildModel, HouseholdModel } from '../types/household.types';
import { HouseholdSummaryCard } from './HouseholdSummaryCard';

interface HouseholdMembersProps {
  household: HouseholdModel;
  getStatusTone: (status: string) => 'success' | 'warning' | 'neutral';
  isRemovingCarer: boolean;
  isRemovingChild: boolean;
  onEditChild: (child: ChildModel) => void;
  onRemoveCarer: (carerId: string) => void;
  onRemoveChild: (childId: string) => void;
}

const formatDate = (value: string): string =>
  new Intl.DateTimeFormat('en-AU', {
    day: '2-digit',
    month: 'short',
    year: 'numeric'
  }).format(new Date(value));

const getInvitationName = (firstName: string | null, lastName: string | null, email: string): string => {
  const fullName = `${firstName ?? ''} ${lastName ?? ''}`.trim();
  return fullName || email;
};

export const HouseholdMembers = ({ household, getStatusTone, isRemovingCarer, isRemovingChild, onEditChild, onRemoveCarer, onRemoveChild }: HouseholdMembersProps) => {
  const hasMembers = household.carers.length > 0 || household.children.length > 0 || household.invitations.length > 0;

  return (
    <HouseholdSummaryCard isEmpty={!hasMembers} emptyMessage="No members are attached to this household yet." title="Household Members">
      <div className="space-y-6">
        <section className="space-y-3" aria-label="Carers">
          {household.carers.map((carer) => {
            const isOwner = carer.role.toLowerCase() === 'owner';

            return (
              <div className="rounded-md border border-brand-border p-4" key={carer.id}>
                <div className="flex flex-col gap-4 sm:flex-row sm:items-center sm:justify-between">
                  <div className="flex min-w-0 items-start gap-4">
                    <div className="flex h-11 w-11 shrink-0 items-center justify-center rounded-md bg-brand-primary-soft text-brand-primary">
                      <UilUsersAlt aria-hidden="true" className="h-6 w-6" />
                    </div>
                    <div className="min-w-0">
                      <h2 className="font-bold text-brand-heading">
                        {carer.firstName} {carer.lastName}
                      </h2>
                      <p className="mt-1 break-all text-sm text-brand-muted">{carer.email}</p>
                      <div className="mt-3 flex flex-wrap gap-2">
                        <AppStatusBadge>{carer.role}</AppStatusBadge>
                        <AppStatusBadge tone={getStatusTone(carer.accountStatus)}>{carer.accountStatus}</AppStatusBadge>
                      </div>
                    </div>
                  </div>
                  {!isOwner ? (
                    <AppIconButton
                      aria-label={`Remove ${carer.firstName} ${carer.lastName}`}
                      disabled={isRemovingCarer}
                      icon={<UilTrashAlt aria-hidden="true" className="h-5 w-5" />}
                      onClick={() => {
                        onRemoveCarer(carer.id);
                      }}
                    />
                  ) : null}
                </div>
              </div>
            );
          })}
        </section>

        {household.children.length > 0 ? (
          <section className="space-y-3" aria-label="Children">
            <h3 className="text-sm font-bold text-brand-heading">Children</h3>
            {household.children.map((child) => (
              <div className="rounded-md border border-brand-border p-4" key={child.id}>
                <div className="flex flex-col gap-4 sm:flex-row sm:items-center sm:justify-between">
                  <div className="flex min-w-0 items-start gap-4">
                    <div className="flex h-11 w-11 shrink-0 items-center justify-center rounded-md bg-brand-primary-soft text-brand-primary">
                      <UilBookOpen aria-hidden="true" className="h-6 w-6" />
                    </div>
                    <div className="min-w-0">
                      <h2 className="font-bold text-brand-heading">
                        {child.firstName} {child.lastName}
                      </h2>
                      <p className="mt-1 text-sm text-brand-muted">Born {formatDate(child.dateOfBirth)}</p>
                    </div>
                  </div>
                  <div className="flex items-center gap-2 self-end sm:self-auto">
                    <AppIconButton
                      aria-label={`Edit ${child.firstName} ${child.lastName}`}
                      icon={<UilEditAlt aria-hidden="true" className="h-5 w-5" />}
                      onClick={() => {
                        onEditChild(child);
                      }}
                    />
                    <AppIconButton
                      aria-label={`Remove ${child.firstName} ${child.lastName}`}
                      disabled={isRemovingChild}
                      icon={<UilTrashAlt aria-hidden="true" className="h-5 w-5" />}
                      onClick={() => {
                        onRemoveChild(child.id);
                      }}
                    />
                  </div>
                </div>
              </div>
            ))}
          </section>
        ) : null}

        {household.invitations.length > 0 ? (
          <section className="space-y-3" aria-label="Invitations">
            <h3 className="text-sm font-bold text-brand-heading">Invitations</h3>
            {household.invitations.map((invitation) => (
              <div className="rounded-md border border-brand-border p-4" key={invitation.id}>
                <div className="flex flex-col gap-4 sm:flex-row sm:items-start sm:justify-between">
                  <div className="flex min-w-0 items-start gap-4">
                    <div className="flex h-11 w-11 shrink-0 items-center justify-center rounded-md bg-brand-primary-soft text-brand-primary">
                      <UilUserPlus aria-hidden="true" className="h-6 w-6" />
                    </div>
                    <div className="min-w-0">
                      <h2 className="font-bold text-brand-heading">{getInvitationName(invitation.firstName, invitation.lastName, invitation.email)}</h2>
                      {invitation.firstName || invitation.lastName ? <p className="mt-1 break-all text-sm text-brand-muted">{invitation.email}</p> : null}
                      <div className="mt-3 flex flex-wrap gap-2">
                        <AppStatusBadge>{invitation.role}</AppStatusBadge>
                        <AppStatusBadge tone={getStatusTone(invitation.status)}>{invitation.status}</AppStatusBadge>
                        <span className="inline-flex items-center gap-1 text-xs font-semibold text-brand-muted">
                          <UilCalendarAlt aria-hidden="true" className="h-4 w-4" />
                          Expires {formatDate(invitation.expiresAt)}
                        </span>
                      </div>
                    </div>
                  </div>
                </div>
              </div>
            ))}
          </section>
        ) : null}
      </div>
    </HouseholdSummaryCard>
  );
};

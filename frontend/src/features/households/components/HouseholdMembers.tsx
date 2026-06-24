import { UilTrashAlt, UilUsersAlt } from '@iconscout/react-unicons';
import { AppIconButton } from '../../../shared/ui/AppIconButton';
import { AppStatusBadge } from '../../../shared/ui/AppStatusBadge';
import type { CarerModel } from '../types/household.types';
import { HouseholdSummaryCard } from './HouseholdSummaryCard';

interface HouseholdMembersProps {
  carers: CarerModel[];
  getStatusTone: (status: string) => 'success' | 'warning' | 'neutral';
  isRemoving: boolean;
  onRemove: (carerId: string) => void;
}

export const HouseholdMembers = ({ carers, getStatusTone, isRemoving, onRemove }: HouseholdMembersProps) => {
  return (
    <HouseholdSummaryCard isEmpty={carers.length === 0} emptyMessage="No members are attached to this household yet." title="Household Members">
      <div className="grid gap-3">
        {carers.map((carer) => {
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
                    disabled={isRemoving}
                    icon={<UilTrashAlt aria-hidden="true" className="h-5 w-5" />}
                    onClick={() => {
                      onRemove(carer.id);
                    }}
                  />
                ) : null}
              </div>
            </div>
          );
        })}
      </div>
    </HouseholdSummaryCard>
  );
};

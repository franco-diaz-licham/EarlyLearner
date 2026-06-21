import { useMemo, useState } from 'react';
import { UilEditAlt, UilEstate, UilPlus, UilTrashAlt, UilUserPlus, UilUsersAlt } from '@iconscout/react-unicons';
import { AppButton } from '../../../shared/ui/AppButton';
import { AppCard } from '../../../shared/ui/AppCard';
import { AppIconButton } from '../../../shared/ui/AppIconButton';
import { AppStatusBadge } from '../../../shared/ui/AppStatusBadge';
import { HouseholdDialog } from '../components/HouseholdDialog';
import { InviteCarerDialog } from '../components/InviteCarerDialog';
import { useCreateHouseholdMutation, useDeleteHouseholdMutation, useHouseholdsQuery, useInviteHouseholdCarerMutation, useUpdateHouseholdMutation } from '../queries/household.queries';
import type { CreateHouseholdRequest, HouseholdModel, InviteHouseholdCarerRequest } from '../types/household.types';

const createEmptyHousehold = (): CreateHouseholdRequest => ({
  name: ''
});

const createEmptyInvite = (): InviteHouseholdCarerRequest => ({
  email: '',
  firstName: '',
  lastName: '',
  role: 2
});

const emptyHouseholds: HouseholdModel[] = [];

export const HouseholdsPage = () => {
  const [dialogMode, setDialogMode] = useState<'create' | 'edit'>('create');
  const [isDialogVisible, setIsDialogVisible] = useState(false);
  const [householdDraft, setHousehold] = useState<CreateHouseholdRequest | HouseholdModel>(() => createEmptyHousehold());
  const [inviteDraft, setInviteDraft] = useState<InviteHouseholdCarerRequest>(() => createEmptyInvite());
  const [inviteHouseholdId, setInviteHouseholdId] = useState<string | null>(null);
  const [deletePendingId, setDeletePendingId] = useState<string | null>(null);

  const householdsQuery = useHouseholdsQuery();
  const createHouseholdMutation = useCreateHouseholdMutation();
  const updateHouseholdMutation = useUpdateHouseholdMutation();
  const inviteHouseholdCarerMutation = useInviteHouseholdCarerMutation();
  const deleteHouseholdMutation = useDeleteHouseholdMutation();

  const households = householdsQuery.data ?? emptyHouseholds;
  const selectedHousehold = useMemo(() => households.find((household) => household.id === deletePendingId), [deletePendingId, households]);
  const inviteHousehold = useMemo(() => households.find((household) => household.id === inviteHouseholdId) ?? null, [inviteHouseholdId, households]);
  const isSaving = createHouseholdMutation.isPending || updateHouseholdMutation.isPending;

  const handleNewHousehold = () => {
    setDialogMode('create');
    setHousehold(createEmptyHousehold());
    setIsDialogVisible(true);
  };

  const handleEditHousehold = (household: HouseholdModel) => {
    setDialogMode('edit');
    setHousehold(household);
    setIsDialogVisible(true);
  };

  const handleInviteCarer = (household: HouseholdModel) => {
    setInviteHouseholdId(household.id);
    setInviteDraft(createEmptyInvite());
  };

  const handleSaveHousehold = () => {
    if (dialogMode === 'create') {
      createHouseholdMutation.mutate(householdDraft, {
        onSuccess: () => {
          setHousehold(createEmptyHousehold());
          setIsDialogVisible(false);
        }
      });
      return;
    }

    if ('id' in householdDraft) {
      updateHouseholdMutation.mutate(
        { householdId: householdDraft.id, request: { name: householdDraft.name } },
        {
          onSuccess: () => {
            setIsDialogVisible(false);
          }
        }
      );
    }
  };

  const handleDeleteHousehold = (householdId: string) => {
    deleteHouseholdMutation.mutate(householdId, {
      onSuccess: () => {
        setDeletePendingId(null);
      }
    });
  };

  const handleSaveInvite = () => {
    if (!inviteHouseholdId) return;

    inviteHouseholdCarerMutation.mutate(
      { householdId: inviteHouseholdId, request: inviteDraft },
      {
        onSuccess: () => {
          setInviteDraft(createEmptyInvite());
          setInviteHouseholdId(null);
        }
      }
    );
  };

  return (
    <section aria-labelledby="households-title" className="space-y-5">
      <AppCard>
        <div className="flex flex-col gap-4 sm:flex-row sm:items-start sm:justify-between">
          <div>
            <p className="text-sm font-semibold text-brand-muted">Identity</p>
            <h1 id="households-title" className="mt-1 text-3xl font-bold text-brand-heading">
              Manage households
            </h1>
            <p className="mt-2 max-w-2xl text-brand-muted">Create family spaces, keep ownership clear, and choose the household that anchors planning, learning and readiness records.</p>
          </div>
          <AppButton icon={<UilPlus aria-hidden="true" className="h-5 w-5" />} label="New household" onClick={handleNewHousehold} />
        </div>
      </AppCard>

      <div className="grid gap-5 xl:grid-cols-[minmax(0,1fr)_360px]">
        <AppCard title="Household Directory">
          {householdsQuery.isLoading ? <p className="text-sm text-brand-muted">Loading households...</p> : null}
          {householdsQuery.isError ? <p className="text-sm font-semibold text-red-600">Households could not be loaded.</p> : null}
          {!householdsQuery.isLoading && households.length === 0 ? (
            <div className="rounded-md border border-dashed border-brand-border p-6 text-center">
              <div className="mx-auto flex h-12 w-12 items-center justify-center rounded-md bg-brand-primary-soft text-brand-primary">
                <UilEstate aria-hidden="true" className="h-6 w-6" />
              </div>
              <h2 className="mt-4 text-lg font-bold text-brand-heading">No households yet</h2>
              <p className="mt-2 text-sm leading-6 text-brand-muted">Add the first household to unlock planning and learning records.</p>
              <AppButton className="mt-4" label="Create household" onClick={handleNewHousehold} />
            </div>
          ) : null}
          <div className="grid gap-3">
            {households.map((household) => (
              <article className="rounded-md border border-brand-border p-4" key={household.id}>
                <div className="flex flex-col gap-4 sm:flex-row sm:items-center sm:justify-between">
                  <div className="flex min-w-0 items-start gap-4">
                    <div className="flex h-11 w-11 shrink-0 items-center justify-center rounded-md bg-brand-primary-soft text-brand-primary">
                      <UilUsersAlt aria-hidden="true" className="h-6 w-6" />
                    </div>
                    <div className="min-w-0">
                      <div className="flex flex-wrap items-center gap-2">
                        <h2 className="font-bold text-brand-heading">{household.name}</h2>
                        <AppStatusBadge tone="success">Active</AppStatusBadge>
                      </div>
                      <p className="mt-1 break-all text-xs font-semibold text-brand-muted">{household.id}</p>
                    </div>
                  </div>
                  <div className="flex items-center gap-2 self-end sm:self-auto">
                    <AppIconButton
                      aria-label={`Invite someone to ${household.name}`}
                      icon={<UilUserPlus aria-hidden="true" className="h-5 w-5" />}
                      onClick={() => {
                        handleInviteCarer(household);
                      }}
                    />
                    <AppIconButton
                      aria-label={`Rename ${household.name}`}
                      icon={<UilEditAlt aria-hidden="true" className="h-5 w-5" />}
                      onClick={() => {
                        handleEditHousehold(household);
                      }}
                    />
                    <AppIconButton
                      aria-label={`Delete ${household.name}`}
                      icon={<UilTrashAlt aria-hidden="true" className="h-5 w-5" />}
                      onClick={() => {
                        setDeletePendingId(household.id);
                      }}
                    />
                  </div>
                </div>
              </article>
            ))}
          </div>
        </AppCard>

        <aside className="space-y-5">
          <AppCard title="Identity Summary">
            <p className="text-sm text-brand-muted">Households</p>
            <p className="mt-2 text-4xl font-bold text-brand-heading">{households.length}</p>
            <p className="mt-4 text-sm leading-6 text-brand-muted">Each household is the boundary used by plans, goals, logs, readiness profiles and stored files.</p>
          </AppCard>

          {selectedHousehold ? (
            <AppCard title="Confirm Delete">
              <h2 className="text-lg font-bold text-brand-heading">{selectedHousehold.name}</h2>
              <p className="mt-2 text-sm leading-6 text-brand-muted">Deleting a household removes the identity record from the API. Continue only when this household is no longer needed.</p>
              <div className="mt-4 flex gap-3">
                <AppButton
                  label="Keep"
                  variant="secondary"
                  onClick={() => {
                    setDeletePendingId(null);
                  }}
                />
                <AppButton
                  disabled={deleteHouseholdMutation.isPending}
                  label={deleteHouseholdMutation.isPending ? 'Deleting...' : 'Delete'}
                  onClick={() => {
                    handleDeleteHousehold(selectedHousehold.id);
                  }}
                />
              </div>
            </AppCard>
          ) : null}
        </aside>
      </div>

      <HouseholdDialog
        mode={dialogMode}
        name={householdDraft.name}
        saving={isSaving}
        visible={isDialogVisible}
        onChange={(name) => {
          setHousehold((current) => ({ ...current, name }));
        }}
        onHide={() => {
          setIsDialogVisible(false);
        }}
        onSave={handleSaveHousehold}
      />
      <InviteCarerDialog
        draft={inviteDraft}
        household={inviteHousehold}
        saving={inviteHouseholdCarerMutation.isPending}
        visible={Boolean(inviteHouseholdId)}
        onChange={setInviteDraft}
        onHide={() => {
          setInviteHouseholdId(null);
        }}
        onSave={handleSaveInvite}
      />
    </section>
  );
};

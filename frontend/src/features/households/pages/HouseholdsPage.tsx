import { useState } from 'react';
import { UilEditAlt, UilEstate, UilPlus, UilTrashAlt, UilUserPlus, UilUsersAlt } from '@iconscout/react-unicons';
import { AppButton } from '../../../shared/ui/AppButton';
import { AppCard } from '../../../shared/ui/AppCard';
import { AppIconButton } from '../../../shared/ui/AppIconButton';
import { AppInputText } from '../../../shared/ui/AppInputText';
import { AppStatusBadge } from '../../../shared/ui/AppStatusBadge';
import { AddChildDialog } from '../components/AddChildDialog';
import { InviteCarerDialog } from '../components/InviteCarerDialog';
import { useAddHouseholdChildMutation, useHouseholdsQuery, useInviteHouseholdCarerMutation, useRemoveHouseholdCarerMutation, useUpdateHouseholdMutation } from '../queries/household.queries';
import type { AddHouseholdChildRequest, HouseholdModel, InviteHouseholdCarerRequest } from '../types/household.types';

const createEmptyInvite = (): InviteHouseholdCarerRequest => ({
  email: '',
  role: 2
});

const createEmptyChild = (): AddHouseholdChildRequest => ({
  firstName: '',
  lastName: '',
  dateOfBirth: ''
});

const formatDate = (value: string): string =>
  new Intl.DateTimeFormat('en-AU', {
    day: '2-digit',
    month: 'short',
    year: 'numeric'
  }).format(new Date(value));

const getStatusTone = (status: string): 'success' | 'warning' | 'neutral' => {
  const normalizedStatus = status.toLowerCase();
  if (normalizedStatus === 'active' || normalizedStatus === 'accepted') return 'success';
  if (normalizedStatus === 'pending' || normalizedStatus === 'invited') return 'warning';
  return 'neutral';
};

export const HouseholdsPage = () => {
  const [isEditingName, setIsEditingName] = useState(false);
  const [nameDraft, setNameDraft] = useState('');
  const [inviteDraft, setInviteDraft] = useState<InviteHouseholdCarerRequest>(() => createEmptyInvite());
  const [inviteHouseholdId, setInviteHouseholdId] = useState<string | null>(null);
  const [childDraft, setChildDraft] = useState<AddHouseholdChildRequest>(() => createEmptyChild());
  const [childHouseholdId, setChildHouseholdId] = useState<string | null>(null);

  const householdsQuery = useHouseholdsQuery();
  const updateHouseholdMutation = useUpdateHouseholdMutation();
  const inviteHouseholdCarerMutation = useInviteHouseholdCarerMutation();
  const removeHouseholdCarerMutation = useRemoveHouseholdCarerMutation();
  const addHouseholdChildMutation = useAddHouseholdChildMutation();

  const household = householdsQuery.data?.[0] ?? null;
  const inviteHousehold = inviteHouseholdId && household?.id === inviteHouseholdId ? household : null;
  const childHousehold = childHouseholdId && household?.id === childHouseholdId ? household : null;

  const startRename = (currentHousehold: HouseholdModel) => {
    setNameDraft(currentHousehold.name);
    setIsEditingName(true);
  };

  const cancelRename = () => {
    setNameDraft('');
    setIsEditingName(false);
  };

  const saveRename = () => {
    if (!household) return;

    const name = nameDraft.trim();
    if (!name || name === household.name) {
      cancelRename();
      return;
    }

    updateHouseholdMutation.mutate(
      { householdId: household.id, request: { name } },
      {
        onSuccess: () => {
          cancelRename();
        }
      }
    );
  };

  const handleInviteCarer = (currentHousehold: HouseholdModel) => {
    setInviteHouseholdId(currentHousehold.id);
    setInviteDraft(createEmptyInvite());
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

  const handleAddChild = (currentHousehold: HouseholdModel) => {
    setChildHouseholdId(currentHousehold.id);
    setChildDraft(createEmptyChild());
  };

  const handleSaveChild = () => {
    if (!childHouseholdId) return;

    addHouseholdChildMutation.mutate(
      { householdId: childHouseholdId, request: childDraft },
      {
        onSuccess: () => {
          setChildDraft(createEmptyChild());
          setChildHouseholdId(null);
        }
      }
    );
  };

  const removeCarer = (carerId: string) => {
    if (!household) return;
    removeHouseholdCarerMutation.mutate({ householdId: household.id, carerId });
  };

  return (
    <section aria-labelledby="households-title" className="space-y-5">
      <AppCard>
        {householdsQuery.isLoading ? <p className="text-sm text-brand-muted">Loading household...</p> : null}
        {householdsQuery.isError ? <p className="text-sm font-semibold text-red-600">Household could not be loaded.</p> : null}
        {!householdsQuery.isLoading && !household ? (
          <div className="rounded-md border border-dashed border-brand-border p-6 text-center">
            <div className="mx-auto flex h-12 w-12 items-center justify-center rounded-md bg-brand-primary-soft text-brand-primary">
              <UilEstate aria-hidden="true" className="h-6 w-6" />
            </div>
            <h2 className="mt-4 text-lg font-bold text-brand-heading">No household yet</h2>
            <p className="mt-2 text-sm leading-6 text-brand-muted">Finish signup to create your household and unlock planning and learning records.</p>
          </div>
        ) : null}

        {household ? (
          <div className="flex flex-col gap-4 sm:flex-row sm:items-start sm:justify-between">
            <div className="min-w-0">
              <p className="text-sm font-semibold text-brand-muted">Current household</p>
              {isEditingName ? (
                <div className="mt-2 flex max-w-xl items-center gap-2">
                  <AppInputText
                    aria-label="Household name"
                    autoFocus
                    value={nameDraft}
                    onChange={(event) => {
                      setNameDraft(event.target.value);
                    }}
                    onKeyDown={(event) => {
                      if (event.key === 'Enter') saveRename();
                      if (event.key === 'Escape') cancelRename();
                    }}
                  />
                  <AppButton disabled={updateHouseholdMutation.isPending} label={updateHouseholdMutation.isPending ? 'Saving...' : 'Save'} onClick={saveRename} />
                  <AppButton label="Cancel" variant="secondary" onClick={cancelRename} />
                </div>
              ) : (
                <div className="mt-1 flex min-w-0 items-center gap-2">
                  <h1 id="households-title" className="truncate text-3xl font-bold text-brand-heading">
                    {household.name}
                  </h1>
                  <AppIconButton
                    aria-label={`Rename ${household.name}`}
                    icon={<UilEditAlt aria-hidden="true" className="h-5 w-5" />}
                    onClick={() => {
                      startRename(household);
                    }}
                  />
                </div>
              )}
              <p className="mt-2 max-w-2xl text-brand-muted">Manage the people who can access this household and keep their responsibilities clear.</p>
            </div>
            <div className="flex flex-wrap gap-3">
              <AppButton
                icon={<UilPlus aria-hidden="true" className="h-5 w-5" />}
                label="Add child"
                onClick={() => {
                  handleAddChild(household);
                }}
              />
              <AppButton
                icon={<UilUserPlus aria-hidden="true" className="h-5 w-5" />}
                label="Invite carer"
                variant="secondary"
                onClick={() => {
                  handleInviteCarer(household);
                }}
              />
            </div>
          </div>
        ) : null}
      </AppCard>

      {household ? (
        <div className="grid gap-5 xl:grid-cols-[minmax(0,1fr)_340px]">
          <AppCard title="Household Members">
            {household.carers.length === 0 ? <p className="text-sm text-brand-muted">No members are attached to this household yet.</p> : null}
            <div className="grid gap-3">
              {household.carers.map((carer) => {
                const isOwner = carer.role.toLowerCase() === 'owner';

                return (
                  <article className="rounded-md border border-brand-border p-4" key={carer.id}>
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
                          disabled={removeHouseholdCarerMutation.isPending}
                          icon={<UilTrashAlt aria-hidden="true" className="h-5 w-5" />}
                          onClick={() => {
                            removeCarer(carer.id);
                          }}
                        />
                      ) : null}
                    </div>
                  </article>
                );
              })}
            </div>
          </AppCard>

          <aside className="space-y-5">
            <AppCard title="Children">
              {household.children.length === 0 ? <p className="text-sm text-brand-muted">No child profiles have been added yet.</p> : null}
              <div className="space-y-3">
                {household.children.map((child) => (
                  <article className="rounded-md border border-brand-border p-4" key={child.id}>
                    <h2 className="font-bold text-brand-heading">
                      {child.firstName} {child.lastName}
                    </h2>
                    <p className="mt-1 text-sm text-brand-muted">Born {formatDate(child.dateOfBirth)}</p>
                  </article>
                ))}
              </div>
            </AppCard>

            <AppCard title="Invitations">
              {household.invitations.length === 0 ? <p className="text-sm text-brand-muted">No carer invitations have been sent.</p> : null}
              <div className="space-y-3">
                {household.invitations.map((invitation) => (
                  <article className="rounded-md border border-brand-border p-4" key={invitation.id}>
                    <div className="flex items-start justify-between gap-3">
                      <div className="min-w-0">
                        <h2 className="font-bold text-brand-heading">
                          {invitation.firstName || invitation.lastName ? `${invitation.firstName} ${invitation.lastName}`.trim() : invitation.email}
                        </h2>
                        {invitation.firstName || invitation.lastName ? <p className="mt-1 break-all text-sm text-brand-muted">{invitation.email}</p> : null}
                      </div>
                      <AppStatusBadge tone={getStatusTone(invitation.status)}>{invitation.status}</AppStatusBadge>
                    </div>
                    <div className="mt-3 flex flex-wrap gap-2">
                      <AppStatusBadge>{invitation.role}</AppStatusBadge>
                      <span className="text-xs font-semibold text-brand-muted">Expires {formatDate(invitation.expiresAt)}</span>
                    </div>
                  </article>
                ))}
              </div>
            </AppCard>

            <AppCard title="Household Summary">
              <p className="text-sm text-brand-muted">Members</p>
              <p className="mt-2 text-4xl font-bold text-brand-heading">{household.carers.length}</p>
              <p className="mt-4 text-sm leading-6 text-brand-muted">
                {household.children.length} child profile{household.children.length === 1 ? '' : 's'} and {household.invitations.length} invitation{household.invitations.length === 1 ? '' : 's'}.
              </p>
            </AppCard>
          </aside>
        </div>
      ) : null}

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
      <AddChildDialog
        draft={childDraft}
        household={childHousehold}
        saving={addHouseholdChildMutation.isPending}
        visible={Boolean(childHouseholdId)}
        onChange={setChildDraft}
        onHide={() => {
          setChildHouseholdId(null);
        }}
        onSave={handleSaveChild}
      />
    </section>
  );
};

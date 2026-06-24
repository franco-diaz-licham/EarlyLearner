import { useState } from 'react';
import { AppStatusBadge } from '../../../shared/ui/AppStatusBadge';
import { AddChildDialog } from '../components/AddChildDialog';
import { HouseholdHeader } from '../components/HouseholdHeader';
import { HouseholdMembers } from '../components/HouseholdMembers';
import { HouseholdSummaryCard } from '../components/HouseholdSummaryCard';
import { InviteCarerDialog } from '../components/InviteCarerDialog';
import { useAddChildForm } from '../hooks/useAddChildForm';
import { useInviteCarerForm } from '../hooks/useInviteCarerForm';
import { useAddHouseholdChildMutation, useHouseholdsQuery, useInviteHouseholdCarerMutation, useRemoveHouseholdCarerMutation, useUpdateHouseholdMutation } from '../queries/household.queries';
import type { HouseholdModel } from '../types/household.types';

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
  const [inviteHouseholdId, setInviteHouseholdId] = useState<string | null>(null);
  const [childHouseholdId, setChildHouseholdId] = useState<string | null>(null);

  const inviteCarerForm = useInviteCarerForm();
  const addChildForm = useAddChildForm();

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
      { householdId: household.id, form: { name } },
      {
        onSuccess: () => {
          cancelRename();
        }
      }
    );
  };

  const handleInviteCarer = (currentHousehold: HouseholdModel) => {
    setInviteHouseholdId(currentHousehold.id);
    inviteCarerForm.reset();
  };

  const handleSaveInvite = () => {
    if (!inviteHouseholdId) return;

    const form = inviteCarerForm.getValidForm();
    if (!form) return;

    inviteHouseholdCarerMutation.mutate(
      { householdId: inviteHouseholdId, form },
      {
        onSuccess: () => {
          inviteCarerForm.reset();
          setInviteHouseholdId(null);
        }
      }
    );
  };

  const handleAddChild = (currentHousehold: HouseholdModel) => {
    setChildHouseholdId(currentHousehold.id);
    addChildForm.reset();
  };

  const handleSaveChild = () => {
    if (!childHouseholdId) return;

    const form = addChildForm.getValidForm();
    if (!form) return;

    addHouseholdChildMutation.mutate(
      { householdId: childHouseholdId, form },
      {
        onSuccess: () => {
          addChildForm.reset();
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
      <HouseholdHeader
        household={household}
        isEditingName={isEditingName}
        isError={householdsQuery.isError}
        isLoading={householdsQuery.isLoading}
        isSavingName={updateHouseholdMutation.isPending}
        nameDraft={nameDraft}
        onAddChild={handleAddChild}
        onCancelRename={cancelRename}
        onInviteCarer={handleInviteCarer}
        onNameDraftChange={setNameDraft}
        onSaveRename={saveRename}
        onStartRename={startRename}
      />

      {household ? (
        <div className="grid gap-5 xl:grid-cols-[minmax(0,1fr)_340px]">
          <HouseholdMembers carers={household.carers} getStatusTone={getStatusTone} isRemoving={removeHouseholdCarerMutation.isPending} onRemove={removeCarer} />

          <aside className="space-y-5">
            <HouseholdSummaryCard isEmpty={household.children.length === 0} emptyMessage="No child profiles have been added yet." title="Children">
              <div className="space-y-3">
                {household.children.map((child) => (
                  <div className="rounded-md border border-brand-border p-4" key={child.id}>
                    <h2 className="font-bold text-brand-heading">
                      {child.firstName} {child.lastName}
                    </h2>
                    <p className="mt-1 text-sm text-brand-muted">Born {formatDate(child.dateOfBirth)}</p>
                  </div>
                ))}
              </div>
            </HouseholdSummaryCard>

            <HouseholdSummaryCard isEmpty={household.invitations.length === 0} emptyMessage="No carer invitations have been sent." title="Invitations">
              <div className="space-y-3">
                {household.invitations.map((invitation) => (
                  <div className="rounded-md border border-brand-border p-4" key={invitation.id}>
                    <div className="flex items-start justify-between gap-3">
                      <div className="min-w-0">
                        <h2 className="font-bold text-brand-heading">{invitation.firstName || invitation.lastName ? `${invitation.firstName} ${invitation.lastName}`.trim() : invitation.email}</h2>
                        {invitation.firstName || invitation.lastName ? <p className="mt-1 break-all text-sm text-brand-muted">{invitation.email}</p> : null}
                      </div>
                      <AppStatusBadge tone={getStatusTone(invitation.status)}>{invitation.status}</AppStatusBadge>
                    </div>
                    <div className="mt-3 flex flex-wrap gap-2">
                      <AppStatusBadge>{invitation.role}</AppStatusBadge>
                      <span className="text-xs font-semibold text-brand-muted">Expires {formatDate(invitation.expiresAt)}</span>
                    </div>
                  </div>
                ))}
              </div>
            </HouseholdSummaryCard>

            <HouseholdSummaryCard title="Household Summary">
              <p className="text-sm text-brand-muted">Members</p>
              <p className="mt-2 text-4xl font-bold text-brand-heading">{household.carers.length}</p>
              <p className="mt-4 text-sm leading-6 text-brand-muted">
                {household.children.length} child profile{household.children.length === 1 ? '' : 's'} and {household.invitations.length} invitation{household.invitations.length === 1 ? '' : 's'}.
              </p>
            </HouseholdSummaryCard>
          </aside>
        </div>
      ) : null}

      <InviteCarerDialog
        canSave={inviteCarerForm.isValid}
        draft={inviteCarerForm.draft}
        errors={inviteCarerForm.errors}
        household={inviteHousehold}
        saving={inviteHouseholdCarerMutation.isPending}
        visible={Boolean(inviteHouseholdId)}
        onChange={inviteCarerForm.setDraft}
        onHide={() => {
          inviteCarerForm.reset();
          setInviteHouseholdId(null);
        }}
        onSave={handleSaveInvite}
      />
      <AddChildDialog
        canSave={addChildForm.isValid}
        draft={addChildForm.draft}
        errors={addChildForm.errors}
        household={childHousehold}
        saving={addHouseholdChildMutation.isPending}
        visible={Boolean(childHouseholdId)}
        onChange={addChildForm.setDraft}
        onHide={() => {
          addChildForm.reset();
          setChildHouseholdId(null);
        }}
        onSave={handleSaveChild}
      />
    </section>
  );
};

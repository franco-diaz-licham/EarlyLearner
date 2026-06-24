import { useState } from 'react';
import { AddChildDialog } from '../components/AddChildDialog';
import { HouseholdHeader } from '../components/HouseholdHeader';
import { HouseholdMembers } from '../components/HouseholdMembers';
import { HouseholdSummaryCard } from '../components/HouseholdSummaryCard';
import { InviteCarerDialog } from '../components/InviteCarerDialog';
import {
  useAddHouseholdChildMutation,
  useHouseholdsQuery,
  useInviteHouseholdCarerMutation,
  useRemoveHouseholdCarerMutation,
  useRemoveHouseholdChildMutation,
  useUpdateHouseholdChildMutation,
  useUpdateHouseholdMutation
} from '../queries/household.queries';
import type { AddChildForm, ChildModel, HouseholdModel, InviteCarerForm } from '../types/household.types';

const getStatusTone = (status: string): 'success' | 'warning' | 'neutral' => {
  const normalizedStatus = status.toLowerCase();
  if (normalizedStatus === 'active' || normalizedStatus === 'accepted') return 'success';
  if (normalizedStatus === 'pending' || normalizedStatus === 'invited') return 'warning';
  return 'neutral';
};

export const HouseholdsPage = () => {
  const householdsQuery = useHouseholdsQuery();
  const updateHouseholdMutation = useUpdateHouseholdMutation();
  const inviteHouseholdCarerMutation = useInviteHouseholdCarerMutation();
  const removeHouseholdCarerMutation = useRemoveHouseholdCarerMutation();
  const addHouseholdChildMutation = useAddHouseholdChildMutation();
  const updateHouseholdChildMutation = useUpdateHouseholdChildMutation();
  const removeHouseholdChildMutation = useRemoveHouseholdChildMutation();

  const [isEditingName, setIsEditingName] = useState(false);
  const [nameDraft, setNameDraft] = useState('');
  const [inviteCarer, setIniviteCarer] = useState<boolean>(false);
  const [addChild, setAddChild] = useState<boolean>(false);
  const [editChild, setEditChild] = useState<boolean>(false);
  const [editingChild, setEditingChild] = useState<ChildModel | null>(null);
  const household = householdsQuery.data?.[0] ?? null;

  const startRename = (currentHousehold: HouseholdModel) => {
    setNameDraft(currentHousehold.name);
    setIsEditingName(true);
  };

  const cancelRename = () => {
    setNameDraft('');
    setIsEditingName(false);
  };

  const saveRename = async () => {
    if (!household) return;

    const name = nameDraft.trim();
    if (!name || name === household.name) {
      cancelRename();
      return;
    }
    await updateHouseholdMutation.mutateAsync({ householdId: household.id, form: { name } });
    cancelRename();
  };

  const handleInviteCarer = () => {
    setIniviteCarer(true);
  };

  const handleSaveInvite = async (form: InviteCarerForm) => {
    if (!inviteCarer || !household) return;
    await inviteHouseholdCarerMutation.mutateAsync({ householdId: household.id, form });
    setIniviteCarer(false);
  };

  const handleAddChild = () => {
    setEditChild(false);
    setEditingChild(null);
    setAddChild(true);
  };

  const handleSaveChild = async (form: AddChildForm) => {
    if (!household) return;

    if (editChild && editingChild) {
      await updateHouseholdChildMutation.mutateAsync({ householdId: household.id, childId: editingChild.id, form });
      setEditChild(false);
      setEditingChild(null);
      return;
    }

    if (!addChild) return;
    await addHouseholdChildMutation.mutateAsync({ householdId: household.id, form });
    setAddChild(false);
  };

  const handleEditChild = (child: ChildModel) => {
    setAddChild(false);
    setEditingChild(child);
    setEditChild(true);
  };

  const handleCloseChildDialog = () => {
    setAddChild(false);
    setEditChild(false);
    setEditingChild(null);
  };

  const removeChild = async (childId: string) => {
    if (!household) return;
    await removeHouseholdChildMutation.mutateAsync({ householdId: household.id, childId });
  };

  const removeCarer = async (carerId: string) => {
    if (!household) return;
    await removeHouseholdCarerMutation.mutateAsync({ householdId: household.id, carerId });
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
          <HouseholdMembers
            household={household}
            getStatusTone={getStatusTone}
            isRemovingCarer={removeHouseholdCarerMutation.isPending}
            isRemovingChild={removeHouseholdChildMutation.isPending}
            onEditChild={handleEditChild}
            onRemoveCarer={(carerId) => {
              void removeCarer(carerId);
            }}
            onRemoveChild={(childId) => {
              void removeChild(childId);
            }}
          />

          <aside className="space-y-5">
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
        household={household}
        saving={inviteHouseholdCarerMutation.isPending}
        visible={inviteCarer}
        onHide={() => {
          setIniviteCarer(false);
        }}
        onSave={(form) => {
          void handleSaveInvite(form);
        }}
      />
      <AddChildDialog
        key={editChild && editingChild ? editingChild.id : addChild ? 'add-child-open' : 'add-child-closed'}
        child={editChild ? editingChild : null}
        household={household}
        saving={addHouseholdChildMutation.isPending || updateHouseholdChildMutation.isPending}
        visible={addChild || editChild}
        onHide={handleCloseChildDialog}
        onSave={(form) => {
          void handleSaveChild(form);
        }}
      />
    </section>
  );
};

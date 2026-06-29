import { useState } from 'react';
import { ChildForm } from '../components/ChildForm';
import { HouseholdHeader } from '../components/HouseholdHeader';
import { HouseholdMembers } from '../components/HouseholdMembers';
import { HouseholdSummaryCard } from '../components/HouseholdSummaryCard';
import { InviteCarerForm } from '../components/InviteCarerForm';
import {
  useAddHouseholdChildMutation,
  useHouseholdsQuery,
  useInviteHouseholdCarerMutation,
  useRemoveHouseholdCarerMutation,
  useRemoveHouseholdChildMutation,
  useUploadHouseholdChildAvatarMutation,
  useUpdateHouseholdChildMutation,
  useUpdateHouseholdMutation
} from '../queries/household.queries';
import type { ChildModel, HouseholdModel, InviteCarerForm as InviteCarerFormModel, SaveChildForm } from '../types/household.types';

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
  const uploadHouseholdChildAvatarMutation = useUploadHouseholdChildAvatarMutation();

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

  const handleSaveInvite = async (form: InviteCarerFormModel) => {
    if (!inviteCarer || !household) return;
    await inviteHouseholdCarerMutation.mutateAsync({ householdId: household.id, form });
    setIniviteCarer(false);
  };

  const handleAddChild = () => {
    setEditChild(false);
    setEditingChild(null);
    setAddChild(true);
  };

  const getAddedChild = (previousHousehold: HouseholdModel, updatedHousehold: HouseholdModel): ChildModel | null => {
    const previousChildIds = new Set(previousHousehold.children.map((child) => child.id));
    return updatedHousehold.children.find((child) => !previousChildIds.has(child.id)) ?? null;
  };

  const uploadChildAvatar = async (childId: string, avatarFile: File | null) => {
    if (!household || !avatarFile) return;
    await uploadHouseholdChildAvatarMutation.mutateAsync({ householdId: household.id, childId, file: avatarFile });
  };

  const handleSaveChild = async ({ child: form, avatarFile }: SaveChildForm) => {
    if (!household) return;

    if (editChild && editingChild) {
      await updateHouseholdChildMutation.mutateAsync({ householdId: household.id, childId: editingChild.id, form });
      await uploadChildAvatar(editingChild.id, avatarFile);
      setEditChild(false);
      setEditingChild(null);
      return;
    }

    if (!addChild) return;
    const updatedHousehold = await addHouseholdChildMutation.mutateAsync({ householdId: household.id, form });
    const addedChild = getAddedChild(household, updatedHousehold);
    if (addedChild) await uploadChildAvatar(addedChild.id, avatarFile);
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

      <InviteCarerForm
        household={household}
        saving={inviteHouseholdCarerMutation.isPending}
        visible={inviteCarer}
        onHide={() => {
          setIniviteCarer(false);
        }}
        onSave={(form: InviteCarerFormModel) => {
          void handleSaveInvite(form);
        }}
      />
      <ChildForm
        key={editChild && editingChild ? editingChild.id : addChild ? 'add-child-open' : 'add-child-closed'}
        child={editChild ? editingChild : null}
        household={household}
        saving={addHouseholdChildMutation.isPending || updateHouseholdChildMutation.isPending || uploadHouseholdChildAvatarMutation.isPending}
        visible={addChild || editChild}
        onHide={handleCloseChildDialog}
        onSave={(form) => {
          void handleSaveChild(form);
        }}
      />
    </section>
  );
};
